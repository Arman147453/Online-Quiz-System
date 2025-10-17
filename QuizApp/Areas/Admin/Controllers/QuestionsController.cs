using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.ViewModels;

namespace QuizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Questions/Create?quizId=1
        public async Task<IActionResult> Create(int quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null) return NotFound();

            var vm = new QuestionCreateViewModel
            {
                QuizId = quizId,
                QuizTitle = quiz.Title,
                Options = new List<OptionCreateViewModel>
                {
                    new OptionCreateViewModel(),
                    new OptionCreateViewModel(),
                    new OptionCreateViewModel(),
                    new OptionCreateViewModel()
                }
            };
            return View(vm);
        }

        // POST: Admin/Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var question = new Question
            {
                QuizId = vm.QuizId,
                Text = vm.Text
            };
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            foreach (var opt in vm.Options.Where(o => !string.IsNullOrWhiteSpace(o.Text)))
            {
                _context.Options.Add(new Option
                {
                    QuestionId = question.Id,
                    Text = opt.Text,
                    IsCorrect = opt.IsCorrect
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Quizzes", new { area = "Admin", id = vm.QuizId });
        }

        // GET: Admin/Questions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return NotFound();

            return View(question);
        }
    }
}
