using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreApplication.Api.Dtos;
using StoreApplication.Application.Admins;
using StoreApplication.Application.Dtos;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreApplication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<LoginRequestDto> _loginRequestValidator;    
    private readonly IConfiguration _configuration;
    private readonly ILogMessageService _resourceManager;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, IValidator<LoginRequestDto> loginRequestValidator, 
        IConfiguration configuration, ILogMessageService resourceManager, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _loginRequestValidator = loginRequestValidator;        
        _configuration = configuration;
        _resourceManager = resourceManager;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AdminDto>> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        _logger.Log(LogLevel.Information, "Method Login - UserController");

        var validationResult = await _loginRequestValidator.ValidateAsync(loginRequestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(failure => failure.ErrorMessage));
        }

        try
        {
            var adminDto = await _mediator.Send(new GetAdminCredentialsQuery(loginRequestDto.Username, loginRequestDto.Password, loginRequestDto.Rol));

            if (adminDto == null)
            {
                return Unauthorized(_resourceManager.ValidationFailed);
            }

            var token = GenerateJwtToken(new User(
                Id: adminDto.Id,
                UserName: adminDto.Username!,
                PasswordHash: "",
                Role: _resourceManager.User,
                Email: adminDto.Email!));


            return Ok(new { Token = token, Admin = adminDto });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(_resourceManager.ClaimId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var tokenExpirationInHours = int.Parse(_configuration["JwtConfig:JwtTokenExpirationInHours"]!);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:Issuer"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(tokenExpirationInHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
