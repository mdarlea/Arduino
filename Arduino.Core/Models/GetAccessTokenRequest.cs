namespace Arduino.Core.Models
{
    public class GetAccessTokenRequest
    {
        public int TokenExpirationInMinutes { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
