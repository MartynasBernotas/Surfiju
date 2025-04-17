using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace DevKnowledgeBase.UI.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
        {
            _sessionStorage = sessionStorage;
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var storedToken = await _sessionStorage.GetAsync<string>("authToken");
            if (storedToken.Success && !string.IsNullOrEmpty(storedToken.Value))
            {
                if (IsTokenExpired(storedToken.Value))
                {
                    var refreshed = await RefreshTokenAsync();
                    if (!refreshed)
                    {
                        await Logout();
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }

                if (_currentUser.Identity is not { IsAuthenticated: true })
                {
                    SetCurrentUserFromToken(storedToken.Value);
                }
            }
            else
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task<bool> IsAdminAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.IsInRole("Admin");
        }

        public async Task SetUser(string token, string refreshToken)
        {
            await _sessionStorage.SetAsync("authToken", token);
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
            SetCurrentUserFromToken(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("authToken");
            await _sessionStorage.DeleteAsync("refreshToken");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = await _sessionStorage.GetAsync<string>("refreshToken");
            var authToken = await _sessionStorage.GetAsync<string>("authToken");
            if (!refreshToken.Success || string.IsNullOrEmpty(refreshToken.Value) || !IsTokenExpired(authToken.Value))
            {
                return false;
            }

            await _sessionStorage.DeleteAsync("authToken");
            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh-token", new { Token = authToken.Value, RefreshToken = refreshToken.Value });

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                await _sessionStorage.SetAsync("authToken", authResponse.Token);
                await _sessionStorage.SetAsync("refreshToken", authResponse.RefreshToken);
                SetCurrentUserFromToken(authResponse.Token);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
                return true;
            }

            return false;
        }

        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var expiration = jwtToken.ValidTo;
            return expiration < DateTime.UtcNow;
        }

        private void SetCurrentUserFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = new List<Claim>(jwtToken.Claims);

            var identity = new ClaimsIdentity(claims, "jwt");
            _currentUser = new ClaimsPrincipal(identity);
        }

        public async Task PreloadAsync()
        {
            var storedToken = await _sessionStorage.GetAsync<string>("authToken");
            if (storedToken.Success && !string.IsNullOrEmpty(storedToken.Value))
            {
                SetCurrentUserFromToken(storedToken.Value);
            }
        }
    }
}
