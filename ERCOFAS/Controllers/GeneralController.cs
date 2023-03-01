﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ERCOFAS.Models;
using System.Globalization;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using ERCOFAS.Resources;
using Newtonsoft.Json;
using System.Security.AccessControl;
using KellermanSoftware.CompareNetObjects;
using static ERCOFAS.Models.GeneralEnum;
using System.Xml.Linq;
using ERCOFAS.Helpers;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class GeneralController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();

        public string GetAppSettingsValue(string key)
        {
            switch (key)
            {
                // smtp setting
                case "smtpUserName": return ConfigurationManager.AppSettings["smtpUserName"].ToString();
                case "smtpPassword": return ConfigurationManager.AppSettings["smtpPassword"].ToString();
                case "smtpHost": return ConfigurationManager.AppSettings["smtpHost"].ToString();
                case "smtpPort": return ConfigurationManager.AppSettings["smtpPort"].ToString();
                //portal general setting
                case "portalName": return ConfigurationManager.AppSettings["portalName"].ToString();
                case "timeZone": return ConfigurationManager.AppSettings["timeZone"].ToString();
                case "confirmEmailToLogin": return ConfigurationManager.AppSettings["confirmEmailToLogin"].ToString();
                case "dateFormat": return ConfigurationManager.AppSettings["dateFormat"].ToString();
                case "dateFormatJs": return ConfigurationManager.AppSettings["dateFormatJs"].ToString();
                case "datetimeFormat": return ConfigurationManager.AppSettings["datetimeFormat"].ToString();
                case "environment": return ConfigurationManager.AppSettings["environment"].ToString();
                default: break;
            }
            return "";
        }

        public DateTime? GetSystemTimeZoneDateTimeNow()
        {
            string timeZone = GetAppSettingsValue("timeZone");
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, timeZone);
            DateTime utcDt = DateTime.UtcNow;
            DateTime localDt = utcDt.ToLocalTime();
            return dateTime;
        }

        public string GetIsoUtcNow()
        {
            return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        }

        public string ReplaceWords(string sentence, string target, string replaceWith)
        {
            string result = sentence.Replace(target, replaceWith);
            return result;
        }

        public void SendEmail(string email, string subject, string body)
        {
            string host = GetAppSettingsValue("smtpHost");
            string strPort = GetAppSettingsValue("smtpPort");
            int port = Int32.Parse(strPort);
            string userName = GetAppSettingsValue("smtpUserName");
            string password = GetAppSettingsValue("smtpPassword");
            var client = new SmtpClient(host, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userName, password);
            client.EnableSsl = true;

            MailMessage mail = new MailMessage(userName, email, subject, body);
            mail.IsBodyHtml = true;
            client.Send(mail);
        }

        public void SaveLoginHistory(string userId)
        {
            LoginHistory loginHistory = new LoginHistory();
            loginHistory.Id = Guid.NewGuid().ToString();
            loginHistory.AspNetUserId = userId;
            loginHistory.LoginDateTime = GetSystemTimeZoneDateTimeNow();
            loginHistory.IsoUtcLoginDateTime = GetIsoUtcNow();
            db.LoginHistories.Add(loginHistory);
            db.SaveChanges();
        }

        public void SaveInitiatoryPleadingAttachment(List<HttpPostedFileBase> files, string initiatoryPleadingId, string userId)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    InitiatoryPleadingAttachment attachment = new InitiatoryPleadingAttachment();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.InitiatoryPleadingId = initiatoryPleadingId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.FileUrl = path;
                    attachment.CreatedBy = userId;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.InitiatoryPleadingAttachments.Add(attachment);
                    db.SaveChanges();
                }
            }
        }

        public void SavePreFiledAttachment(List<HttpPostedFileBase> files, string preFiledId, string userId, string fileStatus, string fileType)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    var preFiledAttachment = db.PreFiledAttachments.Where(x => x.PreFiledCaseId == preFiledId && x.FileTypeId == fileType).FirstOrDefault();

                    //if (preFiledAttachment != null)
                    //{
                    //    preFiledAttachment.FileName = fileNameWithExtension;
                    //    preFiledAttachment.UniqueFileName = uniqueFileName;
                    //    preFiledAttachment.FileUrl = path;
                    //    preFiledAttachment.StatusId = fileStatus;                                           
                    //    preFiledAttachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                        PreFiledAttachment attachment = new PreFiledAttachment();
                        attachment.Id = Guid.NewGuid().ToString();
                        attachment.PreFiledCaseId = preFiledId;
                        attachment.FileName = fileNameWithExtension;
                        attachment.UniqueFileName = uniqueFileName;
                        attachment.FileUrl = path;
                        attachment.StatusId = fileStatus;
                        attachment.FileTypeId = fileType;
                        attachment.CreatedBy = userId;
                        attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;

                        PreFiledCaseLogs logs = new PreFiledCaseLogs();
                        logs.Id = Guid.NewGuid().ToString();
                        logs.PreFiledCaseId = preFiledId;
                        logs.Remarks = "Uploaded new file";
                        logs.CreatedBy = userId;
                        logs.CreatedOn = GetSystemTimeZoneDateTimeNow();
                        db.PreFiledCaseLogs.Add(logs);

                        db.PreFiledAttachments.Add(attachment);
                        db.SaveChanges();
                    //}
                }
            }
        }

        public void SaveUserAttachment(HttpPostedFileBase file, string userProfileId, string attachmentTypeId, string uploadedById)
        {
            var fileNameWithExtension = Path.GetFileName(file.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            //string uniqueid = Guid.NewGuid().ToString();
            string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
            fileNameWithoutExtension = fileNameWithoutExtension.Replace(" ", "");//remove space to form a valid url
            string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
            var path = Path.Combine(HostingEnvironment.MapPath("~/UploadedFiles"), uniqueFileName);
            file.SaveAs(path);
            UserAttachment userAttachment = new UserAttachment();
            userAttachment.Id = Guid.NewGuid().ToString();
            userAttachment.UserProfileId = userProfileId;
            userAttachment.FileName = fileNameWithExtension;
            userAttachment.UniqueFileName = uniqueFileName;
            userAttachment.FileUrl = path;
            userAttachment.AttachmentTypeId = attachmentTypeId;
            userAttachment.CreatedBy = uploadedById;
            userAttachment.CreatedOn = GetSystemTimeZoneDateTimeNow();
            userAttachment.IsoUtcCreatedOn = GetIsoUtcNow();
            db.UserAttachments.Add(userAttachment);
            db.SaveChanges();
        }

        public List<SelectListItem> GetGlobalOptionSets(string type, string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == type && t1.Status == "Active"
                    orderby t1.OptionOrder
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.Id == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public IList<GlobalOptionSet> GetGlobalOptionSets(string type)
        {
            return db.GlobalOptionSets.Where(x => x.Type == type && x.Status == "Active").ToList();
        }

        public string GetGlobalOptionSetId(string code, string type)
        {
            string id = db.GlobalOptionSets.Where(a => a.Code == code && a.Type == type && a.Status == "Active").Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public string GetGlobalOptionSetId(string code, IList<GlobalOptionSet> globalOptionSets)
        {
            string id = globalOptionSets.Where(a => a.Code == code).Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public string GetGlobalOptionSetDisplayName(string Id)
        {
            string id = db.GlobalOptionSets.Where(a => a.Id == Id && a.Status == "Active").Select(a => a.DisplayName).FirstOrDefault();
            return id;
        }

        public string GetGlobalOptionSetCode(string Id)
        {
            string id = db.GlobalOptionSets.Where(a => a.Id == Id && a.Status == "Active").Select(a => a.Code).FirstOrDefault();
            return id;
        }        

        public List<SelectListItem> GetRoles()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.AspNetRoles
                    orderby t1.Name
                    select new SelectListItem
                    {
                        Text = t1.Name,
                        Value = t1.Name
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetRoleList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.AspNetRoles
                    orderby t1.Name
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.Name,
                        Selected = t1.Id == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetRERTypeList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "RERType"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetCountries(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.Countries
                    orderby t1.Name
                    select new SelectListItem
                    {
                        Value = t1.CountryCode,
                        Text = t1.Name + "(+" + t1.CountryCode + ")",
                        Selected = t1.CountryCode == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetCaseTypeList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "CaseType"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetCaseNatureList(string selectedId, string referenceId = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            
            if (string.IsNullOrEmpty(referenceId))
            {
                list = (from t1 in db.GlobalOptionSets
                        where t1.Type == "CaseNature"
                        orderby t1.DisplayName
                        select new SelectListItem
                        {
                            Value = t1.Id,
                            Text = t1.DisplayName,
                            Selected = t1.DisplayName == selectedId ? true : false
                        }).ToList();
            }
            else
            {
                list = (from t1 in db.GlobalOptionSets
                        where t1.Type == "CaseNature"
                        && t1.ReferenceId == referenceId
                        orderby t1.DisplayName
                        select new SelectListItem
                        {
                            Value = t1.Id,
                            Text = t1.DisplayName,
                            Selected = t1.DisplayName == selectedId ? true : false
                        }).ToList();
            }
            return list;
        }

        public List<SelectListItem> GetFileCaseStatusList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "FileCaseStatus"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetRolesForMultiSelect(List<string> selectedRoleNames)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (selectedRoleNames != null)
            {
                list = (from t1 in db.AspNetRoles
                        orderby t1.Name
                        select new SelectListItem
                        {
                            Text = t1.Name,
                            Value = t1.Name,
                            Selected = selectedRoleNames.Contains(t1.Name) ? true : false
                        }).ToList();
            }
            else
            {
                list = (from t1 in db.AspNetRoles
                        orderby t1.Name
                        select new SelectListItem
                        {
                            Text = t1.Name,
                            Value = t1.Name
                        }).ToList();
            }
            return list;
        }

        public List<SelectListItem> GetUserTypesForMultiSelect(List<string> selectedUserTypeNames)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (selectedUserTypeNames != null)
            {
                list = (from t1 in db.GlobalOptionSets
                        where t1.Type == "UserType"
                        orderby t1.OptionOrder
                        select new SelectListItem
                        {
                            Text = t1.DisplayName,
                            Value = t1.DisplayName,
                            Selected = selectedUserTypeNames.Contains(t1.DisplayName) ? true : false
                        }).ToList();
            }
            else
            {
                list = (from t1 in db.GlobalOptionSets
                        where t1.Type == "UserType"
                        orderby t1.DisplayName
                        select new SelectListItem
                        {
                            Text = t1.DisplayName,
                            Value = t1.DisplayName
                        }).ToList();
            }
            return list;
        }

        public List<SelectListItem> GetCountryList(string selectedName)
        {
            List<SelectListItem> countryList = new List<SelectListItem>();
            CultureInfo[] cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo ci in cultureInfos)
            {
                RegionInfo regionInfo = new RegionInfo(ci.LCID);
                if (countryList.Where(a => a.Value == regionInfo.EnglishName).Any() == false)
                {
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = regionInfo.EnglishName;
                    selectListItem.Value = regionInfo.EnglishName;
                    selectListItem.Selected = (string.IsNullOrEmpty(selectedName) || selectedName != regionInfo.EnglishName) ? false : true;
                    countryList.Add(selectListItem);
                }
            }
            return countryList;
        }

        public bool UsernameExists(string username, string currentRecordId)
        {
            bool usernameExist = false;
            if (string.IsNullOrEmpty(currentRecordId))
            {
                usernameExist = db.AspNetUsers.Where(a => a.UserName == username).Any();
            }
            else
            {
                //user is editing record, need to add one more filter "a.Id != currentRecordId"
                usernameExist = db.AspNetUsers.Where(a => a.UserName == username && a.Id != currentRecordId).Any();
            }
            return usernameExist;
        }

        public bool EmailExists(string email, string currentRecordId)
        {
            bool emailExist = false;
            if (string.IsNullOrEmpty(currentRecordId))
            {
                emailExist = db.AspNetUsers.Where(a => a.Email == email).Any();
            }
            else
            {
                //user is editing his/her own record, need to add one more filter "a.Id != currentRecordId"
                emailExist = db.AspNetUsers.Where(a => a.Email == email && a.Id != currentRecordId).Any();
            }
            return emailExist;
        }

        public bool FilenameExists(string filename)
        {
            bool filenameExist = false;
            filenameExist = db.UserAttachments.Where(a => a.FileName == filename).Any();
            return filenameExist;
        }

        public EmailTemplate EmailTemplateForConfirmEmail(string Username, string callbackUrl)
        {
            string websiteName = GetAppSettingsValue("portalName");
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == "ConfirmEmail").FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Url]", callbackUrl);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public EmailTemplate EmailTemplateForPasswordResetByAdmin(string Username = "", string ResetByName = "", string NewPassword = "")
        {
            string websiteName = GetAppSettingsValue("portalName");
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == ProjectEnum.EmailTemplate.PasswordResetByAdmin.ToString()).FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[ResetByName]", ResetByName);
            body = ReplaceWords(body, "[NewPassword]", NewPassword);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public EmailTemplate EmailTemplateForForgotPassword(string Username, string callbackUrl)
        {
            string websiteName = GetAppSettingsValue("portalName");
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == "ForgotPassword").FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Url]", callbackUrl);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public string GetCurrentUserFullName()
        {
            string id = User.Identity.GetUserId();
            string name = db.UserProfiles.Where(a => a.AspNetUserId == id).Select(a => a.FullName).DefaultIfEmpty("").FirstOrDefault();
            return name;
        }

        public CreatedAndModifiedViewModel GetCreatedAndModified(string createdBy, string isoUtcCreatedOn, string modifiedBy, string isoUtcModifiedOn)
        {
            CreatedAndModifiedViewModel model = new CreatedAndModifiedViewModel();
            model.CreatedByName = db.UserProfiles.Where(a => a.AspNetUserId == createdBy).Select(a => a.FullName).FirstOrDefault();
            model.ModifiedByName = db.UserProfiles.Where(a => a.AspNetUserId == modifiedBy).Select(a => a.FullName).FirstOrDefault();
            if (string.IsNullOrEmpty(isoUtcCreatedOn) == false)
            {
                DateTime? createdOn = DateTime.Parse(isoUtcCreatedOn);
                model.FormattedCreatedOn = createdOn?.ToString(GetAppSettingsValue("datetimeFormat"));
            }
            if (string.IsNullOrEmpty(isoUtcModifiedOn) == false)
            {
                DateTime? modifiedOn = DateTime.Parse(isoUtcModifiedOn);
                model.FormattedModifiedOn = modifiedOn?.ToString(GetAppSettingsValue("datetimeFormat"));
            }
            return model;
        }

        public string GetFormattedDateTime(string isoDateTime)
        {
            string result = "";
            DateTime? dt = DateTime.Parse(isoDateTime);
            result = dt?.ToString(GetAppSettingsValue("datetimeFormat"));
            return result;
        }

        public string GetUserProfileId(string aspNetUserId)
        {
            string id = db.UserProfiles.Where(a => a.AspNetUserId == aspNetUserId).Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public UserProfileViewModel GetCurrentUserProfile(string currentUserId)
        {
            UserProfileViewModel model = new UserProfileViewModel();
            string profilePicTypeId = GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
            model = (from t1 in db.UserProfiles
                     join t2 in db.AspNetUsers on t1.AspNetUserId equals t2.Id
                     where t2.Id == currentUserId
                     select new UserProfileViewModel
                     {
                         Id = t1.Id,
                         AspNetUserId = t1.AspNetUserId,
                         FullName = t1.FullName,
                         IDCardNumber = t1.IDCardNumber,
                         FirstName = t1.FirstName,
                         LastName = t1.LastName,
                         DateOfBirth = t1.DateOfBirth,
                         PhoneNumber = t1.PhoneNumber,
                         Address = t1.Address,
                         PostalCode = t1.PostalCode,
                         Username = t2.UserName,
                         EmailAddress = t2.Email,
                         UserStatusId = t1.UserStatusId,
                         GenderId = t1.GenderId,
                         CountryName = t1.CountryName,
                         CreatedBy = t1.CreatedBy,
                         ModifiedBy = t1.ModifiedBy,
                         CreatedOn = t1.CreatedOn,
                         ModifiedOn = t1.ModifiedOn,
                         IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                         IsoUtcModifiedOn = t1.IsoUtcModifiedOn
                     }).FirstOrDefault();
            model.UserStatusName = db.GlobalOptionSets.Where(a => a.Id == model.UserStatusId).Select(a => a.DisplayName).DefaultIfEmpty("").FirstOrDefault();
            model.UserTypeIdList = (from t1 in db.AspNetUserRoles
                                    join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                    where t1.UserId == model.AspNetUserId
                                    select t2.Name).ToList();
            model.UserTypeName = String.Join(", ", model.UserTypeIdList);
            model.GenderName = db.GlobalOptionSets.Where(a => a.Id == model.GenderId).Select(a => a.DisplayName).DefaultIfEmpty("").FirstOrDefault();
            model.ProfilePictureFileName = db.UserAttachments.Where(a => a.UserProfileId == model.Id && a.AttachmentTypeId == profilePicTypeId).OrderByDescending(o => o.CreatedOn).Select(a => a.FileName).FirstOrDefault();
            model.FormattedDateOfBirth = model.DateOfBirth?.ToString(GetAppSettingsValue("dateFormat"));
            model.CreatedAndModified = GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);

            return model;
        }

        public bool ValidateImageFile(string fileName)
        {
            bool validated = false;
            if (fileName.Contains("jpg", StringComparison.OrdinalIgnoreCase) || fileName.Contains("jpeg", StringComparison.OrdinalIgnoreCase) || fileName.Contains("png", StringComparison.OrdinalIgnoreCase))
            {
                validated = true;
            }
            return validated;
        }

        public bool HasNonLetterOrDigit(string value)
        {
            return value.Any(ch => !char.IsLetterOrDigit(ch));
        }
        public bool HasDigit(string value)
        {
            return value.Any(ch => char.IsDigit(ch));
        }
        public bool HasLowercase(string value)
        {
            return value.Any(ch => char.IsLower(ch));
        }
        public bool HasUppercase(string value)
        {
            return value.Any(ch => char.IsUpper(ch));
        }

        public List<string> ValidateImportUserFromExcel(UserProfileViewModel model)
        {
            List<string> errors = new List<string>();
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.EmailAddress)
                    || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword)
                    || string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.CountryName))
                {
                    errors.Add(Resource.SomeRequiredFieldsAreEmpty);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    errors.Add(Resource.PasswordNotMatch);
                }
                PasswordValidation passwordValidation = new PasswordValidation();
                if (passwordValidation.IsValid(model.Password) == false)
                {
                    errors.Add(passwordValidation.ErrorMessage);
                }
                string phonePattern = @"^\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$";
                if (Regex.Match(model.PhoneNumber, phonePattern).Success == false)
                {
                    errors.Add(Resource.InvalidPhoneNumber);
                }
                EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
                bool emailValid = emailAddressAttribute.IsValid(model.EmailAddress);
                if (emailValid == false)
                {
                    errors.Add(Resource.InvalidEmailAddress);
                }
                string usernamePattern = @"^[A-Za-z]\w{3,29}$";
                if (Regex.Match(model.Username, usernamePattern).Success == false)
                {
                    errors.Add(Resource.InvalidUsername);
                }
                var _user = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Username || a.Email == model.EmailAddress);
                if (_user != null)
                {
                    errors.Add($"{Resource.User} {model.Username} {Resource.alreadyexists}.");
                }
            }
            return errors;
        }

        public List<string> ValidateColumns(List<string> dtColumns, List<string> columns)
        {
            var errors = new List<string>();
            //check either the provided columns and required columns length same
            if (dtColumns.Count != columns.Count)
            {
                errors.Add(Resource.ColumnsCountMismatch);
            }
            else
            {
                //check either the required columns exists in provided columns through excel
                foreach (var column in columns)
                {
                    if (!dtColumns.Contains(column))
                        errors.Add($"{Resource.Column} {column} {Resource.notfoundormismatch}");
                }
            }
            return errors;
        }

        public List<string> GetCurrentUserRoleIdList(string aspnetUserId)
        {
            List<string> roles = new List<string>();
            roles = db.AspNetUserRoles.Where(a => a.UserId == aspnetUserId).Select(a => a.RoleId).ToList();
            return roles;
        }

        public List<string> GetCurrentUserRoleNameList(string aspnetUserId)
        {
            List<string> nameList = new List<string>();
            List<string> roleIds = GetCurrentUserRoleIdList(aspnetUserId);
            foreach (var id in roleIds)
            {
                string name = db.AspNetRoles.Where(a => a.Id == id).Select(a => a.Name).FirstOrDefault();
                nameList.Add(name);
            }
            return nameList;
        }

        public CurrentUserPermission CanView(string aspnetUserId, string moduleName)
        {
            string moduleId = db.Modules.Where(a => a.Name == moduleName).Select(a => a.Id).FirstOrDefault();

            CurrentUserPermission currentUserPermission = new CurrentUserPermission();
            List<string> roles = GetCurrentUserRoleIdList(aspnetUserId);

            List<bool> permissions = new List<bool>();
            permissions = (from t1 in db.RoleModulePermissions
                           join t2 in roles on t1.RoleId equals t2
                           where t1.ModuleId == moduleId
                           select t1.ViewRight).ToList();
            if (permissions.Count == 0)
            {
                currentUserPermission.ViewRight = false;
            }
            else
            {
                currentUserPermission.ViewRight = permissions.Any(a => a == true);
            }

            return currentUserPermission;
        }

        public CurrentUserPermission GetCurrentUserPermission(string aspnetUserId, string moduleCode)
        {
            string moduleId = db.Modules.Where(a => a.Code == moduleCode).Select(a => a.Id).FirstOrDefault();

            CurrentUserPermission currentUserPermission = new CurrentUserPermission();
            List<string> roles = GetCurrentUserRoleIdList(aspnetUserId);

            List<RoleModulePermission> permissions = new List<RoleModulePermission>();
            permissions = (from t1 in db.RoleModulePermissions
                           join t2 in roles on t1.RoleId equals t2
                           where t1.ModuleId == moduleId
                           select t1).ToList();
            if (permissions.Count == 0)
            {
                currentUserPermission.ViewRight = false;
                currentUserPermission.AddRight = false;
                currentUserPermission.EditRight = false;
                currentUserPermission.DeleteRight = false;
            }
            else
            {
                currentUserPermission.ViewRight = permissions.Any(a => a.ViewRight == true);
                currentUserPermission.AddRight = permissions.Any(a => a.AddRight == true);
                currentUserPermission.EditRight = permissions.Any(a => a.EditRight == true);
                currentUserPermission.DeleteRight = permissions.Any(a => a.DeleteRight == true);
            }

            return currentUserPermission;
        }

        [AllowAnonymous]
        public ActionResult ChangeLanguage(string lang)
        {
            if (lang != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                lang = "en";
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = lang;
            Response.Cookies.Add(cookie);

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public FileResult Download(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "myfile.ext";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public List<SelectListItem> GetHearingTypeList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.HearingTypes
                    select new SelectListItem
                    {
                        Value = t1.Id.ToString(),
                        Text = t1.Name,
                        Selected = t1.Id == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetUsersList(string userId = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.AspNetUsers
                    join t2 in db.UserProfiles
                    on t1.Id equals t2.AspNetUserId
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t2.FirstName,
                        Selected = !string.IsNullOrEmpty(userId) && t1.Id == userId ? true : false
                    }).ToList();
            return list;
        }

        public string GetCurrentUserFullName(string id)
        {
            string name = db.UserProfiles.Where(a => a.AspNetUserId == id).Select(a => a.FullName).DefaultIfEmpty("").FirstOrDefault();
            return name;
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
            }
            base.Dispose(disposing);
        }


        public void SavePreRegistrationAttachment(List<HttpPostedFileBase> files, long preRegistrationId)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    PreRegistrationAttachment attachment = new PreRegistrationAttachment();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.PreRegistrationId = preRegistrationId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.FileUrl = path;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.PreRegistrationAttachments.Add(attachment);
                    db.SaveChanges();
                }
            }
        }

        public List<SelectListItem> GetRERClassificationList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "RERClassification"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }
        public List<SelectListItem> GetRegistrationStatusList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "RegistrationStatus"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        // For update - lgu list
        public List<SelectListItem> GetPreFiledRequirementsLguList(string userId = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Value = "1", Text = "Abra" });
            list.Add(new SelectListItem() { Value = "2", Text = "Agusan Del Norte" });
            list.Add(new SelectListItem() { Value = "3", Text = "Agusan Del Norte" });
            list.Add(new SelectListItem() { Value = "4", Text = "Aklan" });
            list.Add(new SelectListItem() { Value = "5", Text = "Albay" });
            list.Add(new SelectListItem() { Value = "6", Text = "Angeles City" });
            list.Add(new SelectListItem() { Value = "7", Text = "Bataan" });
            list.Add(new SelectListItem() { Value = "8", Text = "Batanes" });
            list.Add(new SelectListItem() { Value = "9", Text = "Cagayan" });
            list.Add(new SelectListItem() { Value = "10", Text = "Metro Manila" });

            return list;
        }
        public void SaveGeneralLogs(string user, string moduleid, string type, string value, object OldObject, object NewObject)
        {

            if (type == GeneralHelpers.GetData(GeneralEnum.Type.Update).ToString())
            {
                CompareLogic compObjects = new CompareLogic();
                compObjects.Config.MaxDifferences = 99;
                ComparisonResult compResult = compObjects.Compare(OldObject, NewObject);
                List<AuditDelta> DeltaList = new List<AuditDelta>();
                foreach (var change in compResult.Differences)
                {
                    AuditDelta delta = new AuditDelta();
                    delta._UPDATE_ = change.Object1Value;
                    delta._TO_ = change.Object2Value;
                    DeltaList.Add(delta);
                }

                string actiontaken = JsonConvert.SerializeObject(DeltaList).Replace("[", "").Replace("{\"", "")
                   .Replace("\":\"", " ")
                   .Replace("(null)", "<Default>")
                   .Replace("\",\"", " ")
                   .Replace("\"}", ", ")
               .Replace("}", " ")
               .Replace("]", " ");

                AuditLogs auditlogs = new AuditLogs();
                auditlogs.Id = Guid.NewGuid().ToString();
                auditlogs.DateTime = DateTime.Now;
                auditlogs.UserId = user;
                auditlogs.ModuleId = moduleid;
                auditlogs.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                auditlogs.ActionTaken = actiontaken;
                db.AuditLogs.Add(auditlogs);
                db.SaveChanges();

            }
            else if (type == GeneralHelpers.GetData(GeneralEnum.Type.Create).ToString())
            {
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserType).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewUserTypeCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserStatus).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewUserStatusCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.CaseType).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewCaseTypeCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.CaseNature).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewCaseNatureCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.RoleManagement).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewRoleCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserManagement).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewUserCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.PreFiledCase).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewPrefiledCaseCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.FiledCase).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewFiledCaseCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewHearingCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.InitiatoryPleading).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewInitiatoryPleadingCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.PleadingWithCaseNumber).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.NewPleadingWithCaseNumberCreated).Replace("{value}", value);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
            }
            else if (type == GeneralHelpers.GetData(GeneralEnum.Type.Login).ToString())
            {
                string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.Login);

                AuditLogs audit = new AuditLogs();
                audit.Id = Guid.NewGuid().ToString();
                audit.DateTime = DateTime.Now;
                audit.UserId = user;
                audit.ModuleId = moduleid;
                audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                audit.ActionTaken = actiontaken;
                db.AuditLogs.Add(audit);
                db.SaveChanges();
            }
            else if (type == GeneralHelpers.GetData(GeneralEnum.Type.Logoff).ToString())
            {
                string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.Logoff);

                AuditLogs audit = new AuditLogs();
                audit.Id = Guid.NewGuid().ToString();
                audit.DateTime = DateTime.Now;
                audit.UserId = user;
                audit.ModuleId = moduleid;
                audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                audit.ActionTaken = actiontaken;
                db.AuditLogs.Add(audit);
                db.SaveChanges();
            }
            else if (type == GeneralHelpers.GetData(GeneralEnum.Type.Delete).ToString())
            {
                string actiontaken = GeneralHelpers.GetData(GeneralEnum.GeneralActionTaken.Deleted);

                AuditLogs audit = new AuditLogs();
                audit.Id = Guid.NewGuid().ToString();
                audit.DateTime = DateTime.Now;
                audit.UserId = user;
                audit.ModuleId = moduleid;
                audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                audit.ActionTaken = value + actiontaken;
                db.AuditLogs.Add(audit);
                db.SaveChanges();
            }
            else if (type == GeneralHelpers.GetData(GeneralEnum.Type.View).ToString())
            {
                string view = GeneralEnum.Type.View.ToString();
                if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.AuditLog).ToString())
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.AuditLog);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Dashboard))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.Dashboard);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.PreFiledCase))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.PreFiledCase);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.FiledCase))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.FiledCase);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.Hearing);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.InitiatoryPleading))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.InitiatoryPleading);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.PleadingWithCaseNumber))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.PleadingWithCaseNumber);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.OtherLetterCorrespondense))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.OtherLetterCorrespondense);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.DisputeResolution))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.DisputeResolution);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Transaction))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.Transaction);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Payment))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.Payment);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserStatus))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.UserStatus);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserType))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.UserType);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.CaseType))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.CaseType);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.CaseNature))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.CaseNature);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserAttachmentType))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.UserAttachmentType);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.RoleManagement))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.RoleManagement);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.UserManagement))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.UserManagement);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.LoginHistory))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.LoginHistory);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }
                else if (moduleid == GeneralHelpers.GetData(GeneralEnum.ModuleId.Setting))
                {
                    string actiontaken = GeneralHelpers.GetData(GeneralEnum.ActionTakenView.Setting);

                    AuditLogs audit = new AuditLogs();
                    audit.Id = Guid.NewGuid().ToString();
                    audit.DateTime = DateTime.Now;
                    audit.UserId = user;
                    audit.ModuleId = moduleid;
                    audit.IpAddress = Helpers.GeoLocationHelpers.GetClientIpAddress();
                    audit.ActionTaken = view + " " + actiontaken;
                    db.AuditLogs.Add(audit);
                    db.SaveChanges();
                }

            }
        }

        public class AuditDelta
        {
            public string _UPDATE_ { get; set; }
            public string _TO_ { get; set; }
        }

        public void SaveTransactionAttachment(List<HttpPostedFileBase> files, string transactionsId)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    TransactionAttachment attachment = new TransactionAttachment();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.TransactionId = transactionsId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.CreatedBy = transactionsId;
                    attachment.FileUrl = path;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.TransactionAttachments.Add(attachment);
                    db.SaveChanges();
                }
            }
        }

        public List<SelectListItem> GetTransactionStatusList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "TransactionStatus"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public List<SelectListItem> GetPaymentMethodList(string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == "PaymentMethod"
                    orderby t1.DisplayName
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.DisplayName == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public void SaveAmendmentAttachment(List<HttpPostedFileBase> files, string amendmentId)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    AmendmentAttachment attachment = new AmendmentAttachment();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.AmendmentId = amendmentId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.CreatedBy = amendmentId;
                    attachment.FileUrl = path;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.AmendmentAttachments.Add(attachment);
                    db.SaveChanges();
                }
            }
        }
        public void SaveRemarkFileAttachment(List<HttpPostedFileBase> files, string prefiledCaseId, string uid)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    PreFiledCaseRemarkFileLogs attachment = new PreFiledCaseRemarkFileLogs();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.PreFiledCaseId = prefiledCaseId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.CreatedBy = uid;
                    attachment.FileUrl = path;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.PreFiledCaseRemarkFileLogs.Add(attachment);
                    db.SaveChanges();
                }
            }
        }

        public void SaveSealingAcceptanceAttachment(List<HttpPostedFileBase> files, string sealingId, string uid, string sealingStatus, string sealingName)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileNameWithExtension = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
                    string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
                    file.SaveAs(path);

                    SealingAndAcceptanceAttachment attachment = new SealingAndAcceptanceAttachment();
                    attachment.Id = Guid.NewGuid().ToString();
                    attachment.SealingAndAcceptanceId = sealingId;
                    attachment.FileName = fileNameWithExtension;
                    attachment.UniqueFileName = uniqueFileName;
                    attachment.Status = sealingStatus;
                    attachment.CreatedBy = uid;
                    attachment.FileUrl = path;
                    attachment.CreatedOn = GetSystemTimeZoneDateTimeNow().Value;
                    db.SealingAndAcceptanceAttachments.Add(attachment);
                    db.SaveChanges();
                }
            }
        }

        public DateTime? GetEndDate()
        {
            string timeZone = GetAppSettingsValue("timeZone");
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, timeZone);
            DateTime utcDt = DateTime.UtcNow;
            DateTime localDt = utcDt.ToLocalTime();

            // Add 3 days to the current date and time
            dateTime = dateTime.AddDays(3);

            return dateTime;
        }

        // GET: /General/UnderConstruction
        [AllowAnonymous]
        public ActionResult UnderConstruction()
        {
            return View();
        }
    }
}