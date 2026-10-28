// Author: Mani Hassani & Kavan Kamangar
namespace NoSQL_Project.Models
{
    public class DashboardStats
    {
        public int Open { get; set; }
        public int Resolved { get; set; }
        public int Closed { get; set; }

        public int PastDeadline { get; set; }

        public int Total => Open + Resolved + Closed;

        public double OpenPercentage => Percentage(Open);

        public double ResolvedPercentage => Percentage(Resolved);

        public double ClosedPercentage => Percentage(Closed);

        private double Percentage(int count) =>
            Total == 0 ? 0 : Math.Round(100.0 * count / Total, 1);
    }
}
