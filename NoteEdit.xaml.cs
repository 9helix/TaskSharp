using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for NoteEdit.xaml
    /// </summary>
    public partial class NoteEdit : Window
    {
        private readonly NotesContext _context =
    new NotesContext();

        int NoteId;//define them
        int NoteType;

        List<int> todos = new List<int> { 1 };
        public NoteEdit()
        {
            InitializeComponent();
            switch (NoteType)
            {

                case 0:
                    var temp1 = _context.Notes.Where(nt => nt.Id == NoteId).First();
                    NoteContent.Visibility = Visibility.Visible;
                    break;
                case 1:
                    var temp = _context.Events.Where(nt => nt.Id == NoteId).First();
                    EventStartPick.BlackoutDates.AddDatesInPast();
                    EventStartPick.SelectedDate = temp.StartDate;
                    EventEndPick.BlackoutDates.AddDatesInPast();
                    EventEndPick.SelectedDate = temp.EndDate;
                    EventStart.Visibility = Visibility.Visible;
                    EventEnd.Visibility = Visibility.Visible;
                    txtLocation.Visibility = Visibility.Visible;

                    break;
                case 2:
                    var temp3 = _context.Reminders.Where(nt => nt.Id == NoteId).First();
                    ReminderDuePick.BlackoutDates.AddDatesInPast();
                    ReminderDuePick.SelectedDate = temp3.DueDate;
                    ReminderDue.Visibility = Visibility.Visible;

                    (PriorityMenu.Children[(int)temp3.Priority] as ComboBoxItem).IsSelected = true;
                    PriorityMenu.Visibility = Visibility.Visible;

                    break;
                case 3:
                    var note4 = _context.TodoLists.Where(nt => nt.Id == NoteId).First();
                    foreach (KeyValuePair<string, bool> entry in note4.Todos)
                    {
                        AddTodo(entry.Key, entry.Value);
                    }

                    TodoList.Visibility = Visibility.Visible;
                    break;
            }


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            this.Close();
        }

        private void AddTodo(string content = null, bool check = false)
        {
            if (todos.Count < 10)
            {
                StackPanel stk = new StackPanel { Name = $"todo{todos.Last() + 1}", Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                todos.Add(todos.Last() + 1);
                TextBox txt;
                if (content == null)
                {
                    txt = new TextBox { Margin = new Thickness(left: 15, top: 0, right: 0, bottom: 0), MaxLength = 50, Width = 175, Text = $"Todo #{todos.Last()}" };
                }
                else
                {
                    txt = new TextBox { Margin = new Thickness(left: 15, top: 0, right: 0, bottom: 0), MaxLength = 50, Width = 175, Text = content };
                }
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
                CheckBox chk = new CheckBox { IsChecked = check, VerticalAlignment = VerticalAlignment.Center, Padding = new Thickness(left: 0, top: 0, right: 10, bottom: 0) };
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
            int index = NoteType;
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

                    var note1 = _context.Notes.Where(nt => nt.Id == NoteId).First<Note>();
                    note1.Name = name;
                    note1.Tags = tags;
                    note1.Content = content;
                    note1.Pinned = Pin;
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

                    var note2 = _context.Events.Where(nt => nt.Id == NoteId).First<Event>();
                    note2.Name = name;
                    note2.Tags = tags;
                    note2.StartDate = StartEvent;
                    note2.EndDate = EndEvent;
                    note2.Location = location;
                    note2.Pinned = Pin;
                    break;

                case 2: //podsjetnik
                    DateTime dueDate = (DateTime)ReminderDuePick.SelectedDate;

                    int PriorityIndex = PriorityMenuPick.SelectedIndex;
                    ReminderPriority priority = (ReminderPriority)PriorityIndex;
                    Debug.WriteLine($"{dueDate}-{priority}");


                    var note3 = _context.Reminders.Where(nt => nt.Id == NoteId).First<Reminder>();
                    note3.Name = name;
                    note3.Tags = tags;
                    note3.Pinned = Pin;
                    note3.Priority = priority;
                    note3.DueDate = dueDate;
                    break;

                case 3://todo
                    var todos = scroll.Children;
                    Dictionary<string, bool> TodoDict = new();
                    foreach (var child in todos)
                    {
                        var todo = ((child as Border).Child as StackPanel).Children;
                        TodoDict[(todo[0] as TextBox).Text] = (todo[2] as CheckBox).IsChecked.Value;
                        Debug.WriteLine($"{(todo[0] as TextBox).Text}-{(todo[2] as CheckBox).IsChecked.Value}");
                        if ((todo[0] as TextBox).Text == "")
                        {
                            MessageBox.Show("To-Do stavka ne može biti prazna.", "Greška u stvaranju", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    TodoList newTodoList = new TodoList
                    {
                        UserId = uid,
                        Name = name,
                        CreationDate = dateCreate,
                        Tags = tags,
                        Todos = TodoDict,
                        Pinned = Pin
                    };

                    var note4 = _context.TodoLists.Where(nt => nt.Id == NoteId).First<TodoList>();
                    note4.Name = name;
                    note4.Tags = tags;
                    note4.Todos = TodoDict;
                    note4.Pinned = Pin;
                    break;
            }
            _context.SaveChanges();
            MessageBox.Show("Zapis uspješno stvoren!", "Stvaranje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
            var dashboard = new Dashboard();
            dashboard.Show();
        }
    }
}
