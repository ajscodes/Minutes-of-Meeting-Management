using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{

    public class MeetingController : Controller
    {
        public IActionResult MeetingList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MeetingAddEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MeetingAddEdit(MOM.Models.Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                // Here you would typically save data to the database
                // For demonstration, we just return a success message
                TempData["Message"] = "Meeting saved successfully!";
                return RedirectToAction("MeetingList");
            }
            
            // If validation fails, return the same view with validation messages
            return View(meeting);
        }

        public IActionResult ScheduleMeeting(int? meetingTypeId)
        {
            return View();
        }

    }
}
