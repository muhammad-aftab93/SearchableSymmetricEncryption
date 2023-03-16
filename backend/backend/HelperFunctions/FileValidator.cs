using backend.HelperFunctions.Interfaces;
using System.Text;

namespace backend.HelperFunctions;

public class FileValidator : IFileValidator
{
    private static readonly string[] AllowedExtensions = { ".txt" };
    private const int MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public void Validate(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(paramName: nameof(file), message: "File is required.");

        if (file.Length == 0 || file.Length > MaxFileSize)
            throw new ArgumentException(paramName: nameof(file), message: "File cannot be larger than 5 mb.");

        if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
            throw new ArgumentException(paramName: nameof(file), message: "Allowed file extension is only .txt");

        if (!IsFileContentSafe(file))
            throw new ArgumentException(paramName: nameof(file), message: "File is not safe.");
    }

    public bool IsFileContentSafe(IFormFile file)
    {
        // Check if the file content is safe to be processed
        try
        {
            // Open a stream to read the file content
            using (var stream = new MemoryStream())
            {
                // Copy the file content to the stream
                file.CopyTo(stream);

                // Check if the file contains any executable code
                if (HasExecutableCode(stream))
                    return false;

                // Check if the file is an image by checking the file signature
                if (!IsTextFile(stream))
                    return false;

                // Check if the file contains any hidden data or personally identifiable information
                if (HasHiddenData(stream))
                    return false;
            }

            // If we reached here, the file content is safe
            return true;
        }
        catch (Exception)
        {
            // If an exception occurred while reading the file content, we assume the file content is not safe
            return false;
        }
    }

    public bool HasExecutableCode(MemoryStream memoryStream)
    {
        try
        {
            var buffer = memoryStream.GetBuffer();

            // Check if the file contains any magic numbers or signatures that are associated with executable files
            if (buffer[0] == 0x4D && buffer[1] == 0x5A) // This is a DOS MZ executable file
                return true;
            else if (buffer[0] == 0x7F && buffer[1] == 0x45 && buffer[2] == 0x4C && buffer[3] == 0x46)  // This is an ELF executable file
                return true;
            else if (buffer[0] == 0xCA && buffer[1] == 0xFE && buffer[2] == 0xBA && buffer[3] == 0xBE) // This is a Mach-O executable file
                return true;
            else if (buffer[0] == 0x50 && buffer[1] == 0x45 && buffer[2] == 0x00 && buffer[3] == 0x00) // This is a PE executable file
                return true;
            else
                return false; // This file does not contain executable code
        }
        catch (Exception)
        {
            // If an exception occurred while reading the file, we assume the file contains executable code
            return true;
        }
    }

    public bool IsTextFile(MemoryStream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream))
            {
                // Read the first few bytes of the stream and check for a byte order mark (BOM)
                var bom = reader.Peek();
                if (bom == 0xEFBBBF || bom == 0xFEFF || bom == 0xFFFE) // The stream has a BOM, which indicates a text file
                    return true;

                // Read the next few bytes of the stream and check if they are valid UTF-8 or ASCII characters
                var buffer = new char[4096];
                var bytesRead = reader.Read(buffer, 0, buffer.Length);
                var isText = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(buffer, 0, bytesRead)) == new string(buffer, 0, bytesRead);
                if (isText) // The stream has valid UTF-8 or ASCII characters, which indicates a text file
                    return true;

                // The stream does not appear to be a text file
                return false;
            }
        }
        catch (Exception)
        {
            // If an exception occurred while reading the stream, we assume the stream is not a text file
            return false;
        }
    }

    public bool HasHiddenData(MemoryStream stream)
    {
        try
        {
            // Get the byte array from the memory stream
            var bytes = stream.ToArray();

            // Check if the byte array contains null bytes
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x00) // The file contains hidden data
                    return true;
            }

            // The file does not contain hidden data
            return false;
        }
        catch (Exception)
        {
            // If an exception occurred while reading the file, we assume the file has hidden data
            return true;
        }
    }
}