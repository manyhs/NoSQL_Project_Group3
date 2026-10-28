// Author: Mani Hassani & Kavan Kamangar
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Demo
{
    public class ConsoleMenu
    {
        private readonly IEmployeeRepository _employees;
        private readonly ITicketRepository _tickets;

        public ConsoleMenu(IEmployeeRepository employees, ITicketRepository tickets)
        {
            _employees = employees;
            _tickets = tickets;
        }

        public void Run()
        {
            while (true)
            {
                PrintMenu();
                var choice = Console.ReadLine()?.Trim();
                if (choice == null || choice == "0") return;

                try
                {
                    switch (choice)
                    {
                        case "1": ShowEmployees(); break;
                        case "2": AddEmployee(); break;
                        case "3": EditEmployee(); break;
                        case "4": DeleteEmployee(); break;
                        case "5": ShowTickets(); break;
                        case "6": AddTicket(); break;
                        case "7": EditTicket(); break;
                        case "8": ChangeTicketStatus(); break;
                        case "9": DeleteTicket(); break;
                        case "10": ShowDashboard(); break;
                        default: Error("Unknown choice, type a number from the menu."); break;
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }

                Console.Write("\nPress Enter to go back to the menu...");
                Console.ReadLine();
            }
        }

        private static void PrintMenu()
        {
            Title("NoDesk – data layer menu");
            Console.WriteLine(" 1. Show all employees");
            Console.WriteLine(" 2. Add employee");
            Console.WriteLine(" 3. Edit employee");
            Console.WriteLine(" 4. Delete employee");
            Console.WriteLine(" 5. Show tickets (all or one employee)");
            Console.WriteLine(" 6. Add ticket");
            Console.WriteLine(" 7. Edit ticket");
            Console.WriteLine(" 8. Resolve / close / re-open ticket");
            Console.WriteLine(" 9. Delete ticket");
            Console.WriteLine("10. Dashboard (all or one employee)");
            Console.WriteLine(" 0. Exit");
            Console.Write("Choice: ");
        }

        private void ShowEmployees()
        {
            var list = _employees.GetAll();
            Title($"Employees ({list.Count})");
            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                Console.WriteLine($"{i + 1,3}. {Shorten(e.FullName, 24),-24} {Shorten(e.Email, 30),-30} {e.UserType,-22} {e.Location}");
            }
        }

        private void AddEmployee()
        {
            Title("Add employee");
            var employee = new Employee
            {
                FirstName = Ask("First name"),
                LastName = Ask("Last name"),
                Email = Ask("Email"),
                PhoneNumber = Ask("Phone number"),
                UserType = AskUserType(UserTypes.Employee),
                Location = Ask("Location")
            };
            var password = Ask("Password");

            _employees.Create(employee, password);
            Ok($"Added {employee.FullName} (_id {employee.Id})");
        }

        private void EditEmployee()
        {
            Title("Edit employee");
            var employee = FindEmployee("Email of the employee to edit");
            if (employee == null) return;

            Info("Press Enter to keep the value between [ ].");
            employee.FirstName = Ask("First name", employee.FirstName);
            employee.LastName = Ask("Last name", employee.LastName);
            employee.Email = Ask("Email", employee.Email);
            employee.PhoneNumber = Ask("Phone number", employee.PhoneNumber);
            employee.UserType = AskUserType(employee.UserType);
            employee.Location = Ask("Location", employee.Location);
            var newPassword = AskOptional("New password (Enter = keep the current one)");

            _employees.Update(employee, newPassword);
            Ok($"Updated {employee.FullName}");
        }

        private void DeleteEmployee()
        {
            Title("Delete employee");
            var employee = FindEmployee("Email of the employee to delete");
            if (employee == null) return;

            var ticketCount = _tickets.GetTickets(employee.Id).Count;
            if (ticketCount > 0)
                Info($"Note: {employee.FullName} has {ticketCount} ticket(s). They stay, but show '(deleted employee)'.");

            if (!Confirm($"Delete {employee.FullName}?")) return;
            _employees.Delete(employee.Id!);
            Ok($"Deleted {employee.FullName}");
        }

        private void ShowTickets()
        {
            Title("Show tickets");
            var email = AskOptional("Email of an employee (Enter = all tickets)");
            string? employeeId = null;
            if (email != null)
            {
                var employee = _employees.GetByEmail(email);
                if (employee == null) { Error($"No employee with email {email}."); return; }
                employeeId = employee.Id;
            }

            var list = _tickets.GetTickets(employeeId);
            Title($"Tickets ({list.Count})");
            PrintTickets(list);
        }

        private void AddTicket()
        {
            Title("Add ticket");
            var reporter = FindEmployee("Email of the employee who reports the problem");
            if (reporter == null) return;

            var ticket = new Ticket
            {
                EmployeeId = reporter.Id!,
                Subject = Ask("Subject"),
                Description = Ask("Description"),
                Priority = AskEnum("Priority", TicketPriority.Normal),
                Type = AskEnum("Type", TicketType.Software)
            };
            var days = AskDays("Deadline/follow-up in days", 7);
            ticket.Deadline = DateTime.UtcNow.AddDays(days);

            _tickets.Create(ticket);
            Ok($"Added ticket \"{ticket.Subject}\" for {reporter.FullName}, status {ticket.Status}");
        }

        private void EditTicket()
        {
            Title("Edit ticket");
            var picked = PickTicket();
            if (picked == null) return;

            var ticket = _tickets.GetById(picked.Id!)!;
            Info("Press Enter to keep the value between [ ].");
            ticket.Subject = Ask("Subject", ticket.Subject);
            ticket.Description = Ask("Description", ticket.Description);
            ticket.Priority = AskEnum("Priority", ticket.Priority);
            ticket.Type = AskEnum("Type", ticket.Type);
            var newDeadline = AskOptional($"New deadline yyyy-MM-dd (Enter = keep {ticket.Deadline:yyyy-MM-dd})");
            if (newDeadline != null)
            {
                if (!DateTime.TryParse(newDeadline, out var date)) { Error("Not a valid date, nothing saved."); return; }
                ticket.Deadline = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            _tickets.Update(ticket);
            Ok($"Updated ticket \"{ticket.Subject}\"");
        }

        private void ChangeTicketStatus()
        {
            Title("Resolve / close / re-open ticket");
            var ticket = PickTicket();
            if (ticket == null) return;

            var status = AskEnum("New status", ticket.Status);
            var handler = FindEmployee("Email of the service desk employee handling it");
            if (handler == null) return;
            if (handler.UserType != UserTypes.ServiceDesk)
            {
                Error($"{handler.FullName} is not a Service Desk Employee.");
                return;
            }
            var description = Ask("What was done");

            _tickets.ChangeStatus(ticket.Id!, status, handler.Id!, description);
            Ok($"Ticket \"{ticket.Subject}\" is now {status}, handled by {handler.FullName}");
        }

        private void DeleteTicket()
        {
            Title("Delete ticket");
            var ticket = PickTicket();
            if (ticket == null) return;

            if (!Confirm($"Delete ticket \"{ticket.Subject}\"?")) return;
            _tickets.Delete(ticket.Id!);
            Ok($"Deleted ticket \"{ticket.Subject}\"");
        }

        private void ShowDashboard()
        {
            Title("Dashboard");
            var email = AskOptional("Email of an employee (Enter = all tickets)");
            string? employeeId = null;
            var who = "all employees";
            if (email != null)
            {
                var employee = _employees.GetByEmail(email);
                if (employee == null) { Error($"No employee with email {email}."); return; }
                employeeId = employee.Id;
                who = employee.FullName;
            }

            var stats = _tickets.GetDashboardStats(employeeId);
            Title($"Dashboard for {who} ({stats.Total} tickets)");
            Console.WriteLine($"  Open (not yet solved):    {stats.Open,4}   {stats.OpenPercentage,5}%");
            Console.WriteLine($"  Resolved (closed+solved): {stats.Resolved,4}   {stats.ResolvedPercentage,5}%");
            Console.WriteLine($"  Closed without resolve:   {stats.Closed,4}   {stats.ClosedPercentage,5}%");
            Console.WriteLine($"  Open past deadline:       {stats.PastDeadline,4}");
        }

        private static void PrintTickets(List<TicketWithReporter> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                var reporter = t.Reporter?.FullName ?? "(deleted employee)";
                Console.WriteLine($"{i + 1,3}. {t.CreationDate:yyyy-MM-dd}  {Shorten(t.Subject, 36),-36} {Shorten(reporter, 22),-22} {t.Priority,-7} due {t.Deadline:yyyy-MM-dd}  {t.Status}");
            }
        }

        private TicketWithReporter? PickTicket()
        {
            var list = _tickets.GetTickets();
            if (list.Count == 0) { Error("There are no tickets."); return null; }

            PrintTickets(list);
            var number = AskNumber("Ticket number", 1, list.Count);
            return list[number - 1];
        }

        private Employee? FindEmployee(string label)
        {
            var email = Ask(label);
            var employee = _employees.GetByEmail(email);
            if (employee == null) Error($"No employee with email {email}.");
            return employee;
        }

        private static string Ask(string label, string? current = null)
        {
            while (true)
            {
                Console.Write(current == null ? $"{label}: " : $"{label} [{current}]: ");
                var input = Console.ReadLine()?.Trim() ?? "";

                if (input != "")
                    return input;
                if (current != null)
                    return current;

                Error("This field is required.");
            }
        }

        private static string? AskOptional(string label)
        {
            Console.Write($"{label}: ");
            var input = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(input) ? null : input;
        }

        private static T AskEnum<T>(string label, T current) where T : struct, Enum
        {
            var options = string.Join(" / ", Enum.GetNames<T>());
            while (true)
            {
                var input = Ask($"{label} ({options})", current.ToString());
                if (Enum.TryParse<T>(input, ignoreCase: true, out var value) && Enum.IsDefined(value))
                    return value;
                Error($"Type one of: {options}");
            }
        }

        private static string AskUserType(string current)
        {
            var currentLetter = current == UserTypes.ServiceDesk ? "s" : "e";
            while (true)
            {
                var input = Ask("Type: e = Employee, s = Service Desk Employee", currentLetter).ToLower();
                if (input == "e") return UserTypes.Employee;
                if (input == "s") return UserTypes.ServiceDesk;
                Error("Type e or s.");
            }
        }

        private static int AskNumber(string label, int min, int max)
        {
            while (true)
            {
                var input = Ask($"{label} ({min}-{max})");
                if (int.TryParse(input, out var number) && number >= min && number <= max) return number;
                Error($"Type a number from {min} to {max}.");
            }
        }

        private static int AskDays(string label, int current)
        {
            while (true)
            {
                var input = Ask(label, current.ToString());
                if (int.TryParse(input, out var days) && days >= 1 && days <= 365) return days;
                Error("Type a number of days from 1 to 365.");
            }
        }

        private static bool Confirm(string question)
        {
            Console.Write($"{question} (y/n): ");
            var yes = Console.ReadLine()?.Trim().ToLower() == "y";
            if (!yes) Info("Cancelled.");
            return yes;
        }

        private static string Shorten(string text, int max) =>
            text.Length <= max ? text : text.Substring(0, max - 3) + "...";

        private static void Title(string text) => Write($"\n=== {text} ===", ConsoleColor.Cyan);

        private static void Info(string text) => Console.WriteLine(text);

        private static void Ok(string text) => Write($"✓ {text}", ConsoleColor.Green);

        private static void Error(string text) => Write($"✗ {text}", ConsoleColor.Red);

        private static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
