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
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        private readonly NotesContext _context = new();

        public Page4()
        {
            InitializeComponent();
            TextboxTheme.calledTodo += RefreshTodos;
        }

        public void RefreshTodos(List<TodoList> undoneTodos, List<TodoList> doneTodos)
        {
            if (undoneTodos.Count == 0 && doneTodos.Count == 0)
            {
                Todos.Visibility = Visibility.Collapsed;
                TodosEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                TodosEmpty.Visibility = Visibility.Collapsed;
                Todos.Visibility = Visibility.Visible;

                if (undoneTodos.Count == 0)
                {
                    UndoneTodosContainer.Visibility = Visibility.Collapsed;
                    UndoneTodoEmpty.Visibility = Visibility.Visible;

                    DoneTodosContainer.Visibility = Visibility.Visible;
                    DoneTodoEmpty.Visibility = Visibility.Collapsed;
                    DoneTodosContainer.ItemsSource = doneTodos;
                }
                else if (doneTodos.Count == 0)
                {
                    UndoneTodosContainer.Visibility = Visibility.Visible;
                    UndoneTodoEmpty.Visibility = Visibility.Collapsed;
                    UndoneTodosContainer.ItemsSource = undoneTodos;

                    DoneTodosContainer.Visibility = Visibility.Collapsed;
                    DoneTodoEmpty.Visibility = Visibility.Visible;
                }
                else
                {
                    DoneTodosContainer.Visibility = Visibility.Visible;
                    DoneTodoEmpty.Visibility = Visibility.Collapsed;
                    DoneTodosContainer.ItemsSource = doneTodos;

                    UndoneTodosContainer.Visibility = Visibility.Visible;
                    UndoneTodoEmpty.Visibility = Visibility.Collapsed;
                    UndoneTodosContainer.ItemsSource = undoneTodos;
                }
            }
        }

        private void Todos_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.TodoLists.Load();

            var uid = (int)Application.Current.Properties["uid"];
            var undoneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == false)
                .OrderByDescending(x => x.Pinned)
                .ToList();
            var doneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == true)
                .ToList();
            RefreshTodos(undoneTodos, doneTodos);
        }

        private void PinUnpinTodo(object sender, MouseButtonEventArgs e)
        {
            var todoID = (int)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            var todo = _context.TodoLists.Where(x => x.UserId == uid && x.Id == todoID).First();

            todo.PinUnpin();
            _context.SaveChanges();

            var undoneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == false)
                .OrderByDescending(x => x.Pinned)
                .ToList();
            var doneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == true)
                .ToList();
            RefreshTodos(undoneTodos, doneTodos);
        }

        public delegate void EditHandlerTodo();
        public static event EditHandlerTodo callEditTodo;
        private void OpenEditor(object sender, MouseButtonEventArgs e)
        {
            var todoID = (int)((Image)sender).Tag;
            Application.Current.Properties["noteId"] = todoID;
            callEditTodo?.Invoke();
            /*
            var todoView = new TodoViewer();
            todoView.Show();

            var wnd = Window.GetWindow(this);
            wnd.Close();*/
        }

        private void DeleteTodo(object sender, MouseButtonEventArgs e)
        {
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati to-do listu?", "Brisanje to-do liste", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (choice == MessageBoxResult.Yes)
            {
                var todoID = (int)((Image)sender).Tag;
                var uid = (int)Application.Current.Properties["uid"];

                var todo = _context.TodoLists.Where(x => x.UserId == uid && x.Id == todoID).First();
                _context.TodoLists.Remove(todo);
                _context.SaveChanges();

                MessageBox.Show("To-do lista uspješno izbrisana!", "Brisanje to-do liste", MessageBoxButton.OK, MessageBoxImage.Information);

                var undoneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == false)
                    .OrderByDescending(x => x.Pinned)
                    .ToList();
                var doneTodos = _context.TodoLists.Where(x => x.UserId == uid && x.Done == true)
                    .ToList();
                RefreshTodos(undoneTodos, doneTodos);
            }
        }

        public delegate void TodoViewerEvent();
        public static event TodoViewerEvent callTodoViewer;
        private void ViewTodo(object sender, MouseButtonEventArgs e)
        {
            var todoID = ((StackPanel)sender).Tag;
            Application.Current.Properties["noteId"] = todoID;

            callTodoViewer?.Invoke();
        }
    }
}
