using CatalogService.Data;
using CatalogService.DTOs;
using CatalogService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/v1/articles")]
public class ArticlesController : ControllerBase
{
    private readonly CatalogDb _db;

    public ArticlesController(CatalogDb db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ArticleResponse>>> GetAll()
    {
        var articles = await _db.Articles.OrderBy(a => a.Name).ToListAsync();
        return Ok(articles.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArticleResponse>> GetById(Guid id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(Map(article));
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPost]
    public async Task<ActionResult<ArticleResponse>> Create(CreateArticleRequest request)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Brand = request.Brand,
            WarrantyMonths = request.WarrantyMonths,
            Active = true
        };

        _db.Articles.Add(article);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = article.Id }, Map(article));
    }

    [Authorize(Roles = "SAV_MANAGER")]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null)
        {
            return NotFound();
        }

        article.Active = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ArticleResponse Map(Article article) =>
        new(article.Id, article.Name, article.Brand, article.WarrantyMonths, article.Active);
}
