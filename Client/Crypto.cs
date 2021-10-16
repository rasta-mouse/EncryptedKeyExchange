using System;
using System.Linq;
using System.Security.Cryptography;

namespace Client
{
    public class Crypto
    {
        public string ClientPublic { get; }
        public byte[] ClientChallenge { get; set; }
        public byte[] SessionKey { get; set; }
        
        private readonly RSAParameters _clientPrivate;
        private RSAParameters _serverPublic;

        public Crypto()
        {
            using var rsa = RSA.Create(KeyLength);

            ClientPublic = rsa.ToXmlString(false);
            _clientPrivate = rsa.ExportParameters(true);
        }

        public void AddServerKey(string xmlString)
        {
            using var rsa = RSA.Create(KeyLength);
            rsa.FromXmlString(xmlString);

            _serverPublic = rsa.ExportParameters(false);
        }

        public byte[] GetRandomData(int length)
        {
            var buf = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(buf);

            return buf;
        }

        public byte[] EncryptData(byte[] data)
        {
            using var rsa = RSA.Create(KeyLength);
            rsa.ImportParameters(_serverPublic);

            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public byte[] DecryptData(byte[] data)
        {
            using var rsa = RSA.Create(KeyLength);
            rsa.ImportParameters(_clientPrivate);

            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public byte[] DecryptData2(byte[] data)
        {
            var iv = data[..16];
            var hmac = data[16..48];
            var enc = data[48..];

            if (!HmacValid(enc, hmac))
                throw new Exception("Invalid HMAC");
            
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = SessionKey;
            aes.IV = iv;

            using var transform = aes.CreateDecryptor();
            return transform.TransformFinalBlock(enc, 0, enc.Length);
        }

        private byte[] CalculateHmac(byte[] data)
        {
            using var hmac = new HMACSHA256(SessionKey);
            return hmac.ComputeHash(data);
        }

        private bool HmacValid(byte[] data, byte[] hmac)
        {
            var calculated = CalculateHmac(data);
            return calculated.SequenceEqual(hmac);
        }

        private static int KeyLength => 2048;
    }
}