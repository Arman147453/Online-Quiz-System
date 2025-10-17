using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }

        /// <summary>
        /// Navigation to the parent Quiz
        /// </summary>
        public virtual Quiz Quiz { get; set; } = null!;

        [Required(ErrorMessage = "Question text is required")]
        [StringLength(1000, ErrorMessage = "Question text cannot be longer than 1000 characters")]
        public string Text { get; set; } = null!;

        /// <summary>
        /// Navigation property for answer options belonging to this question
        /// </summary>
        public virtual ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
