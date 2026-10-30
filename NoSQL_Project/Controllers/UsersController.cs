// Author: Cristhel Mae Solomon
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NoSQL_Project.Auth;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    [Authorize(Roles = UserTypes.ServiceDesk)]
    public class UsersController : Controller
    {
        private readonly IEmployeeRepository _employees;

        public UsersController(IEmployeeRepository employees)
        {
            _employees = employees;
        }

        public IActionResult Index(string? email)
        {
            var users = _employees.GetAllWithTicketCount();

            if (!string.IsNullOrWhiteSpace(email))
                users = users.Where(u => u.Email.Contains(email.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            return View(new UserListViewModel { Users = users, Email = email });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError(nameof(model.Password), "Please choose a starting password.");

            CheckChoices(model);
            if (!ModelState.IsValid)
                return View(model);

            var employee = new Employee();
            CopyToEmployee(model, employee);

            try
            {
                _employees.Create(employee, model.Password!);
            }
            catch (MongoWriteException)
            {
                ModelState.AddModelError(nameof(model.Email), "This email address is already used by another employee.");
                return View(model);
            }

            TempData["Message"] = $"{employee.FullName} was added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var employee = _employees.GetById(id);
            if (employee == null)
                return NotFound();

            var model = new UserFormViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                UserType = employee.UserType,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Location = employee.Location
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserFormViewModel model)
        {
            var employee = _employees.GetById(model.Id ?? "");
            if (employee == null)
                return NotFound();

            CheckChoices(model);
            if (!ModelState.IsValid)
                return View(model);

            CopyToEmployee(model, employee);
            var newPassword = string.IsNullOrWhiteSpace(model.Password) ? null : model.Password;

            try
            {
                _employees.Update(employee, newPassword);
            }
            catch (MongoWriteException)
            {
                ModelState.AddModelError(nameof(model.Email), "This email address is already used by another employee.");
                return View(model);
            }

            TempData["Message"] = $"{employee.FullName} was updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (id == User.GetEmployeeId())
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            if (_employees.Delete(id))
                TempData["Message"] = "The employee was deleted.";
            else
                TempData["Error"] = "The employee could not be found.";

            return RedirectToAction(nameof(Index));
        }

        private void CheckChoices(UserFormViewModel model)
        {
            if (model.UserType != UserTypes.Employee && model.UserType != UserTypes.ServiceDesk)
                ModelState.AddModelError(nameof(model.UserType), "Please choose a valid type of user.");

            if (!UserFormViewModel.Locations.Contains(model.Location))
                ModelState.AddModelError(nameof(model.Location), "Please choose a valid location.");
        }

        private static void CopyToEmployee(UserFormViewModel model, Employee employee)
        {
            employee.FirstName = model.FirstName.Trim();
            employee.LastName = model.LastName.Trim();
            employee.UserType = model.UserType;
            employee.Email = model.Email.Trim().ToLowerInvariant();
            employee.PhoneNumber = model.PhoneNumber?.Trim() ?? "";
            employee.Location = model.Location;
        }
    }
}
