using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Repositories
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        private readonly DbventaContext _dbContext;

        public GenericRepository(DbventaContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TModel> Get(Expression<Func<TModel, bool>> predicate)
        {
            try
            {
                TModel model = await _dbContext.Set<TModel>().FirstOrDefaultAsync(predicate);
                return model;
            } catch {
                throw;
            }
        }

        public async Task<TModel> Create(TModel model)
        {
            try
            {
                _dbContext.Set<TModel>().Add(model);
                await _dbContext.SaveChangesAsync();
                return model;
            } catch {
                throw;
            }
        }

        public async Task<bool> Edit(TModel model)
        {
            try
            {
                _dbContext.Set<TModel>().Update(model);
                await _dbContext.SaveChangesAsync();
                return true;
            } catch {
                throw;
            }
        }

        public async Task<bool> Delete(TModel model)
        {
            try
            {
                _dbContext.Set<TModel>().Remove(model);
                await _dbContext.SaveChangesAsync();
                return true;
            } catch {
                throw;
            }
        }

        public async Task<IQueryable<TModel>> Consult(Expression<Func<TModel, bool>> predicate = null)
        {
            try
            {
                IQueryable<TModel> queryModel = predicate == null ? _dbContext.Set<TModel>(): _dbContext.Set<TModel>().Where(predicate);
                return queryModel;
            } catch {
                throw;
            }
        }

    }
}
