using System;

namespace TDAmeritradeApi
{
    public class Candle
    {
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public long Volume { get; set; }
        public DateTimeOffset DateTime { get; set; }
    }
}