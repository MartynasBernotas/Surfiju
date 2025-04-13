namespace DevKnowledgeBase.UI.Services
{
    public interface INoteService
    {
        Task<List<NoteDto>> GetNotesAsync();
        Task<NoteDto?> GetNoteByIdAsync(Guid id);
        Task<bool> CreateNoteAsync(NoteDto note);
        Task<bool> UpdateNoteAsync(Guid id, NoteDto note);
        Task<bool> DeleteNoteAsync(Guid id);
    }
}
