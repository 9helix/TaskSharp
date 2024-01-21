using System.ComponentModel.DataAnnotations.Schema;

namespace TaskSharp.Classes
{
    public enum ReminderPriority
    {
        Niski, Srednji, Visoki
    }

    public abstract class BaseNote
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public bool Pinned { get; set; }

        // foreign key on User
        public int UserId { get; set; }
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
        public ReminderPriority Priority { get; set; }
    }

    public class TodoList : BaseNote
    {
        public string Todos { get; set; }
    }
}