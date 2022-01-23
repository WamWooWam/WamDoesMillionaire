using System.Windows;

namespace Millionaire
{
    public class QuestionViewModel : ViewModelBase
    {
        private int i = 0;
        private bool isRevealed;
        private bool answer1Selected;
        private bool answer2Selected;
        private bool answer3Selected;
        private bool answer4Selected;

        public QuestionViewModel(Question question)
        {
            this.Question = question;
            this.Visibilities = new Visibility[4];
            for (int i = 0; i < 4; i++)
                this.Visibilities[i] = Visibility.Visible;
        }

        public Question Question { get; set; }
        public string Text => Question.Text;
        public string[] Choices { get; } = new string[4];

        public bool IsRevealed
        {
            get => isRevealed;
            set => OnPropertySet(ref isRevealed, value, "");
        }

        public bool Answer1Correct => IsRevealed ? Choices[0] == Question.Answer : false;
        public bool Answer2Correct => IsRevealed ? Choices[1] == Question.Answer : false;
        public bool Answer3Correct => IsRevealed ? Choices[2] == Question.Answer : false;
        public bool Answer4Correct => IsRevealed ? Choices[3] == Question.Answer : false;

        public bool Answer1Selected { get => answer1Selected; set => OnPropertySet(ref answer1Selected, value); }
        public bool Answer2Selected { get => answer2Selected; set => OnPropertySet(ref answer2Selected, value); }
        public bool Answer3Selected { get => answer3Selected; set => OnPropertySet(ref answer3Selected, value); }
        public bool Answer4Selected { get => answer4Selected; set => OnPropertySet(ref answer4Selected, value); }

        public Visibility[] Visibilities { get; set; }

        public Visibility Answer1Visible => Visibilities[0];
        public Visibility Answer2Visible => Visibilities[1];
        public Visibility Answer3Visible => Visibilities[2];
        public Visibility Answer4Visible => Visibilities[3];

        internal void RevealAnswer()
        {
            if (i == 4) return;

            Choices[i] = Question.Choices[i];
            i++;

            InvokePropertyChanged("");
        }
    }
}
