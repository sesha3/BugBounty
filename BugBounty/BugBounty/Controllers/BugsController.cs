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
            var user = _bugManagement.GetUser(HttpContext.User.Identity.Name);
            ViewBag.BugsList = _bugManagement.GetBugs(user.Platform, user.Id);
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
            var user = _bugManagement.GetUser(HttpContext.User.Identity.Name);
            bug.CreatedUserID = user.Id;
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

        public ActionResult Rewards()
        {
            var user = _bugManagement.GetUser(HttpContext.User.Identity.Name);
            ViewBag.BugsList = _bugManagement.GetBugs(user.Platform, user.Id);
            return View();
        }
    }
}