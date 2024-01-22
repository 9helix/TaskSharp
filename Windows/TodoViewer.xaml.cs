using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskSharp.Windows
{
    /// <summary>
    /// Interaction logic for TodoViewer.xaml
    /// </summary>
    public partial class TodoViewer : Window
    {
        private readonly NotesContext _context = new NotesContext();

        public TodoViewer()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            var uid = (int)Application.Current.Properties["uid"];
            int NoteId = (int)Application.Current.Properties["noteId"];

            var todos = _context.TodoLists.Where(x => x.UserId == uid && x.Id == NoteId).First();

            TodoName.Text = todos.Name;
            TodoTags.Text = todos.Tags;

            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(todos.Todos);
            TodoElements.ItemsSource = todoDict;

            foreach (var done in todoDict.Values)
            {
                if (!done)
                {
                    return;
                }
            }
            todos.Done = true;
            MessageBox.Show("To-do lista je obavljena!", "Obavljena to-do lista", MessageBoxButton.OK, MessageBoxImage.Information);
            _context.SaveChanges();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.TodoLists.Load();
            Refresh();
        }

        private void CheckUncheck(object sender, MouseButtonEventArgs e)
        {
            var todoID = (string)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            int NoteId = (int)Application.Current.Properties["noteId"];

            var todos = _context.TodoLists.Where(x => x.UserId == uid && x.Id == NoteId).First();
            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(todos.Todos);
            todoDict[todoID] = !todoDict[todoID];

            todos.Todos = JsonSerializer.Serialize(todoDict);
            _context.SaveChanges();
            Refresh();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dashboard = new DashboardTesting();
            dashboard.Show();
        }
    }
}
