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
    public class AnswerControl : Control
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(AnswerControl), new PropertyMetadata(false, OnCorrectOrSelectedChanged));

        public bool IsCorrect
        {
            get { return (bool)GetValue(IsCorrectProperty); }
            set { SetValue(IsCorrectProperty, value); }
        }

        public static readonly DependencyProperty IsCorrectProperty =
            DependencyProperty.Register("IsCorrect", typeof(bool), typeof(AnswerControl), new PropertyMetadata(false, OnCorrectOrSelectedChanged));

        private static void OnCorrectOrSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (!(d is AnswerControl control))
            //    return;

            //var root = (FrameworkElement)control.GetTemplateChild("PART_Root");
            //if (!control.IsSelected && !control.IsCorrect)
            //{
            //    ((Storyboard)root.Resources["UnselectedStoryboard"]).Begin();
            //}
            //else if (control.IsSelected && !control.IsCorrect)
            //{
            //    ((Storyboard)root.Resources["SelectedStoryboard"]).Begin();
            //}
            //else if (!control.IsSelected && control.IsCorrect)
            //{
            //    ((Storyboard)root.Resources["UnselectedCorrectStoryboard"]).Begin();
            //}
            //else if (control.IsSelected && control.IsCorrect)
            //{
            //    ((Storyboard)root.Resources["SelectedCorrectStoryboard"]).Begin();
            //}
        }

        public string AnswerLetter
        {
            get { return (string)GetValue(AnswerLetterProperty); }
            set { SetValue(AnswerLetterProperty, value); }
        }

        public static readonly DependencyProperty AnswerLetterProperty =
            DependencyProperty.Register("AnswerLetter", typeof(string), typeof(AnswerControl), new PropertyMetadata("A"));

        public string AnswerText
        {
            get { return (string)GetValue(AnswerTextProperty); }
            set { SetValue(AnswerTextProperty, value); }
        }

        public static readonly DependencyProperty AnswerTextProperty =
            DependencyProperty.Register("AnswerText", typeof(string), typeof(AnswerControl), new PropertyMetadata(""));

        public Brush AnswerLetterForegound
        {
            get { return (Brush)GetValue(AnswerLetterForegoundProperty); }
            set { SetValue(AnswerLetterForegoundProperty, value); }
        }

        public static readonly DependencyProperty AnswerLetterForegoundProperty =
            DependencyProperty.Register("AnswerLetterForegound", typeof(Brush), typeof(AnswerControl), new PropertyMetadata(null));

        public Visibility AnswerVisible
        {
            get { return (Visibility)GetValue(AnswerVisibleProperty); }
            set { SetValue(AnswerVisibleProperty, value); }
        }

        public static readonly DependencyProperty AnswerVisibleProperty =
            DependencyProperty.Register("AnswerVisible", typeof(Visibility), typeof(AnswerControl), new PropertyMetadata(Visibility.Visible));

        static AnswerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnswerControl), new FrameworkPropertyMetadata(typeof(AnswerControl)));
        }
    }
}
