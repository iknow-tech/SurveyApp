using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userName = User.Identity?.Name;

            if (userName == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            UserEditModel userEditModel = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };

            return View(userEditModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string id, UserEditModel userEditModel)
        {
            if (User.Identity?.Name != userEditModel.UserName)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                user.UserName = userEditModel.UserName;
                user.FullName = userEditModel.FullName;
                user.Email = userEditModel.Email;
                user.PhoneNumber = userEditModel.PhoneNumber;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded && !string.IsNullOrEmpty(userEditModel.Password))
                {
                    await userManager.RemovePasswordAsync(user);
                    result = await userManager.AddPasswordAsync(user, userEditModel.Password);
                }

                if (result.Succeeded)
                {
                    ViewBag.Success = true;
                    return View(userEditModel);
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(userEditModel);
        }

    }
}
