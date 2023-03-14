using backend.Entities;

namespace backend.Repository.Interfaces;

public interface IRepository
{
    Task<int> SaveEncryptedFileAsync(EncryptedFiles file);
    Task<EncryptedFiles?> GetEncryptedFilesAsync(string fileName);
    Task<bool> ResetDatabase();
}