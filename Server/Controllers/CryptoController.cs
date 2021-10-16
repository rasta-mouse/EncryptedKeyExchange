using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Server.Interfaces;

namespace Server.Controllers
{
    [Controller]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        public async Task<IActionResult> Stage1()
        {
            HttpContext.Request.Headers.TryGetValue("ClientGuid", out var clientGuid);

            using var sr = new StreamReader(HttpContext.Request.Body);
            var clientKey = await sr.ReadToEndAsync();
            var serverKey = _cryptoService.AddClientPublicKey(clientGuid, clientKey);

            return new ContentResult
            {
                Content = serverKey,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        public async Task<IActionResult> Stage2()
        {
            HttpContext.Request.Headers.TryGetValue("ClientGuid", out var clientGuid);

            await using var ms = new MemoryStream();
            await HttpContext.Request.Body.CopyToAsync(ms);

            var encryptedChallenge = ms.ToArray();
            var decryptedChallenge = _cryptoService.DecryptData(encryptedChallenge);
            
            _cryptoService.AddClientChallenge(clientGuid, decryptedChallenge);

            var sessionKey = _cryptoService.GetRandomData(32);
            _cryptoService.AddClientSessionKey(clientGuid, sessionKey);

            var encryptedSessionKey = _cryptoService.EncryptData(clientGuid, sessionKey);
            return File(encryptedSessionKey, "application/octet-stream");
        }

        public IActionResult Stage3()
        {
            HttpContext.Request.Headers.TryGetValue("ClientGuid", out var clientGuid);

            var challenge = _cryptoService.GetClientChallenge(clientGuid);
            var encryptedChallenge = _cryptoService.EncryptData2(clientGuid, challenge);

            return File(encryptedChallenge, "application/octet-stream");
        }
    }
}