using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BingoGame.Models;

namespace BingoGame.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BingoGame.Services.QuestionService _questionService;

    public HomeController(ILogger<HomeController> logger, BingoGame.Services.QuestionService questionService)
    {
        _logger = logger;
        _questionService = questionService;
    }

    public IActionResult Index()
    {
        var questions = _questionService.GetQuestions();
        return View(questions);
    }

    [HttpGet]
    public IActionResult Rules()
    {
        var rules = _questionService.GetRules();
        return View((object)rules);
    }

    [HttpPost]
    public IActionResult SaveRules(string rules)
    {
        _questionService.SaveRules(rules);
        return RedirectToAction("Rules");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
