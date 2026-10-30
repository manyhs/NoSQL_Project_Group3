// Author: Mariana da Costa Reis
using NoSQL_Project.Models;

namespace NoSQL_Project.Features
{
    public static class TicketFilter
    {
        public static List<TicketWithReporter> ByKeywords(List<TicketWithReporter> tickets, string? keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return tickets;

            var words = keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return tickets.Where(ticket => words.All(word => Matches(ticket, word))).ToList();
        }

        private static bool Matches(TicketWithReporter ticket, string word)
        {
            var texts = new[]
            {
                ticket.Subject,
                ticket.Description,
                ticket.Type.ToString(),
                ticket.Priority.ToString(),
                ticket.Status.ToString(),
                ticket.Reporter?.FullName ?? ""
            };

            return texts.Any(text => text.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
    }
}
