using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StoreApplication.Api.Middleware;
using StoreApplication.Api.Validators;
using StoreApplication.Infrastructure.DataSource;
using StoreApplication.Infrastructure.Extensions;
using System.Reflection;
using System.Resources;
using System.Security.Claims;
using System.Text;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

builder.Services.AddDomainServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Ingrese el token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});


builder.Services.AddSingleton<ResourceManager>(new ResourceManager("StoreApplication.Infrastructure.Resources.ErrorMessages", typeof(Program).Assembly));

builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(config.GetConnectionString("db"));
});

builder.Services.AddMediatR(Assembly.Load("StoreApplication.Application"), typeof(Program).Assembly);

builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<CredentialValidatorLogin>();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<CreateValidatorProduct>();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<UpdateValidatorProduct>();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<CreateValidatorPlaceOrder>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtConfig:Key"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = config["JwtConfig:Issuer"],
            ValidAudience = config["JwtConfig:Audience"],
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {                
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { error = "Debes estar autenticado para poder realizar la petición" });

                context.Response.WriteAsync(result);
                context.HandleResponse();

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));
    options.AddPolicy("MustBeUser", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireAssertion(context =>
        context.User.HasClaim(c => (c.Type == ClaimTypes.Role && (c.Value == "Administrator" || c.Value == "User")))));
});


var app = builder.Build();

app.UseCustomExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
