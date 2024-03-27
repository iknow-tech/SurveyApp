using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Entities;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginModel accountLoginModel)
        {
            if (ModelState.IsValid)
            {
                if (accountLoginModel.UserName == null)
                {
                    ModelState.AddModelError("", "Lütfen kullanıcı adı giriniz.");

                    return RedirectToAction("Login");
                }

                if (accountLoginModel.Password == null)
                {
                    ModelState.AddModelError("", "Lütfen parola giriniz.");

                    return RedirectToAction("Login");
                }

                var user = await userManager.FindByNameAsync(accountLoginModel.UserName);

                if (user != null)
                {
                    await signInManager.SignOutAsync();

                    var result = await signInManager.PasswordSignInAsync(user, accountLoginModel.Password, accountLoginModel.RememberMe ,false);

                    if (result.Succeeded)
                    {
                        var isAdmin = await userManager.IsInRoleAsync(user, "ADMIN");

                        if (isAdmin)
                        {
                            return RedirectToAction(actionName:"Index", controllerName:"Surveys");
                        }

                        return RedirectToAction(actionName: "AssignedSurveys", controllerName: "SurveyAssign");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Yanlış parola!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu kullanıcı bulunamadı.");
                }
            }

            return View(accountLoginModel);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
