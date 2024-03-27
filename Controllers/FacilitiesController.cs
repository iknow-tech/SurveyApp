using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class FacilitiesController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;

        public FacilitiesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var facilities = context.Facilities.ToList();
            var facilityListModels = new List<FacilityListModel>();

            foreach (var item in facilities)
            {
                var facilityListModel = new FacilityListModel() 
                { 
                    Id = item.Id,
                    Name = item.Name,
                    City = item.City,
                    Local = item.Local,
                };

                facilityListModels.Add(facilityListModel);
            }

            return View(facilityListModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(FacilityCreateModel facilityCreateModel)
        {
            context.Facilities.Add(new Facility()
            {
                Name = facilityCreateModel.Name,
                City = facilityCreateModel.City,
                Local = facilityCreateModel.Local
            });

            context.SaveChanges();

            ViewBag.Success = true;
            ModelState.Clear();
            return View();
        }

        public IActionResult Edit(int id)
        {
            var facility = context.Facilities.Find(id);

            var facilityEditModel = new FacilityEditModel() 
            { 
                Id = facility.Id,
                Name = facility.Name,
                City = facility.City,
                Local = facility.Local
            };

            return View(facilityEditModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, FacilityEditModel facilityEditModel)
        {
            var facility = context.Facilities.Find(id);

            facility.Name = facilityEditModel.Name;
            facility.City = facilityEditModel.City;
            facility.Local = facilityEditModel.Local;

            context.SaveChanges();

            ViewBag.Success = true;
            return View(facilityEditModel);
        }

        public IActionResult Delete(int id)
        {
            var facility = context.Facilities.Find(id);
            context.Facilities.Remove(facility);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("facilities/{facilityId}/addUser")]
        public IActionResult AddUser(int facilityId)
        {
            var facility = context.Facilities
                        .Include(s => s.AppUsers)
                        .FirstOrDefault(s => s.Id == facilityId);

            var userIds = facility.AppUsers.Select(sq => sq.Id).ToList();
            var appUsers = userManager.Users
                            .Include(u => u.Facility)
                            .Where(q => !userIds.Contains(q.Id) && q.UserName != "admin")
                            .ToList();

            FacilityAddUserModel facilityAddUserModel = new FacilityAddUserModel()
            {
                Id = facility.Id,
                Name =  $"{facility.Name}, {facility.City}, {facility.Local}",
                Users = appUsers,
                FacilityUsers = facility.AppUsers
            };

            return View(facilityAddUserModel);
        }

        [HttpGet("facilities/{facilityId}/addUser/{userId}")]
        public IActionResult AddUserToFacility(int facilityId, string userId)
        {
            var user = userManager.Users.Where(user => user.Id == userId).FirstOrDefault();

            var facility = context.Facilities
                        .Include(s => s.AppUsers)
                        .FirstOrDefault(s => s.Id == facilityId);

            facility.AppUsers.Add(user);

            context.SaveChanges();

            return RedirectToAction(nameof(AddUser), new { facilityId = facilityId });
        }

        [HttpGet("facilities/{facilityId}/removeUser/{userId}")]
        public IActionResult RemoveUserFromFacility(int facilityId, string userId)
        {
            var user = userManager.Users.Include(u => u.Surveys).ThenInclude(s => s.Questions).ThenInclude(q => q.Answers).Where(user => user.Id == userId).FirstOrDefault();
           
            var facility = context.Facilities
                        .Include(s => s.AppUsers)
                        .FirstOrDefault(s => s.Id == facilityId);

            foreach (var userSurvey in user.Surveys)
            {
                    foreach (var question in userSurvey.Questions)
                    {
                        context.Answers.RemoveRange(question.Answers.Where(a => a.AppUser == user).ToList());
                    }
            }

            facility.AppUsers.Remove(user);
            user.Surveys.Clear();

            var existingSurveyStatus = context.SurveyStatuses.Where(ss => ss.AppUser == user && ss.Facility == user.Facility).ToList();

            if (existingSurveyStatus != null)
            {
                context.SurveyStatuses.RemoveRange(existingSurveyStatus);
            }

            context.SaveChanges();

            return RedirectToAction(nameof(AddUser), new { facilityId = facilityId });
        }
    }
}
