using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class TestFinisher : UserControl
    {
        Test currentTest;
        PerformWindow performWindow;
        double markInPercent;

        public TestFinisher(PerformWindow perform, Test test, Progress progress)
        {
            InitializeComponent();
            performWindow = perform;
            DataContext = progress;
            markInPercent = GetResultInPercent((double)progress.Mark, (double)test.TotalWeight);
            ShowResult(currentTest = test);
        }

        private void ShowResult(Test test)
        {
            maxMarkRun.Text += test.TotalWeight.ToString();
            markInPercentRun.Text = markInPercent.ToString() + "%";
            SetMarkRunBrush(markInPercent);
        }

        private double GetResultInPercent(double result, double maxResult)
        {
            return Math.Round(result * 100 / maxResult, 2, MidpointRounding.AwayFromZero);
        }

        private void SetMarkRunBrush(double resultInPercent)
        {
            if (resultInPercent <= Math.Round(1.0 / 3.0 * 100, 2, MidpointRounding.AwayFromZero))
            {
                markRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#F44336");
                markInPercentRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#F44336");
            }

            if (resultInPercent > Math.Round(1.0 / 3.0 * 100, 2, MidpointRounding.AwayFromZero) && 
                resultInPercent <= Math.Round(2.0 / 3.0 * 100, 2, MidpointRounding.AwayFromZero))
            {
                markRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF9800");
                markInPercentRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF9800");
            }

            if (resultInPercent > Math.Round(2.0 / 3.0 * 100, 2, MidpointRounding.AwayFromZero))
            {
                markRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#4CAF50");
                markInPercentRun.Foreground = (Brush)new BrushConverter().ConvertFrom("#4CAF50");
            }
        }

        private void ShowDetailsTextBlock_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentTest.CanUserSeeResult)
            {
                performWindow.TryFillWorkspace(new ProgressDetails((Progress)DataContext));
            }
            else
            {
                MessageBox.Show("Sorry, but creator of the test has forbidden viewing of details", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}
