using BlindProjectApproval.Models;

namespace BlindProjectApproval.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
    }
}