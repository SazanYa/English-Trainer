using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace English_Trainer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }

        public static void SetTheme(string themeName)
        {
            Uri uri = new Uri($"MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.{themeName}.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
