using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;

namespace mockAPI.Services
{
    public class UserRolesService : IUserRolesService
    {
        private readonly AppDbContext _dbContext;

        public UserRolesService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UserHasRoleAsync(string userId, string roleName)
        {
            var userRoleIds = await _dbContext.UserRoles
                            .Where(ur => ur.UserId == userId)
                            .Select(ur => ur.RoleId)
                            .ToListAsync();

            if (userRoleIds == null || !userRoleIds.Any())
                return false;

            // 檢查是否有任何角色名稱符合指定的 roleName
            
            var roleIds =await _dbContext.Roles
                            .Where(r => r.Name == roleName)
                            .Select(r => r.Id)
                            .ToListAsync();

            var hasRole = roleIds.Any(roleId => userRoleIds.Contains(roleId));


            return hasRole;   
        }

   
        public async Task<List<string>> GetRoleIds(string userId)
        {
            var roleIds = await _dbContext.UserRoles
                  .Where(ur => ur.UserId == userId)
                  .Select(ur => ur.RoleId)
                  .ToListAsync();

            return roleIds;
        }

        
    }
}