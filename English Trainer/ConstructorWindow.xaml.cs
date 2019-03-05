using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class ConstructorWindow : Window
    {
        Test currentTest;
        Question currentQuestion;
        ObservableCollection<Test> testCollection;
        
        public bool IsTestChanged { get; set; }

        public ConstructorWindow()
        {
            InitializeComponent();
            DataContext = App.CurrentUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTestTreeView();
        }

        // *** ADD TEST ***
        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Test newTest = new Test
                {
                    Title = "Test",
                    Creator = App.CurrentUser.Username,
                    Level = "A1",
                    CreationDate = DateTime.Now,
                    ChangingDate = DateTime.Now,
                    CanUserSeeResult = true
                };

                using (AppDbContext db = new AppDbContext())
                {
                    AddTestToTree(db.Tests.Add(newTest));
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        // *** DELETE TEST ***
        private void DeleteTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (testTreeView.SelectedItem is Test)
                {
                    using (AppDbContext db = new AppDbContext())
                    {
                        Test test = db.Tests.Find(currentTest.Id);
                        MessageBoxResult result = MessageBox.Show("Do you really want to delete this test?", $"Deleting {test.Title}", 
                            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                        if (result == MessageBoxResult.Yes)
                        {
                            TestSaved();
                            RemoveTestFromTree();
                            test.IsDeleted = true;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    throw new Exception("Choose a test");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void UpdateTestTreeView()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    testCollection = new ObservableCollection<Test>(db.Tests.
                        Where(t => t.Creator == App.CurrentUser.Username && !t.IsDeleted).ToList());
                    testTreeView.ItemsSource = testCollection;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        
        // *** ADD QUESTION ***
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentTest != null)
                {
                    using (AppDbContext db = new AppDbContext())
                    {
                        Test test = db.Tests.Find(currentTest.Id);
                        Question newQuestion = new Question { Title = "Question", Weight = 1, Test = test };
                        test.ChangingDate = DateTime.Now;
                        AddQuestionToTree(db.Questions.Add(newQuestion));
                        db.SaveChanges();
                    }
                    UpdateTotalWeight();
                }
                else
                {
                    throw new Exception("Choose a test");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        // *** DELETE QUESTION ***
        private void DeleteQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (testTreeView.SelectedItem is Question)
                {
                    TestSaved();
                    using (AppDbContext db = new AppDbContext())
                    {
                        Test test = db.Tests.First(t => t.Id == currentTest.Id);
                        test.ChangingDate = DateTime.Now;
                        RemoveCurrentQuestion(db);
                        RemoveQuestionFromTree();
                        db.SaveChanges();
                    }
                    UpdateTotalWeight();
                }
                else
                {
                    throw new Exception("Choose a question");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void RemoveCurrentQuestion(AppDbContext db)
        {
            Question question = db.Questions.Find(currentQuestion.Id);
            question.TestId = null;
            db.Entry(question).State = EntityState.Modified;
        }

        private void RemoveTestFromTree()
        {
            testCollection.Remove(currentTest);
            if (testCollection.Count == 0)
            {
                deleteTestButton.IsEnabled = addQuestionButton.IsEnabled = false;
                workspace.Children.Clear();
            }
        }

        private void RemoveQuestionFromTree()
        {
            currentTest.Questions.Remove(currentQuestion);
        }

        private void AddQuestionToTree(Question newQuestion)
        {
            currentTest.Questions.Add(newQuestion);
        }

        private void AddTestToTree(Test newTest)
        {
            testCollection.Add(newTest);
        }

        private void UpdateTotalWeight()
        {
            using (AppDbContext db = new AppDbContext())
            {
                Test test = db.Tests.First(t => t.Id == currentTest.Id);
                test.TotalWeight = (int)test.Questions.Where(q => q.TestId == test.Id).Sum(s => s.Weight);
                db.SaveChanges();
            }
        }

        private void UpdateTestButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTestTreeView();
        }

        private void TestTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            // save the source of the event into the Tag
            testTreeView.Tag = e.OriginalSource;

            if (testTreeView.SelectedItem is Test test)
            {
                addQuestionButton.IsEnabled = deleteTestButton.IsEnabled = true;
                deleteQuestionButton.IsEnabled = false;
                TryFillWorkspace(IsTestChanged, new TestConstructor(this, currentTest = test));
            }

            if (testTreeView.SelectedItem is Question question)
            {
                addQuestionButton.IsEnabled = deleteTestButton.IsEnabled = false;
                deleteQuestionButton.IsEnabled = true;
                TryFillWorkspace(IsTestChanged, new QuestionConstructor(this, currentQuestion = question));

                if (e.OriginalSource is TreeViewItem item)
                {
                    currentTest = (Test)GetSelectedTreeViewItemParent(item).DataContext;
                }
            }
        }
        
        private void ShowResults_Click(object sender, RoutedEventArgs e)
        {
            if (testTreeView.SelectedItem != null)
            {
                if (TryFillWorkspace(IsTestChanged, new ResultTable(this)))
                {
                    if (testTreeView.Tag is TreeViewItem item)
                    {
                        item.IsSelected = false;
                    }
                }
            }            
        }

        public bool TryFillWorkspace(bool isTestChanged, params object[] controls)
        {
            if (isTestChanged)
            {
                if (GetInterruptionMessage() == MessageBoxResult.No) return false;
            }

            workspace.Children.Clear();
            foreach (UIElement i in controls)
            {
                workspace.Children.Add(i);
            }

            IsTestChanged = false;
            testTreeView.IsEnabled = true;
            return true;
        }

        private MessageBoxResult GetInterruptionMessage()
        {
            return MessageBox.Show($"Changes have not saved. Continue?", "Interruption",
                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            popupMenu.IsOpen = true;
        }

        private void ResultTable_Click(object sender, RoutedEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (TryFillWorkspace(IsTestChanged, new ResultTable(this)))
            {
                if (testTreeView.Tag is TreeViewItem item)
                {
                    item.IsSelected = false;
                }
            }
        }

        private void UpdateTree_Click(object sender, MouseButtonEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (TryFillWorkspace(IsTestChanged, new ResultTable(this)))
            {
                UpdateTestTreeView();
            }
        }

        private void SignOut_Click(object sender, MouseButtonEventArgs e)
        {
            popupMenu.IsOpen = false;
            if (IsTestChanged)
            {
                if (GetInterruptionMessage() == MessageBoxResult.No) return;
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            backButton.Visibility = Visibility.Hidden;
            TryFillWorkspace(IsTestChanged, new ResultTable(this));
        }

        private void TestSaved()
        {
            IsTestChanged = false;
            testTreeView.IsEnabled = true;
        }

        private void RangeDragWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
