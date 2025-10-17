using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.ViewModels;

namespace QuizApp.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Quiz/Index
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.IsActive)
                .Include(q => q.Questions)
                .ToListAsync();

            return View(quizzes);
        }

        // GET: /Quiz/Take/5
        public async Task<IActionResult> Take(int id)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.Id == id && q.IsActive)
                .Include(q => q.Questions)
                    .ThenInclude(qn => qn.Options)
                .FirstOrDefaultAsync();

            if (quiz == null) return NotFound();

            return View(quiz);
        }

        // POST: /Quiz/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.Id == id && q.IsActive)
                .Include(q => q.Questions)
                    .ThenInclude(qn => qn.Options)
                .FirstOrDefaultAsync();

            if (quiz == null) return NotFound();

            var form = Request.Form;
            var userId = _userManager.GetUserId(User);

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

            foreach (var question in quiz.Questions)
            {
                string formKey = $"q_{question.Id}";
                var selectedValue = form[formKey].FirstOrDefault();
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

                var ans = new AttemptAnswer
                {
                    QuizAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedOptionId = selectedOptionId,
                    IsCorrect = isCorrect
                };

                _context.AttemptAnswers.Add(ans);
            }

            attempt.EndTime = DateTime.UtcNow;
            attempt.CorrectAnswers = correctCount;
            attempt.ScorePercent = quiz.Questions.Count == 0
                ? 0
                : (double)correctCount / quiz.Questions.Count * 100.0;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Result), new { id = attempt.Id });
        }

        // GET: /Quiz/Result/5
        public async Task<IActionResult> Result(int id)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.SelectedOption)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attempt == null) return NotFound();

            if (!User.IsInRole("Admin") && _userManager.GetUserId(User) != attempt.UserId)
                return Forbid();

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

            return View(model);
        }
    }
}
