// Author: Mani Hassani
using NoSQL_Project.Models;

namespace NoSQL_Project.Features
{
    public static class TicketSorter
    {
        public const string HighestFirst = "priority-desc";
        public const string LowestFirst = "priority-asc";

        public static List<TicketWithReporter> Sort(List<TicketWithReporter> tickets, string? sort)
        {
            if (sort == HighestFirst)
                return tickets.OrderByDescending(t => t.Priority).ThenByDescending(t => t.CreationDate).ToList();

            if (sort == LowestFirst)
                return tickets.OrderBy(t => t.Priority).ThenByDescending(t => t.CreationDate).ToList();

            return tickets;
        }

        public static string NextSort(string? currentSort)
        {
            return currentSort == HighestFirst ? LowestFirst : HighestFirst;
        }

        public static string ButtonText(string? currentSort)
        {
            return currentSort == HighestFirst ? "Sort by priority: low first ↑" : "Sort by priority: high first ↓";
        }

        public static string Arrow(string? currentSort)
        {
            if (currentSort == HighestFirst)
                return "↓";
            if (currentSort == LowestFirst)
                return "↑";
            return "↕";
        }
    }
}
