using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Repositories.interfaces
{
    public interface IGenericRepository
    {
        public interface IGenericRepository<T> where T : class
        {
            Task<List<T>> GetAll();
            Task<List<T>> GetAllBy(Expression<Func<T, bool>> predicate);
            Task Create(T t);
            public Task Delete(int id);
            void Edit(T t);
            public Task<T?> GetBy(Expression<Func<T, bool>> predicate);
        }
    }
}
