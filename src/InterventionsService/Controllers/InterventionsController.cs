using System.Security.Claims;
using InterventionsService.Data;
using InterventionsService.DTOs;
using InterventionsService.Models;
using InterventionsService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterventionsService.Controllers;

[ApiController]
[Route("api/v1/interventions")]
public class InterventionsController : ControllerBase
{
    private readonly InterventionsDb _db;
    private readonly ClaimsClient _claimsClient;
    private readonly CatalogClient _catalogClient;

    public InterventionsController(InterventionsDb db, ClaimsClient claimsClient, CatalogClient catalogClient)
    {
        _db = db;
        _claimsClient = claimsClient;
        _catalogClient = catalogClient;
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPost]
    public async Task<ActionResult<InterventionResponse>> Create(CreateInterventionRequest request, CancellationToken cancellationToken)
    {
        var claim = await _claimsClient.GetClaimAsync(request.ClaimId, cancellationToken);
        if (claim is null)
        {
            return NotFound("Claim not found.");
        }

        var article = await _catalogClient.GetArticleAsync(claim.ArticleId, cancellationToken);
        if (article is null)
        {
            return NotFound("Article not found.");
        }

        var warrantyEnds = claim.PurchaseDate.AddMonths(article.WarrantyMonths);
        var isUnderWarranty = DateOnly.FromDateTime(DateTime.UtcNow) <= warrantyEnds;

        var intervention = new Intervention
        {
            Id = Guid.NewGuid(),
            ClaimId = claim.Id,
            Status = InterventionStatus.Planned,
            IsUnderWarranty = isUnderWarranty,
            LaborCost = isUnderWarranty ? 0 : request.LaborCost,
            InvoiceTotal = isUnderWarranty ? 0 : request.LaborCost,
            Notes = request.Notes
        };

        _db.Interventions.Add(intervention);
        await _db.SaveChangesAsync(cancellationToken);

        await _claimsClient.UpdateClaimStatusAsync(claim.Id, "InProgress", cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = intervention.Id }, Map(intervention));
    }

    [Authorize]
    [HttpGet("by-claim/{claimId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<InterventionResponse>>> GetByClaim(Guid claimId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("CLIENT"))
        {
            var claim = await _claimsClient.GetClaimAsync(claimId, cancellationToken);
            if (claim is null)
            {
                return Forbid();
            }
        }

        var interventions = await _db.Interventions.Where(i => i.ClaimId == claimId).ToListAsync(cancellationToken);
        return Ok(interventions.Select(Map));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InterventionResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var intervention = await _db.Interventions.FindAsync(id);
        if (intervention is null)
        {
            return NotFound();
        }

        if (User.IsInRole("CLIENT"))
        {
            var claim = await _claimsClient.GetClaimAsync(intervention.ClaimId, cancellationToken);
            if (claim is null)
            {
                return Forbid();
            }
        }

        return Ok(Map(intervention));
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/schedule")]
    public async Task<IActionResult> Schedule(Guid id, ScheduleInterventionRequest request)
    {
        var intervention = await _db.Interventions.FindAsync(id);
        if (intervention is null)
        {
            return NotFound();
        }

        intervention.ScheduledAt = request.ScheduledAt;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        var intervention = await _db.Interventions.FindAsync(id);
        if (intervention is null)
        {
            return NotFound();
        }

        intervention.StartedAt = DateTime.UtcNow;
        intervention.Status = InterventionStatus.InProgress;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id, CancellationToken cancellationToken)
    {
        var intervention = await _db.Interventions.Include(i => i.PartLines).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (intervention is null)
        {
            return NotFound();
        }

        intervention.EndedAt = DateTime.UtcNow;
        intervention.Status = InterventionStatus.Done;
        RecalculateInvoice(intervention);
        await _db.SaveChangesAsync(cancellationToken);

        await _claimsClient.UpdateClaimStatusAsync(intervention.ClaimId, "Resolved", cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPost("{id:guid}/parts")]
    public async Task<ActionResult<PartLineResponse>> AddPart(Guid id, AddPartRequest request)
    {
        var intervention = await _db.Interventions.Include(i => i.PartLines).FirstOrDefaultAsync(i => i.Id == id);
        if (intervention is null)
        {
            return NotFound();
        }

        if (intervention.Status == InterventionStatus.Done)
        {
            return BadRequest("Intervention is closed.");
        }

        var part = new PartLine
        {
            Id = Guid.NewGuid(),
            InterventionId = id,
            PartName = request.PartName,
            UnitPrice = request.UnitPrice,
            Qty = request.Qty,
            LineTotal = request.UnitPrice * request.Qty
        };

        intervention.PartLines.Add(part);
        RecalculateInvoice(intervention);
        await _db.SaveChangesAsync();

        return Ok(Map(part));
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/labor")]
    public async Task<IActionResult> UpdateLabor(Guid id, LaborCostRequest request)
    {
        var intervention = await _db.Interventions.Include(i => i.PartLines).FirstOrDefaultAsync(i => i.Id == id);
        if (intervention is null)
        {
            return NotFound();
        }

        if (intervention.Status == InterventionStatus.Done)
        {
            return BadRequest("Intervention is closed.");
        }

        intervention.LaborCost = intervention.IsUnderWarranty ? 0 : request.LaborCost;
        RecalculateInvoice(intervention);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id:guid}/invoice")]
    public async Task<ActionResult<InvoiceResponse>> GetInvoice(Guid id, CancellationToken cancellationToken)
    {
        var intervention = await _db.Interventions.Include(i => i.PartLines).FirstOrDefaultAsync(i => i.Id == id);
        if (intervention is null)
        {
            return NotFound();
        }

        if (User.IsInRole("CLIENT"))
        {
            var claim = await _claimsClient.GetClaimAsync(intervention.ClaimId, cancellationToken);
            if (claim is null)
            {
                return Forbid();
            }
        }

        var lines = intervention.PartLines.Select(Map).ToList();
        var partsTotal = lines.Sum(l => l.LineTotal);
        return Ok(new InvoiceResponse(intervention.Id, intervention.IsUnderWarranty, intervention.LaborCost, partsTotal, intervention.InvoiceTotal, lines));
    }

    private static void RecalculateInvoice(Intervention intervention)
    {
        if (intervention.IsUnderWarranty)
        {
            intervention.InvoiceTotal = 0;
            intervention.LaborCost = 0;
            return;
        }

        var partsTotal = intervention.PartLines.Sum(p => p.LineTotal);
        intervention.InvoiceTotal = intervention.LaborCost + partsTotal;
    }

    private static InterventionResponse Map(Intervention intervention) =>
        new(intervention.Id, intervention.ClaimId, intervention.ScheduledAt, intervention.StartedAt, intervention.EndedAt,
            intervention.Status, intervention.IsUnderWarranty, intervention.LaborCost, intervention.InvoiceTotal, intervention.Notes);

    private static PartLineResponse Map(PartLine part) =>
        new(part.Id, part.InterventionId, part.PartName, part.UnitPrice, part.Qty, part.LineTotal);
}
