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
using ERCOFAS.Enumeration;
using ERCOFAS.Helpers;
using static ERCOFAS.Models.ProjectEnum;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class RequestSealingAcceptanceController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.CaseType, "true", "", "", "")]
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            /*if (RoleHelpers.GetMainRole() == UserTypeEnum.Client.ToString())
            {
                string userId = User.Identity.GetUserId();

                int sealingAcceptance = db.SealingAndAcceptances.Count(x => x.Stakeholder == userId);
                if (sealingAcceptance == 0)
                    return Redirect("/RequestSealingAcceptance/NoFiledYet");
            }*/

            return View();
        }

        /*public ActionResult NoFiledYet()
        {
            return View();
        }

        public ActionResult GetPartialViewSealingAndAcceptance()
        {
            string userId = User.Identity.GetUserId();
            SealingAndAcceptanceListing theListing = new SealingAndAcceptanceListing();
            theListing.Listing = ReadSealingAndAcceptance(userId);
            return PartialView("~/Views/SealingAndAcceptance/_MainList.cshtml", theListing);
        }

        public List<SealingAndAcceptanceViewModel> ReadSealingAndAcceptance(string userId)
        {
            List<SealingAndAcceptanceViewModel> list = new List<SealingAndAcceptanceViewModel>();
            list = (from t1 in db.SealingAndAcceptances.AsNoTracking()
                    join t2 in db.AspNetUsers.AsNoTracking() on t1.CreatedBy equals t2.Id
                    join t3 in db.UserProfiles.AsNoTracking() on t1.Stakeholder equals t3.AspNetUserId
                    orderby t1.CreatedOn descending
                    select new SealingAndAcceptanceViewModel
                    {
                        Id = t1.Id,
                        RequestDescription = t1.RequestDescription,
                        Date = t1.Date,
                        Time = t1.Time,
                        Details = t1.Details,
                        Remarks = t1.Remarks,
                        NotifyBilling = t1.NotifyBilling,
                        ReviewStatus = t1.ReviewStatus,
                        PaymentStatus = t1.PaymentStatus,
                        AssignedNumber = t1.AssignedNumber,
                        AssignedPersonnel = t1.AssignedPersonnel,
                        AssignedPersonnelStatus = t1.AssignedPersonnelStatus,
                        Supervisor = t1.Supervisor,
                        SupervisorStatus = t1.SupervisorStatus,
                        DivisionChief = t1.DivisionChief,
                        DivisionChiefStatus = t1.DivisionChiefStatus,
                        Director = t1.Director,
                        DirectorAssignedNumber = t1.DirectorAssignedNumber,
                        OED = t1.OED,
                        TravelAuthorityStatus = t1.TravelAuthorityStatus,
                    }).OrderByDescending(x => x.CreatedOn).ToList();

            var userRole = RoleHelpers.GetMainRole();
            if (userRole != null && userRole == UserTypeEnum.Client.ToString())
                list = list.Where(x => x.Stakeholder == userId).ToList();

            return list;
        }

        public List<SealingAndAcceptanceAttachment> ReadSealingAndAcceptanceAttachment(string sealingAcceptanceId)
        {
            List<SealingAndAcceptanceAttachment> list = new List<SealingAndAcceptanceAttachment>();
            list = db.SealingAndAcceptanceAttachments.Where(x => x.SealingAndAcceptanceId == sealingAcceptanceId).ToList();
            return list;
        }


        public SealingAndAcceptanceViewModel GetViewModel(string Id, string type)
        {
            SealingAndAcceptanceViewModel model = new SealingAndAcceptanceViewModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                SealingAndAcceptance sealingAcceptance = db.SealingAndAcceptances.Where(a => a.Id == Id).FirstOrDefault();
                AspNetUserRoles role = db.AspNetUserRoles.Where(a => a.UserId == sealingAcceptance.Stakeholder).FirstOrDefault();
                model.Id = sealingAcceptance.Id;
                model.RequestDescription = sealingAcceptance.RequestDescription;
                model.Stakeholder = db.UserProfiles.FirstOrDefault(x => x.AspNetUserId == sealingAcceptance.Stakeholder).FullName;
                model.Remarks = sealingAcceptance.Remarks;
                model.Date = sealingAcceptance.Date;
                model.Time = sealingAcceptance.Time;
                model.ReviewStatus = sealingAcceptance.ReviewStatus;
                model.NotifyBilling = sealingAcceptance.NotifyBilling;
                model.PaymentStatus = sealingAcceptance.PaymentStatus;
                model.AssignedNumber = sealingAcceptance.AssignedNumber;
                model.AssignedPersonnel = sealingAcceptance.AssignedPersonnel;
                model.Supervisor = sealingAcceptance.Supervisor;
                model.SupervisorStatus = sealingAcceptance.SupervisorStatus;
                model.DivisionChief = sealingAcceptance.DivisionChief;
                model.DivisionChiefStatus = sealingAcceptance.DivisionChiefStatus;
                model.Director = sealingAcceptance.Director;
                model.DirectorAssignedNumber = sealingAcceptance.DirectorAssignedNumber;
                model.OED = sealingAcceptance.OED;
                model.TravelAuthorityStatus = sealingAcceptance.TravelAuthorityStatus;
                model.PaymentStatus = sealingAcceptance.PaymentStatus;
                model.CreatedOn = sealingAcceptance.CreatedOn;
                model.ModifiedOn = sealingAcceptance.ModifiedOn;
            }
            return model;
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "true", "true", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(string Id)
        {
            if (Id != null)
            {
                var attachment = db.SealingAndAcceptanceAttachments.FirstOrDefault(x => x.Id == Id);
                db.SealingAndAcceptanceAttachments.Remove(attachment);
                db.SaveChanges();

                if (System.IO.File.Exists(attachment.FileUrl))
                    System.IO.File.Delete(attachment.FileUrl);

                TempData["NotifySuccess"] = "File was deleted successfully.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "true", "true", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Edit(string Id)
        {
            SealingAndAcceptanceViewModel model = new SealingAndAcceptanceViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            model.Attachments = ReadSealingAndAcceptanceAttachment(Id);

            return View(model);
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "true", "", "", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ViewRecord(string Id)
        {
            SealingAndAcceptanceViewModel model = new SealingAndAcceptanceViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            model.Attachments = ReadSealingAndAcceptanceAttachment(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SealingAndAcceptanceViewModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //SaveRecord(model);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        public void ValidateModel(SealingAndAcceptanceViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.SealingAndAcceptances.Where(a => a.RequestDescription == model.RequestDescription && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.SealingAndAcceptances.Where(a => a.RequestDescription == model.RequestDescription).Select(a => a.Id).Any();
                }   

                if (duplicated == true)
                {
                    ModelState.AddModelError("RequestSubject", Resource.RequestSubjectAlreadyExist);
                }
            }
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
        }*/
    }
}