using DevKnowledgeBase.UI.Common;
using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace DevKnowledgeBase.UI.Services
{
    public class CampService : ICampService
    {
        private readonly HttpClient _httpClient;

        public CampService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<PaginatedResult<CampModel>> GetAllCampsAsync(string location, int page, int pageSize,bool includeInactive = false)
        {
            var response = await _httpClient.GetFromJsonAsync<PaginatedResult<CampModel>>($"api/camps?page={page}&pageSize={pageSize}&location={Uri.EscapeDataString(location)}" +
                $"&includeInactive={includeInactive}");


            return response ?? new PaginatedResult<CampModel>();

        }

        public async Task<List<CampModel>> GetOrganizerCampsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<CampModel>>("api/camps/organizer");
            return response ?? new List<CampModel>();
        }

        public async Task<CampModel> GetCampByIdAsync(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<CampModel>($"api/camps/{id}");
            return response;

        }

        public async Task<ResponseMessage> CreateCampAsync(CampModel camp)
        {
            var response = await _httpClient.PostAsJsonAsync("api/camps", camp);
            return await response.GetMessageAsync();

        }

        public async Task<ResponseMessage> UpdateCampAsync(CampModel camp)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/camps/{camp.Id}", camp);
            return await response.GetMessageAsync();
            
        }

        public async Task<ResponseMessage> DeleteCampAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/camps/{id}");
            return await response.GetMessageAsync();

        }

        public async Task<string> UploadPhotoAsync(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5242880)); // 5 MB limit
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/files/upload-photo", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UploadPhotoResponse>();
            return result?.Url;
        }

        private class UploadPhotoResponse
        {
            public string Url { get; set; }
        }
    }
}
