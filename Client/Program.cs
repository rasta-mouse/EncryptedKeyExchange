using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal static class Program
    {
        private static Crypto _crypto;
        private static HttpClient _httpClient;

        private static async Task Main(string[] args)
        {
            _crypto = new Crypto();
            _httpClient = GetHttpClient();

            await ExecuteStage1();
            await ExecuteStage2();
            await ExecuteStage3();
        }

        private static HttpClient GetHttpClient()
        {
            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri("http://localhost:8080");
            httpClient.DefaultRequestHeaders.Add("ClientGuid", $"{Guid.NewGuid()}");

            return httpClient;
        }

        /// <summary>
        /// Send the client's public key.
        /// Returns the server's public key.
        /// </summary>
        private static async Task ExecuteStage1()
        {
            var content = new StringContent(_crypto.ClientPublic, Encoding.UTF8, "application/xml");
            var response = await _httpClient.PostAsync("/stage1", content);
            var serverKey = await response.Content.ReadAsStringAsync();
            
            _crypto.AddServerKey(serverKey);
        }

        /// <summary>
        /// Generate a random challenge and encrypt with the server's public key.
        /// Returns a new session key encrypted with this client's public key.
        /// </summary>
        private static async Task ExecuteStage2()
        {
            var challenge = _crypto.GetRandomData(128);
            _crypto.ClientChallenge = challenge;
            
            var encryptedChallenge = _crypto.EncryptData(challenge);
            var content = new ByteArrayContent(encryptedChallenge);
            var response = await _httpClient.PostAsync("/stage2", content);
            var encryptedSessionKey = await response.Content.ReadAsByteArrayAsync();
            var sessionKey = _crypto.DecryptData(encryptedSessionKey);
            
            _crypto.SessionKey = sessionKey;
        }

        /// <summary>
        /// Send a request to the server and retrieve the original challenge
        /// Encrypted with the new session key
        /// </summary>
        private static async Task ExecuteStage3()
        {
            var response = await _httpClient.GetAsync("/stage3");
            var encryptedChallenge = await response.Content.ReadAsByteArrayAsync();
            var challenge = _crypto.DecryptData2(encryptedChallenge);

            Console.WriteLine(_crypto.ClientChallenge.SequenceEqual(challenge)
                ? "SUCCESS"
                : "FAIL");
        }
    }
}