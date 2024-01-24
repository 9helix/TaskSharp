using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskSharp;
using TaskSharp.Classes;
using TaskSharp.Themes;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private readonly NotesContext _context = new();

        public Page2()
        {
            InitializeComponent();
            TextboxTheme.calledEvent += RefreshEvents;
        }

        public void NotificationChecker(Event row, bool isDeadline)
        {
            if (isDeadline) // if event still ongoing
            {
                if (row.DeadlineNotification && (row.EndDate == DateTime.Today))
                {
                    Color color = (Color)ColorConverter.ConvertFromString("#fa7f05");
                    var notificationManager = new NotificationManager();
                    notificationManager.Show(new NotificationContent
                    {
                        Title = row.Name,
                        Message = "Događaj završava danas!",
                        Type = NotificationType.Information,
                        CloseOnClick = true, // closes message when message is clicked
                        Background = new SolidColorBrush(color)
                    });

                    row.DeadlineNotification = false;
                    _context.SaveChanges();
                }
            }
            else
            { // if event was completed
                if (row.ExpiredNotification && (row.EndDate < DateTime.Today))
                {
                    var notificationManager = new NotificationManager();
                    notificationManager.Show(new NotificationContent
                    {
                        Title = row.Name,
                        Message = "Događaj je završen!",
                        Type = NotificationType.Information,
                        CloseOnClick = true, // closes message when message is clicked
                    });

                    row.ExpiredNotification = false;
                    _context.SaveChanges();
                }
            }
        }

        private void RefreshEvents(
            List<Event> upcomingEvents,
            List<Event> expiredEvents)
        {
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

                    foreach (var ev in expiredEvents)
                    {
                        NotificationChecker(ev, false);
                    }
                    ExpiredEventsContainer.ItemsSource = expiredEvents;
                }
                else if (expiredEvents.ToList().Count == 0)
                {
                    UpcomingEventsContainer.Visibility = Visibility.Visible;
                    UpcomingEventsEmpty.Visibility = Visibility.Collapsed;

                    foreach (var ev in upcomingEvents)
                    {
                        NotificationChecker(ev, true);
                    }
                    UpcomingEventsContainer.ItemsSource = upcomingEvents;

                    ExpiredEventsContainer.Visibility = Visibility.Collapsed;
                    ExpiredEventsEmpty.Visibility = Visibility.Visible;
                }
                else
                {
                    ExpiredEventsContainer.Visibility = Visibility.Visible;
                    ExpiredEventsEmpty.Visibility = Visibility.Collapsed;

                    foreach (var ev in expiredEvents)
                    {
                        NotificationChecker(ev, false);
                    }
                    ExpiredEventsContainer.ItemsSource = expiredEvents;

                    UpcomingEventsContainer.Visibility = Visibility.Visible;
                    UpcomingEventsEmpty.Visibility = Visibility.Collapsed;

                    foreach (var ev in upcomingEvents)
                    {
                        NotificationChecker(ev, true);
                    }
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
            var upcomingEvents = _context.Events
                .Where(x => x.UserId == uid && x.EndDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.EndDate)
                .ToList();
            var expiredEvents = _context.Events
                .Where(x => x.UserId == uid && x.EndDate < DateTime.Today)
                .OrderByDescending(x => x.EndDate)
                .ToList();
            RefreshEvents(upcomingEvents, expiredEvents);
        }

        private void PinUnpinEvent(object sender, MouseButtonEventArgs e)
        {
            var eventID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var ev = _context.Events.Where(x => x.UserId == uid && x.Id == eventID).First();

            ev.Pinned = !ev.Pinned;
            _context.SaveChanges();

            var upcomingEvents = _context.Events
                .Where(x => x.UserId == uid && x.EndDate >= DateTime.Today)
                .OrderByDescending(x => x.Pinned)
                .ThenBy(x => x.EndDate)
                .ToList();
            var expiredEvents = _context.Events
                .Where(x => x.UserId == uid && x.EndDate < DateTime.Today)
                .OrderByDescending(x => x.EndDate)
                .ToList();
            RefreshEvents(upcomingEvents, expiredEvents);
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

                var upcomingEvents = _context.Events.Where(x => x.UserId == uid && x.EndDate >= DateTime.Today)
                    .OrderByDescending(x => x.Pinned)
                    .ThenBy(x => x.EndDate)
                    .ToList();
                var expiredEvents = _context.Events.Where(x => x.UserId == uid && x.EndDate < DateTime.Today)
                    .OrderByDescending(x => x.EndDate)
                    .ToList();
                RefreshEvents(upcomingEvents, expiredEvents);
            }
        }

        private void Events_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }
    }
}
