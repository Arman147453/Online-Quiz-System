using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;

namespace QuizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Options/Create?questionId=5
        public IActionResult Create(int questionId)
        {
            ViewBag.QuestionId = questionId;
            return View(new Option { QuestionId = questionId });
        }

        // POST: Admin/Options/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Option option)
        {
            if (ModelState.IsValid)
            {
                _context.Options.Add(option);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Questions", new { area = "Admin", id = option.QuestionId });
            }
            ViewBag.QuestionId = option.QuestionId;
            return View(option);
        }

        // GET: Admin/Options/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var option = await _context.Options.FindAsync(id);
            if (option == null) return NotFound();

            return View(option);
        }

        // POST: Admin/Options/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Option option)
        {
            if (id != option.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(option);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Questions", new { area = "Admin", id = option.QuestionId });
            }
            return View(option);
        }

        // GET: Admin/Options/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var option = await _context.Options
                .Include(o => o.Question)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (option == null) return NotFound();

            return View(option);
        }

        // POST: Admin/Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var option = await _context.Options.FindAsync(id);
            if (option != null)
            {
                var questionId = option.QuestionId;
                _context.Options.Remove(option);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Questions", new { area = "Admin", id = questionId });
            }
            return NotFound();
        }
    }
}
