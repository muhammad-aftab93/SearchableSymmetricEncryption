using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SSE
{
    class Program
    {
        // A class that represents an encrypted document
        class EncryptedDocument
        {
            public byte[] Data { get; set; } // The encrypted data
            public byte[] IV { get; set; } // The initialization vector for AES
        }

        // A class that represents an encrypted index entry
        class EncryptedIndexEntry
        {
            public string Keyword { get; set; } // The plaintext keyword
            public byte[] Token { get; set; } // The search token for the keyword
            public List<EncryptedDocument> Documents { get; set; } // The list of encrypted documents that contain the keyword
        }

        // A method that generates a random 256-bit key for AES 
        static byte[] GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[32];
                rng.GetBytes(key);
                return key;
            }
        }

        // A method that encrypts a string using AES with CBC mode and PKCS7 padding
        static EncryptedDocument EncryptString(string data, byte[] key)
        {
            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var iv = new byte[aes.BlockSize / 8];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(iv);
                }

                var encryptor = aes.CreateEncryptor(key, iv);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(data);
                        }
                    }
                    var encryptedData = ms.ToArray();
                    return new EncryptedDocument { Data = encryptedData, IV = iv };
                }
            }
        }

        // A method that decrypts an encrypted document using AES with CBC mode and PKCS7 padding
        static string DecryptString(EncryptedDocument document, byte[] key)
        {
            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor(key, document.IV);

                using (var ms = new MemoryStream(document.Data))
                {
                    using (var cs =
                        new CryptoStream(ms,
                            decryptor,
                            CryptoStreamMode.Read))
                    {
                        using (var sr =
                            new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        // A method that generates a search token for a keyword using HMAC-SHA256
        static byte[] GenerateToken(string keyword, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                var data = Encoding.UTF8.GetBytes(keyword);
                return hmac.ComputeHash(data);
            }
        }

        // A method that creates an encrypted index from a collection of plaintext documents
        static List<EncryptedIndexEntry> CreateEncryptedIndex(List<string> documents, byte[] key)
        {
            var index = new List<EncryptedIndexEntry>();

            foreach (var document in documents)
            {
                // Encrypt the document with AES
                var encryptedDocument = EncryptString(document, key);

                // Extract the keywords from the document by splitting on whitespace
                var keywords = document.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var keyword in keywords)
                {
                    // Generate a search token for the keyword with HMAC-SHA256
                    var token = GenerateToken(keyword, key);

                    // Check if there is already an index entry for this keyword
                    var entry = index.Find(e => e.Keyword == keyword);

                    if (entry == null)
                    {
                        // Create a new index entry with the keyword, token and document
                        entry = new EncryptedIndexEntry
                        {
                            Keyword = keyword,
                            Token = token,
                            Documents =
                            new List<EncryptedDocument>()
                        };
                        entry.Documents.Add(encryptedDocument);
                        index.Add(entry);
                    }
                    else
                    {
                        // Add the document to the existing index entry
                        entry.Documents.Add(encryptedDocument);
                    }
                }
            }

            return index;
        }

        // A method that searches an encrypted index for a keyword and returns a list of decrypted documents that match it
        static List<string> SearchEncryptedIndex(List<EncryptedIndexEntry> index, string keyword,
            byte[] key)
        {
            var result = new List<string>();

            // Generate a search token for the keyword with HMAC-SHA256
            var token = GenerateToken(keyword, key);

            // Find the index entry that matches the token
            var entry =
                index.Find(e => e.Token.SequenceEqual(token));

            if (entry != null)
            {
                // Decrypt each document in the entry with AES and add it to the result list
                foreach (var document in entry.Documents)
                {
                    result.Add(DecryptString(document,
                        key));
                }
            }

            return result;
        }

        static void Main(string[] args)
        {// Generate a random key for AES and HMAC-SHA256
            var key = GenerateKey();

            // Create a list of plaintext documents
            var documents = new List<string>
            {
                "Hello world",
                "This is a test",
                "Searchable symmetric encryption",
                "Symmetric encryption is fast",
                "Encryption is important"
            };

            // Create an encrypted index from the documents
            var index = CreateEncryptedIndex(documents, key);

            // Search for a keyword and print the matching documents
            var keyword = "encryption";
            var matchingDocuments = SearchEncryptedIndex(index, keyword, key);

            Console.WriteLine($"Searching for '{keyword}'...");

            if (matchingDocuments.Count > 0)
            {
                Console.WriteLine("Found the following documents:");

                foreach (var document in matchingDocuments)
                {
                    Console.WriteLine(document);
                }
            }
            else
            {
                Console.WriteLine("No documents found.");
            }
        }
    }
}
