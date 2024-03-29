﻿using Microsoft.EntityFrameworkCore;
using SideBar_Nav.Pages;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskSharp.Classes;
using TaskSharp.Themes;

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
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            Notes.callEditNote += Edit;
            Events.callEditEvent += Edit;
            Reminders.callEditReminder += Edit;
            TodoLists.callEditTodo += Edit;
            TodoLists.callTodoViewer += ViewTodo;
            _context.Database.EnsureCreated();
            _context.Users.Load();

            var uid = (int)Application.Current.Properties["uid"];
            var username = _context.Users.Where(usr => usr.UserId == uid)
                            .Select(usr => usr.UserName)
                            .First();
            userChip.Text = username;

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
                    navframe.Navigate(new Notes());
                    break;

                case "p2":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/event-white.png";
                    noteTitle.Text = "Događaji";
                    Application.Current.Properties["noteType"] = 1;
                    navframe.Navigate(new Events());
                    break;
                case "p3":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/reminder-white.png";
                    noteTitle.Text = "Podsjetnici";
                    Application.Current.Properties["noteType"] = 2;
                    navframe.Navigate(new Reminders());
                    break;
                case "p4":
                    ((RadioButton)sender).Tag = "/Resources/Images/Notes/todo-white.png";
                    noteTitle.Text = "To-Do liste";
                    Application.Current.Properties["noteType"] = 3;
                    navframe.Navigate(new TodoLists());
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

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show($"Odjava uspješna!", "Odjava", MessageBoxButton.OK, MessageBoxImage.Information);
            var win = new Login();
            win.Show();
            Close();
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
                Close();
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

        List<int> todoNums = new() { 1 };
        private void AddTodo(object sender, RoutedEventArgs e)
        {
            AddTodo2();
        }

        private void DeleteTodo(object sender, MouseButtonEventArgs e)
        {
            StackPanel stk = (StackPanel)(sender as Image).Parent;
            Border toDelete = (Border)stk.Parent;
            todoNums.RemoveAt(1);

            StackPanel par = (StackPanel)toDelete.Parent;
            par.Children.Remove(toDelete);
        }

        bool editing = false;
        private void ToggleFields(int type)
        {
            NoteName.Visibility = Visibility.Visible;
            Tags.Visibility = Visibility.Visible;
            Flag.Visibility = Visibility.Visible;

            todoName.Visibility = Visibility.Collapsed;
            todoCreationDate.Visibility = Visibility.Collapsed;
            todoTags.Visibility = Visibility.Collapsed;
            itemTxt.Visibility = Visibility.Collapsed;
            todoScroll.Visibility = Visibility.Collapsed;

            if (type != 0)
            {
                NoteContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoteContent.Visibility = Visibility.Visible;
                if (!editing)
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
                if (!editing)
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
                if (!editing)
                    ReminderSelected.IsSelected = true;
            }
            if (type != 3)
            {
                TodoList.Visibility = Visibility.Collapsed;
                itemTxt.Visibility = Visibility.Collapsed;
                todoBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                TodoList.Visibility = Visibility.Visible;
                itemTxt.Visibility = Visibility.Visible;
                todoBtn.Visibility = Visibility.Visible;
                if (!editing)
                    TodoSelected.IsSelected = true;
            }
        }

        private void ToggleNavbar()
        {
            p1.IsHitTestVisible = !p1.IsHitTestVisible;
            p2.IsHitTestVisible = !p2.IsHitTestVisible;
            p3.IsHitTestVisible = !p3.IsHitTestVisible;
            p4.IsHitTestVisible = !p4.IsHitTestVisible;
        }

        private void DialogHost_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            ToggleNavbar();
            if (!Equals(eventArgs.Parameter, true))
                return;

            bool x = SaveNote();
            if (!x)
            {
                ToggleNavbar();
                diHost.IsOpen = true;
            }
        }

        private bool SaveNote()
        {
            int index = NoteTypeMenu.SelectedIndex;
            string name = note_name.Text;
            if (name == "")
            {
                MessageBox.Show("Zapis mora imati ime.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            string tags = this.tags.Text;
            bool pin = flag.IsChecked.Value;

            int uid = (int)Application.Current.Properties["uid"];
            var noteId = Application.Current.Properties["noteId"];
            switch (index)
            {
                case 0: // note
                    string content = this.content.Text;
                    if (content == "")
                    {
                        MessageBox.Show("Sadržaj ne smije biti prazan.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    if (!editing)
                    {
                        Note newNote = new(name, tags, pin, uid, content);
                        _context.Notes.Add(newNote);
                    }
                    else
                    {
                        var note1 = _context.Notes.Where(nt => nt.Id == (int)noteId).First();
                        note1.Update(name, tags, content);
                    }
                    break;

                case 1: // event
                    var startEvent = EventStartPick.SelectedDate;
                    var endEvent = EventEndPick.SelectedDate;

                    if (startEvent is not DateTime || endEvent is not DateTime)
                    {
                        MessageBox.Show("Datumi moraju biti odabrani.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    if (endEvent < startEvent)
                    {
                        MessageBox.Show("Datum kraja događaja ne smije biti manji od datuma početka događaja.", "Greška odabira datuma", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    string location = this.location.Text;

                    if (!editing)
                    {
                        Event newEvent = new(name, tags, pin, uid, (DateTime)startEvent, (DateTime)endEvent, location);
                        _context.Events.Add(newEvent);
                    }
                    else
                    {
                        var note2 = _context.Events.Where(nt => nt.Id == (int)noteId).First();
                        note2.Update(name, tags, (DateTime)startEvent, (DateTime)endEvent, location);
                    }
                    break;

                case 2: // reminder
                    var dueDate = ReminderDuePick.SelectedDate;
                    if (dueDate is not DateTime)
                    {
                        MessageBox.Show("Datum mora biti odabran.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    int PriorityIndex = PriorityMenuPick.SelectedIndex;
                    ReminderPriority priority = (ReminderPriority)PriorityIndex;

                    if (!editing)
                    {
                        Reminder newReminder = new(name, tags, pin, uid, (DateTime)dueDate, priority);
                        _context.Reminders.Add(newReminder);
                    }
                    else
                    {
                        var note3 = _context.Reminders.Where(nt => nt.Id == (int)noteId).First();
                        note3.Update(name, tags, (DateTime)dueDate, priority);
                    }
                    break;

                case 3: // todo
                    var todos = TodoList.Children;
                    Dictionary<string, bool> todoDict = new();

                    foreach (var child in todos)
                    {
                        var todo = ((child as Border).Child as StackPanel).Children;
                        todoDict[(todo[0] as TextBox).Text] = false;

                        if ((todo[0] as TextBox).Text == "")
                        {
                            MessageBox.Show("To-Do stavka ne može biti prazna.", "Greška u stvaranju", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    if (!editing)
                    {
                        TodoList newTodoList = new(name, tags, pin, uid, JsonSerializer.Serialize(todoDict));
                        _context.TodoLists.Add(newTodoList);
                    }
                    else
                    {
                        var note4 = _context.TodoLists.Where(nt => nt.Id == (int)noteId).First();
                        note4.Update(name, tags, JsonSerializer.Serialize(todoDict), false);
                    }
                    break;
            }

            _context.SaveChanges();
            if (!editing)
                MessageBox.Show("Zapis uspješno stvoren!", "Stvaranje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Zapis uspješno uređen!", "Uređivanje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
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
            todoElements.Children.Clear();

            todoNums = new List<int> { 1 };
            StackPanel stk = new();

            TextBox txt = new()
            {
                Margin = new Thickness(left: 0, top: 0, right: 30, bottom: 0),
                MaxLength = 30,
                Width = 150,
                Text = $"Todo #1",
            };
            Border border = new()
            {
                Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 15)
            };

            stk.Children.Add(txt);
            border.Child = stk;
            TodoList.Children.Add(border);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["isSaveNote"] = true;
            Application.Current.Properties["isNotTodoViewer"] = true;
            diHost.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var uid = (int)Application.Current.Properties["uid"];
            var noteType = (int)Application.Current.Properties["noteType"];

            if ((bool)Application.Current.Properties["isSaveNote"])
            { // if note was created or edited
                bool x = SaveNote();
                if (!x)
                {
                    diHost.IsOpen = true;
                }
                else
                {
                    diHost.IsOpen = false;
                    editing = false;
                }
            }
            else // perform saving changes of to-do elements
            {
                var todos = todoElements.Children;
                var noteId = (int)Application.Current.Properties["noteId"];
                Dictionary<string, bool> todoDict = new();

                foreach (var child in todos)
                {
                    var todo = (child as StackPanel).Children;
                    todoDict[(todo[1] as TextBlock).Text] = (todo[2] as CheckBox).IsChecked.Value;
                }

                var selectedTodo = _context.TodoLists.Where(nt => nt.UserId == uid && nt.Id == noteId).First();
                selectedTodo.SaveTodoElements(todoDict);

                _context.SaveChanges();
                diHost.IsOpen = false;
            }
            RefreshData(noteType, uid);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            diHost.IsOpen = false;
            editing = false;
        }

        private void RefreshData(int noteType, int uid)
        {
            switch (noteType)
            { // refresh notes immediately after creating or editing them
                case 0:
                    _context.Notes.Load();
                    var notes = _context.Notes
                        .Where(x => x.UserId == uid)
                        .OrderByDescending(x => x.Pinned)
                        .ThenByDescending(x => x.CreationDate)
                        .ToList();

                    TextboxTheme.CallNoteRefresher(notes);
                    break;

                case 1:
                    _context.Events.Load();

                    var upcomingEvents = _context.Events
                        .Where(x => x.UserId == uid && x.EndDate >= DateTime.Today)
                        .OrderByDescending(x => x.Pinned)
                        .ThenBy(x => x.EndDate)
                        .ToList();
                    var expiredEvents = _context.Events
                        .Where(x => x.UserId == uid && x.EndDate < DateTime.Today)
                        .OrderByDescending(x => x.EndDate)
                        .ToList();

                    TextboxTheme.CallEventRefresher(upcomingEvents, expiredEvents);
                    break;

                case 2:
                    _context.Reminders.Load();

                    var upcomingReminders = _context.Reminders
                        .Where(x => x.UserId == uid && x.DueDate >= DateTime.Today)
                        .OrderByDescending(x => x.Pinned)
                        .ThenBy(x => x.DueDate)
                        .ThenByDescending(x => x.Priority)
                        .ToList();
                    var expiredReminders = _context.Reminders
                        .Where(x => x.UserId == uid && x.DueDate < DateTime.Today)
                        .OrderByDescending(x => x.DueDate)
                        .ToList();

                    TextboxTheme.CallReminderRefresher(upcomingReminders, expiredReminders);
                    break;

                case 3:
                    _context.TodoLists.Load();

                    var undoneTodos = _context.TodoLists
                        .Where(x => x.UserId == uid && x.Done == false)
                        .OrderByDescending(x => x.Pinned)
                        .ToList();
                    var doneTodos = _context.TodoLists
                        .Where(x => x.UserId == uid && x.Done == true)
                        .ToList();

                    TextboxTheme.CallTodoRefresher(undoneTodos, doneTodos);
                    break;
            }
        }

        private void Edit()
        {
            editing = true;
            int noteId = (int)Application.Current.Properties["noteId"];
            int noteType = (int)Application.Current.Properties["noteType"];

            switch (noteType)
            {
                case 0:
                    var temp1 = _context.Notes.Where(nt => nt.Id == noteId).First();

                    note_name.Text = temp1.Name;
                    tags.Text = temp1.Tags;
                    content.Text = temp1.Content;

                    break;

                case 1:
                    var temp2 = _context.Events.Where(nt => nt.Id == noteId).First();

                    note_name.Text = temp2.Name;
                    tags.Text = temp2.Tags;
                    EventStartPick.SelectedDate = temp2.StartDate;
                    EventEndPick.SelectedDate = temp2.EndDate;
                    location.Text = temp2.Location;

                    break;

                case 2:
                    var temp3 = _context.Reminders.Where(nt => nt.Id == noteId).First();

                    note_name.Text = temp3.Name;
                    tags.Text = temp3.Tags;
                    ReminderDuePick.SelectedDate = temp3.DueDate;
                    PriorityMenuPick.SelectedIndex = (int)temp3.Priority;

                    break;

                case 3:
                    var temp4 = _context.TodoLists.Where(nt => nt.Id == noteId).First();
                    var todos = JsonSerializer.Deserialize<Dictionary<string, bool>>(temp4.Todos);

                    note_name.Text = temp4.Name;
                    tags.Text = temp4.Tags;

                    TodoList.Children.Clear();
                    todoNums.Clear();
                    foreach (var entry in todos)
                    {
                        AddTodo2(entry.Key);
                    }
                    break;
            }

            Application.Current.Properties["isSaveNote"] = true;
            ToggleFields(noteType);
            diHost.IsOpen = true;
            Flag.Visibility = Visibility.Collapsed;
        }

        private void AddTodo2(string content = null)
        {
            if (todoNums.Count < 10)
            {
                Image img = new()
                {
                    Width = 15,
                    Cursor = Cursors.Hand,
                    ToolTip = new ToolTip() { Content = "Izbriši stavku" }
                };
                if (todoNums.Count == 0)
                {
                    todoNums.Add(1);
                }
                else
                {
                    todoNums.Add(todoNums.Last() + 1);
                    img.Source = new BitmapImage(new Uri(@"/Resources/Images/deleteRed.png", UriKind.Relative));
                    img.PreviewMouseDown += new MouseButtonEventHandler(DeleteTodo);
                }

                StackPanel stk = new()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBox txt = new()
                {
                    Margin = new Thickness(left: 0, top: 0, right: 0, bottom: 0),
                    MaxLength = 30,
                    Width = 150
                };
                if (content == null)
                {
                    txt.Text = $"Todo #{todoNums.Last()}";
                }
                else
                {
                    txt.Text = content;
                }

                Separator sep = new()
                {
                    Width = 10,
                    Background = Brushes.Transparent
                };
                Border border = new()
                {
                    Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 15)
                };

                stk.Children.Add(txt);
                stk.Children.Add(sep);
                stk.Children.Add(img);
                border.Child = stk;
                TodoList.Children.Add(border);
            }
            else
            {
                MessageBox.Show("Dozvoljeno maksimalno 10 Todo-ova po listi.", "Todo greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ViewTodo()
        {
            NoteName.Visibility = Visibility.Collapsed;
            Tags.Visibility = Visibility.Collapsed;
            Flag.Visibility = Visibility.Collapsed;
            TodoList.Visibility = Visibility.Collapsed;
            todoBtn.Visibility = Visibility.Collapsed;

            todoName.Visibility = Visibility.Visible;
            todoCreationDate.Visibility = Visibility.Visible;
            todoTags.Visibility = Visibility.Visible;
            itemTxt.Visibility = Visibility.Visible;
            todoScroll.Visibility = Visibility.Visible;
            diHost.IsOpen = true;

            var uid = (int)Application.Current.Properties["uid"];
            var noteId = (int)Application.Current.Properties["noteId"];
            var todos = _context.TodoLists.Where(x => x.UserId == uid && x.Id == noteId).First();

            tdName.Text = todos.Name;
            tdCreationDate.Text = todos.CreationDate.ToString("d. M. yyyy.");
            tdTags.Text = todos.Tags;

            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(todos.Todos);
            int counter = 1;
            foreach (var dict in todoDict)
            {
                StackPanel stk = new()
                {
                    Orientation = Orientation.Horizontal
                };
                TextBlock text1 = new()
                {
                    Text = (counter++) + ")",
                    Margin = new Thickness(left: 0, top: 0, right: 5, bottom: 0),
                    FontWeight = FontWeights.Bold
                };
                TextBlock text2 = new()
                {
                    Padding = new Thickness(left: 0, top: 0, right: 5, bottom: 0),
                    Text = dict.Key,
                    Width = 110,
                    Margin = new Thickness(left: 0, top: 0, right: 10, bottom: 0),
                    TextWrapping = TextWrapping.Wrap
                };
                CheckBox chk = new()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(left: 0, top: 0, right: 5, bottom: 0),
                    IsChecked = dict.Value,
                    Cursor = Cursors.Hand
                };

                stk.Children.Add(text1);
                stk.Children.Add(text2);
                stk.Children.Add(chk);
                todoElements.Children.Add(stk);
            }
            Application.Current.Properties["isSaveNote"] = false;
        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            ToggleNavbar();
            if ((bool)Application.Current.Properties["isNotTodoViewer"])
            {
                ToggleFields((int)Application.Current.Properties["noteType"]);
            }
        }

        private void DiHost_DialogClosed(object sender, MaterialDesignThemes.Wpf.DialogClosedEventArgs eventArgs)
        {
            CleanAddNote();
        }

        private void Dashboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
            Notes.callEditNote -= Edit;
            Events.callEditEvent -= Edit;
            Reminders.callEditReminder -= Edit;
            TodoLists.callEditTodo -= Edit;
            TodoLists.callTodoViewer -= ViewTodo;
        }
    }
}