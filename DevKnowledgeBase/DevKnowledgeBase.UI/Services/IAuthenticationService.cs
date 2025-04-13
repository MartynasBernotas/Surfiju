using DevKnowledgeBase.UI.Models;

namespace DevKnowledgeBase.UI.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseMessage> LoginAsync(string email, string password);
        Task<ResponseMessage> RegisterAsync(string username, string email, string password);
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
        Task<string> ConfirmEmail(string userId, string token);
        Task<ResponseMessage> ForgotPasswordAsync(string email);
        Task<ResponseMessage> ChangePasswordAsync(string userId, string token, string newPassword);
    }
}
