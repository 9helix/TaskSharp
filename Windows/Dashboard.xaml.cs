using Microsoft.EntityFrameworkCore;
using SideBar_Nav.Pages;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
            //NoteView();

            var uid = (int)Application.Current.Properties["uid"];
            var username = _context.Users.Where(usr => usr.UserId == uid)
                            .Select(usr => usr.UserName)
                            .First();
            userChip.Text = $"{username}";
            if (Application.Current.Properties["noteType"] == null || (int)Application.Current.Properties["noteType"] == 0)
                p1.IsChecked = true;
            else
            {
                if ((int)Application.Current.Properties["noteType"] == 1)
                    p2.IsChecked = true;
                if ((int)Application.Current.Properties["noteType"] == 2)
                    p3.IsChecked = true;
                if ((int)Application.Current.Properties["noteType"] == 3)
                    p4.IsChecked = true;
            }
            ToggleFields((int)Application.Current.Properties["noteType"]);
        }
        private void NoteChecked(object sender, RoutedEventArgs e)
        {
            string x = ((RadioButton)sender).Name;
            switch (x)
            {
                case "p1":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/note-white.png";
                    noteTitle.Text = "Bilješke";
                    Application.Current.Properties["noteType"] = 0;
                    navframe.Navigate(new Page1());
                    break;

                case "p2":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/event-white.png";
                    noteTitle.Text = "Događaji";
                    Application.Current.Properties["noteType"] = 1;
                    navframe.Navigate(new Page2());
                    break;
                case "p3":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/reminder-white.png";
                    noteTitle.Text = "Podsjetnici";
                    Application.Current.Properties["noteType"] = 2;
                    navframe.Navigate(new Page3());
                    break;
                case "p4":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/todo-white.png";
                    noteTitle.Text = "To-Do liste";
                    Application.Current.Properties["noteType"] = 3;
                    navframe.Navigate(new Page4());
                    break;

            }
            ToggleFields((int)Application.Current.Properties["noteType"]);

        }


        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            DragMove();
        }


        private void Create_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var noteCreate = new NoteCreate();
            noteCreate.Show();
            this.Close();
        }

        private void Path_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            var choice2 = MessageBox.Show("Jeste li sigurni da se želite odjaviti?", "Odjava", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (choice2 == MessageBoxResult.Yes)
            {
                MessageBox.Show($"Odjava uspješna!", "Odjava", MessageBoxButton.OK, MessageBoxImage.Information);
                var win = new Login();
                win.Show();

                this.Close();
            }
        }

        private void Path_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
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

                var win2 = new Login();
                win2.Show();
                this.Close();
            }
        }

        private void NoteUnchecked(object sender, RoutedEventArgs e)
        {
            string x = ((RadioButton)sender).Name;
            switch (x)
            {
                case "p1":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/note.png";
                    break;

                case "p2":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/event.png";
                    break;
                case "p3":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/reminder.png";
                    break;
                case "p4":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/todo.png";
                    break;

            }
        }
        private void Dashboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        List<int> todoNums = new List<int>(1);
        private void AddTodo(object sender, RoutedEventArgs e)
        {
            todoNums.Add(todoNums.Last() + 1);
            StackPanel stk = new StackPanel
            {
                Name = $"todo{todoNums.Last()}",
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBox txt = new TextBox
            {
                Margin = new Thickness(left: 15, top: 0, right: 0, bottom: 0),
                MaxLength = 30,
                Width = 175,
                Text = $"Todo #{todoNums.Last()}",
                Style = null
            };
            Image img = new Image
            {
                Width = 15,
                Source = new BitmapImage(new Uri(@"/Resources/Images/deleteRed.png", UriKind.Relative)),
                Cursor = Cursors.Hand,
                ToolTip = new ToolTip() { Content = "Izbriši stavku" }
            };
            img.PreviewMouseDown += new MouseButtonEventHandler(DeleteTodo);
            stk.Children.Add(txt);
            stk.Children.Add(img);
            TodoList.Items.Add(new ListBoxItem { Content = stk });

        }
        private void DeleteTodo(object sender, MouseButtonEventArgs e)
        {
            StackPanel stk = (StackPanel)(sender as Image).Parent;
            Border toDelete = (Border)stk.Parent;
            todoNums.RemoveAt(1);

            Debug.WriteLine(toDelete.Name);
            StackPanel par = (StackPanel)toDelete.Parent;
            par.Children.Remove(toDelete);
        }

        private void ToggleFields(int type)
        {
            if (type != 0)
            {
                NoteContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoteContent.Visibility = Visibility.Visible;
                NoteSelected.IsSelected = true;
            }
            if (type != 1)
            {
                EventStart.Visibility = Visibility.Collapsed;
                EventEnd.Visibility = Visibility.Collapsed;
                txtLocation.Visibility = Visibility.Collapsed;
            }
            else
            {
                EventStart.Visibility = Visibility.Visible;
                EventEnd.Visibility = Visibility.Visible;
                txtLocation.Visibility = Visibility.Visible;
                EventSelected.IsSelected = true;
            }
            if (type != 2)
            {
                ReminderDue.Visibility = Visibility.Collapsed;
                PriorityMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReminderDue.Visibility = Visibility.Visible;
                PriorityMenu.Visibility = Visibility.Visible;
                ReminderSelected.IsSelected = true;
            }
            if (type != 3)
            {
                TodoList.Visibility = Visibility.Collapsed;
                todoBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                TodoList.Visibility = Visibility.Visible;
                todoBtn.Visibility = Visibility.Visible;

                TodoSelected.IsSelected = true;
            }
        }
    }
}
