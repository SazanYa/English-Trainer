using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace English_Trainer
{
    public partial class TestRunner : UserControl
    {
        Test currentTest;
        Question currentQuestion;
        Progress progress;
        List<Question> questions;
        List<UserAnswer> userAnswers;
        PerformWindow performWindow;
        AppDbContext db;

        int numberOfQuestion = 0;
        int mark = 0;

        public TestRunner()
        {
            InitializeComponent();
        }

        public TestRunner(PerformWindow perform, Test test)
        {
            InitializeComponent();
            currentTest = test;
            performWindow = perform;
            db = new AppDbContext();
            userAnswers = new List<UserAnswer>();
            progress = new Progress()
            {
                UserId = App.CurrentUser.Id,
                Username = App.CurrentUser.Username,
                TestId = test.Id,
                TestTitle = test.Title,
                TestCreator = test.Creator
            };
        }

        private void TestRunner_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // получаем список вопросов
                questions = db.Tests.First(t => t.Id == progress.TestId).Questions.ToList();
                // текущий вопрос - первый из списка, он же контекст данных 
                questionStackPanel.DataContext = currentQuestion = questions[numberOfQuestion];
                // список ответов текущего теста - источник для ItemsControl
                answersList.ItemsSource = currentQuestion.Answers.ToList();
                countQuestionsRun.Text += questions.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                numberOfQuestion++;
                SaveResult(CheckQuestion());
                NextQuestion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private bool CheckQuestion()
        {
            bool isCorrectQuestion = true;
            List<Answer> rightAnswers = db.Questions.First(q => q.Id == currentQuestion.Id).Answers.Where(a => a.IsRight == true).ToList();
            SaveUserAnswers();
            if (answersList.SelectedItems.Count == rightAnswers.Count)
            {
                foreach (Answer i in answersList.SelectedItems)
                {
                    if (!i.IsRight)
                    {
                        isCorrectQuestion = false;
                    }
                }
            }
            else
            {
                isCorrectQuestion = false;
            }

            return isCorrectQuestion;
        }

        private void SaveUserAnswers()
        {
            foreach (Answer i in answersList.SelectedItems)
            {
                userAnswers.Add(new UserAnswer() { AnswerId = i.Id, Progress = progress });
            }
        }

        private void SaveResult(bool result)
        {
            if (result)
            {
                mark += (int)currentQuestion.Weight;
            }
        }

        private void NextQuestion()
        {
            if (numberOfQuestion < questions.Count)
            {
                questionStackPanel.DataContext = currentQuestion = questions[numberOfQuestion];
                answersList.ItemsSource = currentQuestion.Answers.ToList();
                questioNumberRun.Text = (numberOfQuestion + 1).ToString();
            }
            else
            {
                progress.Mark = mark;
                progress.Date = DateTime.Now;

                SaveRusultToDatabase();
                FinishTest();
            }
        }

        private void FinishTest()
        {
            performWindow.IsTestRunning = false;
            performWindow.testsList.IsEnabled = true;
            performWindow.workspace.Children.Clear();
            performWindow.workspace.Children.Add(new TestFinisher(performWindow, currentTest, progress));
        }

        private void SaveRusultToDatabase()
        {
            db.Progresses.Add(progress);
            db.UserAnswers.AddRange(userAnswers);
            db.SaveChanges();
        }

        private void CloseTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (performWindow.IsTestRunning)
            {
                MessageBoxResult result = MessageBox.Show($"Do you really want to abort the test?", "Interruption",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    performWindow.IsTestRunning = false;
                    performWindow.testsList.IsEnabled = true;
                    performWindow.workspace.Children.Clear();
                }
            }
        }
    }
}
