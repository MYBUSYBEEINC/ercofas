using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using ERCOFAS.Enumeration;
using ERCOFAS.Helpers;
using ERCOFAS.Models;
using ERCOFAS.Resources;
using static ERCOFAS.Models.ProjectEnum;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class PreFiledCaseController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "true", "", "", "")]
        public ActionResult Index()
        {
            if (RoleHelpers.GetMainRole() == UserTypeEnum.Client.ToString())
            {
                string userId = User.Identity.GetUserId();

                int preFilings = db.PreFiledCases.Count(x => x.UserId == userId);
                if (preFilings == 0)
                    return Redirect("/PreFiledCase/NoFiledYet");
            }

            return View();
        }

        public ActionResult NoFiledYet()
        {
            return View();
        }

        public ActionResult GetPartialViewPreFiledCases()
        {
            string userId = User.Identity.GetUserId();
            PreFiledCaseListing theListing = new PreFiledCaseListing();
            theListing.Listing = ReadPreFiledCases(userId);
            return PartialView("~/Views/PreFiledCase/_MainList.cshtml", theListing);
        }

        public List<PreFiledCaseViewModel> ReadPreFiledCases(string userId)
        {
            List<PreFiledCaseViewModel> list = new List<PreFiledCaseViewModel>();
            list = (from t1 in db.PreFiledCases.AsNoTracking()
                    join t2 in db.AspNetUsers.AsNoTracking() on t1.UserId equals t2.Id
                    join t3 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "CaseType") on t1.CaseTypeId equals t3.Id
                    join t4 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "CaseNature") on t1.CaseNatureId equals t4.Id
                    join t5 in db.GlobalOptionSets.AsNoTracking().Where(x => x.Type == "FileCaseStatus") on t1.FileCaseStatusId equals t5.Id 
                    into fileCaseStatus from preFiledCasesStatus in fileCaseStatus.DefaultIfEmpty()
                    join t6 in db.UserProfiles.AsNoTracking() on t1.UserId equals t6.AspNetUserId
                    join t7 in db.AspNetUserRoles.AsNoTracking() on t1.UserId equals t7.UserId
                    join t8 in db.AspNetRoles.AsNoTracking() on t7.RoleId equals t8.Id
                    orderby t1.CreatedOn descending
                    select new PreFiledCaseViewModel
                    {
                        Id = t1.Id,
                        RequestSubject = t1.RequestSubject,
                        UserId = t2.Id,
                        UserName = t2.UserName,
                        CaseTypeId = t3.DisplayName,
                        CaseNatureId = t4.DisplayName,
                        FileCaseStatusId = preFiledCasesStatus != null ? preFiledCasesStatus.DisplayName : t1.FileCaseStatusId,
                        InitialReviewStatus = t1.InitialReviewStatus,
                        DocumentsUploadStatus = t1.DocumentsUploadStatus,
                        PaymentStatus = t1.PaymentStatus,
                        CreatedOn = t1.CreatedOn,
                        ModifiedOn = t1.ModifiedOn,
                        FullName = t6.FullName,
                        Email = t2.Email,
                        PhoneNumber = t2.PhoneNumber,
                        UserType = t8.Name,
                        Office = t1.Office,
                        TimeStart = t1.TimeStart,
                        TimeEnd = t1.TimeEnd,
                        PreFilingCaseComplete = !string.IsNullOrEmpty(t1.FileCaseStatusId) && t1.FileCaseStatusId == "Completed"
                    }).OrderByDescending(x => x.CreatedOn).ToList();

            var userRole = RoleHelpers.GetMainRole();
            if (userRole != null && userRole == UserTypeEnum.Client.ToString())
                list = list.Where(x => x.UserId == userId).ToList();

            return list;
        }

        public List<PreFiledAttachment> ReadPreFiledAttachments(string preFiledCaseId)
        {
            List<PreFiledAttachment> list = new List<PreFiledAttachment>();
            list = db.PreFiledAttachments.Where(x => x.PreFiledCaseId == preFiledCaseId).ToList();
            return list;
        }

        public List<PreFiledCaseLogs> ReadPreFiledCaseLogs(string preFiledCaseId)
        {
            List<PreFiledCaseLogs> list = new List<PreFiledCaseLogs>();
            list = db.PreFiledCaseLogs.Where(x => x.PreFiledCaseId == preFiledCaseId).ToList();
            return list;
        }

        public List<PreFiledCaseRemarkFileLogs> ReadPreFiledCaseRemarkFileLogs(string preFiledCaseId)
        {
            List<PreFiledCaseRemarkFileLogs> list = new List<PreFiledCaseRemarkFileLogs>();
            list = db.PreFiledCaseRemarkFileLogs.Where(x => x.PreFiledCaseId == preFiledCaseId).ToList();
            return list;
        }


        public PreFiledCaseViewModel GetViewModel(string Id, string type)
        {
            PreFiledCaseViewModel model = new PreFiledCaseViewModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                PreFiledCases preFiledCase = db.PreFiledCases.Where(a => a.Id == Id).FirstOrDefault();
                AspNetUserRoles role = db.AspNetUserRoles.Where(a => a.UserId == preFiledCase.UserId).FirstOrDefault();
                model.Id = preFiledCase.Id;
                model.RequestSubject = preFiledCase.RequestSubject;
                model.UserId = preFiledCase.UserId;
                model.CaseTypeId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.CaseTypeId).DisplayName;
                model.CaseTypeName = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.CaseTypeId).DisplayName;
                model.CaseNatureId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.CaseNatureId).DisplayName;
                model.CaseNatureName = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.CaseNatureId).DisplayName;
                model.FileCaseStatusId = db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.FileCaseStatusId) != null ? db.GlobalOptionSets.FirstOrDefault(x => x.Id == preFiledCase.FileCaseStatusId).DisplayName : "Completed";
                model.Remarks = preFiledCase.Remarks;
                model.ApprovalRemarks = preFiledCase.ApprovalRemarks;
                model.InitialReviewStatus = preFiledCase.InitialReviewStatus;
                model.InitialReviewRequired = true;
                model.DocumentsUploadStatus = preFiledCase.DocumentsUploadStatus;
                model.PaymentStatus = preFiledCase.PaymentStatus;
                model.CreatedOn = preFiledCase.CreatedOn;
                model.ModifiedOn = preFiledCase.ModifiedOn;
                model.FullName = db.UserProfiles.FirstOrDefault(x => x.AspNetUserId == preFiledCase.UserId).FullName;
                model.Email = db.AspNetUsers.FirstOrDefault(x => x.Id == preFiledCase.UserId).Email;
                model.PhoneNumber = db.AspNetUsers.FirstOrDefault(x => x.Id == preFiledCase.UserId).PhoneNumber;
                model.UserType = db.AspNetRoles.FirstOrDefault(x => x.Id == role.RoleId).Name;
                model.TimeStart = preFiledCase.TimeStart;
                model.TimeEnd= preFiledCase.TimeEnd;
                if (type == "View") 
                {
                    string modifiedBy = preFiledCase.ModifiedOn != null ? preFiledCase.UserId : string.Empty;
                    model.CreatedAndModified = general.GetCreatedAndModified(preFiledCase.UserId, preFiledCase.CreatedOn.ToString(), modifiedBy, preFiledCase.ModifiedOn.ToString());
                }
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "true", "true", "")]
        public ActionResult DeleteFile(string Id)
        {
            if (Id != null)
            {
                var attachment = db.PreFiledAttachments.FirstOrDefault(x => x.Id == Id);
                db.PreFiledAttachments.Remove(attachment);
                db.SaveChanges();

                if (System.IO.File.Exists(attachment.FileUrl))
                    System.IO.File.Delete(attachment.FileUrl);

                TempData["NotifySuccess"] = "File was deleted successfully.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "true", "true", "")]
        public ActionResult Edit(string Id)
        {
            PreFiledCaseViewModel model = new PreFiledCaseViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            model.CaseTypeSelectList = general.GetCaseTypeList(model.CaseTypeId);
            model.CaseNatureSelectList = general.GetCaseNatureList(model.CaseNatureId, model.CaseTypeId);
            model.Attachments = ReadPreFiledAttachments(Id);
            model.PreFiledAttachmentViewModels = GetPreFiledAttachments(Id);
            model.InitialReviewRequired = true;

            var preFiledSurveyInformation = new PreFiledSurveyInformationModel();
            preFiledSurveyInformation.OfficeList = GetOfficeList();
            preFiledSurveyInformation.VisitPurposeList = GetPurposeVisitList();
            preFiledSurveyInformation.SurveyQuestions = GetSurveyQuestions();
            model.PreFiledSurveyInformation = preFiledSurveyInformation;
           
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "true", "", "", "")]
        public ActionResult ViewRecord(string Id)
        {
            PreFiledCaseViewModel model = new PreFiledCaseViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            model.Attachments = ReadPreFiledAttachments(Id);
            model.PreFiledAttachmentViewModels = GetPreFiledAttachments(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PreFiledCaseViewModel model)
        {
            ValidateModel(model);

            if (!ModelState.IsValid)
            {
                model.CaseTypeSelectList = general.GetCaseTypeList(model.CaseTypeId);
                model.CaseNatureSelectList = general.GetCaseNatureList(model.CaseNatureId);
                return View(model);
            }

            SaveRecord(model);
            TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            return RedirectToAction("index");
        }

        public void ValidateModel(PreFiledCaseViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.PreFiledCases.Where(a => a.RequestSubject == model.RequestSubject && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.PreFiledCases.Where(a => a.RequestSubject == model.RequestSubject).Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("RequestSubject", Resource.RequestSubjectAlreadyExist);
                }                
            }
        }

        public void SaveRecord(PreFiledCaseViewModel model)
        {
            if (model != null)
            {
                string preFiledCaseId = string.Empty;
                string userId = User.Identity.GetUserId();
                //edit
                if (model.Id != null)
                {
                    PreFiledCases prefiledCase = db.PreFiledCases.Where(a => a.Id == model.Id).FirstOrDefault();
                    prefiledCase.RequestSubject = model.RequestSubject;
                    prefiledCase.UserId = userId;
                    prefiledCase.CaseTypeId = model.CaseTypeId;
                    prefiledCase.Remarks = model.Remarks;
                    prefiledCase.DocumentsUploadStatus = Constant.DocumentUpload_Status_Pending;
                    prefiledCase.Office = prefiledCase.InitialReviewStatus == Constant.InitialReview_Status_Approved && prefiledCase.Office == "Applicant" ? "LS" : prefiledCase.Office;
                    prefiledCase.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                    db.Entry(prefiledCase).State = EntityState.Modified;
                    db.SaveChanges();

                    preFiledCaseId = prefiledCase.Id;
                }
                //new record
                else
                {
                    PreFiledCases prefiledCase = new PreFiledCases();
                    prefiledCase.Id = Guid.NewGuid().ToString();
                    prefiledCase.RequestSubject = model.RequestSubject;
                    prefiledCase.UserId = userId;
                    prefiledCase.CaseTypeId = model.CaseTypeId;
                    prefiledCase.CaseNatureId = model.CaseNatureId;
                    prefiledCase.FileCaseStatusId = "3188CF59-2C55-47AB-8DF0-AC8EB643C825";
                    prefiledCase.InitialReviewStatus = model.InitialReviewRequired ? Constant.InitialReview_Status_Pending : Constant.InitialReview_Status_Approved;
                    prefiledCase.Office = "ROS";
                    prefiledCase.PaymentStatus = Constant.Payment_Status_Pending;
                    prefiledCase.Remarks = model.Remarks;
                    prefiledCase.CreatedOn = general.GetSystemTimeZoneDateTimeNow();
                    db.PreFiledCases.Add(prefiledCase);
                    db.SaveChanges();

                    preFiledCaseId = prefiledCase.Id;
                }

                string fileStatus = model.InitialReviewRequired ? Constant.PreFileAttachment_Status_Pending : Constant.InitialReview_Status_Approved    ;
                var preFiledAttachments = general.GetGlobalOptionSets("PreFileAttachment");
                if (model.Id == null)
                {
                    general.SavePreFiledAttachment(model.Documents, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.OtherDocument.ToString()));

                    general.SavePreFiledAttachment(model.ApplicationForm, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.ApplicationForm.ToString()));
                }

                if (model.Id != null)
                {
                    var caseTypeName = general.GetGlobalOptionSetDisplayName(model.CaseTypeId);

                    general.SavePreFiledAttachment(model.NotaryPresentedId, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.NotaryPresentedId.ToString()));

                    general.SavePreFiledAttachment(model.CertificationForum, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.CertificationForum.ToString()));

                    general.SavePreFiledAttachment(model.AuthorityCounsel, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.AuthorityCounsel.ToString()));


                    general.SavePreFiledAttachment(model.AuthorityAffiant, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.AuthorityAffiant.ToString()));

                    if (!string.IsNullOrEmpty(caseTypeName) && (caseTypeName != "Rule Making" && caseTypeName != "Miscellaneous Case"))
                    {
                        general.SavePreFiledAttachment(model.ServiceLocalGovernmentUnit, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.ServiceLgu.ToString()));

                        general.SavePreFiledAttachment(model.Publication, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.Publication.ToString()));
                    }

                    if (!string.IsNullOrEmpty(caseTypeName) && caseTypeName == "Prepaid Retail Electric Service")
                    {
                        general.SavePreFiledAttachment(model.ExemptionCompetitive, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.ExemptionCompetitive.ToString()));

                        general.SavePreFiledAttachment(model.Competitive, preFiledCaseId, userId, fileStatus, GetPreFiledAttachmentType(preFiledAttachments, PreFileAttachment.Competitive.ToString()));
                    }
                }
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "", "", "true")]
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    PreFiledCases prefiledCase = db.PreFiledCases.Where(a => a.Id == Id).FirstOrDefault();
                    if (prefiledCase != null)
                    {
                        general.SaveGeneralLogs(User.Identity.GetUserId().ToString(),
                        GeneralHelpers.GetData(GeneralEnum.ModuleId.UserType).ToString(),
                        GeneralHelpers.GetData(GeneralEnum.Type.Delete).ToString(), prefiledCase.RequestSubject.ToString(), "", "");
                        db.PreFiledCases.Remove(prefiledCase);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                PreFiledCases prefiledCase = db.PreFiledCases.Where(a => a.Id == Id).FirstOrDefault();
                if (prefiledCase == null)
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

        private bool InitialReviewComplete(string id)
        {
            bool reviewCompleted = false;

            var preFiledCase = db.PreFiledCases.Where(x => x.Id == id).FirstOrDefault();

            reviewCompleted = preFiledCase != null && (preFiledCase.DocumentsUploadStatus != Constant.DocumentUpload_Status_Pending || preFiledCase.DocumentsUploadStatus == Constant.DocumentUpload_Status_Rejected);

            return reviewCompleted;

        }

        [HttpPost]
        public ActionResult UploadDocument(string id, PreFiledCaseViewModel model)
        {
            bool documentUploadUpdated = false;
            try
            {
                var preFiledCase = db.PreFiledCases.Where(x => x.Id == id).FirstOrDefault();

                if(preFiledCase != null)
                {
                    if (preFiledCase.InitialReviewStatus == "Rejected")
                    {
                        preFiledCase.InitialReviewStatus = "Updated";
                    }
                    else
                    {
                        if (preFiledCase.InitialReviewStatus == "Approved")
                        {
                            if (preFiledCase.DocumentsUploadStatus == "Rejected")
                            {
                                preFiledCase.DocumentsUploadStatus = "Updated";
                                preFiledCase.Office = "LS";

                                documentUploadUpdated = true;
                            }
                            else if(preFiledCase.DocumentsUploadStatus == "Updated")
                            {
                                documentUploadUpdated = true;
                            }
                        }
                    }

                    if (preFiledCase.TimeStart != null)
                    {
                        preFiledCase.TimeStart = general.GetSystemTimeZoneDateTimeNow();
                        preFiledCase.TimeStart = general.GetEndDate();

                    }

                    db.Entry(preFiledCase).State = EntityState.Modified;
                    db.SaveChanges();
                }

                foreach (int i in Enum.GetValues(typeof(PreFileAttachment)))
                {
                    List<HttpPostedFileBase> httpPostedFileBases = new List<HttpPostedFileBase>();

                    switch(i)
                    {
                        case (int)PreFileAttachment.ApplicationForm:
                            httpPostedFileBases = model.ApplicationForm;
                            break;

                        case (int)PreFileAttachment.NotaryPresentedId:
                            httpPostedFileBases = model.NotaryPresentedId;
                            break;

                        case (int)PreFileAttachment.CertificationForum:
                            httpPostedFileBases = model.CertificationForum;
                            break;

                        case (int)PreFileAttachment.AuthorityCounsel:
                            httpPostedFileBases = model.AuthorityCounsel;
                            break;

                        case (int)PreFileAttachment.AuthorityAffiant:
                            httpPostedFileBases = model.AuthorityAffiant;
                            break;

                        case (int)PreFileAttachment.ServiceLgu:
                            httpPostedFileBases = model.ServiceLocalGovernmentUnit;
                            break;

                        case (int)PreFileAttachment.Publication:
                            httpPostedFileBases = model.Publication;
                            break;

                        case (int)PreFileAttachment.ExemptionCompetitive:
                            httpPostedFileBases = model.ExemptionCompetitive;
                            break;

                        case (int)PreFileAttachment.Competitive:
                            httpPostedFileBases = model.Competitive;
                            break;

                        case (int)PreFileAttachment.OtherDocument:
                            httpPostedFileBases = model.Documents;
                            break;
                    }

                    SavePreFileDocumentAttachment(id, httpPostedFileBases, Enum.GetName(typeof(PreFileAttachment), i));
                }                

                TempData["NotifySuccess"] = "File uploaded successfully";
            }
            catch (Exception)
            {
                TempData["NotifyFailed"] = "File upload encountered error. Please check files uploaded.";
            }

            if (documentUploadUpdated)
                return RedirectToAction("Edit", new { id = id });
            
            return RedirectToAction("index");
        }

        private void SavePreFileDocumentAttachment(string id, List<HttpPostedFileBase> files, string type)
        {
            if (files != null && files.First() != null)
            {
                general.SavePreFiledAttachment(files, id, User.Identity.GetUserId(), Constant.DocumentUpload_Status_Pending
                                        , GetPreFiledAttachmentType(general.GetGlobalOptionSets("PreFileAttachment"), type));

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
        }

        // GET: /PreFiledCase/MenuSelection
        [AllowAnonymous]
        public ViewResult MenuSelection()
        {
            return View();
        }

        private string GetPreFiledAttachmentType(IList<GlobalOptionSet> preFiledAttachments, string type)
        {
            return general.GetGlobalOptionSetId(type, preFiledAttachments);
        }

        public IList<PreFiledAttachmentViewModel> GetPreFiledAttachments(string preFiledCaseId)
        {
            IList<PreFiledAttachmentViewModel> list = new List<PreFiledAttachmentViewModel>();
            var preFiledAttachments = db.PreFiledAttachments.Where(x => x.PreFiledCaseId == preFiledCaseId).ToList();
            list = (from x in preFiledAttachments
                    select new PreFiledAttachmentViewModel
                    {
                        Id = x.Id,
                        FileName = x.FileName,
                        FileTypeId = x.FileTypeId,
                        UniqueFileName = x.UniqueFileName,
                        FileTypeName = general.GetGlobalOptionSetDisplayName(x.FileTypeId),
                        CodeName = general.GetGlobalOptionSetCode(x.FileTypeId),
                        StatusId = x.StatusId
                    }).ToList();

            return list;
        }

        #region review
        public ActionResult ApproveFile(string Id, string prefile)
        {
            try

            {
                if (Id != null)
                {
                    // Temporarily set the Payment Status to approved while waiting for SOA/Payment to proceed on completion.
                    PreFiledCases preFiledCase = db.PreFiledCases.Where(x => x.Id == prefile).FirstOrDefault();
                    if (preFiledCase != null)
                    {
                        preFiledCase.PaymentStatus = "Approved";
                        db.Entry(preFiledCase).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                    if (preFiledAttachment != null)
                    {
                        preFiledAttachment.StatusId = "Approved";
                        db.Entry(preFiledAttachment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = "Approved Successfully";
            }
            catch (Exception)
            {
                PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                if (preFiledAttachment == null)
                {
                    TempData["NotifySuccess"] = "Approved Successfully";
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction(nameof(ViewRecord), new { id = prefile });
        }

        public ActionResult DiscardFile(string Id, string prefile)
        {
            try
            {
                if (Id != null)
                {
                    PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                    if (preFiledAttachment != null)
                    {
                        preFiledAttachment.StatusId = "Discarded";
                        db.Entry(preFiledAttachment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = "Discarded Successfully";
            }
            catch (Exception)
            {
                PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                if (preFiledAttachment == null)
                {
                    TempData["NotifySuccess"] = "Discarded Successfully";
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction(nameof(ViewRecord), new { id = prefile });
        }

        public ActionResult RejectFile(string Id, string prefile)
        {
            try
            {
                if (Id != null)
                {
                    PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                    if (preFiledAttachment != null)
                    {
                        preFiledAttachment.StatusId = "Rejected";
                        db.Entry(preFiledAttachment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = "Rejected Successfully";
            }
            catch (Exception)
            {
                PreFiledAttachment preFiledAttachment = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                if (preFiledAttachment == null)
                {
                    TempData["NotifySuccess"] = "Rejected Successfully";
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction(nameof(ViewRecord), new { id = prefile });
        }
        public JsonResult uploadfile()
        {
            // check if the user selected a file to upload
            if (Request.Files.Count > 0)
            {
                try
                {
                    string userId = User.Identity.GetUserId();
                    string fileStatus = "Remarks";
                    var preFiledAttachments = general.GetGlobalOptionSets("PreFileAttachment");
                    string preFiledCaseId = Request.Form["pi"];
                    string status = Request.Form["v"];
                    string remarks = Request.Form["remarks"];

                    HttpFileCollectionBase files = Request.Files;

                    List<HttpPostedFileBase> list = new List<HttpPostedFileBase>();
                    foreach (string file in files)
                    {
                        list.Add(files[file]);
                    }

                    PreFiledCases attachment = db.PreFiledCases.Where(a => a.Id == preFiledCaseId).FirstOrDefault();
                    if (attachment != null)
                    {
                        string s = status;

                        if (s == "ai")
                        {
                            attachment.InitialReviewStatus = "Approved";
                            attachment.Office = "Applicant";
                            attachment.Remarks = remarks;
                        }
                        else if (s == "ri")
                        {
                            attachment.InitialReviewStatus = "Rejected";
                            attachment.Remarks = remarks;

                        }
                        else if (s == "ad")
                        {
                            attachment.DocumentsUploadStatus = "Approved";
                            attachment.Office = "Acctg";
                            attachment.Remarks = remarks;

                        }
                        else if (s == "rd")
                        {
                            attachment.DocumentsUploadStatus = "Rejected";
                            attachment.Office = "Applicant";
                            attachment.Remarks = remarks;

                        }

                        PreFiledCaseLogs logs = new PreFiledCaseLogs();
                        logs.Id = Guid.NewGuid().ToString();
                        logs.PreFiledCaseId = preFiledCaseId;
                        logs.Remarks = remarks;
                        logs.CreatedBy = userId;
                        logs.CreatedOn = general.GetSystemTimeZoneDateTimeNow();
                        db.PreFiledCaseLogs.Add(logs);

                        db.Entry(attachment).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    general.SavePreFiledAttachment(list, preFiledCaseId, userId, fileStatus, "Remarks");
                    general.SaveRemarkFileAttachment(list, preFiledCaseId, userId);

                    return Json(Resource.RecordSavedSuccessfully);
                }

                catch (Exception e)
                {
                    return Json("error" + e.Message);
                }
            }
            else
            {
                try
                {
                    string userId = User.Identity.GetUserId();
                    var preFiledCaseId = Request.Form["pi"];
                    var status = Request.Form["v"];
                    var remarks = Request.Form["remarks"];

                    PreFiledCases attachment = db.PreFiledCases.Where(a => a.Id == preFiledCaseId).FirstOrDefault();
                    if (attachment != null)
                    {
                        string s = status;

                        if (s == "ai")
                        {
                            attachment.InitialReviewStatus = "Approved";
                            attachment.Office = "Applicant";
                            attachment.Remarks = remarks;
                        }
                        else if (s == "ri")
                        {
                            attachment.InitialReviewStatus = "Rejected";
                            attachment.Remarks = remarks;

                        }
                        else if (s == "ad")
                        {
                            attachment.DocumentsUploadStatus = "Approved";
                            attachment.Office = "Acctg";
                            attachment.Remarks = remarks;

                        }
                        else if (s == "rd")
                        {
                            attachment.DocumentsUploadStatus = "Rejected";
                            attachment.Office = "Applicant";
                            attachment.Remarks = remarks;

                        }

                        PreFiledCaseLogs logs = new PreFiledCaseLogs();
                        logs.Id = Guid.NewGuid().ToString();
                        logs.PreFiledCaseId = preFiledCaseId;
                        logs.Remarks = remarks;
                        logs.CreatedBy = userId;
                        logs.CreatedOn = general.GetSystemTimeZoneDateTimeNow();
                        db.PreFiledCaseLogs.Add(logs);

                        db.Entry(attachment).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return Json(Resource.RecordSavedSuccessfully);
                }

                catch (Exception e)
                {
                    return Json("error" + e.Message);
                }

            }
        }

        public ActionResult DeleteRemarksFile(string Id, string prefileId)
        {
            PreFiledAttachment prefiledCases = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
            var id = prefiledCases.PreFiledCaseId;
            try
            {
                if (Id != null)
                {
                    PreFiledAttachment prefiledCase = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                    if (prefiledCase != null)
                    {
                        db.PreFiledAttachments.Remove(prefiledCase);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                PreFiledAttachment prefiledCase = db.PreFiledAttachments.Where(a => a.Id == Id).FirstOrDefault();
                if (prefiledCase == null)
                {
                    TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
                }
                else
                {
                    TempData["NotifyFailed"] = Resource.FailedExceptionError;
                }
            }
            return RedirectToAction(nameof(ViewRecord), new { id = prefileId });
        }

        public FileResult DownloadFile(string fileName)
        {
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/Documents"), fileName);

            if (System.IO.File.Exists(filePath))
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
            }

            return File(filePath, GetContentType(filePath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        #endregion review        

        public ActionResult GetCaseNatureList(string caseTypeId)
        {
            var caseNatureList = general.GetCaseNatureList("", caseTypeId);

            return Json(caseNatureList, JsonRequestBehavior.AllowGet);
        }

        #region Survey

        private IList<string> GetOfficeList()
        {
            IList<string> officeList = new List<string>();

            officeList.Add("CAS");
            officeList.Add("ROS");
            officeList.Add("MOS");
            officeList.Add("LS");
            officeList.Add("OGCS/CRD");
            officeList.Add("FAS");
            officeList.Add("PPIS");
            officeList.Add("CCM");

            return officeList;
        }

        private IList<string> GetPurposeVisitList()
        {
            IList<string> purposeList = new List<string>();

            purposeList.Add("Case Filing");
            purposeList.Add("Certificate of Compliance (COC)");
            purposeList.Add("Meter Reading");
            purposeList.Add("Claim Document");
            purposeList.Add("Document Submission");
            purposeList.Add("Consumer Complaint");
            purposeList.Add("Inquiry");
            purposeList.Add("Payment");
            purposeList.Add("Retail Electricity Supplier (RES)");
            purposeList.Add("Others");

            return purposeList;
        }

        private IList<SurveyQuestionModel> GetSurveyQuestions()
        {
            var surveyQuestions = new List<SurveyQuestionModel>();
            
            surveyQuestions = (from a in db.SurveyQuestions
                               where a.Type == "PreFiledClientSatisfactory"
                               select new SurveyQuestionModel()
                               {
                                   Id = a.Id,
                                   Question = a.Question,
                                   Description = a.Description,
                                   Type = a.Type
                               }).ToList();

            return surveyQuestions;
        }

        [HttpPost]
        public ActionResult SubmitSurvey(PreFiledSurveyInformationModel preFiledSurveyInformationModel)
        {
            var userId = User.Identity.GetUserId();

            var preFiledSurveyInformation = new PreFiledSurveyInformation
            {
                Id = Guid.NewGuid().ToString(),
                Name = preFiledSurveyInformationModel.Name,
                Company = preFiledSurveyInformationModel.Company,
                ContactNumber = preFiledSurveyInformationModel.ContactNumber,
                EmailAddress = preFiledSurveyInformationModel.EmailAddress,
                OfficeVisited = preFiledSurveyInformationModel.OfficeVisited,
                PurposeVisit = preFiledSurveyInformationModel.PurposeVisit,
                PreFiledCaseId = preFiledSurveyInformationModel.PreFiledCaseId,
                Comment = preFiledSurveyInformationModel.Comment,
                CreatedBy = userId,
                CreatedOn = general.GetSystemTimeZoneDateTimeNow()
            };
            db.PreFiledSurveyInformations.Add(preFiledSurveyInformation);
            db.SaveChanges();

            IList<PreFiledSurveyFeedback> preFiledSurveyFeedbacks = new List<PreFiledSurveyFeedback>();
            foreach (var surveyQuestion in preFiledSurveyInformationModel.SurveyQuestions)
            {
                var preFiledSurveyFeedback = new PreFiledSurveyFeedback
                {
                    Id = Guid.NewGuid().ToString(),
                    SurveyQuestion = surveyQuestion.Id,
                    Description = surveyQuestion.Description,
                    SurveyType = "PreFiledClientSatisfactory",
                    SatisfactionRate = surveyQuestion.Rate,
                    PreFiledSurveyInformationId = preFiledSurveyInformation.Id,
                    CreatedBy = userId,
                    CreatedOn = general.GetSystemTimeZoneDateTimeNow()
                };
                preFiledSurveyFeedbacks.Add(preFiledSurveyFeedback);
            }

            if (preFiledSurveyFeedbacks.Any())
            {
                db.PreFiledSurveyFeedbacks.AddRange(preFiledSurveyFeedbacks);
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}