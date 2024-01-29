namespace TaskSharp.Classes
{
    public class User
    {
        public User(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // Collection navigation containing notes
        public ICollection<Note> Notes { get; } = new List<Note>();

        // Collection navigation containing events
        public ICollection<Event> Events { get; } = new List<Event>();

        // Collection navigation containing reminders
        public ICollection<Reminder> Reminders { get; } = new List<Reminder>();

        // Collection navigation containing to-do lists
        public ICollection<TodoList> TodoLists { get; } = new List<TodoList>();
    }
}
