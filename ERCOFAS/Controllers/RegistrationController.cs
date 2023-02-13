using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ERCOFAS.Helpers;
using ERCOFAS.Models;
using ERCOFAS.Resources;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Registration, "true", "", "", "")]
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult GetPartialViewRegistrations()
        {
            PreRegistrationListing theListing = new PreRegistrationListing();
            theListing.Listing = ReadPreRegistrations();
            return PartialView("~/Views/Registration/_MainList.cshtml", theListing);
        }

        public List<PreRegistrationViewModel> ReadPreRegistrations()
        {
            List<PreRegistrationViewModel> list = new List<PreRegistrationViewModel>();
            list = (from t1 in db.PreRegistration
                    join t2 in db.GlobalOptionSets.Where(x => x.Type == "RERType") on t1.RERTypeId equals t2.Id
                    join t3 in db.GlobalOptionSets.Where(x => x.Type == "RegistrationStatus") on t1.RegistrationStatusId equals t3.Id into t3X
                    from t3 in t3X.DefaultIfEmpty()
                    join t4 in db.GlobalOptionSets.Where(x => x.Type == "RERClassification") on t1.RERClassificationId equals t4.Id into t4X
                    from t4 in t4X.DefaultIfEmpty()
                    where t1.RegistrationStatusId != ""
                    orderby t1.CreatedOn descending
                    select new PreRegistrationViewModel()
                    {
                        Id = t1.Id,
                        ERNumber = t1.RegistrationStatusId == null ? "N/A" : t1.ERNumber,
                        Name = t1.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? t1.FirstName + " " + t1.LastName : t1.JuridicalEntityName,
                        RERTypeId = t2.DisplayName,
                        RERClassificationId = t1.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? "N/A" : t4.DisplayName,
                        TempUsername = t1.TempUsername,
                        TempPassword = t1.TempPassword,
                        RegistrationStatusId = t1.RegistrationStatusId == null ? "-No Review Yet-" : t4.DisplayName,
                        CreatedOn = t1.CreatedOn,
                        ApprovedDate = t1.ApprovedDate,
                    }).ToList();
            return list;
        }

        public PreRegistrationViewModel GetViewModel(long Id, string type)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                PreRegistration registration = db.PreRegistration.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = registration.Id;
                model.Name = registration.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? registration.FirstName + " " + registration.LastName : registration.JuridicalEntityName;
                model.CreatedOn = registration.CreatedOn;
                model.Attachments = db.PreRegistrationAttachments.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.Emails = db.PreRegistrationEmails.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.MobileNumbers = db.PreRegistrationMobiles.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.RegistrationStatusId = registration.RegistrationStatusId;

                Session["RegistrationID"] = model.Id;
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.CaseType, "", "true", "true", "")]
        public ActionResult Edit(long id)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            model = GetViewModel(id, "Edit");
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.CaseType, "true", "", "", "")]
        public ActionResult ViewRecord(long id)
        {
            PreRegistrationViewModel model = new PreRegistrationViewModel();
            model = GetViewModel(id, "View");
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GlobalOptionSetViewModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SaveRecord(model);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        public void ValidateModel(GlobalOptionSetViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.GlobalOptionSets.Where(a => a.DisplayName == model.DisplayName && a.Type == "CaseTypes" && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.GlobalOptionSets.Where(a => a.DisplayName == model.DisplayName && a.Type == "CaseTypes").Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("DisplayName", Resource.UserTypeNameAlreadyExist);
                }
            }
        }

        public void SaveRecord(GlobalOptionSetViewModel model)
        {
            if (model != null)
            {
                Regex sWhitespace = new Regex(@"\s+");
                //edit
                if (model.Id != null)
                {
                    GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == model.Id).FirstOrDefault();
                    globalOptionSet.Code = sWhitespace.Replace(model.DisplayName, "");
                    globalOptionSet.DisplayName = model.DisplayName;
                    globalOptionSet.OptionOrder = model.OptionOrder;
                    globalOptionSet.Type = "CaseType";
                    globalOptionSet.ReferenceId = model.ReferenceId;
                    globalOptionSet.Status = "Active";
                    globalOptionSet.ModifiedBy = User.Identity.GetUserId();
                    globalOptionSet.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                    globalOptionSet.IsoUtcModifiedOn = general.GetIsoUtcNow();
                    db.Entry(globalOptionSet).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //new record
                else
                {
                    GlobalOptionSet globalOptionSet = new GlobalOptionSet();
                    globalOptionSet.Id = Guid.NewGuid().ToString();
                    globalOptionSet.Code = sWhitespace.Replace(model.DisplayName, "");
                    globalOptionSet.DisplayName = model.DisplayName;
                    globalOptionSet.OptionOrder = model.OptionOrder;
                    globalOptionSet.Type = "CaseType";
                    globalOptionSet.ReferenceId = model.ReferenceId;
                    globalOptionSet.Status = "Active";
                    globalOptionSet.CreatedBy = User.Identity.GetUserId();
                    globalOptionSet.CreatedOn = general.GetSystemTimeZoneDateTimeNow();
                    globalOptionSet.IsoUtcCreatedOn = general.GetIsoUtcNow();
                    globalOptionSet.SystemDefault = false;
                    db.GlobalOptionSets.Add(globalOptionSet);
                    db.SaveChanges();
                }
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.CaseType, "", "", "", "true")]
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == Id).FirstOrDefault();
                    if (globalOptionSet != null)
                    {
                        db.GlobalOptionSets.Remove(globalOptionSet);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == Id).FirstOrDefault();
                if (globalOptionSet == null)
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

        // POST: /Registration/ApplicationReview
        /// <summary>
        /// Sends the application review result.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationReview(PreRegistrationViewModel model)
        {
            long registrationId = (long)Session["RegistrationID"];
            var registration = db.PreRegistration.FirstOrDefault(x => x.Id == registrationId);
            var settings = db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");
            string notificationTypeId = !model.IsCompleted ? "D2E7A300-2BF9-4C78-A2CD-4D3201BC48D2" : "D1567F9A-A379-4603-ADAB-66B978A34ADD";
            string documents = string.Empty;

            if (!model.IsCompleted)
            {
                foreach (var item in db.PreRegistrationAttachments.Where(x => x.PreRegistrationId == registrationId).ToList())
                {
                    string status = item.IsApproved.HasValue && item.IsApproved.Value ? "Approved" : "Declined";

                    if (status == "Declined")
                        documents += string.Format("<tr style=\"border: 1px solid #ddd; color:#ff0000;\"><td style=\"border-right: 1px solid #ddd;\">{0}</td><td>{1}</td></tr>", item.DocumentName, status);
                    else
                        documents += string.Format("<tr style=\"border: 1px solid #ddd;\"><td style=\"border-right: 1px solid #ddd;\">{0}</td><td>{1}</td></tr>", item.DocumentName, status);
                }
            }
            
            var emailNotification = db.Notifications.FirstOrDefault(x => x.NotificationTypeId == notificationTypeId);
            if (emailNotification != null)
            {
                string subject = emailNotification.Subject.Replace("{fullname}", string.Format("{0} {1}", registration.FirstName, registration.LastName));
                string username = string.Format("{0}{1}", registration.FirstName.Replace(" ", string.Empty), registration.LastName.Replace(" ", string.Empty));
                string generatedPassword = PasswordHelpers.GeneratePassword();

                emailNotification.Content = emailNotification.Content.Replace("{firstname}", registration.FirstName);
                emailNotification.Content = emailNotification.Content.Replace("{baseurl}", settings.BaseUrl);

                foreach (var item in db.PreRegistrationEmails.Where(x => x.PreRegistrationId == registrationId).ToList())
                {
                    if (!model.IsCompleted)
                    {
                        emailNotification.Content = emailNotification.Content.Replace("{documents}", documents);
                        emailNotification.Content = emailNotification.Content.Replace("{remarks}", string.Format("Reviewer Remarks: {0}", model.Remarks));
                        emailNotification.Content = emailNotification.Content.Replace("{url}", settings.BaseUrl);
                        EmailHelpers.SendEmail(item.EmailAddress, subject, emailNotification.Content);
                    } 
                    else
                    {
                        string attachmentPath = "D:/OFAS/ApplicationFiles/Certificate of Registration (COR).pdf";
                    
                        emailNotification.Content = emailNotification.Content.Replace("{username}", username);
                        emailNotification.Content = emailNotification.Content.Replace("{password}", generatedPassword);
                        emailNotification.Content = emailNotification.Content.Replace("{url}", settings.BaseUrl);
                        EmailHelpers.SendEmail(item.EmailAddress, subject, emailNotification.Content, attachmentPath);
                    }
                }

                if (model.IsCompleted)
                {
                    registration.RegistrationStatusId = "84D92AC9-E410-4213-8B52-5900012829BC";
                    db.SaveChanges();

                    CreateUserAccount(registration, username, generatedPassword);
                }
                else
                {
                    registration.RegistrationStatusId = "200257E4-E6C8-4159-8A6F-4475E0A95B32";
                    db.SaveChanges();
                }
                TempData["NotifySuccess"] = "You have submitted a review for this application.";
            }
            return Redirect(string.Format("/Registration/Edit/{0}", registrationId));
        }

        public ActionResult ApproveDocument(int registrationId, string documentId)
        {
            var document = db.PreRegistrationAttachments.FirstOrDefault(x => x.PreRegistrationId == registrationId && x.Id == documentId);
            if (document != null)
            {
                document.IsApproved = true;
                db.SaveChanges();

                TempData["NotifySuccess"] = string.Format("{0} was approved.", document.DocumentName);
            }
            return Redirect(string.Format("/Registration/Edit/{0}", registrationId));
        }

        public ActionResult DeclineDocument(int registrationId, string documentId)
        {
            var document = db.PreRegistrationAttachments.FirstOrDefault(x => x.PreRegistrationId == registrationId && x.Id == documentId);
            if (document != null)
            {
                document.IsApproved = false;
                db.SaveChanges();

                TempData["NotifySuccess"] = string.Format("{0} was declined.", document.DocumentName);
            }
            return Redirect(string.Format("/Registration/Edit/{0}", registrationId));
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

        #region Private 

        private void CreateUserAccount(PreRegistration registration, string username, string generatedPassword)
        {
            var emailAddress = db.PreRegistrationEmails.FirstOrDefault(x => x.PreRegistrationId == registration.Id).EmailAddress;
            var mobileNumber = db.PreRegistrationMobiles.FirstOrDefault(x => x.PreRegistrationId == registration.Id).MobileNumber;

            var preRegistration = db.PreRegistration.FirstOrDefault(x => x.Id == registration.Id);
            preRegistration.ERNumber = CodeGenerator.GenerateERNumber();
            preRegistration.ApprovedDate = DateTime.Now;
            db.SaveChanges();

            AspNetUsers aspNetUser = new AspNetUsers
            {
                Id = Guid.NewGuid().ToString(),
                UserName = username.ToLower(),
                Email = emailAddress,
                EmailConfirmed = false,
                PasswordHash = PasswordHelpers.HashPassword(generatedPassword),
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = mobileNumber,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                RegistrationId = registration.Id
            };
            db.AspNetUsers.Add(aspNetUser);
            db.SaveChanges();

            UserProfile profile = new UserProfile
            {
                Id = Guid.NewGuid().ToString(),
                AspNetUserId = aspNetUser.Id,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                FullName = string.Format("{0} {1}", registration.FirstName, registration.LastName),
                PhoneNumber = aspNetUser.PhoneNumber,
                UserStatusId = "6A1672F3-4C0F-41F4-8D38-B25C97C0BCB2",
                IsoUtcCreatedOn = DateTime.Now.ToString()
            };
            db.UserProfiles.Add(profile);
            db.SaveChanges();

            AspNetUserTypes aspUserType = new AspNetUserTypes
            {
                UserId = aspNetUser.Id,
                UserTypeId = "43B863FB-3E6A-4490-A28F-11E21570436C"
            };
            db.AspNetUserTypes.Add(aspUserType);
            db.SaveChanges();

            AspNetUserRoles aspUserRole = new AspNetUserRoles
            {
                UserId = aspNetUser.Id,
                RoleId = "0aa343bb-b70d-497e-9330-d5b5973774ab"
            };
            db.AspNetUserRoles.Add(aspUserRole);
            db.SaveChanges();
        }

        #endregion Private
    }
}