using System;
using System.ComponentModel.DataAnnotations;

namespace mockAPI.Models;

public class RegisterRoleDTO
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public required string Role  { get; set; } = "User"; // 預設角色為 User
   
   
}
