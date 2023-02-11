using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ExcelDataReader.Log;
using Microsoft.AspNet.Identity;
using ERCOFAS.Helpers;
using ERCOFAS.Models;
using ERCOFAS.Resources;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class PreRegistrationController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "true", "", "", "")]
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult GetPartialViewPreRegistrations()
        {
            ///string userId = User.Identity.GetUserId();
            PreRegistrationListing theListing = new PreRegistrationListing();
            theListing.Listing = ReadPreRegistrations();
            return PartialView("~/Views/PreRegistration/_MainList.cshtml", theListing);
        }

        public List<PreRegistrationViewModel> ReadPreRegistrations()
        {
            List<PreRegistrationViewModel> list = new List<PreRegistrationViewModel>();
            list = (from t1 in db.PreRegistration
                    join t2 in db.GlobalOptionSets.Where(x => x.Type == "RERType") on t1.RERTypeId equals t2.Id
                    join t3 in db.GlobalOptionSets.Where(x => x.Type == "RegistrationStatus") on t1.RegistrationStatusId equals t3.Id
                    where t1.RegistrationStatusId != ""
                    orderby t1.CreatedOn descending
                    select new PreRegistrationViewModel()
                    {
                        Id = t1.Id,
                        LastName = t1.LastName,
                        FirstName = t1.FirstName,
                        RERTypeId = t2.DisplayName,
                        RERClassificationId = t1.RERClassificationId,
                        TempUsername = t1.TempUsername,
                        TempPassword = t1.TempPassword,
                        RegistrationStatusId = t3.DisplayName,
                        CreatedOn = t1.CreatedOn
                    }).ToList();
            return list;
        }

        public List<PreRegistrationAttachment> ReadPreRegistrationAttachments(string preRegistrationId)
        {
            List<PreRegistrationAttachment> list = new List<PreRegistrationAttachment>();
            list = db.PreRegistrationAttachments.Where(x => x.PreRegistrationId == preRegistrationId).ToList();
            return list;
        }

        public PreRegistrationViewModel GetViewModel(string Id, string type)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = preRegistration.Id;
                model.LastName = preRegistration.LastName;
                model.FirstName = preRegistration.FirstName;
                model.RERTypeId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preRegistration.RERTypeId).DisplayName;

                var rerClassification = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preRegistration.RERClassificationId);
                if (rerClassification != null)
                    model.RERClassificationId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preRegistration.RERClassificationId).DisplayName;

                model.RegistrationStatusId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preRegistration.RegistrationStatusId).DisplayName;
                model.CreatedOn = preRegistration.CreatedOn;
            }
            return model;
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "", "true", "true", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(string Id)
        {
            if (Id != null)
            {
                var attachment = db.PreRegistrationAttachments.FirstOrDefault(x => x.Id == Id);
                db.PreRegistrationAttachments.Remove(attachment);
                db.SaveChanges();

                if (System.IO.File.Exists(attachment.FileUrl))
                    System.IO.File.Delete(attachment.FileUrl);

                TempData["NotifySuccess"] = "File was deleted successfully.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        ///[CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "", "true", "true", "")]
        public ActionResult Edit(string Id)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            model.RERTypeList = general.GetRERTypeList(model.RERTypeId);
            model.RERClassificationList = general.GetRERClassificationList(model.RERClassificationId);
            model.RegistrationStatusList = general.GetRegistrationStatusList(model.RegistrationStatusId);
            model.Attachments = ReadPreRegistrationAttachments(Id);
            return View(model);
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "true", "", "", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ViewRecord(string Id)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
                model.Attachments = ReadPreRegistrationAttachments(Id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PreRegistrationViewModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                model.RERTypeList = general.GetRERTypeList(model.RERTypeId);
                model.RERClassificationList = general.GetRERClassificationList(model.RERClassificationId);
                model.RegistrationStatusList = general.GetRegistrationStatusList(model.RegistrationStatusId);
                return View(model);
            }

            SaveRecord(model);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        public void ValidateModel(PreRegistrationViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.PreRegistration.Where(a => a.FirstName == model.FirstName && a.LastName == model.LastName && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.PreRegistration.Where(a => a.FirstName == model.FirstName && a.LastName == model.LastName).Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("FirstName", Resource.RequestSubjectAlreadyExist);
                }
            }
        }

        public void SaveRecord(PreRegistrationViewModel model)
        {
            /// string userId = User.Identity.GetUserId();

            if (model != null)
            {
                //edit
                if (model.Id != null)
                {
                    PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == model.Id).FirstOrDefault();
                    preRegistration.LastName = model.LastName;
                    preRegistration.FirstName = model.FirstName;
                    preRegistration.RERTypeId = model.RERTypeId;
                    preRegistration.RERClassificationId = model.RERClassificationId;
                    preRegistration.RegistrationStatusId = model.RegistrationStatusId;
                    db.Entry(preRegistration).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //new record
                else
                {
                    PreRegistration preRegistration = new PreRegistration();
                    preRegistration.Id = Guid.NewGuid().ToString();
                    preRegistration.LastName = model.LastName;
                    preRegistration.FirstName = model.FirstName;
                    preRegistration.RERTypeId = model.RERTypeId;
                    preRegistration.RERClassificationId = model.RERClassificationId;
                    preRegistration.RegistrationStatusId = model.RegistrationStatusId;
                    preRegistration.CreatedOn = DateTime.Now;
                    db.PreRegistration.Add(preRegistration);
                    db.SaveChanges();

                    model.Id = preRegistration.Id;
                }

                general.SavePreRegistrationAttachment(model.Documents, model.Id);
            }
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "", "", "", "true")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                    if (preRegistration != null)
                    {
                        db.PreRegistration.Remove(preRegistration);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                if (preRegistration == null)
                {
                    TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction("index");
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



        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "", "", "", "true")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Approve(string Id)
        {
            try
            {
                if (Id != null)
                {
                    PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                    if (preRegistration != null)
                    {
                        preRegistration.RegistrationStatusId = "84D92AC9-E410-4213-8B52-5900012829BC";
                        db.Entry(preRegistration).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    var notification = db.Notifications.FirstOrDefault(x => x.NotificationTypeId == "53BFEEG6-05Y1-428N-8Z0B-D5V64GF1B73E");
                    if (notification != null)
                    {
                        string content = notification.Content.Replace("{firstname}", preRegistration.FirstName).Replace("{username}", preRegistration.TempUsername)
                            .Replace("{password}", preRegistration.TempPassword);
                        //EmailHelpers.SendEmail(preRegistration.EmailAddress, "Registration Approve", content, "C:/Projects/ERCOFAS/ApplicationFiles/Certificate of Registration (COR).pdf");
                    }
                }
                TempData["NotifySuccess"] = Resource.RegistrationApprovedSuccessfully;
            }
            catch (Exception)
            {
                PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                if (preRegistration == null)
                {
                    TempData["NotifySuccess"] = Resource.RegistrationApprovedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction("index");
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreRegistration, "", "", "", "true")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Reject(string Id)
        {
            try
            {
                if (Id != null)
                {
                    PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                    if (preRegistration != null)
                    {
                        preRegistration.RegistrationStatusId = "C6EBCA7F-99E9-49C4-BA0E-CF7D5E913E75";
                        db.Entry(preRegistration).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RegistrationRejectedSuccessfully;
            }
            catch (Exception)
            {
                PreRegistration preRegistration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                if (preRegistration == null)
                {
                    TempData["NotifySuccess"] = Resource.RegistrationRejectedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction("index");
        }
    }
}