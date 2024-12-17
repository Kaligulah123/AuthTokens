namespace AuthTokens.Domain.Contracts
{
    public class JwtSetting
    {
        public string? Key { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public double Expires { get; set; }
    }
}
