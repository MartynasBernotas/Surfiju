using System.Net.Http;

namespace DevKnowledgeBase.UI.Services
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty ;
    }

    public class NoteService : INoteService
    {
        private readonly HttpClient _httpClient;

        public NoteService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<NoteDto>> GetNotesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<NoteDto>>("api/notes?page=1&pageSize=10&search=") ?? new List<NoteDto>();
        }

        public async Task<NoteDto?> GetNoteByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<NoteDto>($"api/notes/{id}");
        }

        public async Task<bool> CreateNoteAsync(NoteDto note)
        {
            var response = await _httpClient.PostAsJsonAsync("api/notes", note);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNoteAsync(Guid id, NoteDto note)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/notes/{id}", note);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/notes/{id}");
            return response.IsSuccessStatusCode;
        }

    }
}
