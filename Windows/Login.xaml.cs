using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TaskSharp.Classes;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly NotesContext _context =
    new NotesContext();
        bool registration = false;
        public Login()
        {
            InitializeComponent();
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }

        private void LoginSwitch()
        {

            title.Text = "Dobrodošli natrag!";
            description.Text = "Prijavite se u postojeći račun";
            secondBtn.Content = "Stvorite račun";
            firstBtn.Content = "Prijava";
            txtUser.Text = "";
            txtPass.Password = "";
            txtConf.Visibility = Visibility.Collapsed;
            registration = !registration;
        }
        private void secondBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!registration)
            {
                title.Text = "Pozdrav!";
                description.Text = "Stvorite novi korisnički račun";
                firstBtn.Content = "Registracija";
                secondBtn.Content = "Postojeći račun";
                txtConf.Visibility = Visibility.Visible;
            }
            else
            {
                LoginSwitch();
            }
            txtUser.Text = "";
            txtPass.Password = "";
            registration = !registration;
        }

        private void firstBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;

            if (username.Length < 2 || username.Length > 15)
            {
                MessageBox.Show("Korisničko ime mora imati između 2 i 15 znakova.", $"Greška {(registration == false ? "prijave" : "registracija")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string pass = txtPass.Password;
            if (pass.Length < 4)
            {
                MessageBox.Show("Lozinka mora sadržavati bar 4 znaka.", $"Greška {(registration == false ? "prijave" : "registracija")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            char[] invalidChars = { '/', '\\', ' ', '\"', '\'', '*', '=', '<', '>', '+', ';', '|' };
            foreach (char c in invalidChars)
            {
                if (txtUser.Text.Contains(c) || txtPass.Password.Contains(c))
                {
                    MessageBox.Show("Korisničko ime i lozinka ne smiju sadržavati sljedeće znakove:\nrazmak, /, \\, \', |, \", *, =, <, >, +, i ;", $"Greška {(registration == false ? "prijave" : "registracija")}", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            if (registration)
            {
                if (txtConf.Password != txtPass.Password)
                {
                    MessageBox.Show("Lozinke se ne poklapaju.", "Greška registracije", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    var usernames = _context.Users.Select(usr => usr.UserName).ToList();
                    if (!usernames.Contains(txtUser.Text))
                    {
                        var newUser = new User
                        {
                            UserName = txtUser.Text,
                            Password = txtPass.Password
                        };
                        _context.Users.Add(newUser);
                        _context.SaveChanges();
                        MessageBox.Show("Registracija uspješna!", "Registracija", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoginSwitch();
                    }
                    else
                    {
                        MessageBox.Show("Korisnik s tim imenom već postoji.", "Registracija", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            else
            {
                var loguser = _context.Users.Where(usr => usr.UserName == txtUser.Text && usr.Password == txtPass.Password);
                if (loguser.FirstOrDefault() != null)
                {
                    MessageBox.Show($"Prijava uspješna!", "Prijava", MessageBoxButton.OK, MessageBoxImage.Information);

                    // store logged user's ID
                    var uid = loguser.Select(usr => usr.UserId).First();
                    Application.Current.Properties["uid"] = uid;

                    //var dashboard = new Dashboard();
                    var dashboard = new Dashboard();
                    dashboard.Show();
                    this.Close();
                }

                else
                    MessageBox.Show("Krivi korisnički podaci! Pokušajte ponovo.", "Prijava", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
    }
}
