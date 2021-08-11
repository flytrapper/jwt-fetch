using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace JwtFetch
{
    public class JwtAccess
    {
        public JwtAccess(ISettings settings)
        {
            Settings = settings;
            Client = new HttpClient();
        }

        private ISettings Settings { get; set; }
        private HttpClient Client { get; set; }
        private JwtSecurityToken Token { get; set; }

        public async Task<JwtSecurityToken> GetValidTokenAsync()
        {
            if (!IsValidToken())
            {
                var readSuccessful = await ReadPersistedTokenAsync(Settings.PersistedLocation);

                if (readSuccessful && IsValidToken())
                {
                    return Token;
                }

                bool requestSuccessful = await RequestNewTokenAsync();

                if (requestSuccessful && IsValidToken())
                {
                    await PersistTokenAsync(Settings.PersistedLocation);
                    return Token;
                }

                Token = null;
            }

            return Token;
        }

        private async Task<bool> ReadPersistedTokenAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            using StreamReader reader = new(filePath);
            string rawToken = await reader.ReadToEndAsync();
            Token = new JwtSecurityToken(rawToken);

            return true;
        }

        private async Task PersistTokenAsync(string filePath)
        {
            using StreamWriter writer = new(filePath);
            await writer.WriteAsync(Token.RawData);
        }

        private async Task<bool> RequestNewTokenAsync()
        {
            var body = new JwtRequest
            {
                ClientId = Settings.ClientId,
                ClientSecret = Settings.ClientSecret,
                Audience = Settings.Audience
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{Settings.Authority}/oauth/token")
            {
                Content = JsonContent.Create(body, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json))
            };
            var response = await Client.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var jwt = JsonSerializer.Deserialize<JwtResponse>(await response.Content.ReadAsStringAsync());
            Token = new JwtSecurityToken(jwt.AccessToken);

            return true;
        }

        private bool IsValidToken()
        {
            return
                Token != null &&
                Token.ValidFrom <= DateTime.Now &&
                Token.ValidTo >= DateTime.Now.AddSeconds(-1);
        }
    }
}
