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
    public class ValidationMeterSamplingController : Controller
    {
        private DefaultDBContext _db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        // GET: ValidationMeterSampling
        public ActionResult Index()
        {
            return View();
        }

        /// POST: /ValidationMeterSampling/SubmitProgressReport
        /// <summary>
        /// Submits a progress report with attachment.
        /// <param name="model">The progress report view model.</param>
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitProgressReport(ProgressReportViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ProgressReport report = new ProgressReport
            {
                Description = model.Description,
                SubmittedBy = User.Identity.GetUserId(),
                DateSubmitted = DateTime.Now
            };
            _db.ProgressReports.Add(report);
            _db.SaveChanges();

            TempData["NotifySuccess"] = "Progress report has been successfully submitted";

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
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