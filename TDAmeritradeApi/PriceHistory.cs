using System.Collections.Generic;

namespace TDAmeritradeApi
{
    public class PriceHistory
    {
        public IEnumerable<Candle> Candles { get; set; }
        public string Symbol { get; set; }
        public bool Empty { get; set; }
    }
}