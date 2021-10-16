namespace Server.Interfaces
{
    public interface ICryptoService
    {
        /// <summary>
        /// Add a Client's Public Key
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="xmlString"></param>
        /// <returns>Server's Public Key</returns>
        string AddClientPublicKey(string guid, string xmlString);

        /// <summary>
        /// Store a client challenge
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="challenge"></param>
        void AddClientChallenge(string guid, byte[] challenge);

        /// <summary>
        /// Retrieve a Client's challenge
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        byte[] GetClientChallenge(string guid);

        /// <summary>
        /// Decrypt data using RSA private key
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Decrypted data</returns>
        byte[] DecryptData(byte[] data);

        /// <summary>
        /// Generate some random data
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] GetRandomData(int length);

        /// <summary>
        /// Store a client's session key
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="key"></param>
        void AddClientSessionKey(string guid, byte[] key);

        /// <summary>
        /// Encrypt data with a client's public key
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] EncryptData(string guid, byte[] data);

        /// <summary>
        /// Encrypt data using client's session key
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] EncryptData2(string guid, byte[] data);
    }
}