using BingoGame.Models;
using System.Text.Json;
using System.IO;

namespace BingoGame.Services
{
    public class QuestionService
    {
        private readonly string _filePath;
        private readonly string _rulePath;

        public QuestionService(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "questions.json");
            _rulePath = Path.Combine(env.ContentRootPath, "rules.txt");
        }

        public string GetRules()
        {
            if (File.Exists(_rulePath))
            {
                return File.ReadAllText(_rulePath);
            }
            return "Luật chơi chưa được thiết lập. Hãy nhấn nút Chỉnh sửa để thêm luật chơi.";
        }

        public void SaveRules(string rules)
        {
            File.WriteAllText(_rulePath, rules ?? "");
        }

        public List<QuestionItem> GetQuestions()
        {
            if (!File.Exists(_filePath))
            {
                var defaultQuestions = Enumerable.Range(1, 16).Select(i => new QuestionItem
                {
                    Id = i,
                    QuestionText = $"Câu hỏi {i} - Nhập nội dung câu hỏi tại đây",
                    AnswerText = $"Đáp án {i} - Nhập nội dung đáp án tại đây"
                }).ToList();
                SaveQuestions(defaultQuestions);
                return defaultQuestions;
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<QuestionItem>>(json) ?? new List<QuestionItem>();
        }

        public void SaveQuestions(List<QuestionItem> questions)
        {
            var json = JsonSerializer.Serialize(questions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void UpdateQuestion(QuestionItem updatedQuestion)
        {
            var questions = GetQuestions();
            var index = questions.FindIndex(q => q.Id == updatedQuestion.Id);
            if (index != -1)
            {
                questions[index] = updatedQuestion;
                SaveQuestions(questions);
            }
        }
    }
}
