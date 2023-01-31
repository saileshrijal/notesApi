using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.Models;
using Notes.Services.interfaces;
using Notes.Utilites;
using Notes.ViewModels;
using System.Security.Claims;

namespace Notes.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        public NoteController(INoteService  noteService, ICategoryService categoryService, UserManager<ApplicationUser> userManager)
        {
            _noteService= noteService;
            _categoryService= categoryService;
            _userManager= userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var notes = await _noteService.GetAllNotes(await LoggedInUserId());
            var notesListVm = notes.Select(x => new NoteVM() {
                Id= x.Id,
                Title= x.Title,
                Description= x.Description,
                CategoryId= x.CategoryId,
                CreatedOn   = x.CreatedOn,
                Category = x.Category,
            }).ToList();
            var totalCount = notesListVm.Count();
            var items = notesListVm.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagination = new Pagination<NoteVM> {
                Page =page,
                PageSize =pageSize,
                TotalCount=totalCount,
                Data = items
            };
            return Ok(pagination); 
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id) {
            var note = await _noteService.GetNoteById(id);
            if (await LoggedInUserId() != note.ApplicationUserId)
            {
                return BadRequest("You are not authorized");
            }
            if (note == null) { return BadRequest("Notes could not be found"); }

            var notesListVm = new NoteVM() {
               Id = note.Id,
               Title= note.Title,
               Description= note.Description,
               CategoryId= note.CategoryId,
               CreatedOn = note.CreatedOn,
            };

            return Ok(notesListVm); 
        }

        [HttpPost]
        public async Task<IActionResult> Post(NoteVM vm) {
            if(!ModelState.IsValid) { return BadRequest("some fields are missing"); }
            try
            {
                var checkCategory = await _categoryService.GetCategoryById(vm.CategoryId);
                if (checkCategory == null) { return BadRequest($"Category of Id: {vm.CategoryId} does not exist"); }
                var note = new Note {
                    Id = vm.Id,
                    Title= vm.Title,
                    Description= vm.Description,
                    CategoryId= vm.CategoryId,
                    CreatedOn = vm.CreatedOn,
                    ApplicationUserId = await LoggedInUserId()

                };
                var result = await _noteService.AddNotes(note);
                if (result)
                {
                    return Ok("Notes created successfully");
                }
                return BadRequest("Notes cannot be added");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, NoteVM vm)
        {
            if (!ModelState.IsValid) { return BadRequest("Some fields are missing"); }
            if(id != vm.Id) { return BadRequest("Id do not match"); }
            try
            {
                var existingNotes = await _noteService.GetNoteById(id);
                if (existingNotes == null) { return BadRequest("Notes with the id is not found"); }
                if (await LoggedInUserId() != existingNotes.ApplicationUserId)
                {
                    return BadRequest("You are not authorized");
                }
                var note = new Note
                {
                    Id = vm.Id,
                    Title = vm.Title,
                    Description = vm.Description,
                    CategoryId = vm.CategoryId,
                    CreatedOn = vm.CreatedOn,
                };

                var result = await _noteService.UpdateNotes(note);
                if (result) {
                    return Ok("Note updated successfully");
                }else{
                    return BadRequest("Note cannot be updated");
                }
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingNote = await _noteService.GetNoteById(id);
                if (await LoggedInUserId() != existingNote.ApplicationUserId)
                {
                    return BadRequest("You are not authorized");
                }
                if (existingNote == null) { return BadRequest("Notes cannot be found with the id: " + id); }
                var result = await _noteService.DeleteNotes(id);
                if (result) { return Ok("Note deleted successfully"); }
                return BadRequest("Note cannot be deleted");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        private async Task<string> LoggedInUserId()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await _userManager.FindByNameAsync(username!);
            return loggedInUser!.Id;
        }
    }
}
