using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        
        /// <summary>
        /// 檢查使用者是否具有指定的權限。
        /// </summary>
        /// <param name="userId">使用者 ID</param>
        /// <param name="permissionName">權限名稱</param>
        /// <returns>如果使用者具有指定權限，則返回 true；否則返回 false。</returns>
        Task<bool> RoleHasPermissionAsync(List<string> roleIds, string requiredPermission );
 
    }
}