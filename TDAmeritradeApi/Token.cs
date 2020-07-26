using System;

namespace TDAmeritradeApi
{
    public class Token
    {
        public string Value { get; }
        public DateTimeOffset Expiration { get; }

        public Token(string value, DateTimeOffset expiration)
        {
            Value = value;
            Expiration = expiration;
        }
    }
}