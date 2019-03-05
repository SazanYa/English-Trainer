using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class ProgressDetails : UserControl
    {
        public ProgressDetails(Progress progress)
        {
            InitializeComponent();

            try
            {
                CheckTest(progress);
                ShowDetails(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void CheckTest(Progress progress)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var test = db.Tests.FirstOrDefault(t => t.Id == progress.TestId);
                if (test.ChangingDate > progress.Date)
                {
                    MessageBox.Show("The test has been changed since its passage. The data provided can be incorrect.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                }
            }
        }

        private void ShowDetails(Progress progress)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var userAnswers = db.UserAnswers.Where(an => an.ProgressId == progress.Id).ToList();
                var test = db.Tests.FirstOrDefault(t => t.Id == progress.TestId);

                List<Answer> answersList = new List<Answer>();
                foreach (UserAnswer uAn in userAnswers)
                {
                    answersList.Add(db.Answers.First(an => an.Id == uAn.AnswerId));
                }

                int counter = 0;
                foreach (Question q in db.Questions)
                {
                    if (IsAnsweredQuestion(q, answersList) || q.Test == test)
                    {
                        var rightAnswers = db.Answers.Where(an => an.QuestionId == q.Id && an.IsRight).ToList();

                        resultInfo.Children.Add(GetQuestionTitleTextBlock(++counter));
                        resultInfo.Children.Add(GetQuestionTextBlock(q));

                        resultInfo.Children.Add(GetAnswersTitleTextBlock("User's answers are ..."));
                        foreach (Answer an in answersList)
                        {
                            if (an.QuestionId == q.Id)
                            {
                                resultInfo.Children.Add(GetAnswerTextBlock(an));
                            }
                        }

                        resultInfo.Children.Add(GetAnswersTitleTextBlock("Right answers are ..."));
                        foreach (Answer an in rightAnswers)
                        {
                            resultInfo.Children.Add(GetAnswerTextBlock(an));
                        }
                    }
                }
            }
        }

        // true, если пользователь отвечал на этот вопрос
        private bool IsAnsweredQuestion(Question question, List<Answer> answers)
        {
            bool rc = false;
            foreach (Answer an in answers)
            {
                if (question.Id == an.QuestionId)
                {
                    rc = true;
                    break;
                }
            }

            return rc;
        }

        private TextBlock GetQuestionTitleTextBlock(int number)
        {
            string text = number.ToString();

            switch (number)
            {
                case 1:
                    text += "st";
                    break;
                case 2:
                    text += "nd";
                    break;
                case 3:
                    text += "rd";
                    break;
                default:
                    text += "th";
                    break;
            }

            return new TextBlock()
            {
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 20, 20, 0),
                FontSize = 20,
                Text = text + " question is ..."
            };
        }

        private TextBlock GetAnswersTitleTextBlock(string title)
        {
            return new TextBlock()
            {
                FontStyle = FontStyles.Italic,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(40, 5, 20, 0),
                FontSize = 18,
                Text = title
            };
        }

        private TextBlock GetQuestionTextBlock(Question question)
        {
            return new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20, 10, 20, 0),
                FontSize = 16,
                Text = question.Text
            };
        }

        private TextBlock GetAnswerTextBlock(Answer answer)
        {
            Brush brush = (Brush)new BrushConverter().ConvertFrom("#e53935");

            if (answer.IsRight)
            {
                brush = (Brush)new BrushConverter().ConvertFrom("#43a047");
            }

            return new TextBlock()
            {
                Foreground = brush,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(60, 10, 20, 0),
                TextWrapping = TextWrapping.Wrap,
                Text = answer.Text
            };
        }
    }
}
