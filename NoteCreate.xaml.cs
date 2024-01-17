﻿using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskSharp.Classes;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for NoteCreate.xaml
    /// </summary>
    public partial class NoteCreate : Window
    {
        private readonly NotesContext _context =
    new NotesContext();

        List<int> todos = new List<int> { 1 };
        public NoteCreate()
        {
            InitializeComponent();
            EventStartPick.BlackoutDates.AddDatesInPast();
            EventStartPick.SelectedDate = DateTime.Now;
            EventEndPick.BlackoutDates.AddDatesInPast();
            EventEndPick.SelectedDate = DateTime.Now;
            ReminderDuePick.BlackoutDates.AddDatesInPast();
            ReminderDuePick.SelectedDate = DateTime.Now;
            NoteContent.Visibility = Visibility.Visible;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = NoteTypeMenu.SelectedIndex;

            if (NoteContent != null)
            {
                ToggleFields(index);
                //test.Children.Add(new TextBox { });
            }
            Debug.WriteLine(index);
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
            }
            if (type != 3)
            {
                TodoList.Visibility = Visibility.Collapsed;
            }
            else
            {
                TodoList.Visibility = Visibility.Visible;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddTodo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (todos.Count < 10)
            {
                StackPanel stk = new StackPanel { Name = $"todo{todos.Last() + 1}", Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                todos.Add(todos.Last() + 1);
                TextBox txt = new TextBox { Margin = new Thickness(left: 15, top: 0, right: 0, bottom: 0), MaxLength = 50, Width = 175, Text = $"Todo #{todos.Last()}" };
                Separator sep = new Separator
                {
                    Name = $"septodo{todos.Last()}",
                    Width = 10,
                    Background = Brushes.Transparent
                };
                Border border = new Border
                {
                    Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 7)
                };
                CheckBox chk = new CheckBox { VerticalAlignment = VerticalAlignment.Center, Padding = new Thickness(left: 0, top: 0, right: 10, bottom: 0) };
                Image img = new Image { Width = 15, Source = new BitmapImage(new Uri(@"/Resources/Images/delete.png", UriKind.Relative)) };
                img.PreviewMouseDown += new MouseButtonEventHandler(DeleteTodo);

                stk.Children.Add(txt);
                stk.Children.Add(sep);
                stk.Children.Add(chk);
                stk.Children.Add(img);
                border.Child = stk;
                scroll.Children.Add(border);
            }
            else
            {
                MessageBox.Show("Dozvoljeno maksimalno 10 Todo-ova po listi.", "Todo greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeleteTodo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel stk = (StackPanel)(sender as Image).Parent;
            Border toDelete = (Border)stk.Parent;
            todos.RemoveAt(1);
            Debug.WriteLine(toDelete.Name);
            StackPanel par = (StackPanel)toDelete.Parent;
            par.Children.Remove(toDelete);
        }

        private void SaveNote(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("save");
            int index = NoteTypeMenu.SelectedIndex;
            string name = note_name.Text;
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
                    Note newNote = new Note { UserId = uid, Name = name, CreationDate = dateCreate, Tags = tags, Content = content, Pinned = Pin };
                    _context.BaseNotes.Add(newNote);
                    break;
                case 1: //događaj
                    DateTime StartEvent = (DateTime)EventStartPick.SelectedDate;
                    DateTime EndEvent = (DateTime)EventEndPick.SelectedDate;
                    if (EndEvent < StartEvent)
                    {
                        MessageBox.Show("Datum kraja događaja ne smije biti manji od datuma početka događaja.", "Događaj greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string location = this.location.Text;
                    Debug.WriteLine($"{StartEvent}-{EndEvent}-{location}");
                    Event newEvent = new Event { UserId = uid, Name = name, CreationDate = dateCreate, Tags = tags, StartDate = StartEvent, EndDate = EndEvent, Location = location, Pinned = Pin };
                    _context.BaseNotes.Add(newEvent);

                    break;
                case 2: //podsjetnik
                    DateTime dueDate = (DateTime)ReminderDuePick.SelectedDate;

                    int PriorityIndex = PriorityMenuPick.SelectedIndex;
                    ReminderPriority priority = (ReminderPriority)PriorityIndex;
                    Debug.WriteLine($"{dueDate}-{priority}");
                    Reminder newReminder = new Reminder { UserId = uid, Name = name, CreationDate = dateCreate, Tags = tags, Priority = priority, DueDate = dueDate, Pinned = Pin };
                    _context.BaseNotes.Add(newReminder);

                    break;
                case 3://todo
                    var todos = scroll.Children;
                    Dictionary<string, bool> TodoDict = new();
                    foreach (var child in todos)
                    {
                        var todo = ((child as Border).Child as StackPanel).Children;
                        TodoDict[(todo[0] as TextBox).Text] = (todo[2] as CheckBox).IsChecked.Value;
                        Debug.WriteLine($"{(todo[0] as TextBox).Text}-{(todo[2] as CheckBox).IsChecked.Value}");
                    }
                    TodoList newTodoList = new TodoList { UserId = uid, Name = name, CreationDate = dateCreate, Tags = tags, Todos = TodoDict, Pinned = Pin };
                    _context.BaseNotes.Add(newTodoList);

                    break;
            }
            _context.SaveChanges();
            MessageBox.Show("Zapis uspješno stvoren!", "Stvaranje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }
    }
}
