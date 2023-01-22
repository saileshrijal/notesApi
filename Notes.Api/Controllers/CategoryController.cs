using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes.Models;
using Notes.Services.interfaces;
using Notes.ViewModels;

namespace Notes.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategories();
            var categoryListVM = categories.Select(x => new CategoryVM()
            {
                Id= x.Id,
                Title = x.Title,
                CreatedOn= x.CreatedOn,
                Description = x.Description
            }).ToList();//converting list of models to list of viewmodels
            return Ok(categoryListVM);
        }

        //get by id
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
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
                    CreatedOn = DateTime.Now
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
                if(existingCategory == null) { return BadRequest("Category could not found"); }
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
    }
}
