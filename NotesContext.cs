using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharp
{
    public class ProductContext : DbContext
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
