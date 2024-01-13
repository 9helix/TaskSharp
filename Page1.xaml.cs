using System.Windows;
using System.Windows.Controls;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        bool registration = false;
        public Page1()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;

            if (user.Length < 2)
            {
                MessageBox.Show("Korisničko ime mora imati bar 2 znaka.", $"Greška {(registration == false ? "prijave" : "registracija")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string pass = txtPass.Password;
            if (pass.Length < 4)
            {
                MessageBox.Show("Lozinka mora sadržavati bar 4 znaka.", $"Greška {(registration == false ? "prijave" : "registracija")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (registration)
            {
                if (txtPassConf.Password != txtPass.Password)
                {
                    MessageBox.Show("Lozinke se ne poklapaju.", "Greška registracije", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    MessageBox.Show("Registracija uspješna!", "Registracija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Prijava uspješna!", "Prijava", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new Page2());
            Title.Text = "Registracija";
            AccButton.Content = "Registriraj se";
            AccButton.Width = 130;
            PassConf.Visibility = Visibility.Visible;
            RegisterCall.Visibility = Visibility.Collapsed;
            registration = true;

        }
    }
}
