using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            navframe.Navigate(new Uri("Pages/Notes.xaml", UriKind.Relative));
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            switch (lb.SelectedIndex)
            {
                case 0:
                    navframe.Navigate(new Uri("Pages/Notes.xaml", UriKind.Relative));
                    break;
                case 1:
                    navframe.Navigate(new Uri("Pages/Events.xaml", UriKind.Relative));
                    break;
                case 2:
                    navframe.Navigate(new Uri("Pages/Reminders.xaml", UriKind.Relative));
                    break;
                case 3:
                    navframe.Navigate(new Uri("Pages/TodoLists.xaml", UriKind.Relative));
                    break;
                case 4:
                    MessageBox.Show($"Odjava uspješna!", "Odjava", MessageBoxButton.OK, MessageBoxImage.Information);
                    var win = new MainWindow();
                    win.Show();

                    this.Close();
                    break;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var noteCreate = new NoteCreate();
            noteCreate.Show();
            this.Close();
        }
    }
}
