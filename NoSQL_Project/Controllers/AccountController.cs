// Author: Mariana da Costa Reis
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Auth;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository _employees;

        public AccountController(IEmployeeRepository employees)
        {
            _employees = employees;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var employee = _employees.Login(model.Email.Trim().ToLowerInvariant(), model.Password);

            if (employee == null)
            {
                ModelState.AddModelError("", "The email address or password is incorrect.");
                return View(model);
            }

            var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CurrentUser.Create(employee), properties);

            if (Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl!);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
