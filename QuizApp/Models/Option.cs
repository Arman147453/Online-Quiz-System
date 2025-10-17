using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Option
    {
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; } = null!;

        [Required]
        public string Text { get; set; } = null!;

        // true for the correct option
        public bool IsCorrect { get; set; } = false;
    }
}
