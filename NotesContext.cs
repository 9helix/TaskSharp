using Microsoft.EntityFrameworkCore;
using TaskSharp.Classes;

namespace TaskSharp
{
    public class NotesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BaseNote> BaseNotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=notes.db");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
