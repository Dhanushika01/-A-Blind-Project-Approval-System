using Microsoft.AspNetCore.Mvc;
using BlindProjectApproval.Models;

namespace BlindProjectApproval.Controllers;

public class SupervisorController : Controller
{
    [HttpGet]
    public IActionResult Index([FromQuery] List<int>? selectedExpertiseIds, string? message)
    {
        var model = new SupervisorDashboardViewModel
        {
            ExpertiseAreas = InMemoryData.ExpertiseAreas,
            SelectedExpertiseIds = selectedExpertiseIds ?? new List<int>(),
            MatchingProposals = InMemoryData.GetProposalsByExpertise(selectedExpertiseIds).ToList(),
            Message = message ?? string.Empty
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Review(int id)
    {
        var proposal = InMemoryData.GetProposal(id);
        if (proposal == null)
        {
            return NotFound();
        }

        var viewModel = new ProposalReviewViewModel
        {
            Proposal = proposal,
            ExpertiseArea = InMemoryData.GetExpertise(proposal.ExpertiseAreaId)
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult ReviewAction(ReviewActionInput input)
    {
        var proposal = InMemoryData.GetProposal(input.ProposalId);
        if (proposal == null)
        {
            return NotFound();
        }

        proposal.Status = input.ActionType switch
        {
            "Accept" => ProposalStatus.Accepted,
            "Reject" => ProposalStatus.Rejected,
            _ => proposal.Status
        };

        return RedirectToAction("Index", new { message = $"Proposal '{proposal.Title}' has been {proposal.Status}." });
    }
}
