using System.Security.Claims;
using CustomerClaimsService.Data;
using CustomerClaimsService.DTOs;
using CustomerClaimsService.Models;
using ClaimEntity = CustomerClaimsService.Models.Claim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerClaimsService.Controllers;

[ApiController]
[Route("api/v1/claims")]
public class ClaimsController : ControllerBase
{
    private readonly ClaimsDb _db;

    public ClaimsController(ClaimsDb db)
    {
        _db = db;
    }

    [Authorize(Roles = "CLIENT")]
    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> Create(CreateClaimRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var name = User.FindFirstValue("name") ?? string.Empty;

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer is null)
        {
            customer = new Customer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = email,
                FullName = name
            };
            _db.Customers.Add(customer);
        }

        var claim = new ClaimEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            ArticleId = request.ArticleId,
            SerialNumber = request.SerialNumber,
            PurchaseDate = request.PurchaseDate,
            Description = request.Description,
            Status = ClaimStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = claim.Id }, Map(claim));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClaimResponse>>> GetAll([FromQuery] ClaimStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _db.Claims.AsQueryable();

        if (User.IsInRole("CLIENT"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer is null)
            {
                return Ok(Array.Empty<ClaimResponse>());
            }

            query = query.Where(c => c.CustomerId == customer.Id);
        }
        else if (User.IsInRole("SAV_MANAGER"))
        {
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }
            if (from.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= from.Value);
            }
            if (to.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= to.Value);
            }
        }

        var claims = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return Ok(claims.Select(Map));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> GetById(Guid id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim is null)
        {
            return NotFound();
        }

        if (User.IsInRole("CLIENT"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer is null || claim.CustomerId != customer.Id)
            {
                return Forbid();
            }
        }

        return Ok(Map(claim));
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateClaimStatusRequest request)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim is null)
        {
            return NotFound();
        }

        claim.Status = request.Status;
        claim.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ClaimResponse Map(ClaimEntity claim) =>
        new(claim.Id, claim.CustomerId, claim.ArticleId, claim.SerialNumber, claim.PurchaseDate, claim.Description, claim.Status, claim.CreatedAt, claim.UpdatedAt);
}
