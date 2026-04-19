namespace BlindProjectApproval.Models;

public sealed class ProposalReviewViewModel
{
    public Proposal Proposal { get; set; } = new Proposal();
    public ExpertiseArea? ExpertiseArea { get; set; }
}
