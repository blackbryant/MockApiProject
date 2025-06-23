using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mockAPI.Auth
{
    public class DynamicPermissionAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
             var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 取得 request method (GET/POST...) 與 path (/api/secret)
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path.Value?.ToLower();

            var requiredPermission = $"{method}:{path}";
            Console.WriteLine($"Required Permission: {requiredPermission}");

            // 檢查 JWT 中是否包含此權限
            var hasPermission = user.Claims.Any(c =>
                c.Type == "permission" && c.Value.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine($"hasPermission: {hasPermission}");

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}