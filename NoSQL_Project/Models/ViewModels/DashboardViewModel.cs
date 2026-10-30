// Author: Mani Hassani
namespace NoSQL_Project.Models
{
    public class DashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new();

        public bool ShowsAllTickets { get; set; }

        public List<DashboardTile> Tiles => new()
        {
            new DashboardTile("Unresolved incidents", "All tickets currently open", Stats.Open, Stats.Total, "tile-open"),
            new DashboardTile("Resolved", "Closed and solved", Stats.Resolved, Stats.Total, "tile-resolved"),
            new DashboardTile("Closed without resolve", "Closed but not solved", Stats.Closed, Stats.Total, "tile-closed"),
            new DashboardTile("Incidents past deadline", "These tickets need your immediate attention", Stats.PastDeadline, Stats.Open, "tile-deadline")
        };
    }

    public class DashboardTile
    {
        public DashboardTile(string title, string subtitle, int count, int total, string cssClass)
        {
            Title = title;
            Subtitle = subtitle;
            Count = count;
            Total = total;
            CssClass = cssClass;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public int Count { get; }
        public int Total { get; }
        public string CssClass { get; }

        public double Percentage => Total == 0 ? 0 : Math.Round(100.0 * Count / Total, 1);
    }
}
