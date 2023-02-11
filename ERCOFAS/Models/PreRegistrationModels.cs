using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class PreRegistration
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string JuridicalEntityName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeTelephone { get; set; }
        public string LiaisonOfficer1 { get; set; }
        public string LiaisonOfficer2 { get; set; }
        public string RERTypeId { get; set; }
        public string RERClassificationId { get; set; }
        public string TempUsername { get; set; }
        public string TempPassword { get; set; }
        public string RegistrationStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class PreRegistrationViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string Name { get; set; }
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        public string LastName { get; set; }
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }
        [Display(Name = "MiddleName", ResourceType = typeof(Resource))]
        public string MiddleName { get; set; }
        [Display(Name = "RERType", ResourceType = typeof(Resource))]
        public string RERTypeId { get; set; }
        [Display(Name = "RERClassification", ResourceType = typeof(Resource))]
        public string RERClassificationId { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress { get; set; }
        public string CountryCode { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber { get; set; }
        [Display(Name = "JuridicalEntityName", ResourceType = typeof(Resource))]
        public string JuridicalEntityName { get; set; }
        [Display(Name = "OfficeAddress", ResourceType = typeof(Resource))]
        public string OfficeAddress { get; set; }
        [Display(Name = "OfficeTelephone", ResourceType = typeof(Resource))]
        public string OfficeTelephone { get; set; }
        [Display(Name = "EmailAddress1", ResourceType = typeof(Resource))]
        public string EmailAddress1 { get; set; }
        [Display(Name = "EmailAddress2", ResourceType = typeof(Resource))]
        public string EmailAddress2 { get; set; }
        [Display(Name = "EmailAddress3", ResourceType = typeof(Resource))]
        public string EmailAddress3 { get; set; }
        [Display(Name = "EmailAddress4", ResourceType = typeof(Resource))]
        public string EmailAddress4 { get; set; }
        [Display(Name = "EmailAddress5", ResourceType = typeof(Resource))]
        public string EmailAddress5 { get; set; }
        [Display(Name = "MobileNumber1", ResourceType = typeof(Resource))]
        public string MobileNumber1 { get; set; }
        [Display(Name = "MobileNumber2", ResourceType = typeof(Resource))]
        public string MobileNumber2 { get; set; }
        [Display(Name = "LiaisonOfficer1", ResourceType = typeof(Resource))]
        public string LiaisonOfficer1 { get; set; }
        [Display(Name = "LiaisonOfficer2", ResourceType = typeof(Resource))]
        public string LiaisonOfficer2 { get; set; }
        [Display(Name = "RegistrationStatus", ResourceType = typeof(Resource))]
        public string RegistrationStatusId { get; set; }
        public string TempUsername { get; set; }
        public string TempPassword { get; set; }
        [Display(Name = "RegistrationDate", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public List<SelectListItem> RERTypeList { get; set; }
        public List<SelectListItem> RERClassificationList { get; set; }
        public List<SelectListItem> RegistrationStatusList { get; set; }
        public List<HttpPostedFileBase> Documents { get; set; }
        public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public List<PreRegistrationAttachment> Attachments { get; set; }
        public List<PreRegistrationEmails> Emails { get; set; }
        public List<PreRegistrationMobiles> MobileNumbers { get; set; }
        public List<GlobalOptionSet> RERClassifications { get; set; }
        public List<RequiredDocumentsViewModel> RequiredDocuments { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public bool IsCompleted { get; set; }
        public string Remarks { get; set; }
    }

    public class PreRegistrationListing
    {
        public List<PreRegistrationViewModel> Listing { get; set; }
    }
}