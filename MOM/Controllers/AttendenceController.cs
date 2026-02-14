using Microsoft.AspNetCore.Mvc;
using MOM.Models;

namespace MOM.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult AttendanceReport()
        {
            var list = new List<AttendanceReportRow>
            {
                new AttendanceReportRow
                {
                    MeetingDate = new DateTime(2025, 12, 31, 13, 13, 0),
                    MeetingType = "Board Meeting",
                    MeetingVenue = "Board Room",
                    Department = "Finance",
                    StaffName = "David Martinez",
                    StaffEmail = "david.m@company.com",
                    IsPresent = true
                },
                new AttendanceReportRow
                {
                    MeetingDate = new DateTime(2025, 12, 31, 13, 13, 0),
                    MeetingType = "Board Meeting",
                    MeetingVenue = "Board Room",
                    Department = "Finance",
                    StaffName = "James Wilson",
                    StaffEmail = "james.w@company.com",
                    IsPresent = true
                },
                new AttendanceReportRow
                {
                    MeetingDate = new DateTime(2025, 12, 31, 13, 13, 0),
                    MeetingType = "Board Meeting",
                    MeetingVenue = "Board Room",
                    Department = "Finance",
                    StaffName = "Jennifer Garcia",
                    StaffEmail = "jennifer.g@company.com",
                    IsPresent = false
                }
            };

            return View(list);
        }
    }
}
