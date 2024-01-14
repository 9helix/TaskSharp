using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharp
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // reference on user's notes
        public virtual ICollection<BaseNote> TaskList
            { get; private set;  } = new ObservableCollection<BaseNote>();
    }

    public abstract class BaseNote
    {
        public int BaseNoteId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
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
