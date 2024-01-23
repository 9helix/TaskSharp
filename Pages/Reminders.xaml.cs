using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskSharp;
using TaskSharp.Classes;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        private readonly NotesContext _context =
            new NotesContext();

        public Page3()
        {
            InitializeComponent();
        }

        public void NotificationChecker(Reminder row)
        {
            if (row.Notification && (row.DueDate == DateTime.Today))
            {
                Color color = (Color)ColorConverter.ConvertFromString("#fa7f05");
                var notificationManager = new NotificationManager();
                notificationManager.Show(new NotificationContent
                {
                    Title = "Podsjetnik!",
                    Message = row.Name,
                    Type = NotificationType.Information,
                    CloseOnClick = true, // closes message when message is clicked
                    Background = new SolidColorBrush(color)
                });

                row.Notification = false;
                _context.SaveChanges();
            }
        }

        private void RefreshReminders()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var upcomingReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.Priority)
                .ThenBy(x => x.DueDate)
                .ToList();
            var expiredReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate < DateTime.Today)
                .OrderByDescending(x => x.DueDate)
                .ToList();

            if (upcomingReminders.Count == 0 && expiredReminders.Count == 0)
            {
                Reminders.Visibility = Visibility.Collapsed;
                RemindersEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                RemindersEmpty.Visibility = Visibility.Collapsed;
                Reminders.Visibility = Visibility.Visible;

                if (upcomingReminders.Count == 0)
                {
                    UpcomingRemindersContainer.Visibility = Visibility.Collapsed;
                    UpcomingRemindersEmpty.Visibility = Visibility.Visible;

                    ExpiredRemindersContainer.Visibility = Visibility.Visible;
                    ExpiredRemindersEmpty.Visibility = Visibility.Collapsed;
                    ExpiredRemindersContainer.ItemsSource = expiredReminders;
                }
                else if (expiredReminders.Count == 0)
                {
                    UpcomingRemindersContainer.Visibility = Visibility.Visible;
                    UpcomingRemindersEmpty.Visibility = Visibility.Collapsed;

                    foreach (var reminder in upcomingReminders)
                    {
                        NotificationChecker(reminder);
                    }
                    UpcomingRemindersContainer.ItemsSource = upcomingReminders;

                    ExpiredRemindersContainer.Visibility = Visibility.Collapsed;
                    ExpiredRemindersEmpty.Visibility = Visibility.Visible;
                }
                else
                {
                    ExpiredRemindersContainer.Visibility = Visibility.Visible;
                    ExpiredRemindersEmpty.Visibility = Visibility.Collapsed;
                    ExpiredRemindersContainer.ItemsSource = expiredReminders;

                    UpcomingRemindersContainer.Visibility = Visibility.Visible;
                    UpcomingRemindersEmpty.Visibility = Visibility.Collapsed;
                    foreach (var reminder in upcomingReminders)
                    {
                        NotificationChecker(reminder);
                    }
                    UpcomingRemindersContainer.ItemsSource = upcomingReminders;
                }
            }
        }

        private void Reminders_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.Reminders.Load();
            RefreshReminders();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["noteType"] = 2;
            var noteCreate = new NoteCreate();
            noteCreate.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void PinUnpinReminder(object sender, MouseButtonEventArgs e)
        {
            var reminderID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var reminder = _context.Reminders.Where(x => x.UserId == uid && x.Id == reminderID).First();

            reminder.Pinned = !reminder.Pinned;
            _context.SaveChanges();
            RefreshReminders();
        }

        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var reminderID = ((Image)sender).Tag;
            Application.Current.Properties["noteType"] = 2;
            Application.Current.Properties["noteId"] = reminderID;

            var reminderEdit = new NoteEdit();
            reminderEdit.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void DeleteReminder(object sender, MouseButtonEventArgs e)
        {
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati podsjetnik?", "Brisanje podsjetnika", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (choice == MessageBoxResult.Yes)
            {
                var reminderID = (int)((Image)sender).Tag;
                var uid = (int)Application.Current.Properties["uid"];

                var reminder = _context.Reminders.Where(x => x.UserId == uid && x.Id == reminderID).First();
                _context.Reminders.Remove(reminder);
                _context.SaveChanges();

                MessageBox.Show("Događaj uspješno izbrisan!", "Brisanje događaja", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshReminders();
            }
        }

        private void Reminders_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }
    }
}
