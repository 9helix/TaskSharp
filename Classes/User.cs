using System.Collections.ObjectModel;

namespace TaskSharp.Classes
{
    public class User
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // Collection navigation containing notes
        public virtual ICollection<Note> Notes { get; } = new List<Note>();

        // Collection navigation containing events
        public virtual ICollection<Event> Events { get; } = new List<Event>();

        // Collection navigation containing reminders
        public virtual ICollection<Reminder> Reminders { get; } = new List<Reminder>();

        // Collection navigation containing to-do lists
        public virtual ICollection<TodoList> TodoLists { get; } = new List<TodoList>();

    }
}
