using DevKnowledgeBase.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net;
using System.Net.Http.Headers;

public class AuthenticationDelegatingHandler(CircuitServicesAccessor circuitServicesAccessor): DelegatingHandler
    {
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sessionStorage = circuitServicesAccessor.Services.GetRequiredService<ProtectedSessionStorage>();
        var storedToken = await sessionStorage.GetAsync<string>("authToken");
        if (storedToken.Success && !string.IsNullOrEmpty(storedToken.Value))
        {
            request.Headers.Authorization
                = new AuthenticationHeaderValue("Bearer", storedToken.Value);
        }
        
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var authStateProvider = circuitServicesAccessor.Services.GetRequiredService<AuthenticationStateProvider>();
            var custAuthStateProvider = (CustomAuthStateProvider)authStateProvider;
            var refreshTokenResult = await custAuthStateProvider.RefreshTokenAsync();
            if (refreshTokenResult)
            {
                var token = await custAuthStateProvider.GetAuthenticationStateAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.User.FindFirst("authToken").Value);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}
