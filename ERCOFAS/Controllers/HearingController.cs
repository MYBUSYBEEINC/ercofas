using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NetStarter.Helpers;
using NetStarter.Models;
using NetStarter.Resources;
using static NetStarter.Models.ProjectEnum;

namespace NetStarter.Controllers
{
    [Authorize]
    public class HearingController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private DefaultDBContext db = new DefaultDBContext();
        GeneralController generalController = new GeneralController();

        public HearingController() { }

        public HearingController(ApplicationSignInManager applicationSignInManager, ApplicationUserManager applicationUserManager)
        {
            _signInManager = applicationSignInManager;
            _userManager = applicationUserManager;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "", "", "")]
        public ActionResult Index()
        {
            generalController.SaveGeneralLogs(User.Identity.GetUserId().ToString(),
            GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing).ToString(),
            GeneralHelpers.GetData(GeneralEnum.Type.View).ToString(),
            "", "", "");

            return View();
        }

        public ActionResult GetPartialViewHearings()
        {
            string userId = User.Identity.GetUserId();
            HearingListing hearingListing = new HearingListing();
            hearingListing.Listing = GetHearingsList();

            var hearingTypes = (from type in db.HearingTypes
                                select type).ToList();

            hearingListing.Listing.ForEach(item =>
            {

                item.HearingSchedule = item.Schedule != DateTime.MinValue && item.Schedule != null ? item.Schedule.GetValueOrDefault().ToString("MM/dd/yyyy") : string.Empty;

                string hearingTypeName = string.Empty;
                if (item.HearingType != null)
                    hearingTypeName = hearingTypes.Where(x => x.Id == item.HearingType).FirstOrDefault().Name;

                item.HearingTypeName = hearingTypeName;

                string hearingStatusName = string.Empty;
                if (item.HearingStatus == (int)HearingStatus.Approved)
                    hearingStatusName = "Approved";
                else if (item.HearingStatus == (int)HearingStatus.Cancelled)
                    hearingStatusName = "Cancelled";
                else if (item.HearingStatus == (int)HearingStatus.Pending)
                    hearingStatusName = "Pending";
                else if (item.HearingStatus == (int)HearingStatus.Scheduled)
                    hearingStatusName = "Scheduled";
                else if (item.HearingStatus == (int)HearingStatus.ForSigning)
                    hearingStatusName = "For Signing";
                else if (item.HearingStatus == (int)HearingStatus.Signed)
                    hearingStatusName = "Signed";
                else if (item.HearingStatus == (int)HearingStatus.Confirmed)
                    hearingStatusName = "Confirmed";

                item.HearingStatusName = hearingStatusName;
            });
            return PartialView("~/Views/Hearing/_MainList.cshtml", hearingListing);
        }

        public List<HearingViewModel> GetHearingsList()
        {
            List<HearingViewModel> hearingViewModels = new List<HearingViewModel>();

            hearingViewModels = (from item in db.Hearings
                                 select new HearingViewModel
                                 {
                                     Id = item.Id,
                                     HearingType = item.HearingType,
                                     Description = item.Description,
                                     Schedule = item.Schedule,
                                     MeetingLink = item.MeetingLink,
                                     HearingStatus = item.HearingStatus,
                                     CreatedOn = item.DateCreated,
                                     ModifiedOn = item.DateUpdated
                                 }).OrderByDescending(item => item.Id).ToList();

            return hearingViewModels;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult Add()
        {
            HearingViewModel hearingViewModel = new HearingViewModel
            {
                Description = string.Empty,
                HearingTypeSelectList = generalController.GetHearingTypeList(string.Empty)
            };
            return View(hearingViewModel);
        }

        [HttpPost]
        public ActionResult Add(HearingViewModel hearingViewModel)
        {
            ValidateModel(hearingViewModel);
            if (!ModelState.IsValid)
            {
                hearingViewModel = new HearingViewModel();

                hearingViewModel.Description = string.Empty;
                hearingViewModel.HearingTypeSelectList = generalController.GetHearingTypeList(string.Empty);
                return View(hearingViewModel);
            }

            SaveRecord(hearingViewModel);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        private void SaveRecord(HearingViewModel hearingViewModel)
        {
            if (hearingViewModel != null)
            {
                var hearing = new Hearing()
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = hearingViewModel.Description,
                    HearingType = hearingViewModel.HearingType,
                    Schedule = hearingViewModel.Schedule,
                    MeetingLink = hearingViewModel.MeetingLink,
                    HearingStatus = (int)HearingStatus.Pending,
                    CreatedBy = User.Identity.GetUserId().ToString(),
                    DateCreated = DateTime.Now
                };

                db.Hearings.Add(hearing);
                db.SaveChanges();
            }
        }

        private void UpdateRecord(HearingViewModel model)
        {
            var hearing = db.Hearings.Where(x => x.Id == model.Id).FirstOrDefault();
            bool updated = false;

            if (hearing != null)
            {
                hearing.Schedule = model.Schedule;
                hearing.MeetingLink = model.MeetingLink;
                hearing.HearingStatus = model.HearingStatus;
                hearing.Description = model.Description;
                hearing.MeetingPassword = model.MeetingPassword;
                hearing.UpdatedBy = User.Identity.GetUserId().ToString();
                hearing.DateUpdated = DateTime.Now;

                db.SaveChanges();
                updated = true;
            }

            if (updated && model.HearingStatus == (int)HearingStatus.Cancelled)
            {
                string stakeHolderId = "";
                var emailAddressList = db.AspNetUsers.Where(x => x.Id == stakeHolderId).ToList();

                string subject = "Virtual Hearing Cancelled";
                string htmlMessage = "Dear Stakeholder/services";
                htmlMessage += $"\r\nVirtual Hearing has been cancelled.";
                htmlMessage += $"\r\nId: {hearing.Id}";
                htmlMessage += $"\r\nSchedule: {hearing.Schedule}";
                htmlMessage += $"\r\nMeeting Link: {hearing.MeetingLink}";
                htmlMessage += $"\r\nPassword: {hearing.MeetingPassword}";
                htmlMessage += $"\r\nDescription: {hearing.Description}";

                emailAddressList.ForEach(x =>
                {
                    EmailHelpers.SendEmail(x.Email, subject, htmlMessage);
                });
            }
        }

        private void ValidateModel(HearingViewModel model)
        {
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.Id))
                {
                    if (model.Schedule == DateTime.MinValue || model.Schedule == null)
                        ModelState.AddModelError("Schedule", "Required");

                }

                if (string.IsNullOrEmpty(model.HearingType))
                    ModelState.AddModelError("HearingType", "Required");

            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult Schedule(string id, bool isUserLs = false)
        {
            HearingViewModel hearingViewModel = new HearingViewModel();

            var hearing = db.Hearings.Where(x => x.Id == id).FirstOrDefault();
            hearingViewModel.Id = id;
            hearingViewModel.UserRoleName = isUserLs == true ? "Legal Service" : ""; // for update
            if (isUserLs)
                hearingViewModel.Schedule = hearing.Schedule.GetValueOrDefault().Date;

            hearingViewModel.HearingTypeSelectList = generalController.GetHearingTypeList(hearing.HearingType);

            generalController.SaveGeneralLogs(User.Identity.GetUserId().ToString(),
                GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing).ToString(),
                GeneralHelpers.GetData(GeneralEnum.Type.View).ToString(), "", "", "");

            return View(hearingViewModel);
        }

        [HttpPost]
        public ActionResult Schedule(HearingViewModel hearingViewModel)
        {
            ValidateModel(hearingViewModel);
            if (!ModelState.IsValid)
            {
                var hearing = db.Hearings.Where(x => x.Id == hearingViewModel.Id).FirstOrDefault();

                var hearingModel = new HearingViewModel
                {
                    Id = hearingViewModel.Id,
                    HearingTypeSelectList = generalController.GetHearingTypeList(hearing.HearingType)
                };

                generalController.SaveGeneralLogs(User.Identity.GetUserId().ToString(),
                    GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing).ToString(),
                    GeneralHelpers.GetData(GeneralEnum.Type.Create).ToString(), hearing.Description.ToString(), "", "");

                return View(hearingModel);
            }

            UpdateRecord(hearingViewModel);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult UploadDocuments(string id)
        {
            HearingDocumentsViewModel hearingDocumentsViewModel = new HearingDocumentsViewModel
            {
                HearingId = id,
                PersonnelSelectList = generalController.GetUsersList()
            };
            return View(hearingDocumentsViewModel);
        }

        [HttpPost]
        public ActionResult UploadDocument(HearingDocumentsViewModel hearingDocumentsViewModel, IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> hearingFiles)
        {
            bool documentUploaded = false;
            string filePath = string.Empty;
            string fileName = string.Empty;
            int byteCount = 0;

            if (files == null || files.First() == null)
                files = hearingFiles;

            foreach (HttpPostedFileBase file in files)
            {
                byteCount += file.ContentLength;
            }

            //if total file size is larger than 50mb
            if (byteCount > 52428800)
                ModelState.AddModelError("Files", Resource.TotalFileSizeCannotBeLargerThan50MB);

            if (!ModelState.IsValid)
                return View(hearingDocumentsViewModel);
            try
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null)
                    {
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fileName = testfiles[testfiles.Length - 1];
                        }
                        else
                            fileName = file.FileName;


                        filePath = Path.Combine(Request.MapPath("~/UploadedFiles"), fileName);
                        file.SaveAs(filePath);
                        documentUploaded = true;
                    }
                }


                generalController.SaveGeneralLogs(User.Identity.GetUserId().ToString(),
                    GeneralHelpers.GetData(GeneralEnum.ModuleId.Hearing).ToString(),
                    GeneralHelpers.GetData(GeneralEnum.Type.Create).ToString(), fileName, "", "");

                ModelState.Clear();
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception)
            {
                documentUploaded = false;
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
            }

            if (documentUploaded)
            {
                var request = new HearingDocuments()
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = fileName,
                    FilePath = filePath,
                    DocumentType = hearingDocumentsViewModel.DocumentType,
                    HearingId = hearingDocumentsViewModel.HearingId,
                    CreatedBy = User.Identity.GetUserId().ToString(),
                    CreatedOn = DateTime.Now
                };

                db.HearingDocuments.Add(request);
            }

            string redirectActionName = "uploaddocuments";
            var requestUrl = HttpContext.Request.UrlReferrer;
            if (requestUrl != null)
            {
                if (!requestUrl.ToString().ToLower().Contains(redirectActionName))
                {
                    redirectActionName = "order";

                    var lastIndex = requestUrl.ToString().LastIndexOf("/");
                    var parameterValue = requestUrl.ToString().Substring(lastIndex + 1);
                    hearingDocumentsViewModel.HearingId = parameterValue;
                }
            }

            return RedirectToAction(redirectActionName, new { Id = hearingDocumentsViewModel.HearingId });
        }

        [HttpPost]
        public async Task<ActionResult> AssignPersonnel(HearingDocumentsViewModel hearingDocumentsViewModel)
        {
            bool saved = false;

            var personnel = db.HearingPersonnels.Where(x => x.HearingId == hearingDocumentsViewModel.HearingId
                && x.UserId == hearingDocumentsViewModel.Personnel).FirstOrDefault();

            if (personnel == null)
            {
                var request = new HearingPersonnel()
                {
                    Id = Guid.NewGuid().ToString(),
                    HearingId = hearingDocumentsViewModel.HearingId,
                    UserId = hearingDocumentsViewModel.Personnel,
                    CreatedBy = User.Identity.GetUserId().ToString(),
                    CreatedOn = DateTime.Now
                };

                db.HearingPersonnels.Add(request);
                saved = await db.SaveChangesAsync() > 0;
            }

            if (saved)
            {
                var hearing = db.Hearings.Where(x => x.Id == hearingDocumentsViewModel.HearingId).FirstOrDefault();
                if (hearing != null)
                {
                    hearing.HearingStatus = (int)HearingStatus.ForSigning;
                    hearing.UpdatedBy = User.Identity.GetUserId().ToString();
                    hearing.DateUpdated = DateTime.Now;
                    await db.SaveChangesAsync();
                }
            }

            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult Order(string id)
        {
            HearingViewModel hearingViewModel = new HearingViewModel
            {
                Id = id
            };

            var hearing = db.Hearings.Where(x => x.Id == id).FirstOrDefault();
            if (hearing != null)
            {
                hearingViewModel.Schedule = hearing.Schedule;
                hearingViewModel.HearingType = hearing.HearingType;
                hearingViewModel.Description = hearing.Description;
            }

            return View(hearingViewModel);
        }

        [HttpPost]
        public ActionResult Order(HearingViewModel hearingViewModel)
        {
            ValidateModel(hearingViewModel);
            if (!ModelState.IsValid)
            {
                string errorMessages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                return Json(new { success = !errorMessages.Any(), error = errorMessages }, JsonRequestBehavior.AllowGet);
            }

            UpdateRecord(hearingViewModel);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return Json(new { success = true, error = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult Notify(string id)
        {
            HearingPersonnelViewModel hearingPersonnelViewModel = new HearingPersonnelViewModel
            {
                HearingId = id,
                StakeHolderSelectList = generalController.GetUsersList(),
                ServiceSelectList = generalController.GetUsersList()
            };
            return View(hearingPersonnelViewModel);
        }

        [HttpPost]
        public ActionResult Notify(HearingPersonnelViewModel hearingPersonnelViewModel)
        {
            if (string.IsNullOrEmpty(hearingPersonnelViewModel.StakeHolderUserId) || hearingPersonnelViewModel.StakeHolderUserId == "null")
                ModelState.AddModelError("StakeHolderUserId", "Required");

            if (string.IsNullOrEmpty(hearingPersonnelViewModel.ServiceUserId) || hearingPersonnelViewModel.ServiceUserId == "null")
                ModelState.AddModelError("ServiceUserId", "Required");

            if (!ModelState.IsValid)
            {
                HearingPersonnelViewModel personnelViewModel = new HearingPersonnelViewModel()
                {
                    HearingId = hearingPersonnelViewModel.HearingId,
                    StakeHolderSelectList = generalController.GetUsersList(hearingPersonnelViewModel.StakeHolderUserId),
                    ServiceSelectList = generalController.GetUsersList(hearingPersonnelViewModel.ServiceUserId)
                };

                return View(personnelViewModel);
            }

            var stakeHolderUser = db.AspNetUsers.Where(x => x.Id == hearingPersonnelViewModel.StakeHolderUserId).FirstOrDefault();
            var concernServiceUser = db.AspNetUsers.Where(x => x.Id == hearingPersonnelViewModel.ServiceUserId).FirstOrDefault();

            var hearing = db.Hearings.Where(x => x.Id == hearingPersonnelViewModel.Id).FirstOrDefault();

            string subject = "Virtual Hearing Notification!";
            string htmlMessage = "Dear Stakeholder/services";
            htmlMessage += $"\r\nVirtual Hearing has been scheduled.";
            htmlMessage += $"\r\nId: {hearing.Id}";
            htmlMessage += $"\r\nSchedule: {hearing.Schedule}";
            htmlMessage += $"\r\nMeeting Link: {hearing.MeetingLink}";
            htmlMessage += $"\r\nPassword: {hearing.MeetingPassword}";
            htmlMessage += $"\r\nDescription: {hearing.Description}";

            try
            {
                if (stakeHolderUser != null)
                    generalController.SendEmail(stakeHolderUser.Email, subject, htmlMessage);

                if (concernServiceUser != null)
                    generalController.SendEmail(concernServiceUser.Email, subject, htmlMessage);

            }
            catch { }

            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.Hearing, "true", "true", "true", "")]
        public ActionResult Register(string id)
        {
            HearingPersonnelViewModel hearingPersonnelViewModel = new HearingPersonnelViewModel()
            {
                HearingId = id
            };
            return View(hearingPersonnelViewModel);
        }

        [HttpPost]
        public ActionResult Register(HearingPersonnelViewModel hearingPersonnelViewModel)
        {
            const string requiredField = "Required";
            const string invalidEmailAddress = "Invalid email address";

            if (string.IsNullOrEmpty(hearingPersonnelViewModel.Name1))
                ModelState.AddModelError("Name1", requiredField);

            if (string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress1))
                ModelState.AddModelError("EmailAddress1", requiredField);

            if (!IsValidEmail(hearingPersonnelViewModel.EmailAddress1))
                ModelState.AddModelError("EmailAddress1", invalidEmailAddress);

            if (!string.IsNullOrEmpty(hearingPersonnelViewModel.Name2) || !string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress2))
            {
                if (string.IsNullOrEmpty(hearingPersonnelViewModel.Name2))
                    ModelState.AddModelError("Name2", requiredField);

                if (string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress2))
                    ModelState.AddModelError("EmailAddress2", requiredField);

                if (!IsValidEmail(hearingPersonnelViewModel.EmailAddress2))
                    ModelState.AddModelError("EmailAddress2", invalidEmailAddress);
            }

            if (!string.IsNullOrEmpty(hearingPersonnelViewModel.Name3) || !string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress3))
            {
                if (string.IsNullOrEmpty(hearingPersonnelViewModel.Name3))
                    ModelState.AddModelError("Name3", requiredField);

                if (string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress3))
                    ModelState.AddModelError("EmailAddress3", requiredField);

                if (!IsValidEmail(hearingPersonnelViewModel.EmailAddress3))
                    ModelState.AddModelError("EmailAddress3", invalidEmailAddress);
            }

            if (!string.IsNullOrEmpty(hearingPersonnelViewModel.Name4) || !string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress4))
            {
                if (string.IsNullOrEmpty(hearingPersonnelViewModel.Name4))
                    ModelState.AddModelError("Name4", requiredField);

                if (string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress4))
                    ModelState.AddModelError("EmailAddress4", requiredField);

                if (!IsValidEmail(hearingPersonnelViewModel.EmailAddress4))
                    ModelState.AddModelError("EmailAddress4", invalidEmailAddress);
            }

            if (!string.IsNullOrEmpty(hearingPersonnelViewModel.Name5) || !string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress5))
            {
                if (string.IsNullOrEmpty(hearingPersonnelViewModel.Name5))
                    ModelState.AddModelError("Name5", requiredField);

                if (string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress5))
                    ModelState.AddModelError("EmailAddress5", requiredField);

                if (!IsValidEmail(hearingPersonnelViewModel.EmailAddress5))
                    ModelState.AddModelError("EmailAddress5", invalidEmailAddress);
            }

            if (!ModelState.IsValid)
            {
                HearingPersonnelViewModel personnelViewModel = new HearingPersonnelViewModel()
                {
                    HearingId = hearingPersonnelViewModel.HearingId
                };

                return View(personnelViewModel);
            }

            RedirectToRouteResult redirectToRouteResult = new RedirectToRouteResult("index", new System.Web.Routing.RouteValueDictionary { });

            var hearing = db.Hearings.Where(x => x.Id == hearingPersonnelViewModel.HearingId).FirstOrDefault();
            if (hearing != null)
            {
                hearing.HearingStatus = (int)HearingStatus.Confirmed;
                hearing.UpdatedBy = User.Identity.GetUserId().ToString();
                hearing.DateUpdated = DateTime.Now;
                db.SaveChanges();

                bool saved = false;
                if (!string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress1))
                    saved = SaveParticipant(hearingPersonnelViewModel.HearingId, hearingPersonnelViewModel.Name1, hearingPersonnelViewModel.EmailAddress1);

                if (!string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress2))
                    saved = SaveParticipant(hearingPersonnelViewModel.HearingId, hearingPersonnelViewModel.Name2, hearingPersonnelViewModel.EmailAddress2);

                if (!string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress3))
                    saved = SaveParticipant(hearingPersonnelViewModel.HearingId, hearingPersonnelViewModel.Name3, hearingPersonnelViewModel.EmailAddress3);

                if (!string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress4))
                    saved = SaveParticipant(hearingPersonnelViewModel.HearingId, hearingPersonnelViewModel.Name4, hearingPersonnelViewModel.EmailAddress4);

                if (!string.IsNullOrEmpty(hearingPersonnelViewModel.EmailAddress5))
                    saved = SaveParticipant(hearingPersonnelViewModel.HearingId, hearingPersonnelViewModel.Name1, hearingPersonnelViewModel.EmailAddress5);

                if (!saved)
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                    redirectToRouteResult = new RedirectToRouteResult("register", new System.Web.Routing.RouteValueDictionary { { "Id", hearingPersonnelViewModel.Id } });
                }
                else
                    TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;

            }

            return RedirectToAction(redirectToRouteResult.RouteName, redirectToRouteResult.RouteValues);
        }

        private bool SaveParticipant(string id, string name, string emailAddress)
        {
            bool saved = false;
            var request = new HearingPersonnel()
            {
                Id = Guid.NewGuid().ToString(),
                HearingId = id,
                UserId = new Guid().ToString(),
                CreatedBy = User.Identity.GetUserId().ToString(),
                CreatedOn = DateTime.Now
            };

            db.HearingPersonnels.Add(request);
            db.SaveChanges();

            string error;
            saved = NotifyParticipant(id, name, emailAddress, out error);

            return saved;
        }

        private bool NotifyParticipant(string id, string name, string emailAddress, out string error)
        {
            error = string.Empty;
            var hearing = db.Hearings.Where(x => x.Id == id).FirstOrDefault();
            if (hearing != null)
            {
                string subject = "Virtual Hearing Registration!";
                string htmlMessage = $"Dear {name}";
                htmlMessage += $"\r\nYou have been registered for the virtual hearing with the following details.";
                htmlMessage += $"\r\nId: {hearing.Id}";
                htmlMessage += $"\r\nSchedule: {hearing.Schedule}";
                htmlMessage += $"\r\nMeeting Link: {hearing.MeetingLink}";
                htmlMessage += $"\r\nPassword: {hearing.MeetingPassword}";
                htmlMessage += $"\r\nDescription: {hearing.Description}";

                try
                {
                    generalController.SendEmail(emailAddress, subject, htmlMessage);
                }
                catch (Exception ex)
                {
                    error = $"Error in sending email. Error: {ex.Message}";
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        [AllowAnonymous]
        public ActionResult VirtualHearing()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }

                if (generalController != null)
                {
                    generalController.Dispose();
                    generalController = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}