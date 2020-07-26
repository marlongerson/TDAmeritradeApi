using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TDAmeritradeApi;
using Xunit;

namespace TDAmeritradeApiTests
{
    public class ApiClientTests
    {
        [Fact]
        public async Task GetQuote_GivenSymbol_DeserializesResponseSuccessfully()
        {
            var json = await File.ReadAllTextAsync(Path.Combine("Responses", "QuoteResponse.json"));

            var dataProvider = new Mock<IDataProvider>();
            dataProvider.Setup(dp => dp.GetQuoteJsonAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(json);

            var sut = new ApiClient(dataProvider.Object);

            var result = await sut.GetQuoteAsync("MSFT");

            Assert.Equal("EQUITY", result.AssetType);
            Assert.Equal("EQUITY", result.AssetMainType);
            Assert.Equal("594918104", result.Cusip);
            Assert.Equal("MSFT", result.Symbol);
            Assert.Equal("Microsoft Corporation - Common Stock", result.Description);
            Assert.Equal(200.85m, result.BidPrice);
            Assert.Equal(700, result.BidSize);
            Assert.Equal("P", result.BidId);
            Assert.Equal(200.95m, result.AskPrice);
            Assert.Equal(100, result.AskSize);
            Assert.Equal("K", result.AskId);
            Assert.Equal(200.95m, result.LastPrice);
            Assert.Equal(0, result.LastSize);
            Assert.Equal("K", result.LastId);
            Assert.Equal(200.42m, result.OpenPrice);
            Assert.Equal(202.86m, result.HighPrice);
            Assert.Equal(197.51m, result.LowPrice);
            Assert.Equal(" ", result.BidTick);
            Assert.Equal(202.54m, result.ClosePrice);
            Assert.Equal(-1.59m, result.NetChange);
            Assert.Equal(39826989, result.TotalVolume);
            Assert.Equal(1595635194280, result.QuoteTimeInLong);
            Assert.Equal(1595635194280, result.TradeTimeInLong);
            Assert.Equal(200.95m, result.Mark);
            Assert.Equal("q", result.Exchange);
            Assert.Equal("NASD", result.ExchangeName);
            Assert.True(result.Marginable);
            Assert.False(result.Shortable);
            Assert.Equal(0.0221, result.Volatility, 3);
            Assert.Equal(4, result.Digits);
            Assert.Equal(216.38m, result.FiftyTwoWeekHigh);
            Assert.Equal(130.78m, result.FiftyTwoWeekLow);
            Assert.Equal(0, result.NetAssetValue);
            Assert.Equal(36.7315, result.PeRatio, 4);
            Assert.Equal(2.04m, result.DividendAmount);
            Assert.Equal(1.01, result.DividendYield, 2);
            Assert.Equal(new DateTime(2020, 8, 19), result.DividendDate);
            Assert.Equal("Normal", result.SecurityStatus);
            Assert.Equal(201.30m, result.RegularMarketLastPrice);
            Assert.Equal(18138, result.RegularMarketLastSize);
            Assert.Equal(-1.24m, result.RegularMarketNetChange);
            Assert.Equal(1595620800413, result.RegularMarketTradeTimeInLong);
            Assert.Equal(-0.785, result.NetPercentChangeInDouble, 3);
            Assert.Equal(-1.59, result.MarkChangeInDouble);
            Assert.Equal(-0.785, result.MarkPercentChangeInDouble);
            Assert.Equal(-0.6122, result.RegularMarketPercentChangeInDouble);
            Assert.True(result.Delayed);
        }

        [Fact]
        public async Task GetQuotes_GivenSymbols_DeserializesResponseSuccessfully()
        {
            var json = await File.ReadAllTextAsync(Path.Combine("Responses", "QuotesResponse.json"));
            var symbols = new[] { "AAPL", "AMZN", "TSLA" };

            var dataProvider = new Mock<IDataProvider>();
            dataProvider.Setup(dp => dp.GetQuotesJsonAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>())).ReturnsAsync(json);

            var sut = new ApiClient(dataProvider.Object);

            var result = await sut.GetQuotesAsync(symbols);

            var apple = result["AAPL"];
            var amazon = result["AMZN"];
            var tesla = result["TSLA"];

            Assert.Equal("AAPL", apple.Symbol);
            Assert.Equal("037833100", apple.Cusip);
            Assert.Equal("AMZN", amazon.Symbol);
            Assert.Equal("023135106", amazon.Cusip);
            Assert.Equal("TSLA", tesla.Symbol);
            Assert.Equal("88160R101", tesla.Cusip);
        }

        [Fact]
        public async Task GetPriceHistoryAsync_GivenAuthenticatedArgs_DeserializesResponseSuccessfully()
        {
            var json = await File.ReadAllTextAsync(Path.Combine("Responses", "PriceHistoryResponse.json"));

            var dataProvider = new Mock<IDataProvider>();
            dataProvider.Setup(dp => dp.GetPriceHistoryJsonAsync(
                    It.IsAny<string>(),
                    It.IsAny<PeriodType>(),
                    It.IsAny<int>(),
                    It.IsAny<FrequencyType>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
                .ReturnsAsync(json);

            var sut = new ApiClient(dataProvider.Object);

            var result = await sut.GetPriceHistoryAsync("MSFT", PeriodType.Day, 1, FrequencyType.Minute, 5);

            Assert.Equal(3, result.Candles.Count());
            Assert.Equal("MSFT", result.Symbol);
            Assert.False(result.Empty);

            var candle1 = result.Candles.ElementAt(0);
            var candle2 = result.Candles.ElementAt(1);
            var candle3 = result.Candles.ElementAt(2);

            Assert.Equal(new DateTimeOffset(2020, 7, 20, 11, 0, 0, TimeSpan.Zero), candle1.DateTime);
            Assert.Equal(204.49m, candle1.Open);
            Assert.Equal(204.50m, candle1.High);
            Assert.Equal(204.05m, candle1.Low);
            Assert.Equal(204.23m, candle1.Close);
            Assert.Equal(4430, candle1.Volume);

            Assert.Equal(new DateTimeOffset(2020, 7, 20, 11, 1, 0, TimeSpan.Zero), candle2.DateTime);
            Assert.Equal(204.20m, candle2.Open);
            Assert.Equal(204.35m, candle2.High);
            Assert.Equal(204.20m, candle2.Low);
            Assert.Equal(204.30m, candle2.Close);
            Assert.Equal(2771, candle2.Volume);

            Assert.Equal(new DateTimeOffset(2020, 7, 20, 11, 2, 0, TimeSpan.Zero), candle3.DateTime);
            Assert.Equal(204.27m, candle3.Open);
            Assert.Equal(204.27m, candle3.High);
            Assert.Equal(204.20m, candle3.Low);
            Assert.Equal(204.21m, candle3.Close);
            Assert.Equal(5190, candle3.Volume);
        }
    }
}