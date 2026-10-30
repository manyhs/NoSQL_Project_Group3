// Author: Mariana da Costa Reis
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using NoSQL_Project.Models;

namespace NoSQL_Project.Auth
{
    public static class CurrentUser
    {
        public static ClaimsPrincipal Create(Employee employee)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, employee.Id ?? ""),
                new(ClaimTypes.Name, employee.FullName),
                new(ClaimTypes.Email, employee.Email),
                new(ClaimTypes.Role, employee.UserType)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public static string GetEmployeeId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        public static bool IsServiceDesk(this ClaimsPrincipal user)
        {
            return user.IsInRole(UserTypes.ServiceDesk);
        }
    }
}
