namespace TDAmeritradeApi
{
    /// <summary>
    /// Provides options for updating refresh and access tokens.
    /// </summary>
    public class TokenUpdateOptions
    {
        /// <summary>
        /// The number of minutes before the access token expires at which to request a new access token.
        /// Defaults to 5.
        /// </summary>
        public int AccessTokenBufferMinutes { get; set; } = 5;

        /// <summary>
        /// The number of minutes before the refresh token expires at which to request a new refresh token.
        /// Defaults to 1440.
        /// </summary>
        public int RefreshTokenBufferMinutes { get; set; } = 1440;
    }
}