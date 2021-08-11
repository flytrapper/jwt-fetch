namespace JwtFetch
{
    public interface ISettings
    {
        string PersistedLocation { get; set; }
        string Authority { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Audience { get; set; }
    }

    public class Settings : ISettings
    {
        public string PersistedLocation { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
    }
}
