using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace English_Trainer
{
    public partial class QuestionConstructor : UserControl
    {
        Question currentQuestion;
        ObservableCollection<Answer> answersCollection;
        ConstructorWindow constructorWindow;

        public QuestionConstructor()
        {
            InitializeComponent();
        }

        public QuestionConstructor(ConstructorWindow constructor, Question question)
        {
            constructorWindow = constructor;
            InitializeComponent();

            using (AppDbContext db = new AppDbContext())
            {
                DataContext = currentQuestion = db.Questions.Find(question.Id);
                answersList.ItemsSource = answersCollection = new ObservableCollection<Answer>(currentQuestion.Answers.ToList());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TestSaved();
        }

        // добавить вопрос в коллекцию
        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Answer newAnswer = new Answer { QuestionId = currentQuestion.Id };
            answersCollection.Add(newAnswer);
            TestChanged();
        }

        // удалить вопрос из коллекции
        private void DeleteAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Answer answer = (Answer)answersList.SelectedItem;
            answersCollection.Remove(answer);
            TestChanged();
        }

        // старые ответы - Entity, новые - Answer. новые норм добавляются, повторное добавление старых создает новую таблицу
        // старые нужно преобразвать в чистые Answer, либо их не добавлять повторно
        private void SaveQuestion()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    RemoveAnswers(db);
                    AddAnswers(db);
                    db.Entry(currentQuestion).State = EntityState.Modified;
                    UpdateWeightAndDate(db);
                    db.SaveChanges();
                }
                TestSaved();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void SaveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveQuestion();
            }
        }

        // удалить все ответы текущего вопроса, но оставить их в бд
        private void RemoveAnswers(AppDbContext db)
        {
            currentQuestion.Answers = null;
            var answers = db.Answers.Where(a => a.QuestionId == currentQuestion.Id);
            foreach (var i in answers)
            {
                i.QuestionId = null;
                db.Entry(i).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        // добавить ответы текущего вопроса в бд
        private void AddAnswers(AppDbContext db)
        {
            foreach (var i in answersCollection)
            {
                if (i.Question == null)
                {
                    db.Answers.Add(i);
                }
                else
                {
                    Answer newAnswer = new Answer { Text = i.Text, IsRight = i.IsRight, QuestionId = i.QuestionId };
                    db.Answers.Add(newAnswer);
                }
            }
            db.SaveChanges();
        }

        private void UpdateWeightAndDate(AppDbContext db)
        {
            Test test = db.Tests.First(t => t.Id == currentQuestion.TestId);
            test.TotalWeight = (int)test.Questions.Where(q => q.TestId == test.Id).Sum(s => s.Weight);
            test.ChangingDate = DateTime.Now;
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            answersList.SelectedItem = ((TextBox)sender).DataContext;                
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
