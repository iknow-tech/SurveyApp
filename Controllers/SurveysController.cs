using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class SurveysController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;

        public SurveysController(AppDbContext context, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var surveys = context.Surveys.ToList();
            List<SurveyListModel> surveyListModels = new List<SurveyListModel>();

            foreach (var item in surveys)
            {
                SurveyListModel surveyListModel = new SurveyListModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };

                surveyListModels.Add(surveyListModel);
            }

            return View(surveyListModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SurveyCreateModel surveyCreateModel)
        {
            context.Surveys.Add(new Survey()
            {
                Name = surveyCreateModel.Name
            });

            context.SaveChanges();

            ViewBag.Success = true;
            ModelState.Clear();
            return View();
        }

        public IActionResult Edit(int id)
        {
            var survey = context.Surveys
                        .Include(s => s.Questions)
                        .FirstOrDefault(s => s.Id == id);

            //var questionIds = survey.Questions.Select(q => q.Id).ToList();
            var questions = context.Questions
                            .Where(q => q.Survey == null)
                            .ToList();

            SurveyEditModel surveyEditModel = new SurveyEditModel()
            {
                Id = survey.Id,
                Name = survey.Name,
                Questions = questions,
                SurveyQuestions = survey.Questions
            };

            return View(surveyEditModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, SurveyEditModel surveyEditModel)
        {
            var survey = context.Surveys.Find(id);

            survey.Name = surveyEditModel.Name;

            context.SaveChanges();

            ViewBag.Success = true;
            return View(surveyEditModel);
        }

        public IActionResult Delete(int id)
        {
            var survey = context.Surveys.Find(id);
            context.Surveys.Remove(survey);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("survey/{surveyId}/addQuestion/{questionId}")]
        public IActionResult AddQuestion(int surveyId, int questionId)
        {
            var question = context.Questions.Find(questionId);

            var survey = context.Surveys
                        .Include(s => s.Questions)
                        .FirstOrDefault(s => s.Id == surveyId);

            survey.Questions.Add(question);

            context.SaveChanges();

            return RedirectToAction(nameof(Edit), new {id = surveyId});
        }

        [HttpGet("survey/{surveyId}/removeQuestion/{questionId}")]
        public IActionResult RemoveQuestion(int surveyId, int questionId)
        {
            var question = context.Questions.Find(questionId);

            var survey = context.Surveys
                        .Include(s => s.Questions)
                        .FirstOrDefault(s => s.Id == surveyId);

            survey.Questions.Remove(question);

            context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = surveyId });
        }
    }
}
