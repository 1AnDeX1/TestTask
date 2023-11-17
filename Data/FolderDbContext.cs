using Microsoft.EntityFrameworkCore;
using TestTask.Models;

namespace TestTask.Data
{
    public class FolderDbContext : DbContext
    {
        public FolderDbContext(DbContextOptions<FolderDbContext> options) : base(options)
        {

        }

        public DbSet<Folder> Folders{ get; set; }
    }
}
