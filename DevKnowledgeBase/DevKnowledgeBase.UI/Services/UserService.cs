using DevKnowledgeBase.UI.Common;
using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DevKnowledgeBase.UI.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _stateProvider;

        public UserService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider stateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _stateProvider = stateProvider;
        }

        public async Task<UserProfileModel> GetUserProfileAsync()
        {
            var user = await ((CustomAuthStateProvider)_stateProvider).GetAuthenticationStateAsync();
            var userIdClaim = user.User.Claims.FirstOrDefault();
            if (userIdClaim == null)
            {
                throw new Exception("User not found");
            }
            var response = await _httpClient.GetFromJsonAsync<UserProfileModel>($"api/user/profile?userId={userIdClaim.Value}");
            return response;
        }

        public async Task<List<UserProfileModel>> GetAllOrganizers()
        {
            var isAdmin = await ((CustomAuthStateProvider)_stateProvider).IsAdminAsync();
            if (isAdmin)
            {
                return await _httpClient.GetFromJsonAsync<List<UserProfileModel>>("api/user/organizers") ?? new List<UserProfileModel>();
            }

            return new List<UserProfileModel>();
        }

        public async Task<ResponseMessage> UpdateUserProfileAsync(UserProfileModel model)
        {
            var response = await _httpClient.PutAsJsonAsync("api/user/profile", model);
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> CreateOrganizerUser(UserProfileModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/register/organizer", model);
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> DeleteOrganizerUserAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"api/user/organizer/{userId}");
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> UpdateOrganizerUserAsync(UserProfileModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/user/organizer/{model.Id}", model);
            return await response.GetMessageAsync();
        }
    }
}
