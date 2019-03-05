using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<AppDbContext>());
            InitializeComponent();

            logLoginTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            logPasswordTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            logLoginTextBox.Focus();
        }

        private void Login()
        {
            try
            {
                string username = logLoginTextBox.Text;
                string password = logPasswordTextBox.Password;

                LoginModel loginModel = new LoginModel() { Login = username, Password = password };

                if (Validation.TryValidateObject(loginModel, logLoginTextBox, logPasswordTextBox, null))
                {
                    using (AppDbContext db = new AppDbContext())
                    {
                        var user = db.Users.FirstOrDefault(u => u.Username == username);
                        if (user != null)
                        {
                            if (SaltedHash.Verify(user.Salt, user.Hash, password))
                            {
                                App.CurrentUser = user;

                                if (user.IsTeacher)
                                {
                                    ConstructorWindow constructorWindow = new ConstructorWindow();
                                    Close();
                                    constructorWindow.Show();
                                }
                                else
                                {
                                    PerformWindow performWindow = new PerformWindow();
                                    Close();
                                    performWindow.Show();

                                    if (user.Level == null)
                                    {
                                        LevelWindow levelWindow = new LevelWindow(performWindow);
                                        levelWindow.Show();
                                    }
                                }
                            }
                            else
                            {
                                logPasswordTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                                logPasswordTextBox.ToolTip = new ToolTip() { Content = "Wrong password" };
                            }
                        }
                        else
                        {
                            logLoginTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            logLoginTextBox.ToolTip = new ToolTip() { Content = "User with this username is not found" };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void CreateAccountTextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loginButton.Focus();
                Login();
            }
        }
        
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            passwordBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            passwordBox.ToolTip = null;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            textBox.ToolTip = null;
        }

        private void RangeDragWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
