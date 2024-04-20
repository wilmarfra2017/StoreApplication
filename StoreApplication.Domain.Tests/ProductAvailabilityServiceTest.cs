using Moq;
using StoreApplication.Domain.Entities;
using StoreApplication.Domain.Ports;
using StoreApplication.Domain.Services;

namespace StoreApplication.Domain.Tests;

public class ProductAvailabilityServiceTest
{
    [Fact]
    public async Task CheckProductAvailabilityAsync_AllProductsAvailable_NoExceptionThrown()
    {
        // Arrange
        var productRepositoryMock = new Mock<IProductRepository>();
        var productId = Guid.NewGuid();
        var product = new Product("Product 1", "Description 1", 100.00m, 10);
        product.Id = productId;
        productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

        var orderDetails = new List<OrderDetail>
        {
            new OrderDetail { ProductId = productId, Quantity = 1 }
        };

        var service = new ProductAvailabilityService(productRepositoryMock.Object);

        // Act & Assert
        await service.CheckProductAvailabilityAsync(orderDetails);
    }

    [Fact]
    public async Task CheckProductAvailabilityAsync_ProductNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var productRepositoryMock = new Mock<IProductRepository>();
        var productId = Guid.NewGuid();
        var product = new Product("Product 1", "Description 1", 100.00m, 10);
        product.Id = productId;
        productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

        var orderDetails = new List<OrderDetail>
        {
            new OrderDetail { ProductId = productId, Quantity = 11 }
        };

        var service = new ProductAvailabilityService(productRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CheckProductAvailabilityAsync(orderDetails));
    }
}
