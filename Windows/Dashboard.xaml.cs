using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private readonly NotesContext _context =
    new NotesContext();
        public Dashboard()
        {
            InitializeComponent();
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.Notes.Load();
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            navframe.Navigate(new Uri("../Pages/Notes.xaml", UriKind.Relative));

            var uid = (int)Application.Current.Properties["uid"];
            var username = _context.Users.Where(usr => usr.UserId == uid)
                            .Select(usr => usr.UserName)
                            .First();
            userChip.Inlines.Add(new Run("Korisnik: "));
            userChip.Inlines.Add(new Bold(new Run($"{username}")));
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            switch (lb.SelectedIndex)
            {
                case 0:
                    Application.Current.Properties["noteType"] = 0;
                    navframe.Navigate(new Uri("../Pages/Notes.xaml", UriKind.Relative));
                    break;

                case 1:
                    Application.Current.Properties["noteType"] = 1;
                    navframe.Navigate(new Uri("../Pages/Events.xaml", UriKind.Relative));
                    break;

                case 2:
                    Application.Current.Properties["noteType"] = 2;
                    navframe.Navigate(new Uri("../Pages/Reminders.xaml", UriKind.Relative));
                    break;

                case 3:
                    Application.Current.Properties["noteType"] = 3;
                    navframe.Navigate(new Uri("../Pages/TodoLists.xaml", UriKind.Relative));
                    break;

                case 4:
                    var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati vaš račun? Time ćete pobrisati i sve svoje spremljene bilješke.", "Brisanje računa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (choice == MessageBoxResult.Yes)
                    {
                        var uid = (int)Application.Current.Properties["uid"];

                        var notes = _context.Notes.Where(x => x.UserId == uid).ToList();
                        _context.Notes.RemoveRange(notes);
                        var events = _context.Events.Where(x => x.UserId == uid).ToList();
                        _context.Events.RemoveRange(events);
                        var reminders = _context.Reminders.Where(x => x.UserId == uid).ToList();
                        _context.Reminders.RemoveRange(reminders);
                        var todos = _context.TodoLists.Where(x => x.UserId == uid).ToList();
                        _context.TodoLists.RemoveRange(todos);

                        var user = _context.Users.Where(x => x.UserId == uid).First();
                        _context.Users.Remove(user);
                        _context.SaveChanges();

                        MessageBox.Show("Uspješno izbrisan korisnički račun!", "Brisanje korisničkog računa", MessageBoxButton.OK, MessageBoxImage.Information);

                        var win2 = new MainWindow();
                        win2.Show();
                        this.Close();
                    }
                    break;

                case 5:
                    MessageBox.Show($"Odjava uspješna!", "Odjava", MessageBoxButton.OK, MessageBoxImage.Information);
                    var win = new MainWindow();
                    win.Show();

                    this.Close();
                    break;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var noteCreate = new NoteCreate();
            noteCreate.Show();
            this.Close();
        }

        private void Dashboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }
    }
}
