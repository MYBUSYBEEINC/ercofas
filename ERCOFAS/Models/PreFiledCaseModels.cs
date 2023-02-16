using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class PreFiledCases
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string RequestSubject { get; set; }
        public string UserId { get; set; }
        public string CaseTypeId { get; set; }
        public string CaseNatureId { get; set; }
        public string FileCaseStatusId { get; set; }
        public string Remarks { get; set; }
        public string ApprovalRemarks { get; set; }
        public string InitialReviewStatus { get; set; }
        public string DocumentsUploadStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string Office { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class PreFiledCaseViewModel
    {
        public string Id { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "RequestSubject", ResourceType = typeof(Resource))]
        public string RequestSubject { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Username", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "CaseType", ResourceType = typeof(Resource))]
        public string CaseTypeId { get; set; }
        public string CaseTypeName { get; set; }


        [Display(Name = "CaseNature", ResourceType = typeof(Resource))]
        public string CaseNatureId { get; set; }
        public string CaseNatureName { get; set; }
        [Display(Name = "FileCaseStatus", ResourceType = typeof(Resource))]
        public string FileCaseStatusId { get; set; }
        public string InitialReviewStatus { get; set; }
        public string DocumentsUploadStatus { get; set; }
        public string PaymentStatus { get; set; }
        public bool InitialReviewRequired { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "ApproverRemarks", ResourceType = typeof(Resource))]
        public string ApprovalRemarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public List<SelectListItem> CaseTypeSelectList { get; set; }
        public List<SelectListItem> CaseNatureSelectList { get; set; }
        public List<SelectListItem> FileCaseStatusSelectList { get; set; }
        public List<HttpPostedFileBase> Documents { get; set; }
        public List<HttpPostedFileBase> ApplicationForm { get; set; }
        public List<HttpPostedFileBase> NotaryPresentedId { get; set; }
        public List<HttpPostedFileBase> CertificationForum { get; set; }
        public List<HttpPostedFileBase> AuthorityCounsel { get; set; }
        public List<HttpPostedFileBase> AuthorityAffiant { get; set; }
        public List<HttpPostedFileBase> ServiceLocalGovernmentUnit { get; set; }
        public List<HttpPostedFileBase> Publication { get; set; }
        public List<HttpPostedFileBase> ExemptionCompetitive { get; set; }
        public List<HttpPostedFileBase> Competitive { get; set; }            
        public int InitialRequirementComplete { get; set; }
        public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public List<PreFiledAttachment> Attachments { get; set; }
        public IList<PreFiledAttachmentViewModel> PreFiledAttachmentViewModels { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        public string FullName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; }
        [Display(Name = "UserType", ResourceType = typeof(Resource))]
        public string UserType { get; set; }
        public List<PreRegistrationEmails> Emails { get; set; }
        public List<PreRegistrationMobiles> MobileNumbers { get; set; }
        public string Office { get; set; }
        public IList<string> OfficeList { get; set; }
        public bool PreFilingCaseComplete { get; set; }
        public PreFiledSurveyInformationModel PreFiledSurveyInformation { get; set; }
        public List<PreFiledCaseLogs> CaseLogs { get; set; }
        public List<PreFiledCaseRemarkFileLogs> PreFiledCaseRemarkFileLogs { get; set; }

    }

    public class PreFiledCaseListing
    {
        public List<PreFiledCaseViewModel> Listing { get; set; }
    }
}
