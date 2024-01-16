namespace TaskSharp.Classes
{
    public enum ReminderPriority
    {
        Low, Medium, High
    }
    public class BaseNote
    {
        public int BaseNoteId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        private bool Pinned;

        // foreign key on User
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public void TogglePinned()
        {
            Pinned = !Pinned;
        }
        public bool GetPinned()
        {
            return Pinned;
        }
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
        public DateTime DueDate;
        public ReminderPriority Priority { get; set; }

    }

    public class TodoList : BaseNote
    {
        public Dictionary<string, bool> Todos { get; set; }

        public void ToggleTodo(string todo)
        {
            Todos[todo] = !Todos[todo];
        }

    }
}