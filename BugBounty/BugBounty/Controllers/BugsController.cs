namespace BugBounty.Controllers
{
    using Bug.Bounty.Base;
    using System;
    using System.Web.Mvc;

    public class BugsController : Controller
    {
        private BugManagement _bugManagement = new BugManagement();
        public bool Startup()
        {
            return _bugManagement.StartUp();
        }

        public ActionResult Index()
        {
            var userId = Guid.Parse("7e359732-edec-4d8c-a6f8-a7ae075a4761");
            ViewBag.BugsList = _bugManagement.GetBugs(_bugManagement.GetUserDetails(userId).Platform, userId);
            return View();
        }

        public ActionResult CreateBug()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PostBug()
        {
            return new JsonResult { Data = true };
        }

        [HttpPost]
        public ActionResult UpdateBug()
        {
            return View();
        }

        public ActionResult ListBugs()
        {
            return View();
        }

        public ActionResult ViewBug()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateBug()
        {
            return View();
        }
    }
}