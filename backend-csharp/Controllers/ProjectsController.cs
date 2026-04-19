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
    [Authorize(Roles = "Admin, Supervisor")]
    public IActionResult GetProjects()
    {
        var projects = _context.Projects.Include(p => p.SubmittedBy).ToList();
        return Ok(projects);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public IActionResult GetMyProjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var projects = _context.Projects.Where(p => p.SubmittedById == userId).ToList();
        return Ok(projects);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitProject([FromBody] ProjectModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var project = new Project
        {
            Title = model.Title,
            Abstract = model.Abstract,
            TechStack = model.TechStack,
            ResearchArea = model.ResearchArea,
            SubmittedById = userId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusModel model)
    {

        var project = _context.Projects.Find(id);
        if (project == null) return NotFound();
        project.Status = model.Status;
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpPut("my/{id}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> EditProject(int id, [FromBody] ProjectModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var project = _context.Projects.FirstOrDefault(p => p.Id == id && p.SubmittedById == userId);
        if (project == null) return NotFound();
        if (project.Status != "pending") return BadRequest("Only pending projects can be edited.");

        project.Title = model.Title;
        project.Abstract = model.Abstract;
        project.TechStack = model.TechStack;
        project.ResearchArea = model.ResearchArea;
        
        await _context.SaveChangesAsync();
        return Ok(project);
    }

    [HttpDelete("my/{id}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var project = _context.Projects.FirstOrDefault(p => p.Id == id && p.SubmittedById == userId);
        if (project == null) return NotFound();
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class ProjectModel
{
    public string Title { get; set; }
    public string Abstract { get; set; }
    public string TechStack { get; set; }
    public string ResearchArea { get; set; }
}

public class StatusModel
{
    public string Status { get; set; }
}