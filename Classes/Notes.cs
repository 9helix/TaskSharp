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
        public byte UserId { get; set; }
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
        private DateTime StartDate;
        private DateTime EndDate;
        public string Location { get; set; }

        public int SetEvent(DateTime start, DateTime end, string loc)
        {

            if (end > start)
            {
                Location = loc;
                StartDate = start;
                EndDate = end;
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    public class Reminder : BaseNote
    {
        private DateOnly DueDate;
        public ReminderPriority Priority { get; set; }

        public int SetDueDate(DateOnly duedate)
        {
            if (DateOnly.FromDateTime(DateTime.Now) < duedate)
            {
                DueDate = duedate;
                return 0;
            }
            else
                return -1;
        }

        public DateOnly GetDueDate()
        {
            return DueDate;
        }
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