using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace English_Trainer
{
    public partial class TestConstructor : UserControl
    {
        Test currentTest;
        ConstructorWindow constructorWindow;

        public TestConstructor(ConstructorWindow constructor, Test test)
        {
            constructorWindow = constructor;
            InitializeComponent();

            using (AppDbContext db = new AppDbContext())
            {
                DataContext = currentTest = db.Tests.Find(test.Id);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TestSaved();
        }

        private void SaveTest()
        {
            try
            {
                TestSaved();
                using (AppDbContext db = new AppDbContext())
                {
                    currentTest.ChangingDate = DateTime.Now;
                    db.Entry(currentTest).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void SaveTestButton_Click(object sender, RoutedEventArgs e)
        {
            SaveTest();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveTest();
            }
        }

        private void TestSaved()
        {
            constructorWindow.IsTestChanged = false;
            constructorWindow.testTreeView.IsEnabled = true;
        }

        private void TestChanged()
        {
            constructorWindow.IsTestChanged = true;
            constructorWindow.testTreeView.IsEnabled = false;
        }

        private void OnTestChanged(object sender, TextChangedEventArgs e)
        {
            TestChanged();
        }

        private void OnTestChanged(object sender, SelectionChangedEventArgs e)
        {
            TestChanged();
        }

        private void OnTestChanged(object sender, RoutedEventArgs e)
        {
            TestChanged();
        }
    }
}
