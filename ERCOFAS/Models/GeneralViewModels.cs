using ERCOFAS.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class CreatedAndModifiedViewModel
    {
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedByName { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string FormattedCreatedOn { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedByName { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string FormattedModifiedOn { get; set; }
        public int? OrderCreatedOn { get; set; }
        public int? OrderModifiedOn { get; set; }
    }

    public class ImportFromExcelError
    {
        public string Row { get; set; }
        public List<string> Errors { get; set; }
    }

    public class UserInRoleListing
    {
        public List<UserInRoleViewModel> Listing { get; set; }
        public string RoleName { get; set; }
    }

    public class UserInRoleViewModel
    {
        public string Username { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        public string FullName { get; set; }
        public string UserProfileId { get; set; }
    }

    public class TotalCountViewModel
    {
        public int TotalRegistrations { get; set; }
        public int TotalAmendments { get; set; }
        public int TotalPreFilings { get; set; }
        public int TotalLetterCorrespondents { get; set; }
        public int TotalInitiatorySubsequentPleadings { get; set; }
        public int TotalValidatioMeterSamplings { get; set; }
        public int TotalSealingRequestTestingMeters { get; set; }
        public int TotalVirtualHearings { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalRetailElectricitySuppliers { get; set; }
        public int TotalInServiceMeterSamplings { get; set; }
        public int TotalNotificationTypes { get; set; }
        public int TotalNotificationTemplates { get; set; }
        public int TotalCaseTypes { get; set; }
        public int TotalCaseNatures { get; set; }
        public int TotalRoles { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAuditLogs { get; set; }
        public int TotalUserStatus { get; set; }
        public int TotalUserTypes { get; set; }
        public int TotalRERClassification { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRole { get; set; }
        public string TopStatus { get; set; }
        public string TopStatusId { get; set; }
        public string TopRole { get; set; }
        public string TopRoleId { get; set; }
        public List<Chart> UserByStatus { get; set; }
        public TotalCountViewModel TotalCountModel { get; set; }
    }

    public class Chart
    {
        public string DataLabel { get; set; }
        public int DataValue { get; set; }
    }
}