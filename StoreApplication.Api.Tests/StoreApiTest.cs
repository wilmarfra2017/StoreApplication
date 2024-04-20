using Microsoft.IdentityModel.Tokens;
using StoreApplication.Application.Products;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StoreApplication.Api.Tests;

public class StoreApiTests
{
    private readonly StoreApiApp _webApp;

    public StoreApiTests()
    {
        _webApp = new StoreApiApp();
    }

    [Fact]
    public async Task PostProduct_ReturnsCreatedResponse()
    {
        // Arrange
        var client = _webApp.CreateClient();

        var token = GenerateJwtToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var newProductCommand = new AddProductCommand(
            name: "Product XYZ",
            description: "Test Description",
            price: 9.99m,
            stock: 100
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/products", newProductCommand);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        var productCreationResponse = JsonSerializer.Deserialize<ProductCreationResponse>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(productCreationResponse);
        Assert.Equal("Product created successfully", productCreationResponse.Message);
        Assert.NotEqual(Guid.Empty, productCreationResponse.OrderId);

    }

    private string GenerateJwtToken()
    {
        var claims = new Claim[]
        {
        new Claim(ClaimTypes.Name, "adminUser"),
        new Claim(ClaimTypes.Role, "Administrator")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bX3sUperS3cretK3y9871234!@#Secure"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "MyApiAuthServer",
            audience: "MyApiClientApps",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class ProductCreationResponse
{
    public string Message { get; set; } = default!;
    public Guid OrderId { get; set; } = default!;
}
