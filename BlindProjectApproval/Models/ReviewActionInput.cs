namespace BlindProjectApproval.Models;

public sealed class ReviewActionInput
{
    public int ProposalId { get; set; }
    public string ActionType { get; set; } = string.Empty;
}
