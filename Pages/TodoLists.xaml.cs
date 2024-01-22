using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskSharp;
using TaskSharp.Windows;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        private readonly NotesContext _context =
            new NotesContext();

        public Page4()
        {
            InitializeComponent();
        }

        private void Todos_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.TodoLists.Load();

            var uid = (int)Application.Current.Properties["uid"];
            var todos = _context.TodoLists.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ToList();
            
            UndoneTodosContainer.ItemsSource = todos;
        }

        private void DebugNotes()
        {
            var uid = (int)Application.Current.Properties["uid"];
            var todos = _context.TodoLists.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.Pinned)
                .ToList();
            foreach (var user in todos)
            {
                Debug.WriteLine($"BasenoteID: {user.Id}, UserID: {user.UserId}, datum kreiranja: {user.CreationDate}, name: {user.Name}, tags: {user.Tags}, pinned: {user.Pinned}, todos: {user.Todos}");
                var x = JsonSerializer.Deserialize<Dictionary<string, bool>>(user.Todos);
                foreach (var test in x)
                {
                    Debug.WriteLine($"{test}");
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["noteType"] = 3;
            var noteCreate = new NoteCreate();
            noteCreate.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void PinUnpinTodo(object sender, MouseButtonEventArgs e)
        {
            var todoID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var todo = _context.TodoLists.Where(x => x.UserId == uid && x.Id == todoID).First();

            todo.Pinned = !todo.Pinned;
            _context.SaveChanges();
            //RefreshTodos();
        }

        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var todoID = ((Image)sender).Tag;
            Application.Current.Properties["noteType"] = 3;
            Application.Current.Properties["noteId"] = todoID;

            var todoEdit = new NoteEdit();
            todoEdit.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void DeleteTodo(object sender, MouseButtonEventArgs e)
        {
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati to-do listu?", "Brisanje to-do liste", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (choice == MessageBoxResult.Yes)
            {
                var TodoID = (int)((Image)sender).Tag;
                var uid = (int)Application.Current.Properties["uid"];

                var todo = _context.TodoLists.Where(x => x.UserId == uid && x.Id == TodoID).First();
                _context.TodoLists.Remove(todo);
                _context.SaveChanges();

                MessageBox.Show("To-do lista uspješno izbrisana!", "Brisanje to-do liste", MessageBoxButton.OK, MessageBoxImage.Information);
                //RefreshTodos();
            }
        }

        private void ViewTodo(object sender, MouseButtonEventArgs e)
        {
            var todoID = (int)((StackPanel)sender).Tag;
            Application.Current.Properties["noteId"] = todoID;

            var todoView = new TodoViewer();
            todoView.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();
        }

        private void Todos_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }
    }
}
