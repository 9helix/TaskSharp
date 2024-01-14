namespace TaskSharp.Classes
{
    public class BaseNote
    {
        public int BaseNoteId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public bool Pinned { get; set; }

        // foreign key on User
        public int UserName { get; set; }
        public virtual User User { get; set; }
    }

    public class Note : BaseNote
    {
        public string Content { get; set; }
    }

    public class Event : BaseNote
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
    }

    public class Reminder : BaseNote
    {
        public DateTime DueDate { get; set; }
        public enum ReminderPriority
        {
            Low, Medium, High
        }
    }

    public class TodoList : BaseNote
    {
        public Dictionary<string, bool> Todos { get; set; }
    }
}
