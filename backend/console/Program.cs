using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SearchableSymmetricEncryption
{
    class Program
    {
        // function to generate a random key of 256 bits
        static byte[] GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[32];
                rng.GetBytes(key);
                return key;
            }
        }

        // function to encrypt a plaintext using AES-256 in CBC mode with PKCS7 padding
        static byte[] Encrypt(byte[] plaintext, byte[] key)
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
        static byte[] Decrypt(byte[] ciphertext, byte[] key)
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
        static byte[] ComputeHMAC(byte[] message, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(message);
            }
        }

        // function to encrypt and index a document using searchable symmetric encryption
        static Tuple<byte[], byte[]> SSEncrypt(string documentId,
            string documentContent,
            byte[] encryptionKey,
            byte[] hmacKey)
        {

            // convert the document id and content to bytes
            var docIdBytes =
                Encoding.UTF8.GetBytes(documentId);
            var docContentBytes =
                Encoding.UTF8.GetBytes(documentContent);

            // encrypt the document content with the encryption key
            var encryptedDocContent =
                Encrypt(docContentBytes,
                    encryptionKey);

            // compute the HMAC of the document id with the hmac key
            var hmacDocId =
                ComputeHMAC(docIdBytes,
                    hmacKey);

            // return the encrypted document content and the HMAC as a tuple
            return Tuple.Create(encryptedDocContent,
                hmacDocId);
        }

        // function to decrypt and retrieve a document using searchable symmetric encryption
        static string SSDecrypt(byte[] encryptedDocumentContent,
            byte[] encryptionKey)
        {

            // decrypt the encrypted document content with the encryption key 
            var decryptedDocContent =
                Decrypt(encryptedDocumentContent,
                    encryptionKey);

            // convert the decrypted document content to string 
            var docContentString =
                Encoding.UTF8.GetString(decryptedDocContent);

            // return the decrypted document content as string 
            return docContentString;
        }

        // function to search for documents that contain a keyword 
        static void SSSearch(string keyword,
            Dictionary<byte[], byte[]> documents,
            byte[] encryptionKey,
            byte[] hmacKey)
        {

            Console.WriteLine($"Searching for documents that contain '{keyword}'...");

            // convert the keyword to bytes 
            var keywordBytes =
                Encoding.UTF8.GetBytes(keyword);

            // compute the HMAC of the keyword with the hmac key 
            var hmacKeyword =
                ComputeHMAC(keywordBytes, hmacKey);

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

            Console.WriteLine("Search completed.");

        }

        static void Main(string[] args)
        {
            // create some sample documents with ids and contents
            var documents = new Dictionary<string, string>
            {
                {"doc1", "This is a document about searchable symmetric encryption."},
                {"doc2", "This is another document about cryptography."},
                {"doc3", "This is a document about something else."}
            };

            // create a dictionary to store the encrypted and indexed documents
            var encryptedDocuments = new Dictionary<byte[], byte[]>();

            // encrypt and index each document using SSEncrypt function
            foreach (var entry in documents)
            {
                var docId = entry.Key;
                var docContent = entry.Value;

                var encryptedDoc = SSEncrypt(docId, docContent, encryptionKey, hmacKey);

                encryptedDocuments.Add(encryptedDoc.Item1, encryptedDoc.Item2);
            }

            // search for documents that contain "doc1" as keyword using SSSearch function
            SSSearch("doc1", encryptedDocuments, encryptionKey, hmacKey);

        }
    }
}