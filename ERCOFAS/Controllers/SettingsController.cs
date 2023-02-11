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
    public class SettingsController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        public ViewResult Index()
        {
            return View();
        }

        public ActionResult GetPartialViewSettings()
        {
            SettingsListing settingsListing = new SettingsListing();
            settingsListing.Listing = ReadSettings();
            return PartialView("~/Views/Settings/_MainList.cshtml", settingsListing);
        }

        public List<SettingsModel> ReadSettings()
        {
            List<SettingsModel> result = new List<SettingsModel>();
            result = (from t1 in db.Settings
                      where t1.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB"
                      select new SettingsModel
                      {
                          Id = t1.Id,
                          CompanyName = t1.CompanyName,
                          EmailAddress = t1.EmailAddress,
                          PhoneNumber = t1.PhoneNumber,
                          MobileNumber = t1.MobileNumber,
                          Address = t1.Address,
                          AboutUs = t1.AboutUs,
                          Website = t1.Website,
                          ERNumberPrefix = t1.ERNumberPrefix,
                          SMTPFromEmail = t1.SMTPFromEmail,
                          SMTPPassword = t1.SMTPPassword,
                          SMTPServerName = t1.SMTPServerName,
                          SMTPPort = t1.SMTPPort,
                          SMTPEnableSSL = t1.SMTPEnableSSL,
                          OTPBaseUrl = t1.OTPBaseUrl,
                          OTPSenderId = t1.OTPSenderId,
                          OTPAPIKey = t1.OTPAPIKey,
                          OTPClientId = t1.OTPClientId,
                          OTPUsername = t1.OTPUsername,
                          OTPPassword = t1.OTPPassword,
                          MinPasswordLength = t1.MinPasswordLength,
                          MinSpecialCharacters = t1.MinSpecialCharacters,
                          MaxSignOnAttempts = t1.MaxSignOnAttempts,
                          EnforcePasswordHistoryId = t1.EnforcePasswordHistoryId,
                          EnforcePasswordHistoryDescription = db.GlobalOptionSets.FirstOrDefault(x => x.Id == t1.EnforcePasswordHistoryId).DisplayName,
                          ActivationLinkExpiresIn = t1.ActivationLinkExpiresIn,
                          DeactivateUserAfter = (int)(t1.DeactivateUserAfter != null ? t1.DeactivateUserAfter : 0)
                      }).ToList();
            return result;
        }

        public SettingsModel GetViewModel(string Id, string type)
        {
            SettingsModel model = new SettingsModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                Settings setting = db.Settings.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = setting.Id;
                model.CompanyName = setting.CompanyName;
                model.EmailAddress = setting.EmailAddress;
                model.PhoneNumber = setting.PhoneNumber;
                model.MobileNumber = setting.MobileNumber;
                model.Address = setting.Address;
                model.AboutUs = setting.AboutUs;
                model.Website = setting.Website;
                model.ERNumberPrefix = setting.ERNumberPrefix;
                model.SMTPFromEmail = setting.SMTPFromEmail;
                model.SMTPPassword = setting.SMTPPassword;
                model.SMTPServerName = setting.SMTPServerName;
                model.SMTPPort = setting.SMTPPort;
                model.SMTPEnableSSL = setting.SMTPEnableSSL;
                model.OTPBaseUrl = setting.OTPBaseUrl;
                model.OTPSenderId = setting.OTPSenderId;
                model.OTPAPIKey = setting.OTPAPIKey;
                model.OTPClientId = setting.OTPClientId;
                model.OTPUsername = setting.OTPUsername;
                model.OTPPassword = setting.OTPPassword;
                model.MinPasswordLength = setting.MinPasswordLength;
                model.MinSpecialCharacters = setting.MinSpecialCharacters;
                model.MaxSignOnAttempts = setting.MaxSignOnAttempts;
                model.EnforcePasswordHistoryId = setting.EnforcePasswordHistoryId;
                model.ActivationLinkExpiresIn = setting.ActivationLinkExpiresIn;
                model.DeactivateUserAfter = (int)(setting.DeactivateUserAfter != null ? setting.DeactivateUserAfter : 0);
                if (type == "View")
                {
                    model.CreatedAndModified = general.GetCreatedAndModified(setting.CreatedBy, setting.CreatedOn.ToString(), setting.ModifiedBy, setting.ModifiedOn.ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Setting, "", "true", "true", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Edit(string Id)
        {
            SettingsModel model = new SettingsModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            model.EnforcePasswordHistorySelectList = general.GetGlobalOptionSets("EnforcePasswordHistory", model.EnforcePasswordHistoryId);
            return View(model);
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Setting, "true", "", "", "")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ViewRecord(string Id)
        {
            SettingsModel model = new SettingsModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            model.EnforcePasswordHistorySelectList = general.GetGlobalOptionSets("EnforcePasswordHistory", model.EnforcePasswordHistoryId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SettingsModel model)
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

        public void ValidateModel(SettingsModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.Settings.Where(a => a.CompanyName == model.CompanyName && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.Settings.Where(a => a.CompanyName == model.CompanyName).Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("CompanyName", Resource.CompanyNameAlreadyExist);
                }
            }
        }

        public void SaveRecord(SettingsModel model)
        {
            if (model != null)
            {
                Regex sWhitespace = new Regex(@"\s+");
                //edit
                if (model.Id != null)
                {
                    Settings setting = db.Settings.Where(a => a.Id == model.Id).FirstOrDefault();
                    setting.CompanyName = model.CompanyName;
                    setting.EmailAddress = model.EmailAddress;
                    setting.PhoneNumber = model.PhoneNumber;
                    setting.MobileNumber = model.MobileNumber;
                    setting.Address = model.Address;
                    setting.AboutUs = model.AboutUs;
                    setting.Website = model.Website;
                    setting.ERNumberPrefix = model.ERNumberPrefix;
                    setting.SMTPFromEmail = model.SMTPFromEmail;
                    setting.SMTPPassword = model.SMTPPassword;
                    setting.SMTPServerName = model.SMTPServerName;
                    setting.SMTPPort = model.SMTPPort;
                    setting.SMTPEnableSSL = model.SMTPEnableSSL;
                    setting.OTPBaseUrl = model.OTPBaseUrl;
                    setting.OTPSenderId = model.OTPSenderId;
                    setting.OTPAPIKey = model.OTPAPIKey;
                    setting.OTPClientId = model.OTPClientId;
                    setting.OTPUsername = model.OTPUsername;
                    setting.OTPPassword = model.OTPPassword;
                    setting.MinPasswordLength = model.MinPasswordLength;
                    setting.MinSpecialCharacters = model.MinSpecialCharacters;
                    setting.MaxSignOnAttempts = model.MaxSignOnAttempts;
                    setting.EnforcePasswordHistoryId = model.EnforcePasswordHistoryId;
                    setting.ActivationLinkExpiresIn = model.ActivationLinkExpiresIn;
                    setting.ModifiedBy = User.Identity.GetUserId();
                    setting.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                    setting.DeactivateUserAfter = model.DeactivateUserAfter;
                    db.SaveChanges();
                }
                //new record
                else
                {
                    Settings setting = new Settings();
                    setting.CompanyName = model.CompanyName;
                    setting.EmailAddress = model.EmailAddress;
                    setting.PhoneNumber = model.PhoneNumber;
                    setting.MobileNumber = model.MobileNumber;
                    setting.Address = model.Address;
                    setting.AboutUs = model.AboutUs;
                    setting.Website = model.Website;
                    setting.ERNumberPrefix = model.ERNumberPrefix;
                    setting.SMTPFromEmail = model.SMTPFromEmail;
                    setting.SMTPPassword = model.SMTPPassword;
                    setting.SMTPServerName = model.SMTPServerName;
                    setting.SMTPPort = model.SMTPPort;
                    setting.SMTPEnableSSL = model.SMTPEnableSSL;
                    setting.OTPBaseUrl = model.OTPBaseUrl;
                    setting.OTPSenderId = model.OTPSenderId;
                    setting.OTPAPIKey = model.OTPAPIKey;
                    setting.OTPClientId = model.OTPClientId;
                    setting.OTPUsername = model.OTPUsername;
                    setting.OTPPassword = model.OTPPassword;
                    setting.MinPasswordLength = model.MinPasswordLength;
                    setting.MinSpecialCharacters = model.MinSpecialCharacters;
                    setting.MaxSignOnAttempts = model.MaxSignOnAttempts;
                    setting.EnforcePasswordHistoryId = model.EnforcePasswordHistoryId;
                    setting.ActivationLinkExpiresIn = model.ActivationLinkExpiresIn;
                    setting.ModifiedBy = User.Identity.GetUserId();
                    setting.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                    setting.DeactivateUserAfter = model.DeactivateUserAfter;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Setting, "", "", "", "true")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    Settings setting = db.Settings.Where(a => a.Id == Id).FirstOrDefault();
                    if (setting != null)
                    {
                        db.Settings.Remove(setting);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                Settings setting = db.Settings.Where(a => a.Id == Id).FirstOrDefault();
                if (setting == null)
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
    }
}