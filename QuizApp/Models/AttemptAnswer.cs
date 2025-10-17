namespace QuizApp.Models
{
    public class AttemptAnswer
    {
        public int Id { get; set; }

        public int QuizAttemptId { get; set; }
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; } = null!;

        public int? SelectedOptionId { get; set; }
        public virtual Option? SelectedOption { get; set; }

        public bool IsCorrect { get; set; }
    }
}
