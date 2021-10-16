using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Server.Interfaces;

namespace Server.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly string _publicKey;
        private readonly RSAParameters _privateKey;
        
        private readonly Dictionary<string, RSAParameters> _clientKeys = new();
        private readonly Dictionary<string, byte[]> _sessionKeys = new();
        private readonly Dictionary<string, byte[]> _challenges = new();

        public CryptoService()
        {
            using var rsa = RSA.Create(KeyLength);
            
            _publicKey = rsa.ToXmlString(false);
            _privateKey = rsa.ExportParameters(true);
        }

        public string AddClientPublicKey(string guid, string xmlString)
        {
            using var rsa = RSA.Create(KeyLength);
            rsa.FromXmlString(xmlString);

            _clientKeys.Add(guid, rsa.ExportParameters(false));

            return _publicKey;
        }

        public void AddClientChallenge(string guid, byte[] challenge)
        {
            _challenges.Add(guid, challenge);
        }

        public byte[] GetClientChallenge(string guid)
        {
            return _challenges[guid];
        }

        public byte[] DecryptData(byte[] data)
        {
            using var rsa = RSA.Create(KeyLength);
            rsa.ImportParameters(_privateKey);

            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public byte[] GetRandomData(int length)
        {
            var buf = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(buf);

            return buf;
        }

        public void AddClientSessionKey(string guid, byte[] key)
        {
            _sessionKeys.Add(guid, key);
        }

        public byte[] EncryptData(string guid, byte[] data)
        {
            var publicKey = _clientKeys[guid];
            
            using var rsa = RSA.Create(KeyLength);
            rsa.ImportParameters(publicKey);

            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public byte[] EncryptData2(string guid, byte[] data)
        {
            var sessionKey = _sessionKeys[guid];
            
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = sessionKey;
            aes.GenerateIV();

            using var transform = aes.CreateEncryptor();
            var iv = aes.IV;
            var enc = transform.TransformFinalBlock(data, 0, data.Length);
            var hmac = CalculateHmac(sessionKey, enc);
            var final = new byte[iv.Length + hmac.Length + enc.Length];
            
            Buffer.BlockCopy(iv, 0, final, 0, iv.Length);
            Buffer.BlockCopy(hmac, 0, final, iv.Length, hmac.Length);
            Buffer.BlockCopy(enc, 0, final, iv.Length + hmac.Length, enc.Length);

            return final;
        }

        private byte[] CalculateHmac(byte[] key, byte[] data)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(data);
        }

        private static int KeyLength => 2048;
    }
}