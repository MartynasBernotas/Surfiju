using DevKnowledgeBase.UI.Models;
using DevKnowledgeBase.UI.Common;
using DevKnowledgeBase.UI.Models;
using System.Net.Http.Json;


namespace DevKnowledgeBase.UI.Services
{
    public interface IBookingService
    {
        Task<List<BookingModel>> GetUserBookingsAsync();
        Task<ResponseMessage> CreateBookingAsync(CreateBookingModel booking);
        Task<ResponseMessage> CancelBookingAsync(Guid bookingId);
    }
}


namespace DevKnowledgeBase.UI.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<BookingModel>> GetUserBookingsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<BookingModel>>("api/bookings");
            return response ?? new List<BookingModel>();
        }

        public async Task<ResponseMessage> CreateBookingAsync(CreateBookingModel booking)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings", booking);
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> CancelBookingAsync(Guid bookingId)
        {
            var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
            return await response.GetMessageAsync();
        }
    }
}
