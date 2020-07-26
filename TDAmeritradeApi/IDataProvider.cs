using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TDAmeritradeApi
{
    internal interface IDataProvider
    {
        Task<string> PostAccessTokenAsync(string refreshToken, bool withRefreshToken = true);
        Task<string> GetQuoteJsonAsync(string symbol, string accessToken);
        Task<string> GetQuotesJsonAsync(IEnumerable<string> symbols, string accessToken);
        Task<string> GetMoversJsonAsync(Index index, Direction direction, ChangeType changeType, string accessToken);
        Task<string> GetPriceHistoryJsonAsync(string symbol, PeriodType periodType, int period, FrequencyType frequencyType, int frequency, string accessToken);
    }
}