using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Quiz title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional time limit in minutes (null means unlimited).
        /// </summary>
        [Range(1, 180, ErrorMessage = "Time limit must be between 1 and 180 minutes")]
        public int? TimeLimitMinutes { get; set; }

        /// <summary>
        /// Whether the quiz is currently available for students.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property: all questions that belong to this quiz.
        /// </summary>
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
