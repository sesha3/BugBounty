namespace BugBounty.Controllers
{
    using Bug.Bounty.Base;
    using Newtonsoft.Json;
    using System;
    using System.Web.Mvc;
    using Bug.Bounty.DataClasses;

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
        public JsonResult PostBug(string bugData)
        {
            var bug = JsonConvert.DeserializeObject<Bug>(bugData);
            bool result = false;
            if (bug.Id != null && bug.Id != Guid.Empty)
            {
                result = _bugManagement.UpdateBug(bug);
            }
            else
            {
                result = _bugManagement.AddBug(bug);
            }

            return new JsonResult { Data = result };
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

        public ActionResult ViewBug(Guid id)
        {
            ViewBag.Bug = _bugManagement.GetBug(id);
            return View();
        }

        [HttpPost]
        public ActionResult ValidateBug()
        {
            return View();
        }
    }
}