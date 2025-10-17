namespace QuizApp.ViewModels
{
    public class QuizResultViewModel
    {
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public double ScorePercent { get; set; }

        public List<AnswerResultViewModel> Answers { get; set; } = new();
    }

    public class AnswerResultViewModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? YourAnswer { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
