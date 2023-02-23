using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ERCOFAS.Models;
using ERCOFAS.Resources;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class RequestForSealingController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.CaseType, "true", "", "", "")]
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
                if (general != null)
                {
                    general.Dispose();
                    general = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}