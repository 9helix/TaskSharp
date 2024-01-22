using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for NoteEdit.xaml
    /// </summary>
    public partial class NoteEdit : Window
    {
        private readonly NotesContext _context =
    new NotesContext();

        int NoteId = (int)Application.Current.Properties["noteId"];
        int NoteType = (int)Application.Current.Properties["noteType"];

        List<int> todos = new List<int> { 1 };
        public NoteEdit()
        {
            InitializeComponent();

            switch (NoteType)
            {

                case 0:
                    var temp1 = _context.Notes.Where(nt => nt.Id == NoteId).First();
                    NoteContent.Visibility = Visibility.Visible;

                    note_name.Text = temp1.Name;
                    tags.Text = temp1.Tags;
                    flag.IsChecked = temp1.Pinned;
                    content.Text = temp1.Content;

                    txtNoteType.Text = "Bilješka";
                    break;

                case 1:
                    var temp2 = _context.Events.Where(nt => nt.Id == NoteId).First();
                    txtNoteType.Text = "Događaj";
                    note_name.Text = temp2.Name;
                    tags.Text = temp2.Tags;
                    flag.IsChecked = temp2.Pinned;

                    //EventStartPick.BlackoutDates.AddDatesInPast();
                    EventStartPick.SelectedDate = temp2.StartDate;
                    //EventEndPick.BlackoutDates.AddDatesInPast();
                    EventEndPick.SelectedDate = temp2.EndDate;

                    location.Text = temp2.Location;
                    EventStart.Visibility = Visibility.Visible;
                    EventEnd.Visibility = Visibility.Visible;
                    txtLocation.Visibility = Visibility.Visible;
                    break;

                case 2:
                    txtNoteType.Text = "Podsjetnik";
                    var temp3 = _context.Reminders.Where(nt => nt.Id == NoteId).First();

                    note_name.Text = temp3.Name;
                    tags.Text = temp3.Tags;
                    flag.IsChecked = temp3.Pinned;

                    //ReminderDuePick.BlackoutDates.AddDatesInPast();
                    ReminderDuePick.SelectedDate = temp3.DueDate;
                    ReminderDue.Visibility = Visibility.Visible;

                    PriorityMenuPick.SelectedIndex = (int)temp3.Priority;
                    PriorityMenu.Visibility = Visibility.Visible;
                    break;

                case 3:
                    txtNoteType.Text = "To-Do lista";
                    var temp4 = _context.TodoLists.Where(nt => nt.Id == NoteId).First();
                    var todos = JsonSerializer.Deserialize<Dictionary<string, bool>>(temp4.Todos);

                    note_name.Text = temp4.Name;
                    tags.Text = temp4.Tags;
                    flag.IsChecked = temp4.Pinned;

                    foreach (KeyValuePair<string, bool> entry in todos)
                    {
                        AddTodo2(entry.Key, entry.Value);
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
            this.Close();
        }
        private void AddTodo(object sender, MouseButtonEventArgs e)
        {
            AddTodo2();
        }

        private void AddTodo2(string content = null, bool check = false)
        {
            if (todos.Count < 10)
            {
                StackPanel stk = new StackPanel
                {
                    Name = $"todo{todos.Last() + 1}",
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

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
                CheckBox chk = new CheckBox
                {
                    IsChecked = check,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(left: 0, top: 0, right: 10, bottom: 0)
                };
                Image img = new Image
                {
                    Width = 15,
                    Source = new BitmapImage(new Uri(@"/Resources/Images/delete.png", UriKind.Relative)),
                    Cursor = Cursors.Hand,
                    ToolTip = new ToolTip() { Content = "Izbriši stavku" }
                };
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
        private void DeleteTodo(object sender, MouseButtonEventArgs e)
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
            if (name == "")
            {
                MessageBox.Show("Zapis mora imati ime.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string tags = this.tags.Text;
            bool Pin = flag.IsChecked.Value;

            Debug.WriteLine($"{name}-{tags}-{Pin}");
            //DateTime dateCreate = DateTime.Now;
            //int uid = (int)Application.Current.Properties["uid"];
            //ushort id = GenerateNoteId();

            switch (index)
            {
                case 0: //biljeska
                    string content = this.content.Text;
                    Debug.WriteLine(content);
                    if (content == "")
                    {
                        MessageBox.Show("Sadržaj ne smije biti prazan.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var note1 = _context.Notes.Where(nt => nt.Id == NoteId).First<Note>();
                    note1.Name = name;
                    note1.Tags = tags;
                    note1.Content = content;
                    note1.Pinned = Pin;
                    break;

                case 1: //događaj
                    var StartEvent = EventStartPick.SelectedDate;
                    var EndEvent = EventEndPick.SelectedDate;

                    if (StartEvent is not DateTime || EndEvent is not DateTime)
                    {
                        MessageBox.Show("Datumi moraju biti odabrani.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

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
                    note2.StartDate = (DateTime)StartEvent;
                    note2.EndDate = (DateTime)EndEvent;
                    note2.Location = location;
                    note2.Pinned = Pin;
                    break;

                case 2: //podsjetnik
                    var dueDate = ReminderDuePick.SelectedDate;
                    if (dueDate is not DateTime)
                    {
                        MessageBox.Show("Datum mora biti odabran.", "Greška spremanja", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int PriorityIndex = PriorityMenuPick.SelectedIndex;
                    ReminderPriority priority = (ReminderPriority)PriorityIndex;
                    Debug.WriteLine($"{dueDate}-{priority}");


                    var note3 = _context.Reminders.Where(nt => nt.Id == NoteId).First<Reminder>();
                    note3.Name = name;
                    note3.Tags = tags;
                    note3.Pinned = Pin;
                    note3.Priority = priority;
                    note3.DueDate = (DateTime)dueDate;
                    break;

                case 3://todo
                    var todos = scroll.Children;
                    Dictionary<string, bool> TodoDict = new();

                    if (todos.Count != 0)
                    {
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

                        var note4 = _context.TodoLists.Where(nt => nt.Id == NoteId).First<TodoList>();
                        note4.Name = name;
                        note4.Tags = tags;
                        note4.Todos = JsonSerializer.Serialize(TodoDict);
                        note4.Pinned = Pin;
                    }
                    else
                    {
                        MessageBox.Show("Morate imati barem jedan to-do u listi.", "Todo greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    break;
            }
            _context.SaveChanges();
            MessageBox.Show("Zapis uspješno izmijenjen!", "Ažuriranje zapisa", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
            var dashboard = new DashboardTesting();
            dashboard.Show();
        }
    }
}
