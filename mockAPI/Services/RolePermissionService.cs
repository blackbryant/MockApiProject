using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;
using mockAPI.Repositories;

namespace mockAPI.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repository;

        public RolePermissionService(IRolePermissionRepository repository)
        {
            _repository = repository;
        }

        public Task<RolePermission> AddAsync(RolePermission entity)
        {
            return _repository.AddAsync(entity);
        }

        public void Delete(RolePermission entity)
        {
            _repository.Delete(entity);
        }

        public Task<IReadOnlyList<RolePermission>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<RolePermission?> GetByIdAsync(object id)
        {
            return _repository.GetByIdAsync(id);
        }

        public void Update(RolePermission entity)
        {
            _repository.Update(entity);
        }
    }
}