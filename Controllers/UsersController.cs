using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            List<UserListModel> userListModels = new List<UserListModel>();

            var users = userManager.Users;

            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user,"ADMIN"))
                {
                    var userListModel = new UserListModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    };

                    userListModels.Add(userListModel);
                }
            }

            return View(userListModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel userCreateModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = userCreateModel.UserName,
                    Email = userCreateModel.Email,
                    FullName = userCreateModel.FullName,
                    PhoneNumber = userCreateModel.PhoneNumber
                };

                IdentityResult result = await userManager.CreateAsync(user, userCreateModel.Password);

                if (result.Succeeded)
                {
                    ViewBag.Success = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(userCreateModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(string id, UserEditModel model)
        {
            if (id != model.Id)
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

                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                {
                    await userManager.RemovePasswordAsync(user);
                    result = await userManager.AddPasswordAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    ViewBag.Success = true;
                    return View(model);
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
