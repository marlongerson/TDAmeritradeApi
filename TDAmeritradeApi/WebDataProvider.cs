using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace TDAmeritradeApi
{
    internal class WebDataProvider : IDataProvider
    {
        private readonly string _apiKey;

        public WebDataProvider(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> PostAccessTokenAsync(string refreshToken, bool withRefreshToken = true)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest("oauth2/token");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", $"{_apiKey}@AMER.OAUTHAP");
            request.AddParameter("refresh_token", refreshToken);

            if (withRefreshToken)
            {
                request.AddParameter("access_type", "offline");
            }

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetQuoteJsonAsync(string symbol)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{symbol}/quotes");
            request.AddParameter("apikey", _apiKey);

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetQuoteJsonAsync(string symbol, string accessToken)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{symbol}/quotes");
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetQuotesJsonAsync(IEnumerable<string> symbols)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{string.Join(',', symbols)}/quotes");
            request.AddParameter("apikey", _apiKey);

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetQuotesJsonAsync(IEnumerable<string> symbols, string accessToken)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{string.Join(',', symbols)}/quotes");
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetMoversJsonAsync(Index index, Direction direction, ChangeType changeType)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{GetIndexString(index)}/movers");
            request.AddParameter("apikey", _apiKey);
            request.AddParameter("direction", GetDirectionString(direction));
            request.AddParameter("change", GetChangeTypeString(changeType));

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetMoversJsonAsync(Index index, Direction direction, ChangeType changeType, string accessToken)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{GetIndexString(index)}/movers");
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddParameter("direction", GetDirectionString(direction));
            request.AddParameter("change", GetChangeTypeString(changeType));

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetPriceHistoryJsonAsync(string symbol, PeriodType periodType, int period, FrequencyType frequencyType, int frequency)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{symbol}/pricehistory");
            request.AddParameter("apikey", _apiKey);
            request.AddParameter("periodType", GetPeriodTypeString(periodType));
            request.AddParameter("period", period);
            request.AddParameter("frequencyType", GetFrequencyTypeString(frequencyType));
            request.AddParameter("frequency", frequency);

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string> GetPriceHistoryJsonAsync(string symbol, PeriodType periodType, int period, FrequencyType frequencyType, int frequency, string accessToken)
        {
            var client = new RestClient("https://api.tdameritrade.com/v1");

            var request = new RestRequest($"marketdata/{symbol}/pricehistory");
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddParameter("periodType", GetPeriodTypeString(periodType));
            request.AddParameter("period", period);
            request.AddParameter("frequencyType", GetFrequencyTypeString(frequencyType));
            request.AddParameter("frequency", frequency);

            var response = await client.ExecutePostAsync(request);
            return response.Content;
        }

        private string GetIndexString(Index index) =>
            index switch
            {
                Index.COMPX => "$COMPX",
                Index.DJI => "$DJI",
                Index.SPX => "$SPX.X",
                _ => throw new InvalidEnumArgumentException(nameof(index))
            };

        private string GetDirectionString(Direction direction) =>
            direction switch
            {
                Direction.Up => "up",
                Direction.Down => "down",
                _ => throw new InvalidEnumArgumentException(nameof(direction))
            };

        private string GetChangeTypeString(ChangeType changeType) =>
            changeType switch
            {
                ChangeType.Value => "value",
                ChangeType.Percent => "percent",
                _ => throw new InvalidEnumArgumentException(nameof(changeType))
            };

        private string GetPeriodTypeString(PeriodType periodType) =>
            periodType switch
            {
                PeriodType.Day => "day",
                PeriodType.Month => "month",
                PeriodType.Year => "year",
                PeriodType.YearToDate => "ytd",
                _ => throw new InvalidEnumArgumentException(nameof(periodType))
            };

        private string GetFrequencyTypeString(FrequencyType frequencyType) =>
            frequencyType switch
            {
                FrequencyType.Minute => "minute",
                FrequencyType.Day => "daily",
                FrequencyType.Week => "weekly",
                FrequencyType.Month => "monthly",
                _ => throw new InvalidEnumArgumentException(nameof(frequencyType))
            };
    }
}