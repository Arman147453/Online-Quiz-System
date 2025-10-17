using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.ViewModels;

namespace QuizApp.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]  // only students allowed
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =======================
        // Student Dashboard: List quizzes
        // =======================
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.IsActive)
                .Include(q => q.Questions)
                .ToListAsync();

            return View(quizzes);
        }

        // =======================
        // Take Quiz Page
        // =======================
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            return View(quiz); // Views/Student/Dashboard/TakeQuiz.cshtml
        }

        // =======================
        // Submit Quiz (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            // prevent duplicate attempts (optional)
            // var existing = await _context.QuizAttempts.FirstOrDefaultAsync(a => a.QuizId == id && a.UserId == userId);
            // if (existing != null) return RedirectToAction(nameof(Result), new { id = existing.Id });

            var attempt = new QuizAttempt
            {
                QuizId = quiz.Id,
                UserId = userId!,
                StartTime = DateTime.UtcNow,
                TotalQuestions = quiz.Questions.Count
            };

            int correctCount = 0;
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            // process each answer
            foreach (var question in quiz.Questions)
            {
                string formKey = $"q_{question.Id}";
                var selectedValue = Request.Form[formKey].FirstOrDefault();
                int? selectedOptionId = null;

                if (!string.IsNullOrEmpty(selectedValue) && int.TryParse(selectedValue, out int sid))
                {
                    selectedOptionId = sid;
                }

                var selectedOption = selectedOptionId.HasValue
                    ? question.Options.FirstOrDefault(o => o.Id == selectedOptionId.Value)
                    : null;

                bool isCorrect = selectedOption != null && selectedOption.IsCorrect;
                if (isCorrect) correctCount++;

                _context.AttemptAnswers.Add(new AttemptAnswer
                {
                    QuizAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedOptionId = selectedOptionId,
                    IsCorrect = isCorrect
                });
            }

            // update final result
            attempt.EndTime = DateTime.UtcNow;
            attempt.CorrectAnswers = correctCount;
            attempt.ScorePercent = quiz.Questions.Count == 0
                ? 0
                : (double)correctCount / quiz.Questions.Count * 100.0;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Result), new { id = attempt.Id });
        }

        // =======================
        // Show Quiz Result
        // =======================
        public async Task<IActionResult> Result(int id)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Answers).ThenInclude(ans => ans.Question)
                .Include(a => a.Answers).ThenInclude(ans => ans.SelectedOption)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attempt == null) return NotFound();

            if (_userManager.GetUserId(User) != attempt.UserId)
                return Forbid();

            // convert to ViewModel for clean UI
            var model = new QuizResultViewModel
            {
                QuizTitle = attempt.Quiz.Title,
                Score = attempt.CorrectAnswers,
                TotalQuestions = attempt.TotalQuestions,
                ScorePercent = attempt.ScorePercent,
                Answers = attempt.Answers.Select(a => new AnswerResultViewModel
                {
                    QuestionText = a.Question.Text,
                    YourAnswer = a.SelectedOption?.Text,
                    CorrectAnswer = a.Question.Options.FirstOrDefault(o => o.IsCorrect)?.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            return View(model); // Views/Student/Dashboard/Result.cshtml
        }
    }
}
