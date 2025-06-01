using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Services
{
    public interface IGenericService<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(object id);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}