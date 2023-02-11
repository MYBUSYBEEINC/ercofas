using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class Amendment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string PrefiledCaseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string RERClassificationId { get; set; }
        public string CompanyName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeTelephoneNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public string EmailAddress4 { get; set; }
        public string EmailAddress5 { get; set; }
        public string MobileNumber1 { get; set; }
        public string MobileNumber2 { get; set; }
        public string MobileNumber3 { get; set; }
        public string MobileNumber4 { get; set; }
        public string MobileNumber5 { get; set; }
        public string LiasonOfficer1 { get; set; }
        public string LiasonOfficer2 { get; set; }
        public string PpisStatus { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
    public class AmendmentViewModel
    {
        public string Id { get; set; }
        [Display(Name = "RequestSubject", ResourceType = typeof(Resource))]
        public string PrefiledCaseId { get; set; }
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        public string LastName { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        public string FullName { get; set; }
        [Display(Name = "RERClassification", ResourceType = typeof(Resource))]
        public string RERClassificationId { get; set; }
        [Display(Name = "CompanyName", ResourceType = typeof(Resource))]
        public string CompanyName { get; set; }
        [Display(Name = "OfficeAddress", ResourceType = typeof(Resource))]
        public string OfficeAddress { get; set; }
        [Display(Name = "OfficeTelNumber", ResourceType = typeof(Resource))]
        public string OfficeTelephoneNumber { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress1 { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress2 { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress3 { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress4 { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress5 { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber1 { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber2 { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber3 { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber4 { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber5 { get; set; }
        [Display(Name = "LiasonOfficer", ResourceType = typeof(Resource))]
        public string LiasonOfficer1 { get; set; }
        [Display(Name = "LiasonOfficer", ResourceType = typeof(Resource))]
        public string LiasonOfficer2 { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string PpisStatus { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public List<HttpPostedFileBase> Documents { get; set; }
        public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public List<AmendmentAttachment> Attachments { get; set; }
    }


    public class AmendmentListing
    {
        public List<AmendmentViewModel> Listing { get; set; }
    }
}