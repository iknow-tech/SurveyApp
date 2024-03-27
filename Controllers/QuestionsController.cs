using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class QuestionsController : Controller
    {
        private readonly AppDbContext context;

        public QuestionsController(AppDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var questions = context.Questions.Include(q => q.Survey).ToList();
            List<QuestionListModel> questionListModels = new List<QuestionListModel>();

            foreach (var item in questions)
            {
                QuestionListModel questionListModel = new QuestionListModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    SurveyName = item.Survey?.Name != null ? item.Survey.Name : ""
                };

                questionListModels.Add(questionListModel);
            }

            return View(questionListModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(QuestionCreateModel questionCreateModel)
        {
            context.Questions.Add(new Question()
            {
                Name = questionCreateModel.Name,
                Description = questionCreateModel.Description
            });

            context.SaveChanges();

            ViewBag.Success = true;
            ModelState.Clear();
            return View();
        }

        public IActionResult Edit(int id)
        {
            var question = context.Questions.Find(id);

            QuestionEditModel questionEditModel = new QuestionEditModel()
            {
                Id = question.Id,
                Name = question.Name,
                Description = question.Description
            };

            return View(questionEditModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, QuestionEditModel questionEditModel)
        {
            var question = context.Questions.Find(id);

            question.Name = questionEditModel.Name;
            question.Description = questionEditModel.Description;

            context.SaveChanges();

            ViewBag.Success = true;
            return View(questionEditModel);
        }

        public IActionResult Delete(int id)
        {
            var question = context.Questions.Find(id);
            context.Questions.Remove(question);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
