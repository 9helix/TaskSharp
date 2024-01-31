using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskSharp;
using TaskSharp.Classes;
using TaskSharp.Themes;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Reminders : Page
    {
        private readonly NotesContext _context = new();
        public Reminders()
        {
            InitializeComponent();
            TextboxTheme.calledReminder += RefreshReminders;
        }

        private void RefreshReminders(List<Reminder> upcomingReminders, List<Reminder> expiredReminders)
        {
            if (upcomingReminders.Count == 0 && expiredReminders.Count == 0)
            {
                RemindersContainer.Visibility = Visibility.Collapsed;
                RemindersEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                RemindersEmpty.Visibility = Visibility.Collapsed;
                RemindersContainer.Visibility = Visibility.Visible;

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
                        reminder.NotificationChecker();
                    }
                    _context.SaveChanges();
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
                        reminder.NotificationChecker();
                    }
                    _context.SaveChanges();
                    UpcomingRemindersContainer.ItemsSource = upcomingReminders;
                }
            }
        }

        private void Reminders_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Reminders.Load();

            var uid = (int)Application.Current.Properties["uid"];
            var upcomingReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.DueDate)
                .ThenByDescending(x => x.Priority)
                .ToList();
            var expiredReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate < DateTime.Today)
                .OrderByDescending(x => x.DueDate)
                .ToList();
            RefreshReminders(upcomingReminders, expiredReminders);
        }

        private void PinUnpinReminder(object sender, MouseButtonEventArgs e)
        {
            var reminderID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var reminder = _context.Reminders.Where(x => x.UserId == uid && x.Id == reminderID).First();

            reminder.PinUnpin();
            _context.SaveChanges();

            var upcomingReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.DueDate)
                .ThenByDescending(x => x.Priority)
                .ToList();
            var expiredReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate < DateTime.Today)
                .OrderByDescending(x => x.DueDate)
                .ToList();
            RefreshReminders(upcomingReminders, expiredReminders);
        }

        public delegate void EditHandlerReminder();
        public static event EditHandlerReminder callEditReminder;
        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var reminderID = ((Image)sender).Tag;
            Application.Current.Properties["noteId"] = reminderID;

            Application.Current.Properties["isNotTodoViewer"] = true;
            callEditReminder?.Invoke();
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

                var upcomingReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate >= DateTime.Today)
                    .OrderByDescending(x => x.Pinned)
                    .ThenBy(x => x.DueDate)
                    .ThenByDescending(x => x.Priority)
                    .ToList();
                var expiredReminders = _context.Reminders.Where(x => x.UserId == uid && x.DueDate < DateTime.Today)
                    .OrderByDescending(x => x.DueDate)
                    .ToList();
                RefreshReminders(upcomingReminders, expiredReminders);
            }
        }

        private void Reminders_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            TextboxTheme.calledReminder -= RefreshReminders;
        }
    }
}
