﻿using Notification.Wpf;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace TaskSharp.Classes
{
    public enum ReminderPriority
    {
        Niski, Srednji, Visoki
    }

    public abstract class BaseNote
    {
        public BaseNote(string Name, string Tags, bool Pinned, int UserId)
        {
            CreationDate = DateTime.Now;
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

        // Foreign key on User
        public int UserId { get; set; }

        // Reference navigation on User
        public User User { get; set; }

        public void PinUnpin()
        {
            Pinned = !Pinned;
        }
    }

    public class Note : BaseNote
    {
        public Note(string Name, string Tags, bool Pinned, int UserId, string Content) : base(Name, Tags, Pinned, UserId)
        {
            this.Content = Content;
        }

        public string Content { get; set; }

        public void Update(string name, string tags, string content)
        {
            Name = name;
            Tags = tags;
            Content = content;
        }
    }

    public class Event : BaseNote
    {
        public Event(string Name, string Tags, bool Pinned, int UserId, DateTime StartDate, DateTime EndDate, string Location) : base(Name, Tags, Pinned, UserId)
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

        public void Update(string name, string tags, DateTime startDate, DateTime endDate, string location)
        {
            Name = name;
            Tags = tags;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            ExpiredNotification = true;
            DeadlineNotification = true;
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
        public Reminder(string Name, string Tags, bool Pinned, int UserId, DateTime DueDate, ReminderPriority Priority) : base(Name, Tags, Pinned, UserId)
        {
            this.DueDate = DueDate;
            this.Priority = Priority;
            Notification = true;
        }

        public DateTime DueDate { get; set; }
        public ReminderPriority Priority { get; set; }
        public bool Notification { get; set; }

        public void Update(string name, string tags, DateTime dueDate, ReminderPriority priority)
        {
            Name = name;
            Tags = tags;
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
        public TodoList(string Name, string Tags, bool Pinned, int UserId, string Todos) : base(Name, Tags, Pinned, UserId)
        {
            this.Todos = Todos;
            Done = false;
        }

        public string Todos { get; set; }
        public bool Done { get; set; }

        public void Update(string name, string tags, string todos, bool done)
        {
            Name = name;
            Tags = tags;
            Todos = todos;
            Done = done;
        }

        public void SaveTodoElements(Dictionary<string, bool> todoDict)
        {
            Todos = JsonSerializer.Serialize(todoDict);
            MessageBox.Show("Promjene uspješno spremljene!", "Spremanje promjena", MessageBoxButton.OK, MessageBoxImage.Information);

            foreach (var done in todoDict.Values)
            { // check if all to-do list elements are done
                if (!done)
                {
                    Done = false;
                    return;
                }
            }

            Done = true;
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent
            {
                Title = Name,
                Message = "To-do lista je obavljena!",
                Type = NotificationType.Information,
                CloseOnClick = true // closes message when message is clicked
            });
        }
    }
}