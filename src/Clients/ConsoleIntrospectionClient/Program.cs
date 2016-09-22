using Clients;
using IdentityModel.Client;
using System;
using System.Linq;

namespace ConsoleIntrospectionClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var response = RequestToken();
            Introspection(response.AccessToken);

            Constants.HitEnterToExitPrompt();
        }

        static TokenResponse RequestToken()
        {
            var client = new TokenClient(
                Constants.TokenEndpoint,
                "roclient.reference",
                "secret");

            return client.RequestResourceOwnerPasswordAsync(Constants.UserName, Constants.Password, "api1 api2").Result;
        }

        private static void Introspection(string accessToken)
        {
            var client = new IntrospectionClient(
                Constants.IntrospectionEndpoint,
                "api1",
                "secret");

            var request = new IntrospectionRequest
            {
                Token = accessToken
            };

            var result = client.SendAsync(request).Result;

            if (result.IsError)
            {
                Console.WriteLine(result.Error);
            }
            else
            {
                if (result.IsActive)
                {
                    result.Claims.ToList().ForEach(c => Console.WriteLine("{0}: {1}",
                        c.Type, c.Value));
                }
                else
                {
                    Console.WriteLine("token is not active");
                }
            }
        }
    }
}