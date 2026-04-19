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

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReview([FromBody] ReviewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return Unauthorized();
        if (user.Role != "reviewer" && user.Role != "admin") return Forbid();

        var review = new Review
        {
            ReviewerId = userId.Value,
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
    public async Task<IActionResult> GetReviews(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return Unauthorized();

        if (user.Role == "admin")
        {
            var reviews = await _context.Reviews.Where(r => r.ProjectId == id).Include(r => r.Reviewer).ToListAsync();
            return Ok(reviews);
        }
        else
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.ProjectId == id && (r.ReviewerId == userId.Value || project.SubmittedById == userId.Value))
                .Include(r => r.Reviewer)
                .ToListAsync();

            // Blind the reviewer identity for non-admins if it was anonymous, unless the viewer is the reviewer themselves
            foreach (var review in reviews)
            {
                if (review.Anonymous && review.ReviewerId != userId.Value)
                {
                    review.Reviewer = null;
                }
            }

            return Ok(reviews);
        }
    }
}

public class ReviewModel
{
    public int ProjectId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool Anonymous { get; set; } = true;
}