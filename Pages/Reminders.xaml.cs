using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                    UpcomingRemindersContainer.ItemsSource = upcomingReminders;
                }
            }
        }

        private void Reminders_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.Reminders.Load();

            var uid = (int)Application.Current.Properties["uid"];
            var username = _context.Users.Where(usr => usr.UserId == uid)
                .Select(usr => usr.UserName)
                .FirstOrDefault();

            UsernameField.Text = username;
            RefreshReminders();
        }

        private void DebugNotes()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var notes = _context.Notes.Where(x => x.UserId == uid).OrderByDescending(x => x.Pinned).ToList();
            foreach (var user in notes)
            {
                Debug.WriteLine($"BasenoteID: {user.Id}, UserID: {user.UserId}, datum kreiranja: {user.CreationDate}, name: {user.Name}, tags: {user.Tags}, pinned: {user.Pinned}, content: {user.Content}");
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["noteType"] = 2;
            var noteCreate = new NoteCreate();
            noteCreate.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void PinUnpinEvent(object sender, MouseButtonEventArgs e)
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

                var reminder = _context.Events.Where(x => x.UserId == uid && x.Id == reminderID).First();
                _context.Events.Remove(reminder);
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
