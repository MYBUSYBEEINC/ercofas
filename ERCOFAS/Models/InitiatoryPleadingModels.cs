using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class InitiatoryPleading
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public string DocketNumber { get; set; }
        public string CaseTitle { get; set; }
        public string ApplicantName { get; set; }
        public string PreFiledCaseId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class InitiatoryPleadingModel
    {
        public string Id { get; set; }
        [Display(Name = "DocumentName", ResourceType = typeof(Resource))]
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        //[FileExtensions(Extensions = ".pdf,.xlsx,.xls,.csv,.png,.jpg", ErrorMessage = "Incorrect file format")]
        public List<HttpPostedFileBase> Documents { get; set; }
        public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public List<InitiatoryPleadingAttachment> Attachments { get; set; }
        public IList<PreFiledAttachmentViewModel> PreFiledAttachmentViewModels { get; set; }
        public string DocketNumber { get; set; }
        public string DocketNumberYear { get; set; }
        public string DocketNumberSequence { get; set; }
        public string DocketNumberCaseType { get; set; }
        public string PreFiledCaseId { get; set; }
        public string CaseType { get; set; }
        public string CaseNature { get; set; }
        public string CaseTitle { get; set; }
        public string ApplicantName { get; set; }
    }

    public class InitiatoryPleadingListing
    {
        public List<InitiatoryPleadingModel> Listing { get; set; }
    }
}