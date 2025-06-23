using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;
using mockAPI.Repositories;

namespace mockAPI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repository;

        public PermissionService(IPermissionRepository repository)
        {
            _repository = repository;
        }

        public Task<Permission> AddAsync(Permission entity)
        {
            return _repository.AddAsync(entity);
        }

        public void Delete(Permission entity)
        {
            _repository.Delete(entity);
        }

        public Task<IReadOnlyList<Permission>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Permission?> GetByIdAsync(object id)
        {
            return _repository.GetByIdAsync(id);
        }

        public void Update(Permission entity)
        {
            _repository.Update(entity);
        }
    }
    
}