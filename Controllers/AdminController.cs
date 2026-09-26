using Microsoft.AspNetCore.Mvc;
using BingoGame.Models;
using BingoGame.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BingoGame.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuestionService _questionService;
        private readonly SettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public AdminController(QuestionService questionService, SettingsService settingsService, IWebHostEnvironment env)
        {
            _questionService = questionService;
            _settingsService = settingsService;
            _env = env;
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

        [HttpGet]
        public IActionResult Appearance()
        {
            ViewBag.BackgroundImage = _settingsService.GetBackgroundImage();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBackground(IFormFile backgroundImage)
        {
            if (backgroundImage != null && backgroundImage.Length > 0)
            {
                string ext = Path.GetExtension(backgroundImage.FileName);
                string fileName = "bg_" + DateTime.Now.Ticks + ext;
                
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) {
                    Directory.CreateDirectory(uploadsFolder);
                }

                try 
                {
                    var oldFiles = Directory.GetFiles(uploadsFolder, "bg_*");
                    foreach (var oldFile in oldFiles)
                    {
                        try { System.IO.File.Delete(oldFile); } catch { }
                    }
                }
                catch { }
                
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundImage.CopyToAsync(stream);
                }
                
                _settingsService.SetBackgroundImage($"/images/{fileName}");
            }
            return RedirectToAction("Appearance");
        }

        [HttpPost]
        public IActionResult ResetBackground()
        {
            _settingsService.SetBackgroundImage("");
            return RedirectToAction("Appearance");
        }

        [HttpGet]
        public IActionResult Audio()
        {
            ViewBag.BackgroundAudio = _settingsService.GetBackgroundAudio();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAudio(IFormFile backgroundAudio)
        {
            if (backgroundAudio != null && backgroundAudio.Length > 0)
            {
                string ext = Path.GetExtension(backgroundAudio.FileName);
                string fileName = "bg_audio_" + DateTime.Now.Ticks + ext;
                
                string uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
                if (!Directory.Exists(uploadsFolder)) {
                    Directory.CreateDirectory(uploadsFolder);
                }

                try 
                {
                    var oldFiles = Directory.GetFiles(uploadsFolder, "bg_audio*");
                    foreach (var oldFile in oldFiles)
                    {
                        try { System.IO.File.Delete(oldFile); } catch { }
                    }
                }
                catch { }
                
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundAudio.CopyToAsync(stream);
                }
                
                _settingsService.SetBackgroundAudio($"/audio/{fileName}");
            }
            return RedirectToAction("Audio");
        }

        [HttpPost]
        public IActionResult ResetAudio()
        {
            _settingsService.SetBackgroundAudio("");
            return RedirectToAction("Audio");
        }
    }
}
