using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using System.Windows.Media;

namespace English_Trainer
{
    public class Validation
    {
        public static bool TryValidateObject(object obj, TextBox loginTextBox, PasswordBox passwordBox, PasswordBox confirmPasswordBox)
        {
            bool isValidate = true;

            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(obj);

            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                foreach (var error in results)
                {
                    var n = error.MemberNames;
                    foreach (var name in error.MemberNames)
                    {
                        if (name == "Login")
                        {
                            loginTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            loginTextBox.ToolTip = new ToolTip() { Content = error };
                        }

                        if (name == "Password")
                        {
                            passwordBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            passwordBox.ToolTip = new ToolTip() { Content = error };
                        }

                        if (name == "ConfirmPassword")
                        {
                            confirmPasswordBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            confirmPasswordBox.ToolTip = new ToolTip() { Content = error };
                        }
                    }

                    // для атрибута Compare свойства (name) не находит
                    if (confirmPasswordBox != null && !String.Equals(passwordBox.Password, confirmPasswordBox.Password))
                    {
                        confirmPasswordBox.BorderBrush = new SolidColorBrush(Colors.Red);
                        confirmPasswordBox.ToolTip = new ToolTip() { Content = error };
                    }
                }
                isValidate = false;
            }
            return isValidate;
        }
    }
}
