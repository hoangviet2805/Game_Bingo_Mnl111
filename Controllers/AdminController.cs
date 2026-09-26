using Microsoft.AspNetCore.Mvc;
using BingoGame.Models;
using BingoGame.Services;

namespace BingoGame.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuestionService _questionService;

        public AdminController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        public IActionResult Index()
        {
            var questions = _questionService.GetQuestions();
            return View(questions);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var questions = _questionService.GetQuestions();
            var question = questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        [HttpPost]
        public IActionResult Edit(QuestionItem model)
        {
            if (ModelState.IsValid)
            {
                _questionService.UpdateQuestion(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
