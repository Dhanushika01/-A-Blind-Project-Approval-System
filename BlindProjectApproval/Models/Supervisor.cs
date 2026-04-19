namespace BlindProjectApproval.Models;

public sealed class Supervisor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> SelectedExpertiseIds { get; set; } = new();
}
