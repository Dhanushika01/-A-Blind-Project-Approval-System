namespace BlindProjectApproval.Models;

public sealed class Proposal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ExpertiseAreaId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public string StatusLabel => Status.ToString();
}
