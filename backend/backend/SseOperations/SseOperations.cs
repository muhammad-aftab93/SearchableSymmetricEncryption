using backend.SseOperations.Interfaces;
using System.Text;
using backend.Encryption.Interfaces;

namespace backend.SseOperations;

public class SseOperations : ISseOperations
{
    private readonly IEncryption _encryption;

    public SseOperations(IEncryption encryption)
    {
        _encryption = encryption;
    }

    // function to encrypt and index a document using searchable symmetric encryption
    public Tuple<byte[], byte[]> SSEncrypt(string documentId, string documentContent, byte[] encryptionKey, byte[] hmacKey)
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
    public string SSDecrypt(byte[] encryptedDocumentContent, byte[] encryptionKey)
    {

        // decrypt the encrypted document content with the encryption key 
        var decryptedDocContent = _encryption.Decrypt(encryptedDocumentContent, encryptionKey);

        // convert the decrypted document content to string 
        var docContentString = Encoding.UTF8.GetString(decryptedDocContent);

        // return the decrypted document content as string 
        return docContentString;
    }

    // function to search for documents that contain a keyword 
    public void SSSearch(string keyword, Dictionary<byte[], byte[]> documents, byte[] encryptionKey, byte[] hmacKey)
    {
        // convert the keyword to bytes 
        var keywordBytes = Encoding.UTF8.GetBytes(keyword);

        // compute the HMAC of the keyword with the hmac key 
        var hmacKeyword = _encryption.ComputeHMAC(keywordBytes, hmacKey);

        // loop over the documents dictionary
        foreach (var entry in documents)
        {
            // get the encrypted document content and the HMAC from the entry
            var encryptedDocContent = entry.Key;
            var hmacDocId = entry.Value;

            // check if the HMAC of the document id matches the HMAC of the keyword
            if (hmacDocId.SequenceEqual(hmacKeyword))
            {
                // decrypt and retrieve the document content with the encryption key
                var docContentString = SSDecrypt(encryptedDocContent, encryptionKey);

                // print the document content
                Console.WriteLine(docContentString);
            }
        }
    }
}