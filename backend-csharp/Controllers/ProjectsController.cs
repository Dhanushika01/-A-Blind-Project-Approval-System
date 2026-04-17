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

    [HttpGet]
    public IActionResult GetProjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _context.Users.Find(userId);
        if (user.Role == "admin" || user.Role == "reviewer")
        {
            var projects = _context.Projects.Include(p => p.SubmittedBy).ToList();
            return Ok(projects);
        }
        return Forbid();
    }

    [HttpGet("my")]
    public IActionResult GetMyProjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var projects = _context.Projects.Where(p => p.SubmittedById == userId).ToList();
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitProject([FromBody] ProjectModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var project = new Project
        {
            Title = model.Title,
            Description = model.Description,
            SubmittedById = userId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _context.Users.Find(userId);
        if (user.Role != "admin") return Forbid();

        var project = _context.Projects.Find(id);
        if (project == null) return NotFound();
        project.Status = model.Status;
        await _context.SaveChangesAsync();
        return Ok(project);
    }
}

public class ProjectModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class StatusModel
{
    public string Status { get; set; }
}