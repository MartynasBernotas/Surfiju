using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace DevKnowledgeBase.UI.Services
{
    public interface ITripService
    {
        Task<List<TripModel>> GetAllTripsAsync(bool includeInactive = false);
        Task<List<TripModel>> GetOrganizerTripsAsync();
        Task<TripModel> GetTripByIdAsync(Guid id);
        Task<ResponseMessage> CreateTripAsync(TripModel trip);
        Task<ResponseMessage> UpdateTripAsync(TripModel trip);
        Task<ResponseMessage> DeleteTripAsync(Guid id);
        Task<string> UploadPhotoAsync(IBrowserFile file);
    }
}
