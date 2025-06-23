using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _dbContext; // 假設您的 DbContext 名稱為 ApplicationDbContext

        public RolePermissionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RolePermission> AddAsync(RolePermission entity)
        {
            _dbContext.RolePermissions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public void Delete(RolePermission entity)
        {
            _dbContext.RolePermissions.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task<IReadOnlyList<RolePermission>> GetAllAsync()
        {
            return await _dbContext.RolePermissions.ToListAsync();
        }

        public async Task<RolePermission?> GetByIdAsync(object id)
        {
            return await _dbContext.RolePermissions.FindAsync(id);
        }

        public void Update(RolePermission entity)
        {
            _dbContext.RolePermissions.Update(entity);
            _dbContext.SaveChanges();
        }

        public async Task<IReadOnlyCollection<RolePermission>> GetPermissionsByRoleIdAsync(string roleId)
        {
            return await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<bool> AddPermissionToRoleAsync(string roleId, int permissionId)
        {
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
            _dbContext.RolePermissions.Add(rolePermission);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;

        }

    }

   
}