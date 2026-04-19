using System.ComponentModel.DataAnnotations;

namespace BlindProjectApproval.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool Anonymous { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}