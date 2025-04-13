using DevKnowledgeBase.UI.Models;

namespace DevKnowledgeBase.UI.Services
{
    public interface IUserService 
    {
        Task<UserProfileModel> GetUserProfileAsync();
        Task<ResponseMessage> UpdateUserProfileAsync(UserProfileModel model);
        Task<List<UserProfileModel>> GetAllOrganizers();
        Task<ResponseMessage> CreateOrganizerUser(UserProfileModel model);
        Task<ResponseMessage> DeleteOrganizerUserAsync(string userId);
        Task<ResponseMessage> UpdateOrganizerUserAsync(UserProfileModel model);
    }
}
