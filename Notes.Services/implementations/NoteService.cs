using Notes.Models;
using Notes.Repositories.interfaces;
using Notes.Services.interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Notes.Services.implementations
{
    public class NoteService: INoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NoteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddNotes(Note note)
        {
            await _unitOfWork.NoteRepository.Create(note);
            if(await _unitOfWork.SaveAsync()> 0) {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteNotes(int id)
        {
            await _unitOfWork.NoteRepository.Delete(id);
            if(await _unitOfWork.SaveAsync()> 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Note>> GetAllNotes()
        {
            return await _unitOfWork.NoteRepository.GetAll();
        }

        public async Task<Note> GetNoteById(int id)
        {
            var note = await _unitOfWork.NoteRepository.GetBy(x=>x.Id == id);
            return note!;
        }

        public async Task<bool> UpdateNotes(Note note)
        {
             _unitOfWork.NoteRepository.Edit(note);
            if(await _unitOfWork.SaveAsync()> 0)
            {
                return true;
            }
            return false;
        }
    }
}
