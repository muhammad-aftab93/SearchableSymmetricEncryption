using backend.Encryption.Interfaces;
using System.Security.Cryptography;

namespace backend.Encryption;

public class Encryption : IEncryption
{
    // function to generate a random key of 256 bits
    public byte[] GenerateKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var key = new byte[32];
            rng.GetBytes(key);
            return key;
        }
    }

    // function to encrypt a plaintext using AES-256 in CBC mode with PKCS7 padding
    public byte[] Encrypt(byte[] plaintext, byte[] key)
    {
        using (var aes = new AesManaged())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // generate a random IV and prepend it to the ciphertext
            aes.GenerateIV();
            var iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }
    }

    // function to decrypt a ciphertext using AES-256 in CBC mode with PKCS7 padding
    public byte[] Decrypt(byte[] ciphertext, byte[] key)
    {
        using (var aes = new AesManaged())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // extract the IV from the first 16 bytes of the ciphertext
            var iv = new byte[16];
            Array.Copy(ciphertext, 0, iv, 0, iv.Length);

            using (var decryptor = aes.CreateDecryptor(key, iv))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(ciphertext, iv.Length, ciphertext.Length - iv.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }
    }

    // function to compute the HMAC-SHA256 of a message using a key
    public byte[] ComputeHMAC(byte[] message, byte[] key)
    {
        using (var hmac = new HMACSHA256(key))
        {
            return hmac.ComputeHash(message);
        }
    }
}