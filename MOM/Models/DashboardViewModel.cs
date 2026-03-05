using System.Collections.Generic;

namespace MOM.Models
{
    public class DashboardViewModel
    {
        public int TotalMeetings { get; set; }
        public int UpcomingMeetings { get; set; }
        public int CompletedMeetings { get; set; }
        public int CancelledMeetings { get; set; }

        public List<string> MeetingsByTypeLabels { get; set; } = new List<string>();
        public List<int> MeetingsByTypeSeries { get; set; } = new List<int>();

        public List<string> MeetingsByDepartmentLabels { get; set; } = new List<string>();
        public List<int> MeetingsByDepartmentSeries { get; set; } = new List<int>();
    }
}

