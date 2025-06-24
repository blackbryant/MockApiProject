using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mockAPI.DataContext;
using mockAPI.Services;

namespace mockAPI.Auth
{
    public class DynamicPermissionAttribute : Attribute, IAuthorizationFilter
    {
        



        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var userId = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            // 取得 request method (GET/POST...) 與 path (/api/secret)
             var endpoint = context.HttpContext.GetEndpoint();
            var routePattern = endpoint?.Metadata.GetMetadata<RouteEndpoint>()?.RoutePattern.RawText
                         ?? context.ActionDescriptor.AttributeRouteInfo?.Template;
            var method = context.HttpContext.Request.Method;
            //var path = context.HttpContext.Request.Path.Value?.ToLower();
            var normalizedPath = $"/{routePattern?.TrimStart('/').ToLower()}";

            var requiredPermission = $"{method}:{normalizedPath}";
            Console.WriteLine($"Required Permission: {requiredPermission}");

            // 取得資料庫上下文（需設定為 Scoped，請搭配 DI 注入）
            //var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var userRolesService = context.HttpContext.RequestServices.GetRequiredService<IUserRolesService>();
            var rolePermissionService  = context.HttpContext.RequestServices.GetRequiredService<IRolePermissionService>();

             Console.WriteLine($"userId: {user.Identity}-{userId}");

            List<string> userRoleIds =  userRolesService.GetRoleIds(userId ).Result;
            Console.WriteLine($"User Roles: {string.Join(", ", userRoleIds)}");

            var hasPermission  =  rolePermissionService.RoleHasPermissionAsync(userRoleIds, requiredPermission).Result;

            // 檢查 JWT 中是否包含此權限
            // var hasPermission = user.Claims.Any(c =>
            //     c.Type == "permission" && c.Value.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine($"hasPermission: {hasPermission}");

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}