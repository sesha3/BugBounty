namespace BugBounty.Controllers
{
    using Bug.Bounty.Base;
    using Bug.Bounty.DataClasses;
    using Newtonsoft.Json;
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
            var userId = Guid.Parse("f1b74874-5fff-4af5-a718-da8d7f35c49c");
            ViewBag.BugsList = _bugManagement.GetBugs(_bugManagement.GetUserDetails(userId).Platform, userId);
            return View();
        }

        public ActionResult CreateBug()
        {
            string[] names = Enum.GetNames(typeof(Platform));
            ViewBag.Platforms = names;
            return View();
        }

        public void Upload()
        {
            new FileUpload().ProcessRequest(System.Web.HttpContext.Current);
        }

        [HttpPost]
        public JsonResult PostBug(string bugData)
        {
            var bug = JsonConvert.DeserializeObject<Bug>(bugData);
            bug.CreatedUserID = Guid.Parse("f1b74874-5fff-4af5-a718-da8d7f35c49c");
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