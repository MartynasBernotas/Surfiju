using DevKnowledgeBase.UI.Models;
using DevKnowledgeBase.UI.Common;
using System.Net.Http.Json;


namespace DevKnowledgeBase.UI.Services
{
    public interface IBookingService
    {
        Task<List<BookingModel>> GetUserBookingsAsync(BookingStatus? status = null);
        Task<CreateBookingResponse?> CreateBookingAsync(CreateBookingModel booking);
        Task<ResponseMessage> CancelBookingAsync(Guid bookingId);
        Task<ResponseMessage> ConfirmBookingAsync(Guid bookingId, string paymentIntentId);
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

        public async Task<List<BookingModel>> GetUserBookingsAsync(BookingStatus? status = null)
        {
            var url = status.HasValue ? $"api/bookings?status={(int)status.Value}" : "api/bookings";
            var response = await _httpClient.GetFromJsonAsync<List<BookingModel>>(url);
            return response ?? new List<BookingModel>();
        }

        public async Task<CreateBookingResponse?> CreateBookingAsync(CreateBookingModel booking)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings", booking);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CreateBookingResponse>();
        }

        public async Task<ResponseMessage> CancelBookingAsync(Guid bookingId)
        {
            var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
            return await response.GetMessageAsync();
        }

        public async Task<ResponseMessage> ConfirmBookingAsync(Guid bookingId, string paymentIntentId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/bookings/{bookingId}/confirm", new { paymentIntentId });
            return await response.GetMessageAsync();
        }
    }
}
