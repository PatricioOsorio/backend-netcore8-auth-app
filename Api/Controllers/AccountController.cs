using Api.Common.Enums;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    // api/account/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = new AppUser
      {
        UserName = $"{registerDto.Names}-app-auth",
        Email = registerDto.Email,
        Names = registerDto.Names,
        PaternalLastName = registerDto.PaternalLastName,
        MothersLastName = registerDto.MothersLastName,
      };

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) return BadRequest(result.Errors);

      if (registerDto.Roles == null)
      {
        await _userManager.AddToRoleAsync(user, RoleConstants.BASIC);
      }
      else
      {
        foreach (var role in registerDto.Roles)
        {
          await _userManager.AddToRoleAsync(user, role.ToUpper());
        }
      }

      return Ok(new AuthRespondeDto
      {
        IsSuccess = true,
        Message = "Usuario creado con éxito"
      });
    }

    // api/account/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = await _userManager.FindByEmailAsync(loginDto.Email);

      if (user == null) return Unauthorized(new AuthRespondeDto { IsSuccess = false, Message = "Correo no encontrado" });

      var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

      // sumer un intento de intento a usuario usando identity

      if (!result)
      {
        await _userManager.AccessFailedAsync(user);
      }

      if (!result) return Unauthorized(new AuthRespondeDto { IsSuccess = false, Message = "Contraseña inválida" });

      var token = await GenerateJwtTokenAsync(user);


      return Ok(new AuthRespondeDto
      {
        Token = token,
        IsSuccess = true,
        Message = "Inicio de sesión exitoso"
      });
    }

    private async Task<string> GenerateJwtTokenAsync(AppUser user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);

      var roles = await _userManager.GetRolesAsync(user);

      var claims = new List<Claim>
      {
        new (JwtRegisteredClaimNames.Email , user.Email!),
        new (JwtRegisteredClaimNames.Name , user.Names),
        new (JwtRegisteredClaimNames.NameId , user.Id),
        new (JwtRegisteredClaimNames.Aud , _configuration["JwtSettings:ValidAudience"]!),
        new (JwtRegisteredClaimNames.Iss , _configuration["JwtSettings:ValidIssuer"]!),
      };


      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

    // api/account/detail
    [HttpGet("detail")]
    [Authorize]
    public async Task<IActionResult> Detail()
    {
      var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (currentUserId == null) return NotFound(new AuthRespondeDto
      {
        IsSuccess = false,
        Message = "Usuario no encontrado"
      });

      var user = await _userManager.FindByIdAsync(currentUserId);

      if (user == null) return NotFound(new AuthRespondeDto
      {
        IsSuccess = false,
        Message = "Usuario no encontrado"
      });

      return Ok(new UserDetailDto
      {
        Id = user.Id,
        Names = user.Names,
        PaternalLastName = user.PaternalLastName,
        MothersLastName = user.MothersLastName,
        Email = user.Email,
        Roles = [.. await _userManager.GetRolesAsync(user)],
        PhoneNumber = user.PhoneNumber,
        IsDoubleFactoEnabled = user.TwoFactorEnabled,
        IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
        AccessFailedCount = user.AccessFailedCount
      });
    }

    // api/account/getUsers
    [HttpGet("getUsers")]
    public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
    {
      var users = await _userManager.Users.ToListAsync();
      var userDetailDtos = new List<UserDetailDto>();

      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        userDetailDtos.Add(new UserDetailDto
        {
          Id = user.Id,
          Names = user.Names,
          PaternalLastName = user.PaternalLastName,
          MothersLastName = user.MothersLastName,
          Email = user.Email,
          Roles = [.. roles]
        });
      }

      return Ok(userDetailDtos);
    }

    // api/account/forgotPassword
    [HttpPost("forgotPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
      var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

      if (user is null) return Ok(new AuthRespondeDto
      {
        IsSuccess = false,
        Message = "El usuario no existe con este email"
      });

      var token = await _userManager.GeneratePasswordResetTokenAsync(user);

      var resetLink = $"{_configuration["Client:ClientUrl"]!}/resetPassword?email={user.Email}&token={WebUtility.UrlEncode(token)}";

      var client = new RestClient("https://send.api.mailtrap.io/api/send");

      var request = new RestRequest()
      {
        Method = Method.Post,
        RequestFormat = DataFormat.Json,
      };

      request.AddHeader("Authorization", "Bearer 8a67a0b53cdc9ca3b0ad98918eb90f3e");
      request.AddJsonBody(new
      {
        from = new { email = "mailtrap@demomailtrap.com", },
        to = new[] { new { email = user.Email } },
        template_uuid = "0af3d8b1-c997-4640-900c-be1b5c80bf21",
        template_variables = new
        {
          user_email = user.Email,
          pass_reset_link = resetLink
        }
      });

      var response = await client.ExecuteAsync(request);

      if (!response.IsSuccessful) return BadRequest(new AuthRespondeDto
      {
        IsSuccess = false,
        Message = "Error al enviar el correo"
        // Message = response.Content!.ToString()
      });

      return Ok(new AuthRespondeDto
      {
        IsSuccess = true,
        Message = resetLink
      });
    }
  }
}
