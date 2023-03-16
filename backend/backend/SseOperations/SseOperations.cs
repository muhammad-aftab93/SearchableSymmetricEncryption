using backend.SseOperations.Interfaces;
using System.Text;
using backend.Encryption.Interfaces;
using backend.Repository.Interfaces;

namespace backend.SseOperations;

public class SseOperations : ISseOperations
{
    private readonly IEncryption _encryption;
    private readonly IRepository _repository;

    public SseOperations(IEncryption encryption,
        IRepository repository)
    {
        _encryption = encryption;
        _repository = repository;
    }

    // function to encrypt and index a document using searchable symmetric encryption
    public Tuple<byte[], byte[]> SsEncrypt(string documentId, string documentContent, byte[] encryptionKey, byte[] hmacKey)
    {
        // convert the document id and content to bytes
        var docIdBytes = Encoding.UTF8.GetBytes(documentId);
        var docContentBytes = Encoding.UTF8.GetBytes(documentContent);

        // encrypt the document content with the encryption key
        var encryptedDocContent = _encryption.Encrypt(docContentBytes, encryptionKey);

        // compute the HMAC of the document id with the hmac key
        var hmacDocId = _encryption.ComputeHMAC(docIdBytes, hmacKey);

        // return the encrypted document content and the HMAC as a tuple
        return Tuple.Create(encryptedDocContent, hmacDocId);
    }

    // function to decrypt and retrieve a document using searchable symmetric encryption
    public string SsDecrypt(byte[] encryptedDocumentContent, byte[] encryptionKey)
    {

        // decrypt the encrypted document content with the encryption key 
        var decryptedDocContent = _encryption.Decrypt(encryptedDocumentContent, encryptionKey);

        // convert the decrypted document content to string 
        var docContentString = Encoding.UTF8.GetString(decryptedDocContent);

        // return the decrypted document content as string 
        return docContentString;
    }

    // function to search for documents that contain a keyword 
    public async Task<(string? fileName, string? fileContent)> SsSearch(string keyword, byte[] encryptionKey, byte[] hmacKey)
    {
        // convert the keyword to bytes 
        var keywordBytes = Encoding.UTF8.GetBytes(keyword);

        // compute the HMAC of the keyword with the hmac key 
        var hmacKeyword = Convert.ToBase64String(_encryption.ComputeHMAC(keywordBytes, hmacKey));

        var ecnryptedFile = await _repository.GetEncryptedFilesAsync(hmacKeyword);
        return (ecnryptedFile?.FileName, ecnryptedFile?.Content);
    }
}
