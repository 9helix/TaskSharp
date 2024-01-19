using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskSharp;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        private readonly NotesContext _context =
            new NotesContext();

        public Page1()
        {
            InitializeComponent();
        }

        private void Notes_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.Notes.Load();

            var uid = (int)Application.Current.Properties["uid"];

            var username = _context.Users.Where(usr => usr.UserId == uid)
                .Select(usr => usr.UserName)
                .FirstOrDefault();
            var notes = _context.Notes.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.CreationDate)
                .ToList();

            UsernameField.Text = username;
            
            if (notes.Count == 0)
            {
                NotesContainer.Visibility = Visibility.Collapsed;
                NotesEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                NotesEmpty.Visibility = Visibility.Collapsed;
                NotesContainer.Visibility = Visibility.Visible;
                NotesContainer.ItemsSource = notes;
            }
        }

        private void RefreshNotes()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var notes = _context.Notes.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.CreationDate)
                .ToList();
            NotesContainer.ItemsSource = notes;
        }

        private void PinUnpinNote(object sender, MouseButtonEventArgs e)
        {
            var noteID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var note = _context.Notes.Where(x => x.UserId == uid && x.Id == noteID).First();

            note.Pinned = !note.Pinned;
            _context.SaveChanges();
            RefreshNotes();
        }

        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var noteID = ((Image)sender).Tag;
            Application.Current.Properties["noteType"] = 0;
            Application.Current.Properties["noteId"] = noteID;

            var noteEdit = new NoteEdit();
            noteEdit.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void DeleteNote(object sender, MouseButtonEventArgs e)
        {
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati bilješku?", "Brisanje bilješke", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (choice == MessageBoxResult.Yes)
            {
                var noteID = (int)((Image)sender).Tag;
                var uid = (int)Application.Current.Properties["uid"];

                var note = _context.Notes.Where(x => x.UserId == uid && x.Id == noteID).First();
                _context.Notes.Remove(note);
                _context.SaveChanges();

                MessageBox.Show("Bilješka uspješno izbrisana!", "Brisanje bilješke", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshNotes();
            }
        }

        private void Notes_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }
    }
}
