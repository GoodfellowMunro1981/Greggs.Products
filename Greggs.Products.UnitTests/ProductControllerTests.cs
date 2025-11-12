using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Conversions;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Dtos;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductControllerTests
{
    private readonly Mock<IDataAccess<Product>> _mockDataAccess;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockDataAccess = new Mock<IDataAccess<Product>>();
        var mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockDataAccess.Object, mockLogger.Object);
    }

    [Fact]
    public void Get_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var testProducts = new List<Product>
        {
            new() {
                Name = "Sausage Roll",
                PriceInPounds = 1.0m
            },
            new() {
                Name = "Steak Bake",
                PriceInPounds = 1.2m
            }
        };

        _mockDataAccess.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(0, 5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, products.Count());

        Assert.Equal("Sausage Roll", products.First().Name);
        Assert.Equal(1.0m, products.First().Price);

        Assert.Equal("Steak Bake", products.Last().Name);
        Assert.Equal(1.2m, products.Last().Price);
    }

    [Fact]
    public void Get_WithDefaultParameters_ReturnsFirstFiveProducts()
    {
        // Arrange
        var testProducts = Enumerable.Range(1, 5)
            .Select(i => new Product { Name = $"Product {i}", PriceInPounds = i })
            .ToList();

        _mockDataAccess.Setup(x => x.List(0, 5))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(0, 5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(5, products.Count());

        Assert.Equal("Product 1", products.First().Name);
        Assert.Equal(1.0m, products.First().Price);

        Assert.Equal("Product 5", products.Last().Name);
        Assert.Equal(5.0m, products.Last().Price);
    }

    [Fact]
    public void Get_WithCustomPageStart_ReturnsCorrectProducts()
    {
        // Arrange
        var testProducts = new List<Product>
        {
            new() {
                Name = "Product 6",
                PriceInPounds = 2.5m
            },
            new() {
                Name = "Product 7",
                PriceInPounds = 3.0m
            }
        };

        _mockDataAccess.Setup(x => x.List(1, 5))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(1, 5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, products.Count());

        Assert.Equal("Product 6", products.First().Name);
        Assert.Equal(2.5m, products.First().Price);

        Assert.Equal("Product 7", products.Last().Name);
        Assert.Equal(3.0m, products.Last().Price);
    }

    [Fact]
    public void Get_WithCustomPageSize_ReturnsCorrectNumberOfProducts()
    {
        // Arrange
        var testProducts = Enumerable.Range(1, 10)
            .Select(i => new Product { Name = $"Product {i}", PriceInPounds = i })
            .ToList();

        _mockDataAccess.Setup(x => x.List(0, 10))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(0, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(10, products.Count());

        Assert.Equal("Product 1", products.First().Name);
        Assert.Equal(1.0m, products.First().Price);

        Assert.Equal("Product 10", products.Last().Name);
        Assert.Equal(10.0m, products.Last().Price);
    }

    [Fact]
    public void Get_WhenNoProducts_ReturnsBadRequest()
    {
        // Arrange
        _mockDataAccess.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new List<Product>());

        // Act
        var result = _controller.Get(0, 5);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: No products found for query params", errorMessage);
    }

    [Fact]
    public void Get_VerifiesDataAccessIsCalledWithCorrectParameters()
    {
        // Arrange
        _mockDataAccess.Setup(x => x.List(10, 20))
            .Returns(new List<Product>());

        // Act
        _controller.Get(10, 20);

        // Assert
        _mockDataAccess.Verify(x => x.List(10, 20), Times.Once);
    }

    [Fact]
    public void Get_ReturnsOkResult_WithListOfProductsAndPriceConversion()
    {
        // Arrange
        var testProducts = new List<Product>
        {
            new() {
                Name = "Sausage Roll",
                PriceInPounds = 1.0m
            },
            new() {
                Name = "Steak Bake",
                PriceInPounds = 1.2m
            }
        };

        _mockDataAccess.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(0, 5, CurrencyConversion.CURRENCY_CODE_EURO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, products.Count());

        Assert.Equal("Sausage Roll", products.First().Name);
        Assert.Equal(1.10m, products.First().Price);

        Assert.Equal("Steak Bake", products.Last().Name);
        Assert.Equal(1.32m, products.Last().Price);
    }

    [Fact]
    public void Get_WithDefaultParameters_ReturnsFirstFiveProductsAndPriceConversion()
    {
        // Arrange
        var testProducts = Enumerable.Range(1, 5)
            .Select(i => new Product { Name = $"Product {i}", PriceInPounds = i })
            .ToList();

        _mockDataAccess.Setup(x => x.List(0, 5))
            .Returns(testProducts);

        // Act
        var result = _controller.Get(0, 5, CurrencyConversion.CURRENCY_CODE_EURO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(5, products.Count());

        Assert.Equal("Product 1", products.First().Name);
        Assert.Equal(1.10m, products.First().Price);

        Assert.Equal("Product 5", products.Last().Name);
        Assert.Equal(5.50m, products.Last().Price);
    }


    [Fact]
    public void Get_WithNegativePageStart_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Get(-1, 5);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: pageStart must be >= 0 and pageSize must be > 0", errorMessage);
    }

    [Fact]
    public void Get_WithNegativePageSize_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Get(0, -1);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: pageStart must be >= 0 and pageSize must be > 0", errorMessage);
    }

    [Fact]
    public void Get_WithZeroPageSize_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Get(0, 0);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: pageStart must be >= 0 and pageSize must be > 0", errorMessage);
    }

    [Fact]
    public void Get_WithExcessivePageSize_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Get(0, 1000);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: pageSize must be <= 500", errorMessage);
    }

    [Fact]
    public void Get_WithUnsupportedCurrencyCode_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Get(0, 5, "USD");

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsAssignableFrom<string>(badRequest.Value);
        Assert.Equal("Error: CurrencyCode is not supported", errorMessage);
    }
}