namespace backend.HelperFunctions.Interfaces;

public interface IFileValidator
{
    void Validate(IFormFile file);
    bool IsFileContentSafe(IFormFile file);
    bool HasExecutableCode(MemoryStream memoryStream);
    bool IsTextFile(MemoryStream stream);
    bool HasHiddenData(MemoryStream stream);
}