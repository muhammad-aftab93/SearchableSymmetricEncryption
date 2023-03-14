using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class TDbContext : DbContext
    {
        public TDbContext(DbContextOptions<TDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public virtual DbSet<EncryptedFiles> EncryptedFiles { get; set; } = null!;
    }
}