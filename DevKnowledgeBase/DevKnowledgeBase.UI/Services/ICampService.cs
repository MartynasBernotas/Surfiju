using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace DevKnowledgeBase.UI.Services
{
    public interface ICampService
    {
        Task<List<CampModel>> GetAllCampsAsync(bool includeInactive = false);
        Task<List<CampModel>> GetOrganizerCampsAsync();
        Task<CampModel> GetCampByIdAsync(Guid id);
        Task<ResponseMessage> CreateCampAsync(CampModel Camps);
        Task<ResponseMessage> UpdateCampAsync(CampModel Camps);
        Task<ResponseMessage> DeleteCampAsync(Guid id);
        Task<string> UploadPhotoAsync(IBrowserFile file);
    }
}
