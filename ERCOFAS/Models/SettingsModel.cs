using NetStarter.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetStarter.Models
{
    public class Settings
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string AboutUs { get; set; }
        public string Website { get; set; }
        public string ERNumberPrefix { get; set; }
        public string SMTPFromEmail { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPServerName { get; set; }
        public int SMTPPort { get; set; }
        public bool SMTPEnableSSL { get; set; }
        public string OTPBaseUrl { get; set; }
        public string OTPSenderId { get; set; }
        public string OTPAPIKey { get; set; }
        public string OTPClientId { get; set; }
        public string OTPUsername { get; set; }
        public string OTPPassword { get; set; }
        public int MinPasswordLength { get; set; }
        public int MinSpecialCharacters { get; set; }
        public int MaxSignOnAttempts { get; set; }
        public string EnforcePasswordHistoryId { get; set; }
        public string BaseUrl { get; set; }
        public int? ActivationLinkExpiresIn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeactivateUserAfter { get; set; }
    }
    public class SettingsModel
    {
        public string Id { get; set; }
        [Display(Name = "CompanyName", ResourceType = typeof(Resource))]
        public string CompanyName { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(Resource))]
        public string EmailAddress { get; set; }
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; }
        [Display(Name = "MobileNumber", ResourceType = typeof(Resource))]
        public string MobileNumber { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "AboutUs", ResourceType = typeof(Resource))]
        public string AboutUs { get; set; }
        [Display(Name = "Website", ResourceType = typeof(Resource))]
        public string Website { get; set; }
        [Display(Name = "ERNumberPrefix", ResourceType = typeof(Resource))]
        public string ERNumberPrefix { get; set; }
        [Display(Name = "SMTPFromEmail", ResourceType = typeof(Resource))]
        public string SMTPFromEmail { get; set; }
        [Display(Name = "SMTPPassword", ResourceType = typeof(Resource))]
        public string SMTPPassword { get; set; }
        [Display(Name = "SMTPServerName", ResourceType = typeof(Resource))]
        public string SMTPServerName { get; set; }
        [Display(Name = "SMTPPort", ResourceType = typeof(Resource))]
        public int SMTPPort { get; set; }
        [Display(Name = "SMTPEnableSSL", ResourceType = typeof(Resource))]
        public bool SMTPEnableSSL { get; set; }
        [Display(Name = "OTPBaseUrl", ResourceType = typeof(Resource))]
        public string OTPBaseUrl { get; set; }
        [Display(Name = "OTPSenderId", ResourceType = typeof(Resource))]
        public string OTPSenderId { get; set; }
        [Display(Name = "OTPAPIKey", ResourceType = typeof(Resource))]
        public string OTPAPIKey { get; set; }
        [Display(Name = "OTPClientId", ResourceType = typeof(Resource))]
        public string OTPClientId { get; set; }
        [Display(Name = "OTPUsername", ResourceType = typeof(Resource))]
        public string OTPUsername { get; set; }
        [Display(Name = "OTPPassword", ResourceType = typeof(Resource))]
        public string OTPPassword { get; set; }
        [Display(Name = "MinPasswordLength", ResourceType = typeof(Resource))]
        public int MinPasswordLength { get; set; }
        [Display(Name = "MinSpecialCharacters", ResourceType = typeof(Resource))]
        public int MinSpecialCharacters { get; set; }
        [Display(Name = "MaxSignOnAttempts", ResourceType = typeof(Resource))]
        public int MaxSignOnAttempts { get; set; }
        [Display(Name = "ActivationLinkExpiresIn", ResourceType = typeof(Resource))]
        public int? ActivationLinkExpiresIn { get; set; }
        [Display(Name = "EnforcePasswordHistory", ResourceType = typeof(Resource))]
        public string EnforcePasswordHistoryId { get; set; }
        public string EnforcePasswordHistoryDescription { get; set; }
        public List<SelectListItem> EnforcePasswordHistorySelectList { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public int DeactivateUserAfter { get; set; }
    }


    public class SettingsListing
    {
        public List<SettingsModel> Listing { get; set; }
    }
}