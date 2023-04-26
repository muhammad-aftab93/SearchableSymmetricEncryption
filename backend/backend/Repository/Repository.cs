using backend.Data;
using backend.Entities;
using backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class Repository : IRepository
{
    private readonly TDbContext _context;

    public Repository(TDbContext context)
        => _context = context;

    public async Task<int> SaveEncryptedFileAsync(EncryptedFiles file)
    {
        if (_context.EncryptedFiles.Any(x => x.FileName == file.FileName))
            throw new Exception("Filename already exists.");

        await _context.EncryptedFiles.AddAsync(file);
        return await _context.SaveChangesAsync();
    }

    public async Task<EncryptedFiles?> GetEncryptedFilesAsync(string fileName)
        => await _context.EncryptedFiles.FirstOrDefaultAsync(x => x.FileName == fileName);

    public async Task<bool> ResetDatabase()
        => await _context.EncryptedFiles.ExecuteDeleteAsync() > 0;
}