using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Repositories;

namespace mockAPI.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        public GenericService(IGenericRepository<TEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        public Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                return _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while adding the entity.", ex);
            }
        }

        public void Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _repository.Delete(entity);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
            }
        }

        public Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            try
            {
                return _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while retrieving all entities.", ex);
            }
        }

        public Task<TEntity?> GetByIdAsync(object id)
        {
            try
            {
                return _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while retrieving the entity.", ex);
            }
        }

        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _repository.Update(entity);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while updating the entity.", ex);
            }
        }
    }
}