using System.Text.Json.Serialization;

namespace TDAmeritradeApi
{
    public class Mover
    {
        public decimal Change { get; set; }
        public string Description { get; set; }
        public Direction Direction { get; set; }
        public decimal Last { get; set; }
        public string Symbol { get; set; }
        public long TotalVolume { get; set; }
    }
}