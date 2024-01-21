using Microsoft.EntityFrameworkCore;
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

        private void RefreshEvents()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var upcomingEvents = _context.Events.Where(x => x.UserId == uid && x.EndDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.EndDate)
                .ToList();
            var expiredEvents = _context.Events.Where(x => x.UserId == uid && x.EndDate < DateTime.Today)
                .OrderByDescending(x => x.EndDate)
                .ToList();

            if (upcomingEvents.Count == 0 && expiredEvents.Count == 0)
            {
                Events.Visibility = Visibility.Collapsed;
                EventsEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                EventsEmpty.Visibility = Visibility.Collapsed;
                Events.Visibility = Visibility.Visible;

                if (upcomingEvents.Count == 0)
                {
                    UpcomingEventsContainer.Visibility = Visibility.Collapsed;
                    UpcomingEventsEmpty.Visibility = Visibility.Visible;

                    ExpiredEventsContainer.Visibility = Visibility.Visible;
                    ExpiredEventsEmpty.Visibility = Visibility.Collapsed;
                    ExpiredEventsContainer.ItemsSource = expiredEvents;
                }
                else if (expiredEvents.Count == 0)
                {
                    UpcomingEventsContainer.Visibility = Visibility.Visible;
                    UpcomingEventsEmpty.Visibility = Visibility.Collapsed;
                    UpcomingEventsContainer.ItemsSource = upcomingEvents;

                    ExpiredEventsContainer.Visibility = Visibility.Collapsed;
                    ExpiredEventsEmpty.Visibility = Visibility.Visible;
                }
                else
                {
                    ExpiredEventsContainer.Visibility = Visibility.Visible;
                    ExpiredEventsEmpty.Visibility = Visibility.Collapsed;
                    ExpiredEventsContainer.ItemsSource = expiredEvents;

                    UpcomingEventsContainer.Visibility = Visibility.Visible;
                    UpcomingEventsEmpty.Visibility = Visibility.Collapsed;
                    UpcomingEventsContainer.ItemsSource = upcomingEvents;
                }
            }
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

            RefreshEvents();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["noteType"] = 1;
            var noteCreate = new NoteCreate();
            noteCreate.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
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
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati događaj?", "Brisanje događaja", MessageBoxButton.YesNo, MessageBoxImage.Warning);

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
