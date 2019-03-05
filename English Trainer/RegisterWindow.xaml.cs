using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            regLoginTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            regPasswordTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            regConfirmPasswordTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#89000000");
            regLoginTextBox.Focus();
        }

        private void SignInAccountTextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            OpenLoginWindow();
        }

        private void Register()
        {
            try
            {
                string username = regLoginTextBox.Text;
                string password = regPasswordTextBox.Password;
                string confirmPassword = regConfirmPasswordTextBox.Password;

                RegisterModel registerModel = new RegisterModel()
                {
                    Login = username,
                    Password = password,
                    ConfirmPassword = confirmPassword
                };

                if (Validation.TryValidateObject(registerModel, regLoginTextBox, regPasswordTextBox, regConfirmPasswordTextBox))
                {
                    SaltedHash saltedHash = new SaltedHash(password);
                    bool isTeacher = (bool)isTeacherCheckBox.IsChecked;

                    using (AppDbContext db = new AppDbContext())
                    {
                        var sameUser = db.Users.FirstOrDefault(u => u.Username == username);
                        if (sameUser == null)
                        {
                            User user = new User()
                            {
                                Username = username,
                                Salt = saltedHash.Salt,
                                Hash = saltedHash.Hash,
                                IsTeacher = isTeacher
                            };

                            db.Users.Add(user);
                            db.SaveChanges();
                            OpenLoginWindow();
                        }
                        else
                        {
                            regLoginTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            regLoginTextBox.ToolTip = new ToolTip() { Content = "This username is already taken" };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void OpenLoginWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
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

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Register();
            }
        }
    }
}
