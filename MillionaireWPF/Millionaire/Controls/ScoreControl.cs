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

namespace Millionaire.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Millionaire.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Millionaire.Controls;assembly=Millionaire.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ScoreControl/>
    ///
    /// </summary>
    public class ScoreControl : Control
    {
        static ScoreControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScoreControl), new FrameworkPropertyMetadata(typeof(ScoreControl)));
        }

        public bool IsWon
        {
            get { return (bool)GetValue(IsWonProperty); }
            set { SetValue(IsWonProperty, value); }
        }

        public static readonly DependencyProperty IsWonProperty =
            DependencyProperty.Register("IsWon", typeof(bool), typeof(ScoreControl), new PropertyMetadata(false));

        public bool IsTopScore
        {
            get { return (bool)GetValue(IsTopScoreProperty); }
            set { SetValue(IsTopScoreProperty, value); }
        }

        public static readonly DependencyProperty IsTopScoreProperty =
            DependencyProperty.Register("IsTopScore", typeof(bool), typeof(ScoreControl), new PropertyMetadata(false));

        public string Position
        {
            get { return (string)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string), typeof(ScoreControl), new PropertyMetadata(""));

        public string Amount
        {
            get { return (string)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(string), typeof(ScoreControl), new PropertyMetadata(""));
    }
}
