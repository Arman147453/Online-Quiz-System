using System.ComponentModel.DataAnnotations;

namespace QuizApp.ViewModels
{
    public class OptionCreateViewModel
    {
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuestionCreateViewModel
    {
        public int QuizId { get; set; }
        public string? QuizTitle { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        public List<OptionCreateViewModel> Options { get; set; } = new();
    }
}
