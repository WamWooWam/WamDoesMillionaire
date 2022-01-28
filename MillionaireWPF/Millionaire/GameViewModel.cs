using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Millionaire
{
    public class GameViewModel : ViewModelBase
    {
        private QuestionViewModel _question;
        private AskTheAudienceViewModel askTheAudience;

        private bool has5050 = true;
        private bool hasAskTheAudience = true;
        private bool hasPhoneAFriend = true;
        private int questionNumber = 0;

        private string[] winningsTable = new string[]
        {
            "¢1",
            "¢2",
            "¢3",
            "¢5",
            "¢10",
            "¢20",
            "¢40",
            "¢60",
            "¢80",
            "¢100",
            "¢150",
            "¢200",
            "¢250",
            "¢500",
            "¢1000",
        };

        private string[] winningsTableNoCents = new string[]
        {
            "1",
            "2",
            "3",
            "5",
            "10",
            "20",
            "40",
            "60",
            "80",
            "100",
            "150",
            "200",
            "250",
            "500",
            "1000",
        };

        public GameViewModel(Question[] questions)
        {
            Questions = questions;
        }

        public string CurrentWinnings
            => QuestionNumber > 0 ? winningsTable[QuestionNumber - 1] : "¢0";

        public Visibility ShowLastQuestion => QuestionNumber > 0 && QuestionNumber < 15 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowNextQuestion => QuestionNumber < 14 ? Visibility.Visible : Visibility.Collapsed;

        public string LastQuestionWinnings =>
            QuestionNumber > 0 ? winningsTableNoCents[QuestionNumber - 1] : "";
        public string CurrentQuestionWinnings =>
            QuestionNumber >= 15 ? "1000" : winningsTableNoCents[QuestionNumber];
        public string NextQuestionWinnings =>
            QuestionNumber < 14 ? winningsTableNoCents[QuestionNumber + 1] : "1000";
        public string LastQuestionNumber =>
            $"{QuestionNumber}";
        public string CurrentQuestionNumber =>
            $"{Math.Min(QuestionNumber + 1, 15)}";
        public string NextQuestionNumber =>
            $"{Math.Min(QuestionNumber + 2, 15)}";

        public int QuestionNumber { get => questionNumber; set => OnPropertySet(ref questionNumber, value, ""); }

        public bool Has5050 { get => has5050; set => OnPropertySet(ref has5050, value); }
        public bool HasAskTheAudience { get => hasAskTheAudience; set => OnPropertySet(ref hasAskTheAudience, value); }
        public bool HasPhoneAFriend { get => hasPhoneAFriend; set => OnPropertySet(ref hasPhoneAFriend, value); }

        public Question[] Questions { get; set; }
        public QuestionViewModel Question { get => _question; set => OnPropertySet(ref _question, value); }
        public AskTheAudienceViewModel AskTheAudience { get => askTheAudience; set => OnPropertySet(ref askTheAudience, value); }
        public PhoneAFriendViewModel PhoneAFriend { get; set; } = new PhoneAFriendViewModel();

        public bool IsTop1 => QuestionNumber == 0;
        public bool IsTop2 => QuestionNumber == 1;
        public bool IsTop3 => QuestionNumber == 2;
        public bool IsTop4 => QuestionNumber == 3;
        public bool IsTop5 => QuestionNumber == 4;
        public bool IsTop6 => QuestionNumber == 5;
        public bool IsTop7 => QuestionNumber == 6;
        public bool IsTop8 => QuestionNumber == 7;
        public bool IsTop9 => QuestionNumber == 8;
        public bool IsTop10 => QuestionNumber == 9;
        public bool IsTop11 => QuestionNumber == 10;
        public bool IsTop12 => QuestionNumber == 11;
        public bool IsTop13 => QuestionNumber == 12;
        public bool IsTop14 => QuestionNumber == 13;
        public bool IsTop15 => QuestionNumber == 14;

        public bool HasWon1 => QuestionNumber > 0;
        public bool HasWon2 => QuestionNumber > 1;
        public bool HasWon3 => QuestionNumber > 2;
        public bool HasWon4 => QuestionNumber > 3;
        public bool HasWon5 => QuestionNumber > 4;
        public bool HasWon6 => QuestionNumber > 5;
        public bool HasWon7 => QuestionNumber > 6;
        public bool HasWon8 => QuestionNumber > 7;
        public bool HasWon9 => QuestionNumber > 8;
        public bool HasWon10 => QuestionNumber > 9;
        public bool HasWon11 => QuestionNumber > 10;
        public bool HasWon12 => QuestionNumber > 11;
        public bool HasWon13 => QuestionNumber > 12;
        public bool HasWon14 => QuestionNumber > 13;
        public bool HasWon15 => QuestionNumber > 14;
    }
}
