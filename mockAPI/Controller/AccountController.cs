using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using mockAPI.Models;
using mockAPI.Middleware.Setting;
using Microsoft.Extensions.Options;

namespace mockAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IOptions<JwtSettings> jwtSettings;

        public AccountController(UserManager<IdentityUser> userManager,  RoleManager<IdentityRole> roleManager, IOptions<JwtSettings> jwtSettings)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this.jwtSettings = jwtSettings;
            Console.WriteLine($"AccountControllerAA {jwtSettings.Value.Key} {jwtSettings.Value.Issuer} {jwtSettings.Value.Audience} {jwtSettings.Value.ExpirationInMinutes}");
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


           var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "註冊成功" });
            }
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("AddRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRole([FromBody] RegisterRoleDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            
            if (user == null)
                return NotFound(new { message = "找不到用戶" });

            if (string.IsNullOrEmpty(model.Role))
                return BadRequest(new { message = "角色名稱不能為空" });
                
            Console.WriteLine($"Adding role {model.Role} to user {user.UserName}");

            // 確保角色存在
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Role));
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors.Select(e => e.Description));
                }
            }

            if (user != null && !await _userManager.IsInRoleAsync(user, model.Role))
            {
                var result = await _userManager.AddToRoleAsync(user, model.Role);
                if (result.Succeeded)
                    return Ok(new { message = "角色添加成功" });
            }
           

            return BadRequest( new { message = "角色添加失敗" });
        }




        [HttpDelete("delete/{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { message = "找不到用戶" });
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok(new { message = "刪除成功" });

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDTO  model)
        {
             
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
             
            Console.WriteLine($"Login attempt for user: {model.Email}");
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = GenerateJwtToken(user);

                Console.WriteLine($"token: {token.Result}");
                return Ok(new { token });
                
            }

            return Unauthorized(new { message = "用戶名或密碼錯誤" });
        }



        [NonAction]
        public async Task<string> GenerateJwtToken(IdentityUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (jwtSettings.Value == null)
                throw new ArgumentNullException(nameof(jwtSettings));

            if(string.IsNullOrEmpty(user.UserName))
             
            {
                throw new ArgumentException("User does not have a valid username");
            }

            // Create claims for the JWT token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            // Add user roles to claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                Console.WriteLine($"User {user.UserName} has role: {role}");
            }

            claims.Add(new Claim(ClaimTypes.Name, user.UserName??string.Empty));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(jwtSettings.Value.ExpirationInMinutes);
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
