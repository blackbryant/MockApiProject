using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    延伸 UserRoles應用，提供使用者角色相關的服務。

*/

namespace mockAPI.Services
{
    public interface IUserRolesService 
    {
        /// <summary>
        /// 檢查使用者是否具有指定的角色。
        /// </summary>
        /// <param name="userId">使用者 ID</param>
        /// <param name="roleName">角色名稱</param>
        /// <returns>如果使用者具有指定角色，則返回 true；否則返回 false。</returns>
        Task<bool> UserHasRoleAsync(string userId, string roleName);


        Task<List<string>> GetRoleIds(string userId);


    }
}