using Clients;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public class Program
    {
        static TokenClient _tokenClient;

        static void Main(string[] args)
        {
            _tokenClient = new TokenClient(
                Constants.TokenEndpoint,
                "roclient",
                "secret");

            var response = RequestToken();
            ShowResponse(response);

            Constants.HitEnterToContinuePrompt();

            var refresh_token = response.RefreshToken;

            while (true)
            {
                response = RefreshToken(refresh_token);
                ShowResponse(response);

                Constants.HitEnterToContinuePrompt();

                CallService(response.AccessToken);

                if (response.RefreshToken != refresh_token)
                {
                    refresh_token = response.RefreshToken;
                }
            }
        }

        static TokenResponse RequestToken()
        {
            return _tokenClient.RequestResourceOwnerPasswordAsync
                (Constants.UserName, Constants.Password, "api1 api2 offline_access").Result;
        }

        private static TokenResponse RefreshToken(string refreshToken)
        {
            Console.WriteLine("Using refresh token: {0}", refreshToken);

            return _tokenClient.RequestRefreshTokenAsync(refreshToken).Result;
        }

        static void CallService(string token)
        {
            var baseAddress = Constants.AspNetWebApiSampleApi;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = client.GetStringAsync("identity").Result;

            "\n\nService claims:".ConsoleGreen();
            Console.WriteLine(JArray.Parse(response));
        }

        private static void ShowResponse(TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);

                if (response.AccessToken.Contains("."))
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];

                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.ErrorType == TokenResponse.ResponseErrorType.Http)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.Error);
                    "HTTP status code: ".ConsoleGreen();
                    Console.WriteLine(response.HttpStatusCode);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Json);
                }
            }
        }
    }
}