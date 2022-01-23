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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Millionaire
{

    /// <summary>
    /// Interaction logic for ControllerWindow.xaml
    /// </summary>
    public partial class ControllerWindow : Window
    {
        //private BackgroundWindow backgroundWindow;
        private OverlayWindow overlayWindow;
        private ContestantWindow contestantWindow;
        private ControllerViewModel vm;

        private record QuestionAction(string LetsPlay, string BackingTrack, string FinalAnswer, string SoundCueWon, string SoundCueLost, bool StopMusic = true);

        private QuestionAction[] actions = new QuestionAction[]
        {
            new QuestionAction("Q1-6_LetsPlay", "Q1_5_Background", null, "Q1-4_Yes", "Q1-5_No", false),
            new QuestionAction(null, "Q1_5_Background", null, "Q1-4_Yes","Q1-5_No", false),
            new QuestionAction(null, "Q1_5_Background", null, "Q1-4_Yes","Q1-5_No", false),
            new QuestionAction(null, "Q1_5_Background", null, "Q1-4_Yes","Q1-5_No", false),
            new QuestionAction(null, "Q1_5_Background", null, "Q5_Yes","Q1-5_No"),
            new QuestionAction(null, "Q6_Background", "Q6_FinalAnswer", "Q6_Win","Q6_Lose"),
            new QuestionAction("Q7_LetsPlay", "Q7_Background", "Q7_FinalAnswer", "Q7_Win","Q7_Lose"),
            new QuestionAction("Q8_LetsPlay", "Q8_Background", "Q8_FinalAnswer", "Q8_Win","Q8_Lose"),
            new QuestionAction("Q9_LetsPlay", "Q9_Background", "Q7_FinalAnswer", "Q9_Win","Q9_Lose"),
            new QuestionAction("Q10_LetsPlay", "Q10_Background", "Q10_FinalAnswer", "Q10_Win","Q10_Lose"),
            new QuestionAction(null, "Q11_Background", "Q11_FinalAnswer", "Q11_Win","Q11_Lose"),
            new QuestionAction("Q12_LetsPlay", "Q12_Background", "Q12_FinalAnswer", "Q12_Win","Q12_Lose"),
            new QuestionAction("Q13_LetsPlay", "Q13_Background", "Q13_FinalAnswer", "Q13_Win","Q13_Lose"),
            new QuestionAction("Q14_LetsPlay", "Q14_Background", "Q14_FinalAnswer", "Q14_Win","Q14_Lose"),
            new QuestionAction("Q15_LetsPlay", "Q15_Background", "Q15_FinalAnswer", "Q15_Win","Q15_Lose"),
        };

        public ControllerWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            DataContext = vm = new ControllerViewModel(this);
            vm.StartListening();

            AudioManager.Load();

            var binding = new Binding(nameof(vm.GameViewModel));
            binding.Source = vm;

            overlayWindow = new OverlayWindow();
            //overlayWindow.DataContext = vm.GameViewModel;
            overlayWindow.SetBinding(DataContextProperty, binding);

            contestantWindow = new ContestantWindow();
            //contestantWindow.DataContext = vm.GameViewModel;
            contestantWindow.SetBinding(DataContextProperty, binding);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            contestantWindow.Show();
            overlayWindow.Show();
        }

        private async void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.IsInPlay)
            {
                if (vm.IsLost || vm.GameViewModel.QuestionNumber == 15)
                {
                    ((Storyboard)overlayWindow.Resources["HideQuestion"]).Begin();
                    ((Storyboard)contestantWindow.Resources["HideQuestion"]).Begin();
                    ((Storyboard)overlayWindow.Question.Resources["ShowQuestion"]).Begin();
                    ((Storyboard)contestantWindow.Question.Resources["ShowQuestion"]).Begin();
                    await Task.Delay(1000);
                    vm.NewGame();
                }
                else
                {
                    vm.IsInPlay = true;
                    var action = actions[vm.GameViewModel.QuestionNumber];
                    AQuestionButton.Foreground = Brushes.Black;
                    BQuestionButton.Foreground = Brushes.Black;
                    CQuestionButton.Foreground = Brushes.Black;
                    DQuestionButton.Foreground = Brushes.Black;

                    vm.GameViewModel.Question = new QuestionViewModel(vm.Questions[new Random().Next(vm.Questions.Length)]);

                    if (action.LetsPlay != null)
                    {
                        AudioManager.PlayCue(action.LetsPlay);
                        await Task.Delay(1500);
                        AudioManager.StopSong();
                        await Task.Delay(3000);
                    }

                    AudioManager.PlaySong(action.BackingTrack);
                }
            }
        }

        private void ShowQuestion_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)overlayWindow.Resources["ShowQuestion"]).Begin();
            ((Storyboard)overlayWindow.Question.Resources["ShowQuestion"]).Begin();
            ((Storyboard)contestantWindow.Resources["ShowQuestion"]).Begin();
            ((Storyboard)contestantWindow.Question.Resources["ShowQuestion"]).Begin();
        }

        private async void HideQuestion_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)overlayWindow.Resources["HideQuestion"]).Begin();
            ((Storyboard)contestantWindow.Resources["HideQuestion"]).Begin();
            await Task.Delay(1000);
            ((Storyboard)overlayWindow.Question.Resources["ShowQuestion"]).Begin();
            ((Storyboard)contestantWindow.Question.Resources["ShowQuestion"]).Begin();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                (sender as ListBox).SelectedIndex = -1;
        }

        internal async void GoToScene(int state)
        {
            if (vm.State != state && state == 3)
            {
                ((Storyboard)overlayWindow.Resources["ShowSidebar"]).Begin();
            }
            else
            {
                ((Storyboard)overlayWindow.Resources["HideSidebar"]).Begin();
            }

            await vm.SendSceneChangeAsync(state);
        }

        private void FiftyFiftyButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            var incorrectAnswers = question.Question.Choices.Where(c => c != question.Question.Answer).ToList();
            incorrectAnswers.RemoveAt(vm.Random.Next(0, incorrectAnswers.Count));

            foreach (var answer in incorrectAnswers)
            {
                question.Visibilities[Array.IndexOf(question.Question.Choices, answer)] = Visibility.Collapsed;
            }

            question.InvokePropertyChanged("");

            AudioManager.PlayCue("50-50");
            vm.GameViewModel.Has5050 = false;
        }

        private void PhoneAFriendButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (vm.GameViewModel.PhoneAFriend.State == 0)
            {
                AudioManager.PushState();
                vm.GameViewModel.PhoneAFriend.Reset();
                vm.GameViewModel.PhoneAFriend.OnCompleted += OnPhoneAFriendCompleted;
                vm.GameViewModel.PhoneAFriend.State = 1;
                AudioManager.PlaySong("Phone_a_Friend_Ready");
                ((Storyboard)overlayWindow.Resources["ShowPhoneAFriend"]).Begin();
                ((Storyboard)contestantWindow.Resources["ShowPhoneAFriend"]).Begin();
            }
            else if (vm.GameViewModel.PhoneAFriend.State == 1)
            {
                AudioManager.PlaySong("Phone_a_Friend_Progress", false);
                vm.GameViewModel.PhoneAFriend.Start();
            }
            else if (vm.GameViewModel.PhoneAFriend.State == 2)
            {
                vm.GameViewModel.PhoneAFriend.Stop();
            }
        }

        private async void OnPhoneAFriendCompleted(object sender, EventArgs e)
        {
            vm.GameViewModel.HasPhoneAFriend = false;
            vm.GameViewModel.PhoneAFriend.State = 0;
            vm.GameViewModel.PhoneAFriend.OnCompleted -= OnPhoneAFriendCompleted;
            ((Storyboard)overlayWindow.Resources["HidePhoneAFriend"]).Begin();
            ((Storyboard)contestantWindow.Resources["HidePhoneAFriend"]).Begin();

            await Task.Delay(3000);

            AudioManager.PopState();
        }

        private void AskTheAudienceButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //vm.GameViewModel.AskTheAudience = vm.AskTheAudience;
            if (vm.GameViewModel.AskTheAudience.State == 0)
            {
                vm.GameViewModel.AskTheAudience.Reset();
                vm.GameViewModel.AskTheAudience.State = 1;
                AudioManager.PushState();
                AudioManager.PlaySong("Ask_the_Audience_Ready");
                ((Storyboard)overlayWindow.Resources["ShowAskTheAudience"]).Begin();
                ((Storyboard)contestantWindow.Resources["ShowAskTheAudience"]).Begin();
            }
            else if (vm.GameViewModel.AskTheAudience.State == 1)
            {
                AudioManager.PlaySong("Ask_the_Audience_Progress");
                vm.GameViewModel.AskTheAudience.Start(vm.GameViewModel.Question.Question);
            }
            else if (vm.GameViewModel.AskTheAudience.State == 2)
            {
                vm.GameViewModel.AskTheAudience.Stop();
            }
            else if (vm.GameViewModel.AskTheAudience.State == 3)
            {
                AudioManager.PopState();
                ((Storyboard)overlayWindow.Resources["HideAskTheAudience"]).Begin();
                ((Storyboard)contestantWindow.Resources["HideAskTheAudience"]).Begin();
                vm.GameViewModel.HasAskTheAudience = false;
                vm.GameViewModel.AskTheAudience.State = 0;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            if (question == null) return;

            if (question.Answer1Selected || question.Answer2Selected || question.Answer3Selected || question.Answer4Selected)
            {
                var action = actions[vm.GameViewModel.QuestionNumber];

                question.IsRevealed = true;
                if ((question.Answer1Selected && question.Answer1Correct) ||
                    (question.Answer2Selected && question.Answer2Correct) ||
                    (question.Answer3Selected && question.Answer3Correct) ||
                    (question.Answer4Selected && question.Answer4Correct))
                {
                    // correct
                    vm.GameViewModel.QuestionNumber++;
                    vm.IsInPlay = false;
                    AudioManager.PlayCue(action.SoundCueWon);
                    if (action.StopMusic)
                        AudioManager.StopSong();

                    await Task.Delay(1000);
                    ((Storyboard)overlayWindow.Question.Resources["ShowWinnings"]).Begin();
                    ((Storyboard)contestantWindow.Question.Resources["ShowWinnings"]).Begin();
                }
                else
                {
                    // incorrect
                    vm.IsInPlay = false;
                    vm.IsLost = true;
                    AudioManager.StopSong();
                    AudioManager.PlayCue(action.SoundCueLost);
                }
            }
            else
            {
                question.RevealAnswer();
            }
        }

        private void FinalAnswerCue()
        {
            var action = actions[vm.GameViewModel.QuestionNumber];
            if (action.FinalAnswer != null)
            {
                AudioManager.PlaySong(action.FinalAnswer);
            }

            var question = vm.GameViewModel.Question;
            if (question.Choices[0] == question.Question.Answer)
                AQuestionButton.Foreground = Brushes.Green;
            if (question.Choices[1] == question.Question.Answer)
                BQuestionButton.Foreground = Brushes.Green;
            if (question.Choices[2] == question.Question.Answer)
                CQuestionButton.Foreground = Brushes.Green;
            if (question.Choices[3] == question.Question.Answer)
                DQuestionButton.Foreground = Brushes.Green;
        }

        private void AQuestionButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            if (question == null) return;

            if (!question.Answer1Selected && !question.Answer2Selected && !question.Answer3Selected && !question.Answer4Selected)
            {
                question.Answer1Selected = true;
                FinalAnswerCue();
            }
        }

        private void BQuestionButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            if (question == null) return;

            if (!question.Answer1Selected && !question.Answer2Selected && !question.Answer3Selected && !question.Answer4Selected)
            {
                question.Answer2Selected = true;
                FinalAnswerCue();
            }
        }

        private void CQuestionButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            if (question == null) return;

            if (!question.Answer1Selected && !question.Answer2Selected && !question.Answer3Selected && !question.Answer4Selected)
            {
                question.Answer3Selected = true;
                FinalAnswerCue();
            }
        }

        private void DQuestionButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var question = vm.GameViewModel.Question;
            if (question == null) return;

            if (!question.Answer1Selected && !question.Answer2Selected && !question.Answer3Selected && !question.Answer4Selected)
            {
                question.Answer4Selected = true;
                FinalAnswerCue();
            }
        }

        private void FiftyFiftyButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.State == 3)
            {
                ((Storyboard)overlayWindow.Sidebar.Resources["Highlight5050"]).Begin();
                AudioManager.PlayCue("Lifeline_1");
            }
        }

        private void PhoneAFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.State == 3)
            {
                ((Storyboard)overlayWindow.Sidebar.Resources["HighlightPAF"]).Begin();
                AudioManager.PlayCue("Lifeline_2");
            }
        }

        private void AskTheAudienceButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.State == 3)
            {
                ((Storyboard)overlayWindow.Sidebar.Resources["HighlightATA"]).Begin();
                AudioManager.PlayCue("Lifeline_3");
            }
        }

        private void IntroduceContestant_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlaySong("Classic_Entrance", false);
        }

        private void ExplainRules_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlaySong("Explain_Rules", false);
        }

        private void WalkAway_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)overlayWindow.Question.Resources["ShowWinnings"]).Begin();
            ((Storyboard)contestantWindow.Question.Resources["ShowWinnings"]).Begin();
            AudioManager.PlaySong("Walk_Away", false);
            vm.IsInPlay = false;
            vm.IsLost = true;
        }
    }
}
