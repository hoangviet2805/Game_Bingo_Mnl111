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
                TempData["SuccessMessage"] = "Đã lưu câu hỏi thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Appearance()
        {
            ViewBag.BackgroundImages = _settingsService.GetBackgroundImages();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBackground(List<IFormFile> backgroundImages)
        {
            if (backgroundImages != null && backgroundImages.Count > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                var urls = _settingsService.GetBackgroundImages();
                foreach (var file in backgroundImages)
                {
                    if (file.Length > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);
                        string fileName = "bg_" + Guid.NewGuid().ToString("N") + ext;
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        urls.Add($"/images/{fileName}");
                    }
                }
                
                _settingsService.SetBackgroundImages(urls);
                TempData["SuccessMessage"] = "Đã thêm hình nền vào danh sách thành công!";
            }
            return RedirectToAction("Appearance");
        }

        [HttpPost]
        public IActionResult ResetBackground()
        {
            _settingsService.SetBackgroundImages(new List<string>());
            
            try 
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (Directory.Exists(uploadsFolder)) 
                {
                    var oldFiles = Directory.GetFiles(uploadsFolder, "bg_*");
                    foreach (var oldFile in oldFiles)
                    {
                        try { System.IO.File.Delete(oldFile); } catch { }
                    }
                }
            }
            catch { }

            TempData["SuccessMessage"] = "Đã xóa toàn bộ hình nền thành công!";
            return RedirectToAction("Appearance");
        }

        [HttpPost]
        public IActionResult RemoveBackground(int index)
        {
            var urls = _settingsService.GetBackgroundImages();
            if (index >= 0 && index < urls.Count)
            {
                string url = urls[index];
                urls.RemoveAt(index);
                _settingsService.SetBackgroundImages(urls);
                
                try 
                {
                    string fileName = Path.GetFileName(url);
                    string filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                    if (System.IO.File.Exists(filePath)) 
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch { }
                
                TempData["SuccessMessage"] = "Đã xóa hình nền thành công!";
            }
            return RedirectToAction("Appearance");
        }

        [HttpGet]
        public IActionResult Audio()
        {
            ViewBag.BackgroundAudios = _settingsService.GetBackgroundAudios();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAudio(IFormFile backgroundAudio, int repeatCount = 1)
        {
            if (backgroundAudio != null && backgroundAudio.Length > 0)
            {
                string ext = Path.GetExtension(backgroundAudio.FileName);
                string fileName = "bg_audio_" + DateTime.Now.Ticks + ext;
                
                string uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
                if (!Directory.Exists(uploadsFolder)) {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundAudio.CopyToAsync(stream);
                }
                
                var currentAudios = _settingsService.GetBackgroundAudios();
                currentAudios.Add(new BingoGame.Models.AudioTrack { Url = $"/audio/{fileName}", RepeatCount = repeatCount > 0 ? repeatCount : 1 });
                _settingsService.SetBackgroundAudios(currentAudios);
                TempData["SuccessMessage"] = "Đã tải lên và thêm vào danh sách phát thành công!";
            }
            return RedirectToAction("Audio");
        }

        [HttpPost]
        public IActionResult ResetAudio()
        {
            _settingsService.SetBackgroundAudios(new List<BingoGame.Models.AudioTrack>());
            
            try 
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "audio");
                if (Directory.Exists(uploadsFolder)) 
                {
                    var oldFiles = Directory.GetFiles(uploadsFolder, "bg_audio*");
                    foreach (var oldFile in oldFiles)
                    {
                        try { System.IO.File.Delete(oldFile); } catch { }
                    }
                }
            }
            catch { }

            TempData["SuccessMessage"] = "Đã xóa toàn bộ nhạc nền thành công!";
            return RedirectToAction("Audio");
        }

        [HttpPost]
        public IActionResult RemoveAudio(int index)
        {
            var currentAudios = _settingsService.GetBackgroundAudios();
            if (index >= 0 && index < currentAudios.Count)
            {
                string url = currentAudios[index].Url;
                currentAudios.RemoveAt(index);
                _settingsService.SetBackgroundAudios(currentAudios);
                
                try 
                {
                    string fileName = Path.GetFileName(url);
                    string filePath = Path.Combine(_env.WebRootPath, "audio", fileName);
                    if (System.IO.File.Exists(filePath)) 
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch { }
                
                TempData["SuccessMessage"] = "Đã xóa bài nhạc thành công!";
            }
            return RedirectToAction("Audio");
        }

        [HttpPost]
        public IActionResult UpdateAudioRepeatCount(int index, int repeatCount)
        {
            var currentAudios = _settingsService.GetBackgroundAudios();
            if (index >= 0 && index < currentAudios.Count && repeatCount > 0)
            {
                currentAudios[index].RepeatCount = repeatCount;
                _settingsService.SetBackgroundAudios(currentAudios);
                TempData["SuccessMessage"] = "Đã cập nhật số lần lặp thành công!";
            }
            return RedirectToAction("Audio");
        }
    }
}
