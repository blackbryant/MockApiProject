using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Repositories
{
    public interface IPermissionRepository : IGenericRepository<Models.Permission>
    {
        Task<IReadOnlyCollection<Models.Permission>> GetPermissionsByRoleIdAsync(string roleId);

        Task<IReadOnlyCollection<Models.Permission>> GetAllPermissionsAsync();
        
        Task<bool> RoleHasPermissionAsync(string roleId, int permissionId);
        
    }
}