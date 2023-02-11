using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NetStarter.Models;
using NetStarter.Resources;

namespace NetStarter.Controllers
{
    [Authorize]
    public class RERClassificationController : Controller
    {
        private DefaultDBContext db = new DefaultDBContext();
        private GeneralController general = new GeneralController();

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RERClassification, "true", "", "", "")]
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult GetPartialViewRERClassification()
        {
            RERClassificationListing listing = new RERClassificationListing();
            listing.Listing = ReadRERClassification();
            return PartialView("~/Views/RERClassification/_MainList.cshtml", listing);
        }

        public List<RERClassificationViewModel> ReadRERClassification()
        {
            List<RERClassificationViewModel> result = new List<RERClassificationViewModel>();
            result = (from t1 in db.GlobalOptionSets
                      join t2 in db.UserProfiles on t1.CreatedBy equals t2.AspNetUserId
                      where t1.Type == "RERClassification" && t1.Status == "Active"
                      select new RERClassificationViewModel
                      {
                          Id = t1.Id,
                          Code = t1.Code,
                          DisplayName = t1.DisplayName,
                          CreatedBy= t2.FullName,
                          CreatedOn=t1.CreatedOn,
                          ModifiedOn = t1.ModifiedOn,
                          ModifiedBy = t1.ModifiedBy,
                          ReferenceId = t1 == null ? "" : t1.DisplayName,
                          OptionOrder = t1.OptionOrder,
                          SystemDefault = t1.SystemDefault
                      }).ToList();
            return result;
        }

        public RERClassificationViewModel GetViewModel(string Id, string type)
        {
            RERClassificationViewModel model = new RERClassificationViewModel();
            using (DefaultDBContext db = new DefaultDBContext())
            {
                GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == Id).FirstOrDefault();
                model.Id = globalOptionSet.Id;
                model.Code = globalOptionSet.Code;
                model.DisplayName = globalOptionSet.DisplayName;
                model.ReferenceId = globalOptionSet.ReferenceId;
                model.Status = globalOptionSet.Status;
                model.OptionOrder = globalOptionSet.OptionOrder;
                model.IsoUtcCreatedOn = globalOptionSet.IsoUtcCreatedOn;
                model.IsoUtcModifiedOn = globalOptionSet.IsoUtcModifiedOn;
                model.SystemDefault = globalOptionSet.SystemDefault;
                model.CreatedBy = globalOptionSet.CreatedBy;
                model.CreatedOn = globalOptionSet.CreatedOn;
                model.ModifiedBy = globalOptionSet.ModifiedBy;
                model.ModifiedOn = globalOptionSet.ModifiedOn;
                if (type == "View")
                {
                    model.CreatedAndModified = general.GetCreatedAndModified(globalOptionSet.CreatedBy, globalOptionSet.IsoUtcCreatedOn, globalOptionSet.ModifiedBy, globalOptionSet.IsoUtcModifiedOn);
                }
            }
            return model;
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RERClassification, "", "true", "true", "")]
        public ActionResult Edit(string Id)
        {
            RERClassificationViewModel model = new RERClassificationViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            else
            {
                //display order
                int? maxOrder = db.GlobalOptionSets.Where(a => a.Type == "RERClassification" && a.Status == "Active").Select(a => a.OptionOrder).OrderByDescending(a => a.Value).FirstOrDefault();
                model.OptionOrder = maxOrder + 1;
            }
            return View(model);
        }

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RERClassification, "true", "", "", "")]
        public ActionResult ViewRecord(string Id)
        {
            RERClassificationViewModel model = new RERClassificationViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RERClassificationViewModel model)
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

        public void ValidateModel(RERClassificationViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.GlobalOptionSets.Where(a => a.DisplayName == model.DisplayName && a.Type == "RERClassification" && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.GlobalOptionSets.Where(a => a.DisplayName == model.DisplayName && a.Type == "RERClassification").Select(a => a.Id).Any();
                }

                if (duplicated == true)
                {
                    ModelState.AddModelError("DisplayName", Resource.alreadyexists);
                }
            }
        }

        public void SaveRecord(RERClassificationViewModel model)
        {
            if (model != null)
            {
                Regex sWhitespace = new Regex(@"\s+");
                //edit
                if (model.Id != null)
                {
                    GlobalOptionSet globalOptionSet = db.GlobalOptionSets.Where(a => a.Id == model.Id).FirstOrDefault();
                    globalOptionSet.Code = model.Code;
                    globalOptionSet.DisplayName = model.DisplayName;
                    globalOptionSet.OptionOrder = model.OptionOrder;
                    globalOptionSet.Type = "RERClassification";
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
                    globalOptionSet.Code = model.Code;
                    globalOptionSet.DisplayName = model.DisplayName;
                    globalOptionSet.OptionOrder = model.OptionOrder;
                    globalOptionSet.Type = "RERClassification";
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

        [CustomAuthorizeFilter(ProjectEnum.ModuleCode.RERClassification, "", "", "", "true")]
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