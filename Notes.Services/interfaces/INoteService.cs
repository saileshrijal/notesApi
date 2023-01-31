using Notes.Models;
namespace Notes.Services.interfaces
{
    public interface INoteService
    {
        Task<bool> AddNotes(Note note);
        Task<bool> DeleteNotes(int id);
        Task<bool> UpdateNotes(Note note);
        Task<Note> GetNoteById(int id);
        Task <List<Note>> GetAllNotes(string userId);

    }
}
