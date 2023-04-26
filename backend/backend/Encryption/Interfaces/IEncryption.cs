namespace backend.Encryption.Interfaces;

public interface IEncryption
{
    byte[] GenerateKey();
    byte[] Encrypt(byte[] plaintext, byte[] key);
    byte[] Decrypt(byte[] ciphertext, byte[] key);
    byte[] ComputeHMAC(byte[] message, byte[] key);
}