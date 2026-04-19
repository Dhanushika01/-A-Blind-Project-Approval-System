namespace BlindProjectApproval.Models;

public sealed class SupervisorDashboardViewModel
{
    public List<ExpertiseArea> ExpertiseAreas { get; set; } = new();
    public List<int> SelectedExpertiseIds { get; set; } = new();
    public IEnumerable<Proposal> MatchingProposals { get; set; } = Enumerable.Empty<Proposal>();
    public string Message { get; set; } = string.Empty;
}
