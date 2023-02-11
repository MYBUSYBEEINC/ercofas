using ERCOFAS.Models;
using System.Linq;
using System.Web.Mvc;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class MenuSelectionController : Controller
    {
        private readonly DefaultDBContext _db = new DefaultDBContext();

        // GET: MenuSelection/CasePleading
        public ActionResult CasePleading()
        {
            TotalCountViewModel model = new TotalCountViewModel()
            {
                TotalPreFilings = _db.PreFiledCases.Count(),
                TotalInitiatorySubsequentPleadings = _db.InitiatoryPleadings.Count(),
                TotalVirtualHearings = _db.Hearings.Count()
            };
            return View(model);
        }

        // GET: MenuSelection/Security
        public ActionResult Security()
        {
            TotalCountViewModel model = new TotalCountViewModel()
            {
                TotalRoles = _db.AspNetRoles.Count(),
                TotalUsers = _db.UserProfiles.Count(),
                TotalAuditLogs = _db.AuditLogs.Count(),
                TotalUserStatus = _db.GlobalOptionSets.Count(x => x.Type == "UserStatus"),
                TotalUserTypes = _db.AspNetUserTypes.Count()
            };
            return View(model);
        }

        // GET: MenuSelection/GeneralSettings
        public ActionResult GeneralSettings()
        {
            TotalCountViewModel model = new TotalCountViewModel()
            {
                TotalNotificationTypes = _db.GlobalOptionSets.Count(x => x.Type == "NotificationType"),
                TotalNotificationTemplates = _db.Notifications.Count(),
                TotalCaseTypes = _db.GlobalOptionSets.Count(x => x.Type == "CaseType"),
                TotalCaseNatures = _db.GlobalOptionSets.Count(x => x.Type == "CaseNature"),
                TotalRERClassification = _db.GlobalOptionSets.Count(x => x.Type == "RERClassification")
            };
            return View(model);
        }
    }
}