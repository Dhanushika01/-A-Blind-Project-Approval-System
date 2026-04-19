using System.Collections.Generic;
using System.Linq;

namespace BlindProjectApproval.Models;

public static class InMemoryData
{
    public static List<ExpertiseArea> ExpertiseAreas { get; } = new()
    {
        new ExpertiseArea { Id = 1, Name = "Infrastructure" },
        new ExpertiseArea { Id = 2, Name = "Technology" },
        new ExpertiseArea { Id = 3, Name = "Healthcaare" },
        new ExpertiseArea { Id = 4, Name = "Energy" },
        new ExpertiseArea { Id = 5, Name = "Education" }
    };

    public static List<Proposal> Proposals { get; } = new()
    {
        new Proposal { Id = 1, Title = "Smart Traffic Prediction", Description = "A machine learning system that forecasts city traffic flow.", ExpertiseAreaId = 1, StudentName = "Student A", Status = ProposalStatus.Pending },
        new Proposal { Id = 2, Title = "E-commerce Accessibility Audit Tool", Description = "A web-based review system that finds accessibility issues on online stores.", ExpertiseAreaId = 2, StudentName = "Student B", Status = ProposalStatus.Pending },
        new Proposal { Id = 3, Title = "Customer Sentiment Analyzer", Description = "Analyze customer feedback using NLP to highlight product improvements.", ExpertiseAreaId = 3, StudentName = "Student C", Status = ProposalStatus.Pending },
        new Proposal { Id = 4, Title = "Secure IoT Access Gateway", Description = "An authentication gateway for smart devices with zero trust principles.", ExpertiseAreaId = 4, StudentName = "Student D", Status = ProposalStatus.Pending },
        new Proposal { Id = 5, Title = "Cross-platform Campus App", Description = "A mobile app for students to manage schedules, events, and project submissions.", ExpertiseAreaId = 5, StudentName = "Student E", Status = ProposalStatus.Pending }
    };

    public static IEnumerable<Proposal> GetProposalsByExpertise(IEnumerable<int>? expertiseIds)
    {
        if (expertiseIds == null || !expertiseIds.Any())
        {
            return Enumerable.Empty<Proposal>();
        }

        return Proposals.Where(p => expertiseIds.Contains(p.ExpertiseAreaId));
    }

    public static Proposal? GetProposal(int id)
        => Proposals.FirstOrDefault(p => p.Id == id);

    public static ExpertiseArea? GetExpertise(int id)
        => ExpertiseAreas.FirstOrDefault(e => e.Id == id);
}
