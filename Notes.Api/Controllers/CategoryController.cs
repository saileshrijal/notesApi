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
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoryController(ICategoryService categoryService, UserManager<ApplicationUser> userManager)
        {
            _categoryService = categoryService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]int page=1, [FromQuery]int pageSize=10)
        {
            var categories = await _categoryService.GetAllCategories(await LoggedInUserId());
            var categoryListVM = categories.Select(x => new CategoryVM()
            {
                Id= x.Id,
                Title = x.Title,
                CreatedOn= x.CreatedOn,
                Description = x.Description
            }).ToList();//converting list of models to list of viewmodels
            var totalCount = categoryListVM.Count();
            var items = categoryListVM.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagination = new Pagination<CategoryVM>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = items,
            };
            return Ok(pagination);
        }

        //get by id
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            
            var category = await _categoryService.GetCategoryById(id);
            if(await LoggedInUserId() != category.ApplicationUserId)
            {
                return BadRequest("You are not authorized");
            }
            if (category == null) { return BadRequest("Category could not found"); }
            var categoryVM = new CategoryVM()//converting model to viewmodel
            {
                Id = category.Id,
                Title = category.Title,
                Description = category.Description,
                CreatedOn = DateTime.Now
            };
            return Ok(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryVM vm)
        {
            if(!ModelState.IsValid) { return BadRequest("Some field is missing"); }
            try
            {
                var category = new Category()
                {
                    Id = vm.Id,
                    Title = vm.Title,
                    Description = vm.Description,
                    CreatedOn = DateTime.Now,
                    ApplicationUserId = await LoggedInUserId()
                };
                var result = await _categoryService.AddCategory(category);
                if (result)
                {
                    return Ok("Category created successfully");
                }
                return BadRequest("Category cannot be added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, CategoryVM vm)
        {
            if (!ModelState.IsValid) { return BadRequest("Some field is missing"); }
            try
            {
                if (id != vm.Id) { return BadRequest("Id do not match"); }
                var existingCategory = await _categoryService.GetCategoryById(id);
                if(existingCategory == null) { return BadRequest("Category could not found"); }

                if (await LoggedInUserId() != existingCategory.ApplicationUserId)
                {
                    return BadRequest("You are not authorized");
                }
                existingCategory.Title = vm.Title;
                existingCategory.Description = vm.Description;
                existingCategory.CreatedOn = vm.CreatedOn;
                var result = await _categoryService.UpdateCategory(existingCategory);
                if (result)
                {
                    return Ok("Category updated successfully");
                }
                else
                {
                    return BadRequest("Category cannot be added");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingCategory = await _categoryService.GetCategoryById(id);
                if (await LoggedInUserId() != existingCategory.ApplicationUserId)
                {
                    return BadRequest("You are not authorized");
                }
                if (existingCategory == null) { return BadRequest("Category could not found"); }
                var result = await _categoryService.DeleteCategory(id);
                if (result)
                {
                    return Ok("Category deleted successfully");
                }
                return BadRequest("Category cannot be deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> LoggedInUserId()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await _userManager.FindByNameAsync(username!);
            return loggedInUser!.Id;
        }
    }
}
