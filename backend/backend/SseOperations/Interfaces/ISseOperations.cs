namespace backend.SseOperations.Interfaces;

public interface ISseOperations
{
    Tuple<byte[], byte[]> SsEncrypt(string documentId,
        string documentContent,
        byte[] encryptionKey,
        byte[] hmacKey);

    string SsDecrypt(byte[] encryptedDocumentContent,
        byte[] encryptionKey);

    Task<(string? fileName, string? fileContent)> SsSearch(string keyword,
        byte[] encryptionKey,
        byte[] hmacKey);
}