using Notes.Models;
using Notes.Repositories.interfaces;
using Notes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Services.implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.Create(category);
            if(await _unitOfWork.SaveAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            await _unitOfWork.CategoryRepository.Delete(id);
            if(await _unitOfWork.SaveAsync()>0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Category>> GetAllCategories(string userId)
        {
            return await _unitOfWork.CategoryRepository.GetAllBy(x=>x.ApplicationUserId==userId);
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var categories = await _unitOfWork.CategoryRepository.GetBy(x=>x.Id==id);
            return categories!;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _unitOfWork.CategoryRepository.Edit(category);
            if (await _unitOfWork.SaveAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
