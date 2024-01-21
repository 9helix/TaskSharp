using Microsoft.EntityFrameworkCore;
using TaskSharp.Classes;

namespace TaskSharp
{
    public class NotesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<TodoList> TodoLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=notes.db");
        }
    }
}
