using System;
using System.Text.Json.Serialization;

namespace TDAmeritradeApi
{
    public class Quote
    {
        public string AssetType { get; set; }
        public string AssetMainType { get; set; }
        public string Cusip { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public decimal BidPrice { get; set; }
        public int BidSize { get; set; }
        public string BidId { get; set; }
        public decimal AskPrice { get; set; }
        public int AskSize { get; set; }
        public string AskId { get; set; }
        public decimal LastPrice { get; set; }
        public int LastSize { get; set; }
        public string LastId { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public string BidTick { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal NetChange { get; set; }
        public decimal TotalVolume { get; set; }
        public long QuoteTimeInLong { get; set; }
        public long TradeTimeInLong { get; set; }
        public decimal Mark { get; set; }
        public string Exchange { get; set; }
        public string ExchangeName { get; set; }
        public bool Marginable { get; set; }
        public bool Shortable { get; set; }
        public float Volatility { get; set; }
        public int Digits { get; set; }

        [JsonPropertyName("52WkHigh")]
        public decimal FiftyTwoWeekHigh { get; set; }

        [JsonPropertyName("52WkLow")]
        public decimal FiftyTwoWeekLow { get; set; }

        [JsonPropertyName("nAV")]
        public int NetAssetValue { get; set; }

        public float PeRatio { get; set; }

        [JsonPropertyName("divAmount")]
        public decimal DividendAmount { get; set; }

        [JsonPropertyName("divYield")]
        public float DividendYield { get; set; }

        [JsonPropertyName("divDate")]
        public DateTimeOffset? DividendDate { get; set; }

        public string SecurityStatus { get; set; }
        public decimal RegularMarketLastPrice { get; set; }
        public int RegularMarketLastSize { get; set; }
        public decimal RegularMarketNetChange { get; set; }
        public long RegularMarketTradeTimeInLong { get; set; }
        public double NetPercentChangeInDouble { get; set; }
        public double MarkChangeInDouble { get; set; }
        public double MarkPercentChangeInDouble { get; set; }
        public double RegularMarketPercentChangeInDouble { get; set; }
        public bool Delayed { get; set; }
    }
}