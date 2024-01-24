using Microsoft.EntityFrameworkCore;
using SideBar_Nav.Pages;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskSharp.Classes;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private readonly NotesContext _context = new();

        public Dashboard()
        {
            InitializeComponent();
            _context.Database.EnsureCreated();
            _context.Users.Load();
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
            ReminderDuePick.SelectedDate = DateTime.Now;
            EventStartPick.SelectedDate = DateTime.Now;
            EventEndPick.SelectedDate = DateTime.Now;
            ToggleFields((int)Application.Current.Properties["noteType"]);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Create_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var noteCreate = new NoteCreate();
            noteCreate.Show();
            this.Close();
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Path_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var choice = MessageBox.Show("Jeste li sigurni da želite izbrisati vaš račun? Time ćete pobrisati i sve svoje spremljene bilješke.", "Brisanje računa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (choice == MessageBoxResult.Yes)
            {
                var uid = (int)Application.Current.Properties["uid"];
                _context.Notes.Load();
                _context.Events.Load();
                _context.Reminders.Load();
                _context.TodoLists.Load();

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

        List<int> todoNums = new List<int> { 1 };
        private void AddTodo(object sender, RoutedEventArgs e)
        {
            todoNums.Add(todoNums.Last() + 1);
            StackPanel stk = new StackPanel
            {
                Name = $"todo{todoNums.Last()}",
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Separator sep = new Separator
            {
                Name = $"septodo{todoNums.Last()}",
                Width = 10,
                Background = Brushes.Transparent
            };
            TextBox txt = new TextBox
            {
                Margin = new Thickness(left: 0, top: 0, right: 0, bottom: 0),
                MaxLength = 30,
                Width = 150,
                Text = $"Todo #{todoNums.Last()}",
                //Style = null
            };
            Border border = new Border
            {
                Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 5)
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
            stk.Children.Add(sep);

            stk.Children.Add(img);
            border.Child = stk;
            TodoList.Children.Add(border);
        }

        private void DeleteTodo(object sender, MouseButtonEventArgs e)
        {
            StackPanel stk = (StackPanel)(sender as Image).Parent;
            Border toDelete = (Border)stk.Parent;
            todoNums.RemoveAt(1);

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

        private void DialogHost_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

            if (!Equals(eventArgs.Parameter, true))
                return;

            //if (!string.IsNullOrWhiteSpace(FruitTextBox.Text))
            int x = SaveNote();
            if (x == -1)
                diHost.IsOpen = true;

        }

        private int SaveNote()
        {
            int index = NoteTypeMenu.SelectedIndex;
            string name = note_name.Text;
            if (name == "")
            {
                MessageBox.Show("Zapis mora imati ime.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
            string tags = this.tags.Text;
            bool Pin = flag.IsChecked.Value;

            Debug.WriteLine($"{name}-{tags}-{Pin}");
            DateTime dateCreate = DateTime.Now;
            int uid = (int)Application.Current.Properties["uid"];
            //ushort id = GenerateNoteId();

            switch (index)
            {
                case 0: //biljeska
                    string content = this.content.Text;
                    Debug.WriteLine(content);
                    if (content == "")
                    {
                        MessageBox.Show("Sadržaj ne smije biti prazan.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    Note newNote = new Note
                    {
                        UserId = uid,
                        Name = name,
                        CreationDate = dateCreate,
                        Tags = tags,
                        Content = content,
                        Pinned = Pin
                    };
                    _context.Notes.Add(newNote);
                    break;

                case 1: //događaj
                    var StartEvent = EventStartPick.SelectedDate;
                    var EndEvent = EventEndPick.SelectedDate;

                    if (StartEvent is not DateTime || EndEvent is not DateTime)
                    {
                        MessageBox.Show("Datumi moraju biti odabrani.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    if (EndEvent < StartEvent)
                    {
                        MessageBox.Show("Datum kraja događaja ne smije biti manji od datuma početka događaja.", "Događaj greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    string location = this.location.Text;
                    Debug.WriteLine($"{StartEvent}-{EndEvent}-{location}");
                    Event newEvent = new Event
                    {
                        UserId = uid,
                        Name = name,
                        CreationDate = dateCreate,
                        Tags = tags,
                        StartDate = (DateTime)StartEvent,
                        EndDate = (DateTime)EndEvent,
                        Location = location,
                        Pinned = Pin,
                        DeadlineNotification = true,
                        ExpiredNotification = true
                    };
                    _context.Events.Add(newEvent);
                    break;

                case 2: //podsjetnik
                    var dueDate = ReminderDuePick.SelectedDate;
                    if (dueDate is not DateTime)
                    {
                        MessageBox.Show("Datum mora biti odabran.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    int PriorityIndex = PriorityMenuPick.SelectedIndex;
                    ReminderPriority priority = (ReminderPriority)PriorityIndex;
                    Debug.WriteLine($"{dueDate}-{priority}");
                    Reminder newReminder = new Reminder
                    {
                        UserId = uid,
                        Name = name,
                        CreationDate = dateCreate,
                        Tags = tags,
                        Priority = priority,
                        DueDate = (DateTime)dueDate,
                        Pinned = Pin,
                        Notification = true
                    };
                    _context.Reminders.Add(newReminder);
                    break;

                case 3://todo
                    var todos = TodoList.Children;
                    Dictionary<string, bool> TodoDict = new();
                    foreach (var child in todos)
                    {
                        Debug.WriteLine(child);
                        var todo = ((child as Border).Child as StackPanel).Children;
                        TodoDict[(todo[0] as TextBox).Text] = false;

                        if ((todo[0] as TextBox).Text == "")
                        {
                            MessageBox.Show("To-Do stavka ne može biti prazna.", "Greška u stvaranju", MessageBoxButton.OK, MessageBoxImage.Error);
                            return -1;
                        }
                    }
                    TodoList newTodoList = new TodoList
                    {
                        UserId = uid,
                        Name = name,
                        CreationDate = dateCreate,
                        Tags = tags,
                        Todos = JsonSerializer.Serialize(TodoDict),
                        Pinned = Pin,
                        Done = false
                    };
                    _context.TodoLists.Add(newTodoList);
                    break;
            }
            _context.SaveChanges();

            MessageBox.Show("Zapis uspješno stvoren!", "Stvaranje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);
            return 0;
            //this.Close();
        }
        private void CleanAddNote()
        {
            note_name.Text = "";
            tags.Text = "";
            flag.IsChecked = false;
            content.Text = "";
            ReminderDuePick.SelectedDate = DateTime.Now;
            EventStartPick.SelectedDate = DateTime.Now;
            EventEndPick.SelectedDate = DateTime.Now;
            location.Text = "";
            (PriorityMenuPick.Items[0] as ComboBoxItem).IsSelected = true;

            TodoList.Children.Clear();

            todoNums = new List<int> { 1 };
            StackPanel stk = new StackPanel
            {

            };

            TextBox txt = new TextBox
            {
                Margin = new Thickness(left: 0, top: 0, right: 30, bottom: 0),
                MaxLength = 30,
                Width = 150,
                Text = $"Todo #1",
                //Style = null
            };
            Border border = new Border
            {
                Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 5)
            };

            stk.Children.Add(txt);
            border.Child = stk;
            TodoList.Children.Add(border);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            diHost.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int x = SaveNote();
            if (x == -1)
            {
                diHost.IsOpen = true;
                CleanAddNote();
            }
            else diHost.IsOpen = false;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            diHost.IsOpen = false;
            CleanAddNote();
        }
    }
}
