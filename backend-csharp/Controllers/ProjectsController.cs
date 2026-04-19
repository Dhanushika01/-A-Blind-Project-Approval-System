using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlindProjectApproval.Models;
using BlindProjectApproval.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return Unauthorized();

        if (user.Role == "admin" || user.Role == "reviewer")
        {
            var projects = await _context.Projects.Include(p => p.SubmittedBy).ToListAsync();
            return Ok(projects);
        }
        return Forbid();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyProjects()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var projects = await _context.Projects.Where(p => p.SubmittedById == userId.Value).ToListAsync();
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitProject([FromBody] ProjectModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var project = new Project
        {
            Title = model.Title,
            Description = model.Description,
            SubmittedById = userId.Value
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return Unauthorized();
        if (user.Role != "admin") return Forbid();

        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();

        project.Status = model.Status;
        await _context.SaveChangesAsync();
        return Ok(project);
    }
}

public class ProjectModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class StatusModel
{
    public string Status { get; set; } = string.Empty;
}