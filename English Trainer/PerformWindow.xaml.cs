using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace English_Trainer
{
    public partial class PerformWindow : Window
    {        
        Test currentTest;
        ObservableCollection<Test> testCollection;

        public bool IsTestRunning { get; set; }

        public PerformWindow()
        {
            InitializeComponent();
            App.SetTheme("Yellow");
            DataContext = App.CurrentUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTestList();
        }

        public void UpdateTestList()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    testCollection = new ObservableCollection<Test>(db.Tests.
                        Where(t => t.Level == App.CurrentUser.Level && !t.IsDeleted && t.IsReady).ToList());
                    testsList.ItemsSource = testCollection;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void StartTest(Test test)
        {
            if (test != null)
            {
                if (!IsTestEmpty(test))
                {
                    testsList.UnselectAll();
                    testsList.IsEnabled = false;
                    TryFillWorkspace(new TestRunner(this, currentTest));
                    IsTestRunning = true;
                }
                else
                {
                    throw new Exception("Test is empty");
                }
            }
            else
            {
                throw new Exception("Choose your destiny!");
            }
        }
        
        private bool IsTestEmpty(Test test)
        {
            using (AppDbContext db = new AppDbContext())
            {
                return db.Tests.First(t => t.Id == currentTest.Id).Questions.Count == 0;
            }
        }

        public bool TryFillWorkspace(params object[] controls)
        {
            if (IsTestRunning)
            {
                if (GetInterruptionMessage() == MessageBoxResult.No)  return false;             
            }

            workspace.Children.Clear();
            backButton.Visibility = Visibility.Hidden;
            foreach (UIElement i in controls)
            {
                workspace.Children.Add(i);
            }

            IsTestRunning = false;
            return true;
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            popupMenu.IsOpen = true;
        }

        private void ResultTable_Click(object sender, RoutedEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (TryFillWorkspace(new ResultTable(this)))
            {
                testsList.UnselectAll();
                testsList.IsEnabled = true;
            }
        }

        private void ChangeLevel_Click(object sender, MouseButtonEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (TryFillWorkspace())
            {
                testsList.UnselectAll();
                testsList.IsEnabled = true;
                LevelWindow levelWindow = new LevelWindow(this);
                levelWindow.Show();
            }
        }
        
        private void SignOut_Click(object sender, MouseButtonEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (IsTestRunning)
            {
                if (GetInterruptionMessage() == MessageBoxResult.No) return;
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            App.SetTheme("Blue");
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TryFillWorkspace(new ResultTable(this));
        }

        private MessageBoxResult GetInterruptionMessage()
        {
            return MessageBox.Show($"Do you really want to abort the test?", "Interruption",
                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        }

        private void TestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (testsList.SelectedItem is Test test)
            {
                TryFillWorkspace(new TestInfo(currentTest = test), GetStartButton());
            }
        }

        private Button GetStartButton()
        {
            Button startTestButton = new Button()
            {
                Content = "START TEST",
                Margin = new Thickness(0, 0, 20, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            startTestButton.Click += StartTest_Click;
            return startTestButton;
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (testsList.SelectedItem is Test test)
                {
                    StartTest(test);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void TestSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchTextBox = (TextBox)sender;
            testsList.ItemsSource = testCollection.Where(t => t.Title.StartsWith(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                                                              t.Creator.StartsWith(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                                                              t.Title.Contains(searchTextBox.Text) ||
                                                              t.Creator.Contains(searchTextBox.Text));
        }

        private void OnRangeDragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsTestRunning)
            {
                if (GetInterruptionMessage() == MessageBoxResult.No) return;
            }
            Close();
        }
    }
}
