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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Millionaire.Controls
{
    /// <summary>
    /// Interaction logic for QuestionLowerThird.xaml
    /// </summary>
    public partial class QuestionLowerThird : UserControl
    {
        public QuestionLowerThird()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //while (true)
            //{
            //    await Task.Delay(5000);
            //    ((Storyboard)FindResource("ShowWinnings")).Begin();
            //    await Task.Delay(5000);
            //    ((Storyboard)FindResource("ShowQuestion")).Begin();
            //}
        }
    }
}
