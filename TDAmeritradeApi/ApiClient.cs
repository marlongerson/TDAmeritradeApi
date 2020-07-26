using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Threading.Tasks;

namespace TDAmeritradeApi
{
    /// <summary>
    /// Provides methods for making requests to the TD Ameritrade Developer API.
    /// </summary>
    public class ApiClient
    {
        private readonly TokenUpdateOptions _tokenUpdateOptions;
        private readonly IDataProvider _dataProvider;

        private Token _refreshToken;
        private Token _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The API key.
        /// </param>
        /// <param name="refreshToken">
        /// The initial refresh token.
        /// </param>
        /// <param name="refreshTokenExpiration">
        /// The expiration date of the refresh token.
        /// </param>
        /// <param name="tokenUpdateOptions">
        /// The options for updating refresh and access tokens.
        /// </param>
        public ApiClient(string apiKey, string refreshToken, DateTimeOffset refreshTokenExpiration, TokenUpdateOptions tokenUpdateOptions)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("Refresh token is null or empty.", nameof(refreshToken));
            }

            if (DateTimeOffset.Now > refreshTokenExpiration)
            {
                throw new ArgumentException("Refresh token is expired.", nameof(refreshTokenExpiration));
            }

            _dataProvider = new WebDataProvider(apiKey);
            _tokenUpdateOptions = tokenUpdateOptions;
            _refreshToken = new Token(refreshToken, refreshTokenExpiration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The API key.
        /// </param>
        /// <param name="refreshToken">
        /// The initial refresh token.
        /// </param>
        /// <param name="refreshTokenExpiration">
        /// The expiration date of the refresh token.
        /// </param>
        public ApiClient(string apiKey, string refreshToken, DateTimeOffset refreshTokenExpiration)
            : this(apiKey, refreshToken, refreshTokenExpiration, new TokenUpdateOptions())
        {
        }

        internal ApiClient(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _tokenUpdateOptions = new TokenUpdateOptions();

            _refreshToken = new Token(string.Empty, DateTimeOffset.MaxValue);
            _accessToken = new Token(string.Empty, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Gets the current refresh token.
        /// </summary>
        public Token RefreshToken => _refreshToken;

        /// <summary>
        /// Gets the current access token.
        /// </summary>
        public Token AccessToken => _accessToken;

        /// <summary>
        /// Gets a quote for a symbol.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to get the quote for.
        /// </param>
        /// <returns>
        /// A <see cref="TDAmeritradeApi.Quote"/> object containing the quote data.
        /// </returns>
        public async Task<Quote> GetQuoteAsync(string symbol)
        {
            ThrowIfRefreshTokenExpired();

            await TryUpdateTokensAsync();

            var json = await _dataProvider.GetQuoteJsonAsync(symbol, _accessToken.Value);

            return DeserializeQuoteJson(json, symbol);
        }

        /// <summary>
        /// Gets a quote for one or more symbols.
        /// </summary>
        /// <param name="symbols">
        /// The symbols to get quotes for.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{String, Quote}"/> that contains the quotes for the specified symbols.
        /// </returns>
        public async Task<Dictionary<string, Quote>> GetQuotesAsync(IEnumerable<string> symbols)
        {
            ThrowIfRefreshTokenExpired();

            await TryUpdateTokensAsync();

            var json = await _dataProvider.GetQuotesJsonAsync(symbols, _accessToken.Value);

            return DeserializeQuotesJson(json);
        }

        /// <summary>
        /// Gets the top 10 movers for a specified market.
        /// </summary>
        /// <param name="index">
        /// The index of which to get the top 10 movers.
        /// </param>
        /// <param name="direction">
        /// The direction to get the top 10 movers.
        /// </param>
        /// <param name="changeType">
        /// A value that specifies the format of the change value.
        /// </param>
        /// <returns>
        /// The top 10 movers for a specified market.
        /// </returns>
        public async Task<IEnumerable<Mover>> GetMoversAsync(Index index, Direction direction, ChangeType changeType)
        {
            ThrowIfRefreshTokenExpired();

            await TryUpdateTokensAsync();

            var json = await _dataProvider.GetMoversJsonAsync(index, direction, changeType, _accessToken.Value);

            var movers = JsonSerializer.Deserialize<IEnumerable<Mover>>(json);

            return movers;
        }

        /// <summary>
        /// Gets price history for a symbol.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to get price history for.
        /// </param>
        /// <param name="periodType">
        /// The type of period to show.
        /// </param>
        /// <param name="period">
        /// The number of periods to show.
        /// </param>
        /// <param name="frequencyType">
        /// The type of frequency with which a new candle is formed.
        /// </param>
        /// <param name="frequency">
        /// The number of the <paramref name="frequencyType"/> to be included in each candle.
        /// </param>
        /// <returns>
        /// The price history for a symbol.
        /// </returns>
        public async Task<PriceHistory> GetPriceHistoryAsync(string symbol, PeriodType periodType, int period, FrequencyType frequencyType, int frequency)
        {
            ThrowIfRefreshTokenExpired();

            await TryUpdateTokensAsync();

            var json = await _dataProvider.GetPriceHistoryJsonAsync(symbol, periodType, period, frequencyType, frequency, _accessToken.Value);

            return DeserializePriceHistoryJson(json);
        }

        private Quote DeserializeQuoteJson(string json, string symbol)
        {
            var dict = DeserializeQuotesJson(json);

            return dict[symbol.ToUpperInvariant()];
        }

        private Dictionary<string, Quote> DeserializeQuotesJson(string json)
        {
            var quoteDictionary = JsonSerializer.Deserialize<Dictionary<string, Quote>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new NullableDateTimeOffsetConverter()
                }
            });

            return quoteDictionary;
        }

        private PriceHistory DeserializePriceHistoryJson(string json)
        {
            var priceHistory = JsonSerializer.Deserialize<PriceHistory>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new MillisecondDateTimeOffsetConverter()
                }
            });

            return priceHistory;
        }

        private bool ShouldUpdateRefreshToken() =>
            DateTimeOffset.Now > RefreshToken.Expiration.Subtract(TimeSpan.FromMinutes(_tokenUpdateOptions.RefreshTokenBufferMinutes));

        private bool ShouldUpdateAccessToken() =>
            DateTimeOffset.Now > AccessToken.Expiration.Subtract(TimeSpan.FromMinutes(_tokenUpdateOptions.AccessTokenBufferMinutes));

        private async Task TryUpdateTokensAsync()
        {
            if (ShouldUpdateRefreshToken())
            {
                var json = await _dataProvider.PostAccessTokenAsync(_refreshToken.Value, true);
                var response = JsonSerializer.Deserialize<EASObject>(json);

                _accessToken = new Token(response.AccessToken, DateTimeOffset.Now.AddMinutes(response.ExpiresIn));
                _refreshToken = new Token(response.RefreshToken, DateTimeOffset.Now.AddMinutes(response.RefreshTokenExpiresIn));
            }
            else if (ShouldUpdateAccessToken())
            {
                var json = await _dataProvider.PostAccessTokenAsync(_refreshToken.Value, false);
                var response = JsonSerializer.Deserialize<EASObject>(json);

                _accessToken = new Token(response.AccessToken, DateTimeOffset.Now.AddMinutes(response.ExpiresIn));
                _refreshToken = new Token(response.RefreshToken, DateTimeOffset.Now.AddMinutes(response.RefreshTokenExpiresIn));
            }
        }

        private void ThrowIfRefreshTokenExpired()
        {
            if (DateTimeOffset.Now > _refreshToken.Expiration)
            {
                throw new InvalidOperationException("The current refresh token is expired.");
            }
        }
    }
}