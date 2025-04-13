using DevKnowledgeBase.UI.Common;
using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace DevKnowledgeBase.UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigation;
        private readonly AuthenticationStateProvider _stateProvider;

        public AuthenticationService(IHttpClientFactory httpClientFactory, NavigationManager navigation,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _navigation = navigation;
            _stateProvider = authenticationStateProvider;
        }

        public async Task<ResponseMessage> LoginAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                var token = authResponse.Token;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await ((CustomAuthStateProvider)_stateProvider).SetUser(token, authResponse.RefreshToken);
            }

            return await response.GetMessageAsync();
        }

        // Register logic (returns UserDTO)
        public async Task<ResponseMessage> RegisterAsync(string username, string email, string password)
        {
            var registerData = new { Username = username, Email = email, Password = password };

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerData);
            return await response.GetMessageAsync();
        }

        public async Task LogoutAsync()
        {
            await ((CustomAuthStateProvider)_stateProvider).Logout();
            _httpClient.DefaultRequestHeaders.Authorization = null; // Clear Authorization header
            _navigation.NavigateTo("/login"); // Redirect to login page
        }

        // Check if the user is authenticated
        public async Task<bool> IsAuthenticatedAsync()
        {
            var authResult = await ((CustomAuthStateProvider)_stateProvider).GetAuthenticationStateAsync();
            return authResult.User.Identity.IsAuthenticated;
        }

        public async Task<string> ConfirmEmail(string userId, string token)
        {
            var response = await _httpClient.GetAsync($"api/auth/confirm-email?userId={userId}&token={token}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<ResponseMessage> ForgotPasswordAsync(string email)
        {
            var data = new { Email = email};
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", data);
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> ChangePasswordAsync(string userId, string token, string newPassword)
        {
            var data = new { UserId = userId, Token = token, NewPassword = newPassword };
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", data);
            return await response.GetMessageAsync();
        }

        public async Task<bool> RefreshTokenAsync()
        {
            return await ((CustomAuthStateProvider)_stateProvider).RefreshTokenAsync();
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
