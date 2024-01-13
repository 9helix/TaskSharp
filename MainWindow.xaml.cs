using System.Diagnostics;
using System.Windows;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //ystem.Diagnostics.Process.Start(e.Uri.AbsoluteUri);

            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            if (user.Length < 2)
                MessageBox.Show("Korisničko ime mora imati bar 2 znaka.", "Greška prijave", MessageBoxButton.OK, MessageBoxImage.Warning);
            string pass = txtPass.Password;
            if (pass.Length < 4)
                MessageBox.Show("Lozinka mora sadržavati bar 4 znaka.", "Greška prijave", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }
}