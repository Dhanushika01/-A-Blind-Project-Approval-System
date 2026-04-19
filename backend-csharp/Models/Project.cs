using System.ComponentModel.DataAnnotations;

namespace BlindProjectApproval.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string TechStack { get; set; }
        [Required]
        public string ResearchArea { get; set; }
        public int SubmittedById { get; set; }
        public User SubmittedBy { get; set; }
        public string Status { get; set; } = "pending"; // pending, matched, approved, rejected
        public List<Review> Reviews { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}