# NoDesk – NoSQL Project (Group 3)

Incident management system for The Garden Group, built with ASP.NET Core MVC (.NET 8) and MongoDB Atlas.

## Run it in JetBrains Rider
1. Open `NoSQL_Project.sln`. NuGet packages are restored automatically.
2. Copy `.env.example` to `.env` (same folder as the .sln) and put the Atlas connection string in it.
3. Run the app (green ▶, profile `http`) and open http://localhost:5080.
4. Log in with a test account:

| Role | Email | Password |
|---|---|---|
| Employee | anna.jansen1@tgg.nl | Password001 |
| Service Desk Employee | mark.devries2@tgg.nl | Password002 |

Every employee in the test data uses `Password` + their number with three digits (e.g. employee 45 → `Password045`).
Odd numbers are regular employees, even numbers are service desk employees.

Tests: right-click `NoSQL_Project_Tests` → **Run Unit Tests** (they use a separate database `garden_group_test`).
Terminal menu for the data layer: double-click `demo.command` (Mac).

## Structure
```
NoSQL_Project/
  Auth/                 login claims and role checks
  Controllers/          Account, Dashboard, Tickets, TicketEditor, Escalation, Users
  Features/             individual features: TicketSorter, TicketFilter, TicketTransferService, TicketEscalationService
  Models/ + ViewModels/ data and page models
  Repositories/         all MongoDB code (CRUD + aggregations)
  Views/                Razor pages
  wwwroot/css/site.css  NoDesk styling
NoSQL_Project_Tests/    NUnit tests for the repositories
```
