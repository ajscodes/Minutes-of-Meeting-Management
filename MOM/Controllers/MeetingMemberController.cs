using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        public IActionResult MeetingMemberList()
        {
            return View();
        }
        public IActionResult MeetingMemberAddEdit()
        {
            return View();
        }
    }
}
