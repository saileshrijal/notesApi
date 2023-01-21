﻿using Microsoft.EntityFrameworkCore;
using Notes.Data;
using System.Linq.Expressions;
using static Notes.Repositories.interfaces.IGenericRepository;

namespace Notes.Repositories.implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public ApplicationDbContext _context;
        private readonly DbSet<T> entities;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }
        virtual public async Task Create(T t)
        {
            await entities.AddAsync(t);
        }

        virtual public async Task Delete(int id)
        {
            var entity = await entities.FindAsync(id);
            if (entity != null)
            {
                entities.Remove(entity);
            }
        }

        virtual public void Edit(T t)
        {
            entities.Update(t);
        }

        virtual public async Task<List<T>> GetAll()
        {
            return await entities.ToListAsync();
        }

        virtual public async Task<T?> GetBy(Expression<Func<T, bool>> predicate)
        {
            return await entities.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAllBy(Expression<Func<T, bool>> predicate)
        {
            return await entities.Where(predicate).ToListAsync();
        }
    }
}
