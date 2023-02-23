using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ERCOFAS.Enumeration;
using ERCOFAS.Helpers;
using ERCOFAS.Models;
using ERCOFAS.Resources;

namespace ERCOFAS.Controllers
{
    [Authorize]
    public class InitiatoryPleadingController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "true", "", "", "")]
        public ActionResult Index()
        {
            
            /*if (RoleHelpers.GetMainRole() == UserTypeEnum.Client.ToString())
            {
                string userId = User.Identity.GetUserId();

                int initiatoryPleadings = db.InitiatoryPleadings.Count(x => x.CreatedBy == userId);
                if (initiatoryPleadings == 0)
                    return Redirect("/InitiatoryPleading/NoFiledYet");
            }*/

            return View();
        }

        public ActionResult Submission()
        {
            return View();
        }

        public ActionResult Manifestation()
        {
            return View();
        }

        public ActionResult Compliance()
        {
            return View();
        }

        public ActionResult Petition()
        {
            return View();
        }

        public ActionResult Motion()
        {
            return View();
        }

        public ActionResult Others()
        {
            return View();
        }

        public ActionResult NoticeVirtualHearing()
        {
            return View();
        }

        public ActionResult InitialOrder()
        {
            return View();
        }

        public ActionResult NoFiledYet()
        {
            return View();
        }

        public ActionResult SelectAction()
        {
            return View();
        }

        public ActionResult GetPartialViewInitiatoryPleadings()
        {
            string userId = User.Identity.GetUserId();
            InitiatoryPleadingListing theListing = new InitiatoryPleadingListing();
            theListing.Listing = ReadInitiatoryPleadings(userId);
            return PartialView("~/Views/InitiatoryPleading/_MainList.cshtml", theListing);
        }

        public List<InitiatoryPleadingModel> ReadInitiatoryPleadings(string userId)
        {
            List<InitiatoryPleadingModel> list = new List<InitiatoryPleadingModel>();

            if (RoleHelpers.GetMainRole() == UserTypeEnum.Client.ToString())
            {
                list = (from t1 in db.InitiatoryPleadings
                        join t2 in db.AspNetUsers on t1.CreatedBy equals t2.Id
                        join t3 in db.UserProfiles on t2.Id equals t3.AspNetUserId                        
                        where t1.CreatedBy == userId
                        orderby t1.CreatedOn descending
                        select new InitiatoryPleadingModel()
                        {
                            Id = t1.Id,
                            DocumentName = t1.DocumentName,
                            Description = t1.Description,
                            Barcode = t1.Barcode,
                            DocketNumber = t1.DocketNumber,
                            CreatedBy = t3.FullName,
                            CreatedOn = t1.CreatedOn
                        }).ToList();
            }
            else
            {
                list = (from t1 in db.InitiatoryPleadings
                        join t2 in db.AspNetUsers on t1.CreatedBy equals t2.Id
                        join t3 in db.UserProfiles on t2.Id equals t3.AspNetUserId
                        orderby t1.CreatedOn descending
                        select new InitiatoryPleadingModel()
                        {
                            Id = t1.Id,
                            DocumentName = t1.DocumentName,
                            Description = t1.Description,
                            Barcode = t1.Barcode,
                            DocketNumber = t1.DocketNumber,
                            CreatedBy = t3.FirstName + " " + t3.LastName,
                            CreatedOn = t1.CreatedOn
                        }).ToList();

            }

            return list;
        }

        public List<InitiatoryPleadingAttachment> ReadInitiatoryPleadingAttachments(string initiatoryPleadingId)
        {
            List<InitiatoryPleadingAttachment> list = new List<InitiatoryPleadingAttachment>();
            list = db.InitiatoryPleadingAttachments.Where(x => x.InitiatoryPleadingId == initiatoryPleadingId).ToList();
            return list;
        }

        public InitiatoryPleadingModel GetViewModel(string Id, string type)
        {
            InitiatoryPleadingModel model = new InitiatoryPleadingModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                InitiatoryPleading initiatoryPleading = db.InitiatoryPleadings.Where(a => a.Id == Id).FirstOrDefault();
                PreFiledCases preFiledCase = db.PreFiledCases.Where(a => a.Id == initiatoryPleading.PreFiledCaseId).FirstOrDefault();

                model.Id = initiatoryPleading.Id;
                model.DocumentName = initiatoryPleading.DocumentName;
                model.Description = initiatoryPleading.Description;
                model.Barcode = initiatoryPleading.Barcode;
                model.DocketNumber = initiatoryPleading.DocketNumber;
                model.CaseTitle = initiatoryPleading.CaseTitle;
                model.ApplicantName = initiatoryPleading.ApplicantName;
                model.CaseType = preFiledCase != null ? GetCaseCode(general.GetGlobalOptionSetCode(preFiledCase.CaseTypeId)) : string.Empty;
                model.CaseNature = preFiledCase != null ? general.GetGlobalOptionSetDisplayName(preFiledCase.CaseNatureId) : string.Empty;
                model.DocketNumberCaseType = DateTime.Now.ToString("yyyy");
                model.DocketNumberSequence = string.Empty;
                model.PreFiledAttachmentViewModels = GetPreFiledAttachments(initiatoryPleading.PreFiledCaseId);
                model.CreatedBy = db.UserProfiles.FirstOrDefault(x => x.AspNetUserId == initiatoryPleading.CreatedBy).FullName;
                model.CreatedOn = initiatoryPleading.CreatedOn;
                model.ModifiedBy = initiatoryPleading.ModifiedBy;
                model.ModifiedOn = initiatoryPleading.ModifiedOn;
                if (type == "View")
                {
                    model.CreatedAndModified = general.GetCreatedAndModified(initiatoryPleading.CreatedBy, initiatoryPleading.CreatedOn.ToString(), initiatoryPleading.ModifiedBy, initiatoryPleading.ModifiedOn.ToString());
                }
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.InitiatoryPleading, "", "true", "true", "")]
        public ActionResult DeleteFile(string Id)
        {
            if (Id != null)
            {
                var attachment = db.InitiatoryPleadingAttachments.FirstOrDefault(x => x.Id == Id);
                db.InitiatoryPleadingAttachments.Remove(attachment);
                db.SaveChanges();

                if (System.IO.File.Exists(attachment.FileUrl))
                    System.IO.File.Delete(attachment.FileUrl);

                TempData["NotifySuccess"] = "File was deleted successfully.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        //[CustomAuthorizeFilter(ProjectEnum.ModuleCode.InitiatoryPleading, "", "true", "true", "")]
        public ActionResult Edit(string Id)
        {
            InitiatoryPleadingModel model = new InitiatoryPleadingModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            model.Attachments = ReadInitiatoryPleadingAttachments(Id);
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.InitiatoryPleading, "true", "", "", "")]
        public ActionResult ViewRecord(string Id)
        {
            InitiatoryPleadingModel model = new InitiatoryPleadingModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
                model.Attachments = ReadInitiatoryPleadingAttachments(Id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(InitiatoryPleadingModel model)
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

        public void ValidateModel(InitiatoryPleadingModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.InitiatoryPleadings.Where(a => a.DocumentName == model.DocumentName && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.InitiatoryPleadings.Where(a => a.DocumentName == model.DocumentName).Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("DocumentName", Resource.RequestSubjectAlreadyExist);
                }
            }
        }

        public void SaveRecord(InitiatoryPleadingModel model)
        {
            string userId = User.Identity.GetUserId();

            if (model != null)
            {
                //edit
                if (model.Id != null)
                {
                    InitiatoryPleading initiatoryPleading = db.InitiatoryPleadings.Where(a => a.Id == model.Id).FirstOrDefault();
                    initiatoryPleading.DocumentName = model.DocumentName;
                    initiatoryPleading.Description = model.Description;
                    initiatoryPleading.Barcode = string.IsNullOrEmpty(model.Barcode) ? Guid.NewGuid().ToString() : initiatoryPleading.Barcode;
                    initiatoryPleading.DocketNumber = model.DocketNumberYear + model.DocketNumberSequence + model.CaseType;
                    initiatoryPleading.CaseTitle = model.CaseTitle;
                    initiatoryPleading.ApplicantName = model.ApplicantName;
                    initiatoryPleading.ModifiedBy = userId;
                    initiatoryPleading.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                    db.Entry(initiatoryPleading).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //new record
                else
                {
                    InitiatoryPleading initiatoryPleading = new InitiatoryPleading();
                    initiatoryPleading.Id = Guid.NewGuid().ToString();
                    initiatoryPleading.DocumentName = model.DocumentName;
                    initiatoryPleading.Description = model.Description;
                    initiatoryPleading.Barcode = model.Barcode;
                    initiatoryPleading.DocketNumber = model.DocketNumber;
                    initiatoryPleading.CaseTitle = model.CaseTitle;
                    initiatoryPleading.ApplicantName = model.ApplicantName;
                    initiatoryPleading.PreFiledCaseId = model.PreFiledCaseId;
                    initiatoryPleading.CreatedBy = userId;
                    initiatoryPleading.CreatedOn = general.GetSystemTimeZoneDateTimeNow();
                    db.InitiatoryPleadings.Add(initiatoryPleading);
                    db.SaveChanges();

                    model.Id = initiatoryPleading.Id;
                }

                if(model.Documents != null && model.Documents.First() != null)
                    general.SaveInitiatoryPleadingAttachment(model.Documents, model.Id, User.Identity.GetUserId());
            }
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.PreFiledCase, "", "", "", "true")]
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    InitiatoryPleading initiatoryPleading = db.InitiatoryPleadings.Where(a => a.Id == Id).FirstOrDefault();
                    if (initiatoryPleading != null)
                    {
                        db.InitiatoryPleadings.Remove(initiatoryPleading);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception)
            {
                InitiatoryPleading initiatoryPleading = db.InitiatoryPleadings.Where(a => a.Id == Id).FirstOrDefault();
                if (initiatoryPleading == null)
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

        [HttpGet]
        public ActionResult Add(string preFileCaseId)
        {
            bool saved = false;
            var preFiledCase = db.PreFiledCases.Where(x => x.Id == preFileCaseId).FirstOrDefault();
            if(preFiledCase != null)
            {
                preFiledCase.FileCaseStatusId = "Completed";                
                preFiledCase.ModifiedOn = general.GetSystemTimeZoneDateTimeNow();
                db.Entry(preFiledCase).State = EntityState.Modified;
                db.SaveChanges();

                var initiatoryPleading = db.InitiatoryPleadings.Where(x => x.DocumentName == preFiledCase.RequestSubject).FirstOrDefault();

                if (initiatoryPleading == null)
                {
                    var caseType = preFiledCase != null ? general.GetGlobalOptionSetCode(preFiledCase.CaseTypeId) : string.Empty;
                    InitiatoryPleadingModel initiatoryPleadingModel = new InitiatoryPleadingModel
                    {
                        DocumentName = preFiledCase.RequestSubject,
                        Description = preFiledCase.RequestSubject,
                        Barcode = Guid.NewGuid().ToString(),
                        DocketNumber = GenerateDocketNumber(caseType),
                        PreFiledCaseId = preFiledCase.Id
                    };
                    SaveRecord(initiatoryPleadingModel);
                }
                saved = true;
            }

            return Json(saved, JsonRequestBehavior.AllowGet);
        }

        private string GenerateDocketNumber(string caseType)
        {
            if (string.IsNullOrEmpty(caseType))
                return caseType;

            string caseCode = GetCaseCode(caseType);
            string docketNumber = $"{DateTime.Now.ToString("yyyy")}-{000:D3}{caseCode}";
            return docketNumber;
        }

        private string GetCaseCode(string caseType)
        {
            string caseCode = string.Empty;
            switch (caseType)
            {
                case "RateCase":
                    caseCode = "RC";
                    break;

                case "MiscellaneousCase":
                    caseCode = "MC";
                    break;

                case "ComplianceFiling":
                    caseCode = "CF";
                    break;

                case "DisputeResolution":
                    caseCode = "DP";
                    break;

                case "PrepaidRetailElectricService":
                case "PRES":
                    caseCode = "PRES";
                    break;

                case "RuleMaking":
                    caseCode = "RM";
                    break;
            }

            return caseCode;
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