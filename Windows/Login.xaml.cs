using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TaskSharp.Classes;

namespace TaskSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotesContext _context =
            new NotesContext();
        bool registration = false;

        public MainWindow()
        {
            InitializeComponent();
            //PurgeDatabase();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
        }

        /*private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            if (user.Length < 2)
                MessageBox.Show("Korisničko ime mora imati bar 2 znaka.", "Greška prijave", MessageBoxButton.OK, MessageBoxImage.Warning);
            string pass = txtPass.Password;
            if (pass.Length < 4)
                MessageBox.Show("Lozinka mora sadržavati bar 4 znaka.", "Greška prijave", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        */
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            PasswordUnmask.Visibility = Visibility.Visible;
            txtPass.Visibility = Visibility.Collapsed;
            PasswordUnmask.Text = txtPass.Password;
        }
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordUnmask.Visibility = Visibility.Collapsed;
            txtPass.Visibility = Visibility.Visible;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
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
                if (txtPassConf.Password != txtPass.Password)
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
                    var dashboard = new DashboardTesting();
                    dashboard.Show();
                    this.Close();
                }

                else
                    MessageBox.Show("Krivi korisnički podaci! Pokušajte ponovo.", "Prijava", MessageBoxButton.OK, MessageBoxImage.Error);

                // login logic
            }
        }
        private void LoginSwitch()
        {
            Title.Text = "Prijava";
            AccButton.Content = "Prijavi se";
            AccButton.Width = 110;
            PassConf.Visibility = Visibility.Collapsed;
            RegisterCall.Visibility = Visibility.Visible;
            registration = false;
            txtPass.Password = "";
            txtUser.Text = "";
            LoginCall.Visibility = Visibility.Collapsed;
            txtPassConf.Password = "";
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (!registration)
            {
                //this.NavigationService.Navigate(new Page2());
                txtPassConf.Width = txtPass.Width;
                Title.Text = "Registracija";
                AccButton.Content = "Registriraj se";
                AccButton.Width = 130;
                PassConf.Visibility = Visibility.Visible;
                RegisterCall.Visibility = Visibility.Collapsed;
                registration = true;
                txtPass.Password = "";
                txtUser.Text = "";
                LoginCall.Visibility = Visibility.Visible;
            }
            else
            {
                LoginSwitch();
            }

        }

        private void PurgeDatabase()
        {
            var allUsers = _context.Users.ToList();

            _context.Users.RemoveRange(allUsers);
            _context.SaveChanges();

        }
        private void DebugUsers()
        {
            var usrs = _context.Users.ToList();
            foreach (var user in usrs)
            {
                Debug.WriteLine($"ID: {user.UserId}, Ime: {user.UserName}, Pass: {user.Password}");
            }
        }

        private void ShowPassword()
        {
            PasswordUnmask.Visibility = Visibility.Visible;
            txtPass.Visibility = Visibility.Collapsed;
            PasswordUnmask.Text = txtPass.Password;
            ShowPass.Source = new BitmapImage(new Uri(@"../Resources/Images/hide.png", UriKind.Relative));
        }

        private void HidePassword()
        {
            PasswordUnmask.Visibility = Visibility.Collapsed;
            txtPass.Visibility = Visibility.Visible;
            ShowPass.Source = new BitmapImage(new Uri(@"../Resources/Images/show.png", UriKind.Relative));

        }

        private void Image_MouseLeave(object sender, MouseEventArgs e) => HidePassword();
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e) => ShowPassword();
        private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e) => HidePassword();

        private void Login_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DebugUsers();
            _context.Dispose();
        }
    }
}