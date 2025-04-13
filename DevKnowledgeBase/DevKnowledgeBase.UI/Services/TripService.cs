using DevKnowledgeBase.UI.Common;
using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace DevKnowledgeBase.UI.Services
{
    public class TripService : ITripService
    {
        private readonly HttpClient _httpClient;

        public TripService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<TripModel>> GetAllTripsAsync(bool includeInactive = false)
        {
            var response = await _httpClient.GetFromJsonAsync<List<TripModel>>($"api/trips?includeInactive={includeInactive}");
            return response ?? new List<TripModel>();

        }

        public async Task<List<TripModel>> GetOrganizerTripsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<TripModel>>("api/trips/organizer");
            return response ?? new List<TripModel>();
        }

        public async Task<TripModel> GetTripByIdAsync(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<TripModel>($"api/trips/{id}");
            return response;

        }

        public async Task<ResponseMessage> CreateTripAsync(TripModel trip)
        {
            var response = await _httpClient.PostAsJsonAsync("api/trips", trip);
            return await response.GetMessageAsync();

        }

        public async Task<ResponseMessage> UpdateTripAsync(TripModel trip)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/trips/{trip.Id}", trip);
            return await response.GetMessageAsync();
            
        }

        public async Task<ResponseMessage> DeleteTripAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/trips/{id}");
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
