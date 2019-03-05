using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class ResultTable : UserControl
    {
        List<Progress> progressList;
        PerformWindow performWindow;
        ConstructorWindow constructorWindow;

        public ResultTable(object mainWindow)
        {
            InitializeComponent();
            SetResultTable(App.CurrentUser);

            performWindow = mainWindow as PerformWindow;
            constructorWindow = mainWindow as ConstructorWindow;
        }

        private void SetResultTable(User user)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (user.IsTeacher)
                {
                    progressList = new List<Progress>();
                    List<Test> tests = db.Tests.Where(t => t.Creator == user.Username).ToList();
                    foreach (Test t in tests)
                    {
                        progressList.AddRange(db.Progresses.Where(p => p.TestId == t.Id && !p.IsDeleted).ToList());
                    }
                    resultTable.Columns.RemoveAt(1);
                    resultTable.ItemsSource = progressList;
                }
                else
                {
                    commentButton.Visibility = Visibility.Collapsed;
                    deleteProgressButton.Visibility = Visibility.Collapsed;
                    resultTable.Columns.RemoveAt(2);
                    resultTable.ItemsSource = db.Progresses.Where(p => p.UserId == user.Id && !p.IsDeleted).ToList();
                }
            }
        }

        private void ShowDetails()
        {
            if (resultTable.SelectedItem is Progress progress)
            {
                if (performWindow != null)
                {
                    using (AppDbContext db = new AppDbContext())
                    {
                        Test test = db.Tests.Find(progress.TestId);
                        if (test.CanUserSeeResult)
                        {
                            performWindow.TryFillWorkspace(new ProgressDetails(progress));
                            performWindow.backButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBox.Show("Sorry, but creator of the test has forbidden viewing of details", "Information", 
                                MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        }
                    }
                }

                if (constructorWindow != null)
                {
                    constructorWindow.TryFillWorkspace(false, new ProgressDetails(progress));
                    constructorWindow.backButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void ShowDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDetails();
        }

        private void ResultTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowDetails();
        }


        private void DeleteProgressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (resultTable.SelectedItem is Progress p)
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete this progress?", $"Deleting {p.TestTitle}",
                           MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteProgressFromTable(p);
                        using (AppDbContext db = new AppDbContext())
                        {
                            Progress progress = db.Progresses.Find(p.Id);
                            progress.IsDeleted = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void DeleteProgressFromTable(Progress p)
        {
            progressList.Remove(p);
            resultTable.Items.Refresh();
            commentButton.IsEnabled = deleteProgressButton.IsEnabled = detailsButton.IsEnabled = false;
        }

        private void ResultTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            detailsButton.IsEnabled = commentButton.IsEnabled = deleteProgressButton.IsEnabled = true;
        }

        private void ResultTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            using (AppDbContext db = new AppDbContext())
            {
                db.Entry(resultTable.SelectedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            commentColumn.IsReadOnly = true;
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                commentColumn.IsReadOnly = false;
                DataGridCell cell = GetCell(resultTable.SelectedIndex, 4);
                if (cell != null)
                {
                    cell.Focus();
                    resultTable.BeginEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                return (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
            }
            return null;
        }

        private DataGridRow GetRow(int index)
        {
            return (DataGridRow)resultTable.ItemContainerGenerator.ContainerFromIndex(index);
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resultTable.CommitEdit();
        }
    }
}
