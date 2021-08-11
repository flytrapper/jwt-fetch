using System;

namespace JwtFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("JwtFetch <file path to persist> <authority> <clientid> <clientsecret> <audience>");
                return;
            }

            ISettings settings = new Settings
            {
                PersistedLocation = args[0],
                Authority = args[1],
                ClientId = args[2],
                ClientSecret = args[3],
                Audience = args[4]
            };

            var jwtAccess = new JwtAccess(settings);
            var token = jwtAccess.GetValidTokenAsync();
            token.Wait();
            Console.WriteLine(token.Result.RawData);
        }
    }
}
