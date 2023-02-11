using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ERCOFAS.Controllers.GeneralController;

namespace ERCOFAS.Models
{
    public class AuditLogs
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public DateTime? DateTime { get; set; }
        public string UserId { get; set; }
        public string ModuleId { get; set; }
        public string ActionTaken { get; set; }
        public string IpAddress { get; set; }
    }
    public class AuditLogsViewModel
    {
        public string Id { get; set; }
        [Display(Name = "DateTime", ResourceType = typeof(Resource))]
        public DateTime? DateTime { get; set; }
        [Display(Name = "User", ResourceType = typeof(Resource))]
        public string UserId { get; set; }
        [Display(Name = "ModuleName", ResourceType = typeof(Resource))]
        public string ModuleId { get; set; }
        [Display(Name = "ActionTaken", ResourceType = typeof(Resource))]
        public string ActionTaken { get; set; }
        [Display(Name = "IpAddress", ResourceType = typeof(Resource))]
        public string IpAddress { get; set; }
    }

    public class AuditChange
    {
        public List<AuditDelta> Changes { get; set; }
        public AuditChange()
        {
            Changes = new List<AuditDelta>();
        }
    }


    public class AuditLogsListing
    {
        public List<AuditLogsViewModel> Listing { get; set; }
    }
}