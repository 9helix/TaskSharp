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

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ListBoxItem lbi = lb.SelectedItem as ListBoxItem;
            switch (lbi.Content)
            {
                case "Bilješke":
                    navframe.Navigate(new Uri("Pages/Notes.xaml", UriKind.Relative));
                    break;
                case "Događaji":
                    navframe.Navigate(new Uri("Pages/Events.xaml", UriKind.Relative));
                    break;
                case "Podsjetnici":
                    navframe.Navigate(new Uri("Pages/Reminders.xaml", UriKind.Relative));
                    break;
                case "To-do liste":
                    navframe.Navigate(new Uri("Pages/TodoLists.xaml", UriKind.Relative));
                    break;
                case "Odjava":
                    this.Close();
                    break;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
