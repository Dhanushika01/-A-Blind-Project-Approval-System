using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlindProjectApproval.Models;
using BlindProjectApproval.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReview([FromBody] ReviewModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _context.Users.Find(userId);
        if (user.Role != "reviewer" && user.Role != "admin") return Forbid();

        var review = new Review
        {
            ReviewerId = userId,
            ProjectId = model.ProjectId,
            Rating = model.Rating,
            Comment = model.Comment,
            Anonymous = model.Anonymous
        };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return Ok(review);
    }

    [HttpGet("project/{id}")]
    public IActionResult GetReviews(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _context.Users.Find(userId);
        if (user.Role == "admin")
        {
            var reviews = _context.Reviews.Where(r => r.ProjectId == id).Include(r => r.Reviewer).ToList();
            return Ok(reviews);
        }
        else
        {
            var reviews = _context.Reviews.Where(r => r.ProjectId == id && r.ReviewerId == userId).Include(r => r.Reviewer).ToList();
            return Ok(reviews);
        }
    }
}

public class ReviewModel
{
    public int ProjectId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public bool Anonymous { get; set; } = true;
}