using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mockAPI.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = default!;
        public IdentityRole Role { get; set; } = default!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }
}