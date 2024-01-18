using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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

        private void DebugNotes()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var notes = _context.Notes.Where(x => x.UserId == uid && x is Note).OrderByDescending(x => x.Pinned).ToList();
            foreach (var user in notes)
            {
                Debug.WriteLine($"BasenoteID: {user.Id}, UserID: {user.UserId}, datum kreiranja: {user.CreationDate}, name: {user.Name}, tags: {user.Tags}, pinned: {user.Pinned}, content: {user.Content}");
            }
        }

        private void Notes_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
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
            var noteID = ((Image)sender).Tag;
        }
    }
}
