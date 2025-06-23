using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context) 
        {
             this._context = context;
        }

        public async Task<Permission> AddAsync(Permission entity)
        {
            _context.Permissions.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> AddPermissionToRoleAsync(string roleId, int permissionId)
        {
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
            _context.Add(rolePermission);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public void Delete(Permission entity)
        {
            _context.Permissions.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IReadOnlyList<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync<Permission>();
        }

        public async Task<IReadOnlyCollection<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(object id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<IReadOnlyCollection<Permission>> GetPermissionsByRoleIdAsync(string roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, int permissionId)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (rolePermission == null)
                return false;
            _context.RolePermissions.Remove(rolePermission);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RoleHasPermissionAsync(string roleId, int permissionId)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public void Update(Permission entity)
        {
            _context.Permissions.Update(entity);
            _context.SaveChanges();
        }
    }
}