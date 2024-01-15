using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for NoteCreate.xaml
    /// </summary>
    public partial class NoteCreate : Window
    {
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
            Debug.WriteLine("hi");

            int index = NoteTypeMenu.SelectedIndex;
            string type = "";
            switch (index)
            {
                case 0:
                    type = "Bilješka";

                    break;
                case 1:
                    type = "Događaj";

                    break;
                case 2:
                    type = "Podsjetnik";

                    break;
                case 3:
                    type = "ToDo";

                    break;
            }
            if (NoteContent != null)
            {
                ToggleFields(type);
                //test.Children.Add(new TextBox { });
            }
            Debug.WriteLine(type);
        }
        private void ToggleFields(string type)
        {
            if (type != "Bilješka")
            {
                NoteContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoteContent.Visibility = Visibility.Visible;
            }
            if (type != "Događaj")
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
            if (type != "Podsjetnik")
            {
                ReminderDue.Visibility = Visibility.Collapsed;
                PriorityMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReminderDue.Visibility = Visibility.Visible;
                PriorityMenu.Visibility = Visibility.Visible;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddTodo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Todos.Children.Add(new TextBox { }); //continue here
        }
    }
}
