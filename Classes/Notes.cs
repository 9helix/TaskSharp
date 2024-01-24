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
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public bool Pinned { get; set; }

        // foreign key on User
        public int UserId { get; set; }
        public User User { get; set; }
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
        public bool DeadlineNotification { get; set; }
        public bool ExpiredNotification { get; set; }

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
        public DateTime DueDate { get; set; }
        public ReminderPriority Priority { get; set; }
        public bool Notification { get; set; }

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
        public string Todos { get; set; }
        public bool Done { get; set; }

        public void CheckUncheck(string todoID)
        {
            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(Todos);
            todoDict[todoID] = !todoDict[todoID];
            Todos = JsonSerializer.Serialize(todoDict);
        }
    }
}