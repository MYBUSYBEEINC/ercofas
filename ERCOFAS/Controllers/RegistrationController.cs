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
using System.Configuration;

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
                    join t2 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "RERType") on t1.RERTypeId equals t2.Id
                    join t3 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "RegistrationStatus") on t1.RegistrationStatusId equals t3.Id into t3X
                    from t3 in t3X.DefaultIfEmpty()
                    join t4 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "RERClassification") on t1.RERClassificationId equals t4.Id into t4X
                    from t4 in t4X.DefaultIfEmpty()
                    where t1.RegistrationStatusId != ""
                    orderby t1.CreatedOn descending
                    select new PreRegistrationViewModel()
                    {
                        Id = t1.Id,
                        ERNumber = t1.RegistrationStatusId == null ? "N/A" : t1.ERNumber,
                        Name = t1.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? t1.FirstName + " " + t1.MiddleName + " " + t1.LastName : t1.JuridicalEntityName,
                        RERTypeId = t2.DisplayName,
                        RERClassificationId = t1.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? "N/A" : t4.DisplayName,
                        TempUsername = t1.TempUsername,
                        TempPassword = t1.TempPassword,
                        RegistrationStatusId = t1.RegistrationStatusId == null ? "-No Review Yet-" : t3.DisplayName,
                        CreatedOn = t1.CreatedOn,
                        ApprovedDate = t1.ApprovedDate,
                        DeclinedDate = t1.DeclinedDate
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
                model.Name = registration.RERTypeId == "CA4ECCA6-63E0-4F84-92CC-301323C1D4F9" ? registration.FirstName + " " + registration.MiddleName + " " + registration.LastName : registration.JuridicalEntityName;
                model.CreatedOn = registration.CreatedOn;
                model.Attachments = db.PreRegistrationAttachments.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.Emails = db.PreRegistrationEmails.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.MobileNumbers = db.PreRegistrationMobiles.Where(x => x.PreRegistrationId == registration.Id).ToList();
                model.RegistrationStatusId = registration.RegistrationStatusId;
                model.ApprovedDate = registration.ApprovedDate;
                model.DeclinedDate = registration.DeclinedDate;

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
        /// Sends review feedback on the application if pass or fail.
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
            string entityName = registration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? registration.JuridicalEntityName : registration.FirstName;
            string username = registration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? registration.JuridicalEntityName.Replace(" ", string.Empty) :
                   string.Format("{0} {1}", registration.FirstName.Replace(" ", string.Empty), registration.LastName.Replace(" ", string.Empty));
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
                string generatedPassword = PasswordHelpers.GeneratePassword();

                if (model.IsCompleted)
                {
                    registration.RegistrationStatusId = "84D92AC9-E410-4213-8B52-5900012829BC";
                    registration.ERNumber = CodeGenerator.GenerateERNumber();
                    registration.CORFilePath = GenerateCORFile(registration);
                    db.SaveChanges();

                    CreateUserAccount(registration, username, generatedPassword);
                }
                else
                {
                    registration.RegistrationStatusId = "200257E4-E6C8-4159-8A6F-4475E0A95B32";
                    registration.TempKey = Guid.NewGuid().ToString();
                    registration.DeclinedDate = DateTime.Now;
                    db.SaveChanges();
                }

                emailNotification.Content = emailNotification.Content.Replace("{firstname}", entityName);
                emailNotification.Content = emailNotification.Content.Replace("{baseurl}", settings.BaseUrl);

                foreach (var item in db.PreRegistrationEmails.Where(x => x.PreRegistrationId == registrationId).ToList())
                {
                    if (!model.IsCompleted)
                    {
                        string reUploadDocumentLink = string.Format("{0}/Account/ReUploadDocument?id={1}&key={2}", settings.BaseUrl, registration.Id, registration.TempKey);
                        emailNotification.Content = emailNotification.Content.Replace("{documents}", documents);
                        emailNotification.Content = emailNotification.Content.Replace("{reupload-document-link}", reUploadDocumentLink);
                        emailNotification.Content = emailNotification.Content.Replace("{remarks}", !string.IsNullOrEmpty(model.Remarks) ? 
                            string.Format("Reviewer Remarks: {0}", model.Remarks): string.Empty);
                        EmailHelpers.SendEmail(item.EmailAddress, subject, emailNotification.Content);
                    } 
                    else
                    {            
                        emailNotification.Content = emailNotification.Content.Replace("{username}", username);
                        emailNotification.Content = emailNotification.Content.Replace("{password}", generatedPassword);
                        EmailHelpers.SendEmail(item.EmailAddress, subject, emailNotification.Content, registration.CORFilePath);
                    }
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

        #region Methods

        /// <summary>
        /// Pre-validates the form if there is an unverified email or mobile number. 
        /// </summary>
        public JsonResult PreValidator(long id)
        {
            bool isValid = true;
            int totalErrorCount = 0;
            string validationMessage = string.Empty;
            string unverifiedData = string.Empty;

            int declinedDocument = db.PreRegistrationAttachments.Count(x => x.PreRegistrationId == id && x.IsApproved == false);
            if (declinedDocument > 0)
            {
                unverifiedData += string.Format(" {0} declined document", declinedDocument);
                totalErrorCount += 1;
                isValid = false;
            }

            int unverifiedEmail = db.PreRegistrationEmails.Count(x => x.PreRegistrationId == id && x.IsVerified == false);
            if (unverifiedEmail > 0)
            {
                unverifiedData += string.Format("{0} {1} unverified email", declinedDocument > 0 ? "," : string.Empty, unverifiedEmail);
                totalErrorCount += 1;
                isValid = false;
            }

            int unverifiedMobile = db.PreRegistrationMobiles.Count(x => x.PreRegistrationId == id && x.IsVerified == false);
            if (unverifiedMobile > 0)
            {
                unverifiedData += string.Format("{0} {1} unverified phone", declinedDocument > 0 || unverifiedEmail > 0 ? "," : string.Empty, unverifiedMobile);
                totalErrorCount += 1;
                isValid = false;
            }

            if (!isValid)
                unverifiedData = string.Format("There {0} {1} on this registration. Do you want to approve?", totalErrorCount > 1 ? "are" : "is", unverifiedData);
            else
                unverifiedData = string.Empty;

            validationMessage = unverifiedData;

            return Json(new { isValid, validationMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generates certificate of registration.
        /// <param name="registration">The registration.</param>
        /// </summary>
        /// <returns></returns>
        public string GenerateCORFile(PreRegistration registration)
        {
            string templateBasePath = ConfigurationManager.AppSettings["CORTemplateBasePath"].ToString();
            string templateOutputPath = ConfigurationManager.AppSettings["CORTemplateOutputBasePath"].ToString();
            string fileName = string.Format("Certificate of Registration_{0}_{1}_{2}.pdf", DateTime.Now.ToString("MMddyyyy"), 
                registration.ERNumber, Guid.NewGuid().ToString().Split('-')[0]);

            if (System.IO.File.Exists(templateBasePath))
            {
                var htmlContent = string.Format(System.IO.File.ReadAllText(templateBasePath), DateTime.Now);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                string entityName = registration.RERTypeId == "AE2DCD91-DACC-4C75-A2A3-51644ABE69BB" ? registration.JuridicalEntityName :
                    string.Format("{0} {1}", registration.FirstName, registration.LastName);
                string emailAddresses = string.Empty;
                string mobileNumbers = string.Empty;

                foreach (var item in db.PreRegistrationEmails.Where(x => x.PreRegistrationId == registration.Id).ToList())
                    emailAddresses += string.Format("<div style=\"border-top: 2px solid black; padding: 8px;\">{0}</div>", item.EmailAddress);

                foreach (var item in db.PreRegistrationMobiles.Where(x => x.PreRegistrationId == registration.Id).ToList())
                    mobileNumbers += string.Format("<div style=\"border-top:2px solid black; padding: 8px;\">{0}</div>", item.MobileNumber);

                htmlContent = htmlContent.Replace("ernumber", registration.ERNumber);
                htmlContent = htmlContent.Replace("companyname", entityName);
                htmlContent = htmlContent.Replace("emailaddress", emailAddresses);
                htmlContent = htmlContent.Replace("mobilenumbers", mobileNumbers);
                htmlContent = htmlContent.Replace("dedicatedliaison1", registration.LiaisonOfficer1);
                htmlContent = htmlContent.Replace("dedicatedliaison2", registration.LiaisonOfficer2);
                htmlContent = htmlContent.Replace("issuedon", DateTime.Now.ToString("MM/dd/yyyy"));
                htmlToPdf.GeneratePdf(htmlContent, string.Empty, string.Format("{0}/{1}", templateOutputPath, fileName));

                return string.Format(@"{0}\{1}", templateOutputPath, fileName);
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates new user account.
        /// <param name="registration">The registration.</param>
        /// </summary>
        /// <returns></returns>
        private void CreateUserAccount(PreRegistration registration, string username, string generatedPassword)
        {
            var emailAddress = db.PreRegistrationEmails.FirstOrDefault(x => x.PreRegistrationId == registration.Id).EmailAddress;
            var mobileNumber = db.PreRegistrationMobiles.FirstOrDefault(x => x.PreRegistrationId == registration.Id).MobileNumber;
            var preRegistration = db.PreRegistration.FirstOrDefault(x => x.Id == registration.Id);
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

        #endregion Methods
    }
}