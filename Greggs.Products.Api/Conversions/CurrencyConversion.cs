using System;

namespace Greggs.Products.Api.Conversions
{
    public static class CurrencyConversion
    {
        public const string CURRENCY_CODE_EURO = "EUR";
        public const string CURRENCY_CODE_UNITED_KINGDOM = "GBP";

        private const decimal GBP_EUR_CONVERSION = 1.1m;

        /// <summary>
        /// currencyCode param as ISO 4217 currency symbols and applies conversion if required
        /// </summary>
        /// <param name="price"></param>
        /// <param name="currencyCode"></param>
        /// <returns>decimal</returns>
        /// <exception cref="Exception"></exception>
        public static decimal ConvertPrice(decimal price, string currencyCode)
        {
            return currencyCode.ToUpperInvariant() switch
            {
                CURRENCY_CODE_EURO => price * GBP_EUR_CONVERSION,
                CURRENCY_CODE_UNITED_KINGDOM => price,
                _ => throw new Exception("ISO 4217 currency symbol not supported"),
            };
        }
    }
}