using Fintrack.Contracts.DTOs.User;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace FinTrack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuário e retorna tokens de acesso e atualização.
    /// </summary>
    /// <param name="loginDto">Os dados de login do usuário (email e senha).</param>
    /// <returns>Um objeto contendo o token de acesso, o token de atualização e a data de expiração.</returns>
    /// <response code="200">Retorna os tokens e a data de expiração.</response>
    /// <response code="401">Se as credenciais do usuário forem inválidas.</response>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="registerDto">Os dados para o registro do novo usuário.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="200">Indica que o usuário foi criado com sucesso.</response>
    /// <response code="500">Se o usuário já existir ou se ocorrer um erro durante a criação.</response>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var userExists = await _userManager.FindByEmailAsync(registerDto.Email!);

        if (userExists is not null)
            return StatusCode((int)HttpStatusCode.InternalServerError, "Já existe um usuário cadastrado!");


        User user = new()
        {
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerDto.UserName,
            PhoneNumber = registerDto.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password!);

        _ = await _userManager.AddToRoleAsync(user, "User");

        if (!result.Succeeded)
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro ao criar usuário. \n{result}");

        return StatusCode((int)HttpStatusCode.OK, "Usuário criado com sucesso!");
    }

    /// <summary>
    /// Gera um novo token de acesso a partir de um token de atualização.
    /// </summary>
    /// <param name="tokenDto">O objeto contendo o token de acesso expirado e o token de atualização.</param>
    /// <returns>Um novo token de acesso e um novo token de atualização.</returns>
    /// <response code="200">Retorna o novo token de acesso e o novo token de atualização.</response>
    /// <response code="400">Se a requisição for inválida ou os tokens forem inválidos.</response>
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        if (tokenDto is null)
            return BadRequest("Requisição invalida");

        string? accessToken = tokenDto.AccessToken ?? throw new ArgumentNullException(nameof(tokenDto));

        string? refreshToken = tokenDto.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDto));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal is null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Refresh/Access token inválido.");

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username!);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.UtcNow)
            return StatusCode((int)HttpStatusCode.BadRequest, "Refresh/Access token inválido.");

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga o token de atualização de um usuário. (Requer autorização)
    /// </summary>
    /// <param name="username">O nome de usuário para o qual o token será revogado.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que o token foi revogado com sucesso.</response>
    /// <response code="400">Se o usuário for inválido.</response>
    [Authorize]
    [HttpPost]
    [Route("revoke")]
    public async Task<IActionResult> Revoke([FromQuery] string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return StatusCode((int)HttpStatusCode.BadRequest, "Usuário inválido.");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Cria um novo perfil de usuário (role). (Requer privilégios de Administrador)
    /// </summary>
    /// <param name="roleName">O nome do perfil a ser criado.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="200">Indica que o perfil foi criado com sucesso.</response>
    /// <response code="400">Se o perfil já existir ou se ocorrer um erro.</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole([FromQuery] string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "RolesAdded");
                return StatusCode((int)HttpStatusCode.OK, $"Perfil {roleName} Criado com sucesso!");
            }
            else
            {
                _logger.LogError(2, "Error");
                return StatusCode((int)HttpStatusCode.BadRequest, $"Houve um problema ao criar o perfil {roleName}.");
            }
        }

        return StatusCode((int)HttpStatusCode.BadRequest, $"Perfil já existente.");
    }

    /// <summary>
    /// Adiciona um usuário a um perfil (role). (Requer privilégios de Administrador)
    /// </summary>
    /// <param name="email">O email do usuário.</param>
    /// <param name="roleName">O nome do perfil ao qual o usuário será adicionado.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="200">Indica que o usuário foi adicionado ao perfil com sucesso.</response>
    /// <response code="400">Se o usuário não for encontrado ou se ocorrer um erro.</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole([FromQuery] string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"Usuário {user.Email} foi adicionado ao perfil {roleName}");
                return StatusCode((int)HttpStatusCode.OK, $"Usuário {user.Email} foi adicionado ao perfil {roleName} com sucesso!");
            }
            else
            {
                _logger.LogError(1, $"Error: Erro ao adicionar o usuário {user.Email} ao perfil {roleName}.");
                return StatusCode((int)HttpStatusCode.BadRequest, $"Error: Erro ao adicionar o usuário {user.Email} ao perfil {roleName}.");
            }
        }

        return BadRequest("Usuário não encontrado.");
    }
}