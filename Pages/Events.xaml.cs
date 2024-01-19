using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskSharp;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private readonly NotesContext _context =
            new NotesContext();

        public Page2()
        {
            InitializeComponent();
        }

        private void Events_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.Events.Load();

            var uid = (int)Application.Current.Properties["uid"];

            var username = _context.Users.Where(usr => usr.UserId == uid)
                .Select(usr => usr.UserName)
                .FirstOrDefault();
            var events = _context.Events.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.EndDate)
                .ToList();

            UsernameField.Text = username;

            if (events.Count == 0)
            {
                EventsContainer.Visibility = Visibility.Collapsed;
                EventsEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                EventsEmpty.Visibility = Visibility.Collapsed;
                EventsContainer.Visibility = Visibility.Visible;
                EventsContainer.ItemsSource = events;
            }
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

        private void RefreshEvents()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var events = _context.Events.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.CreationDate)
                .ToList();
            EventsContainer.ItemsSource = events;
        }

        private void PinUnpinEvent(object sender, MouseButtonEventArgs e)
        {
            var eventID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var ev = _context.Events.Where(x => x.UserId == uid && x.Id == eventID).First();

            ev.Pinned = !ev.Pinned;
            _context.SaveChanges();
            RefreshEvents();
        }

        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var eventID = ((Image)sender).Tag;
            Application.Current.Properties["noteType"] = 1;
            Application.Current.Properties["noteId"] = eventID;

            var eventEdit = new NoteEdit();
            eventEdit.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void DeleteEvent(object sender, MouseButtonEventArgs e)
        {
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati događaj?", "Brisanje događaja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (choice == MessageBoxResult.Yes)
            {
                var eventID = (int)((Image)sender).Tag;
                var uid = (int)Application.Current.Properties["uid"];

                var ev = _context.Events.Where(x => x.UserId == uid && x.Id == eventID).First();
                _context.Events.Remove(ev);
                _context.SaveChanges();

                MessageBox.Show("Događaj uspješno izbrisan!", "Brisanje događaja", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshEvents();
            }
        }

        private void Events_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }
    }
}
