using Notes.Data;
using Notes.Models;
using Notes.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Repositories.implementations
{
    internal class NoteRepository : GenericRepository<Note>, INoteRepository
    {
        public NoteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
