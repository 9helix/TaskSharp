using Notification.Wpf;
using System.Text.Json;
using System.Windows.Media;

namespace TaskSharp.Classes
{
    public enum ReminderPriority
    {
        Niski, Srednji, Visoki
    }

    public abstract class BaseNote
    {
        public BaseNote (DateTime CreationDate, string Name, string Tags, bool Pinned, int UserId) {
            this.CreationDate = CreationDate;
            this.Name = Name;
            this.Tags = Tags;
            this.Pinned = Pinned;
            this.UserId = UserId;
        }

        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public bool Pinned { get; set; }

        // foreign key on User
        public int UserId { get; set; }
        public User User { get; set; }

        public void PinUnpin()
        {
            Pinned = !Pinned;
        }
    }

    public class Note : BaseNote
    {
        public Note(DateTime CreationDate, string Name, string Tags, bool Pinned, int UserId, string Content) :base (CreationDate, Name, Tags, Pinned, UserId)
        {
            this.Content = Content;
        }

        public string Content { get; set; }

        public void Update(string name, string tags, bool pin, string content)
        {
            Name = name;
            Tags = tags;
            Pinned = pin;
            Content = content;   
        }
    }

    public class Event : BaseNote
    {
        public Event(DateTime CreationDate, string Name, string Tags, bool Pinned, int UserId, DateTime StartDate, DateTime EndDate, string Location) : base(CreationDate, Name, Tags, Pinned, UserId)
        {
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.Location = Location;
            ExpiredNotification = true;
            DeadlineNotification = true;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public bool DeadlineNotification { get; set; }
        public bool ExpiredNotification { get; set; }

        public void Update(string name, string tags, bool pin, DateTime startDate, DateTime endDate, string location)
        {
            Name = name;
            Tags = tags;
            Pinned = pin;
            DeadlineNotification = true;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            ExpiredNotification = true;
        }

        public void NotificationChecker(bool isDeadline)
        {
            if (isDeadline) // if event is still ongoing
            {
                if (DeadlineNotification && (EndDate == DateTime.Today))
                {
                    Color color = (Color)ColorConverter.ConvertFromString("#fa7f05");
                    var notificationManager = new NotificationManager();
                    notificationManager.Show(new NotificationContent
                    {
                        Title = Name,
                        Message = "Događaj završava danas!",
                        Type = NotificationType.Information,
                        CloseOnClick = true, // closes message when message is clicked
                        Background = new SolidColorBrush(color)
                    });

                    DeadlineNotification = false;
                }
            }
            else
            { // if event was completed
                if (ExpiredNotification && (EndDate < DateTime.Today))
                {
                    var notificationManager = new NotificationManager();
                    notificationManager.Show(new NotificationContent
                    {
                        Title = Name,
                        Message = "Događaj je završen!",
                        Type = NotificationType.Information,
                        CloseOnClick = true, // closes message when message is clicked
                    });

                    ExpiredNotification = false;
                }
            }
        }
    }

    public class Reminder : BaseNote
    {
        public Reminder(DateTime CreationDate, string Name, string Tags, bool Pinned, int UserId, DateTime DueDate, ReminderPriority Priority) : base(CreationDate, Name, Tags, Pinned, UserId)
        {
            this.DueDate = DueDate;
            this.Priority = Priority;
            Notification = true;
        }

        public DateTime DueDate { get; set; }
        public ReminderPriority Priority { get; set; }
        public bool Notification { get; set; }

        public void Update(string name, string tags, bool pin, DateTime dueDate, ReminderPriority priority)
        {
            Name = name;
            Tags = tags;
            Pinned = pin;
            DueDate = dueDate;
            Priority = priority;
            Notification = true;
        }

        public void NotificationChecker()
        {
            if (Notification && (DueDate == DateTime.Today))
            {
                Color color = (Color)ColorConverter.ConvertFromString("#fa7f05");
                var notificationManager = new NotificationManager();
                notificationManager.Show(new NotificationContent
                {
                    Title = "Podsjetnik!",
                    Message = Name,
                    Type = NotificationType.Information,
                    CloseOnClick = true, // closes message when message is clicked
                    Background = new SolidColorBrush(color)
                });

                Notification = false;
            }
        }
    }

    public class TodoList : BaseNote
    {
        public TodoList(DateTime CreationDate, string Name, string Tags, bool Pinned, int UserId, string Todos) : base(CreationDate, Name, Tags, Pinned, UserId)
        {
            this.Todos = Todos;
            Done = false;
        }

        public string Todos { get; set; }
        public bool Done { get; set; }

        public void Update(string name, string tags, bool pin, string todos, bool done)
        {
            Name = name;
            Tags = tags;
            Pinned = pin;
            Todos = todos;
            Done = done;
        }

        public void CheckUncheck(string todoID)
        {
            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(Todos);
            todoDict[todoID] = !todoDict[todoID];
            Todos = JsonSerializer.Serialize(todoDict);
        }
    }
}