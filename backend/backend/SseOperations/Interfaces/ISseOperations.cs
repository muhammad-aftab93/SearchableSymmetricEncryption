namespace backend.SseOperations.Interfaces;

public interface ISseOperations
{
    Tuple<byte[], byte[]> SSEncrypt(string documentId,
        string documentContent,
        byte[] encryptionKey,
        byte[] hmacKey);

    string SSDecrypt(byte[] encryptedDocumentContent,
        byte[] encryptionKey);

    void SSSearch(string keyword,
        Dictionary<byte[], byte[]> documents,
        byte[] encryptionKey,
        byte[] hmacKey);
}