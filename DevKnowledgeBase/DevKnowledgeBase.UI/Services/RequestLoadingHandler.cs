namespace DevKnowledgeBase.UI.Services
{
    public class RequestLoadingDelegatingHandler(CircuitServicesAccessor circuitServicesAccessor) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var loadingService = circuitServicesAccessor.Services.GetRequiredService<LoadingService>();
            return await loadingService.RunWithLoading(async () =>
            {
                return await base.SendAsync(request, cancellationToken);
            });

            
        }
    }
}
