using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    public class SurveyAssignController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;


        public SurveyAssignController(AppDbContext context, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var facilities = context.Facilities
                .Include(facility => facility.AppUsers.Where(user => user.UserName != "admin"))
                .ToList();

            SurveyAssignListModel surveyAssignListModel = new SurveyAssignListModel();
            surveyAssignListModel.Facilities = facilities;

            return View(surveyAssignListModel);
        }

        public IActionResult Edit(string id)
        {
            var user = userManager.Users.Include(u => u.Surveys).Where(u => u.Id == id).FirstOrDefault();

            var surveyIds = user.Surveys.Select(s => s.Id).ToList();

            var surveys = context.Surveys
                            .Where(q => !surveyIds.Contains(q.Id))
                            .ToList();

            SurveyAssignEditModel surveyAssignEditModel = new SurveyAssignEditModel()
            {
                Id = user.Id,
                AppUser = user,
                Surveys = surveys,
                UserSurveys = user.Surveys
            };

            return View(surveyAssignEditModel);
        }

        [HttpGet("surveyAssign/{userId}/addSurvey/{surveyId}")]
        public IActionResult AddSurvey(string userId, int surveyId)
        {
            var survey = context.Surveys.Find(surveyId);

            var user = userManager.Users.Include(u => u.Surveys).Include(f => f.Facility).FirstOrDefault(u => u.Id == userId);

            user.Surveys.Add(survey);

            var existingSurveyStatus = context.SurveyStatuses.Where(ss => ss.Survey == survey && ss.AppUser == user && ss.Facility == user.Facility).FirstOrDefault();

            if (existingSurveyStatus == null)
            {
                SurveyStatus surveyStatus = new SurveyStatus();
                surveyStatus.Status = "ATANDI";
                surveyStatus.AppUser = user;
                surveyStatus.Survey = survey;
                surveyStatus.Facility = user.Facility;
                surveyStatus.CreatedTime = DateTime.Now;
                surveyStatus.UpdatedTime = DateTime.Now;
                surveyStatus.CompletedTime = null;

                context.SurveyStatuses.Add(surveyStatus);
            }

            context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = userId });
        }

        [HttpGet("surveyAssign/{userId}/removeSurvey/{surveyId}")]
        public IActionResult RemoveSurvey(string userId, int surveyId)
        {
            var survey = context.Surveys.Find(surveyId);

            var user = userManager.Users.Include(u => u.Surveys).ThenInclude(s => s.Questions).ThenInclude(q => q.Answers).Include(f => f.Facility).FirstOrDefault(u => u.Id == userId);

            foreach (var userSurvey in user.Surveys)
            {
                if (userSurvey == survey)
                {
                    foreach (var question in survey.Questions)
                    {
                        context.Answers.RemoveRange(question.Answers.Where(a => a.AppUser == user).ToList());
                    }
                }
            }

            user.Surveys.Remove(survey);

            var existingSurveyStatus = context.SurveyStatuses.Where(ss => ss.Survey == survey && ss.AppUser == user && ss.Facility == user.Facility).FirstOrDefault();

            if (existingSurveyStatus != null)
            {
                context.SurveyStatuses.Remove(existingSurveyStatus);
            }

            context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = userId });
        }

        public IActionResult AssignedSurveys()
        {
            SurveyAssignedListModel surveyAssignedListModel = new SurveyAssignedListModel();

            if (User.IsInRole("ADMIN"))
            {
                //var surveys = context.Surveys.Include(s => s.AppUsers).ThenInclude(au => au.Facility).ToList();
                var surveys = context.SurveyStatuses.Include(s => s.Survey).Include(au => au.AppUser).Include(f => f.Facility).ToList();

                surveyAssignedListModel.Surveys = surveys;
            }
            else
            {
                var surveys = context.SurveyStatuses.Include(s => s.Survey).Include(s => s.AppUser).Include(f => f.Facility).Where(s => s.AppUser.UserName == User.Identity.Name).ToList();

                if (surveys.Count == 0)
                {
                    return RedirectToAction(nameof(WorkOrders));
                }

                surveyAssignedListModel.Surveys = surveys;
            }

            return View(surveyAssignedListModel);
        }

        [HttpGet("surveyAssign/details/{surveyId}/user/{userId}")]
        public IActionResult AssignedSurveysDetails(int surveyId, string userId)
        {
            var survey = context.Surveys
                .Where(s => s.Id == surveyId)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers.Where(a => a.AppUser.Id == userId))
                .Include(u => u.AppUsers.Where(u => u.Id == userId))
                .ThenInclude(u => u.Facility)
                .FirstOrDefault();

            var answers = context.Answers.Where(a => a.AppUser.Id == userId && survey.Questions.Contains(a.Question)).ToList();

            if (answers.Count != survey.Questions.Count)
            {
                foreach (var question in survey.Questions)
                {
                    if (question.Answers.Count == 0)
                    {
                        Answer answer = new Answer();
                        answer.Question = question;
                        answer.AppUser = survey.AppUsers.FirstOrDefault();
                        answer.CreatedTime = DateTime.Now;

                        context.Answers.Add(answer);
                        context.SaveChanges();
                    }
                }
            }

            var surveya = context.Surveys.Find(surveyId);
            var usera = userManager.Users.Include(u => u.Surveys).Include(f => f.Facility).FirstOrDefault(u => u.Id == userId);
            var existingSurveyStatus = context.SurveyStatuses.Where(ss => ss.Survey == surveya && ss.AppUser == usera && ss.Facility == usera.Facility).FirstOrDefault();
            if (answers.Where(a => a.isSuccess == false).ToList().Count != 0)
            {
                existingSurveyStatus.Status = "DEVAM EDİYOR";
                existingSurveyStatus.CompletedTime = null;
                existingSurveyStatus.UpdatedTime = DateTime.Now;
            }

            context.SaveChanges();

            SurveyAssignedModel surveyAssignedModel = new SurveyAssignedModel();
            surveyAssignedModel.Survey = survey;
            surveyAssignedModel.QuestionAnswers = new Dictionary<int, bool>();
            surveyAssignedModel.AppUsers = userManager.Users.ToList();
            surveyAssignedModel.CreatedTime = existingSurveyStatus.CreatedTime;
            surveyAssignedModel.UpdatedTime = existingSurveyStatus.UpdatedTime;
            surveyAssignedModel.CompletedTime = existingSurveyStatus.CompletedTime;

            return View(surveyAssignedModel);
        }

        [HttpPost]
        public IActionResult AssignedSurveysDetailsEdit(int surveyId, string userId, SurveyAssignedModel surveyAssignedModel)
        {
            var questionIds = surveyAssignedModel.QuestionAnswers.Keys.ToList();

            var answers = context.Answers
                                .Include(a => a.AppUser)
                                .Include(a => a.Question)
                                .Where(a => a.AppUser.Id == userId && questionIds.Contains(a.Question.Id))
                                .ToList();

            foreach (var item in answers)
            {
                if (surveyAssignedModel.QuestionAnswers.TryGetValue(item.Question.Id, out bool isSuccess))
                {
                    item.isSuccess = isSuccess;
                    item.UpdatedTime = DateTime.Now;
                }

                if (surveyAssignedModel.QuestionPhotos.TryGetValue(item.Question.Id, out string? photo))
                {
                    item.Photo = photo;
                    item.UpdatedTime = DateTime.Now;
                }

                if (surveyAssignedModel.QuestionUsers.TryGetValue(item.Question.Id, out string? selectedUserId))
                {
                    item.EmployeeId = selectedUserId;
                    item.UpdatedTime = DateTime.Now;
                }

                if (surveyAssignedModel.QuestionDescriptions.TryGetValue(item.Question.Id, out string? description))
                {
                    item.Description = description;
                    item.UpdatedTime = DateTime.Now;
                }
            }

            var surveya = context.Surveys.Find(surveyId);
            var usera = userManager.Users.Include(u => u.Surveys).Include(f => f.Facility).FirstOrDefault(u => u.Id == userId);
            var existingSurveyStatus = context.SurveyStatuses.Where(ss => ss.Survey == surveya && ss.AppUser == usera && ss.Facility == usera.Facility).FirstOrDefault();
            existingSurveyStatus.UpdatedTime = DateTime.Now;
            if (answers.Where(a => a.isSuccess == false).ToList().Count == 0)
            {
                existingSurveyStatus.Status = "TAMAMLANDI";
                existingSurveyStatus.CompletedTime = DateTime.Now;
                existingSurveyStatus.UpdatedTime = null;
            } else
            {
                existingSurveyStatus.Status = "DEVAM EDİYOR";
                existingSurveyStatus.CompletedTime = null;
                existingSurveyStatus.UpdatedTime = DateTime.Now;
            }

            context.SaveChanges();

            return RedirectToAction(nameof(AssignedSurveys));
        }

        public IActionResult WorkOrders()
        {
            var workOrders = context.Surveys.Include(s => s.Questions.Where(q => q.Answers.Any(a => a.isSuccess == false))).ThenInclude(q => q.Answers.Where(a => a.isSuccess == false)).ThenInclude(a => a.AppUser).ThenInclude(au => au.Facility).ToList();
            return View(new WorkOrdersListModel() { Surveys = workOrders, AppUsers = userManager.Users.ToList() });
        }
    }
}
