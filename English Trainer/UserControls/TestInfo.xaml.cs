using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace English_Trainer
{
    /// <summary>
    /// Логика взаимодействия для TestInfo.xaml
    /// </summary>
    public partial class TestInfo : UserControl
    {
        public TestInfo(Test test)
        {
            InitializeComponent();
            SetCountOfQuestions(test);
            DataContext = test;
        }
        
        private void SetCountOfQuestions(Test test)
        {
            using (AppDbContext db = new AppDbContext())
            {
                countOfQuestions.Text = db.Questions.Where(q => q.TestId == test.Id).ToList().Count.ToString();
            }
        }        
    }
}
