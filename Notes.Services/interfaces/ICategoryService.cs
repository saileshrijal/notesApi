using Notes.Models;
using Notes.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Services.interfaces
{
    public interface ICategoryService
    {
        Task<bool> AddCategory(Category category);
        Task<bool> DeleteCategory(int id);
        Task<bool> UpdateCategory(Category category);
        Task<Category> GetCategoryById(int id);
        Task<List<Category>> GetAllCategories(string userId);
    }
}
