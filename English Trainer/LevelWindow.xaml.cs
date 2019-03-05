using System.Data.Entity;
using System.Windows;
using System.Windows.Media;

namespace English_Trainer
{
    public partial class LevelWindow : Window
    {
        PerformWindow performWindow;

        public LevelWindow(PerformWindow perform)
        {
            InitializeComponent();
            performWindow = perform;
            DataContext = App.CurrentUser;
        }

        private void LevelComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            acceptButton.IsEnabled = true;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            using (AppDbContext db = new AppDbContext())
            { 
                db.Entry(App.CurrentUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            performWindow.UpdateTestList();
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
