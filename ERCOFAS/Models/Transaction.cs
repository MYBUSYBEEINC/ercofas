using NetStarter.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace NetStarter.Models
{
    public class Transaction
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string PrefiledCaseId { get; set; }
        public string RequestSubject { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string CaseTypeId { get; set; }
        public string CaseNatureId { get; set; }
        public string ReferenceNo { get; set; }
        public string TransactionDate { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public string PaidThru { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }
    public class TransactionViewModel
    {
        public string Id { get; set; }
        [Display(Name = "PreFiledCases", ResourceType = typeof(Resource))]
        public string PrefiledCaseId { get; set; }
        [Display(Name = "RequestSubject", ResourceType = typeof(Resource))]
        public string RequestSubject { get; set; }
        public string UserId { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        public string FullName { get; set; }
        [Display(Name = "CaseType", ResourceType = typeof(Resource))]
        public string CaseTypeId { get; set; }
        [Display(Name = "CaseNature", ResourceType = typeof(Resource))]
        public string CaseNatureId { get; set; }
        [Display(Name = "ReferenceNo", ResourceType = typeof(Resource))]
        public string ReferenceNo { get; set; }
        [Display(Name = "PaymentDate", ResourceType = typeof(Resource))]
        public string TransactionDate { get; set; }
        [Display(Name = "Time", ResourceType = typeof(Resource))]
        public string Time { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string Status { get; set; }
        [Display(Name = "Details", ResourceType = typeof(Resource))]
        public string Details { get; set; }
        [Display(Name = "PaidThru", ResourceType = typeof(Resource))]
        public string PaidThru { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }

        public List<SelectListItem> CaseTypeSelectList { get; set; }
        public List<SelectListItem> CaseNatureSelectList { get; set; }
        public List<SelectListItem> TransactionStatus { get; set; }
        public List<SelectListItem> PaymentMethod { get; set; }
        public List<HttpPostedFileBase> Documents { get; set; }
        public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public List<TransactionAttachment> Attachments { get; set; }
    }

    public class TransactionListing
    {
        public List<TransactionViewModel> Listing { get; set; }
    }
}