# NoDesk – NoSQL Project (Group 3) – Deliverable 2

Data layer (CRUD + aggregations) for tickets and employees, using ASP.NET Core MVC and MongoDB Atlas.

## Run it in JetBrains Rider
1. Open `NoSQL_Project.sln` in Rider. NuGet packages are restored automatically.
2. Copy `.env.example` to `.env` (same folder as the .sln) and put your Atlas connection string in it.
3. Run the tests: right-click `NoSQL_Project_Tests` → **Run Unit Tests**.
   The tests use a separate database `garden_group_test`, so your real data is never touched.
4. Run the app (green ▶, profile `http`) and open http://localhost:5080 to see the dashboard numbers
   and the ticket list from the real `garden_group` database.

5. **Menu in the Terminal (Mac):** double-click `demo.command`. A menu opens where you can show, add,
   edit and delete employees and tickets, resolve or close tickets and see the dashboard.
   Changes are made in the real database (check them in Compass).
   (Any OS: `dotnet run --project NoSQL_Project -- --demo`.)
   If macOS blocks it, run once in Terminal: `xattr -dr com.apple.quarantine <path to this folder>`.

Requires the .NET 8 SDK.

## Structure
```
NoSQL_Project/
  Models/                      Employee, Ticket, HandledBy, enums, DashboardStats, TicketWithReporter
  Repositories/Interfaces/     IEmployeeRepository, ITicketRepository  (documented contracts)
  Repositories/                EmployeeRepository, TicketRepository    (MongoDB code)
  Database/MongoIndexes.cs     indexes, created at startup
  Demo/ConsoleMenu.cs          Terminal menu to try every repository method (demo.command)
  Controllers/HomeController   test page showing the data layer works
  Program.cs                   dependency injection setup (from the teacher's guide)
NoSQL_Project_Tests/           NUnit tests for every repository method
```

## Before handing in
Every file starts with an `Author:` line naming the group members responsible for it.
