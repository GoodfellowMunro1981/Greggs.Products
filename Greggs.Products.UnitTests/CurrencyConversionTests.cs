using Greggs.Products.Api.Conversions;
using Xunit;

namespace Greggs.Products.UnitTests;

public class CurrencyConversionTests
{
    [Theory]
    [InlineData(100, CurrencyConversion.CURRENCY_CODE_EURO, 110)]
    [InlineData(0.7, CurrencyConversion.CURRENCY_CODE_EURO, 0.77)]
    [InlineData(1.95, CurrencyConversion.CURRENCY_CODE_EURO, 2.145)]
    [InlineData(100, CurrencyConversion.CURRENCY_CODE_UNITED_KINGDOM, 100)]
    [InlineData(0.7, CurrencyConversion.CURRENCY_CODE_UNITED_KINGDOM, 0.7)]
    [InlineData(1.95, CurrencyConversion.CURRENCY_CODE_UNITED_KINGDOM, 1.95)]
    public void ConvertPrice_ConvertsCorrectly(decimal price, string currencyCode, decimal expected)
    {
        // Arrange

        // Act
        var result = CurrencyConversion.ConvertPrice(price, currencyCode);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertPrice_ThrowsException_ForUnsupportedCurrency()
    {
        // Arrange
        decimal price = 100;
        string currencyCode = "USD"; // Unsupported currency code

        // Act & Assert
        Assert.Throws
            <System.Exception>(() =>
            {
                CurrencyConversion.ConvertPrice(price, currencyCode);
            });
    }
}