using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace NetStarter.Models
{
    public class Hearing
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        public string Description { get; set; }

        public string HearingType { get; set; }

        public DateTime? Schedule { get; set; }

        public string MeetingLink { get; set; }

        public int? HearingStatus { get; set; }

        public string MeetingPassword { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }

    public class HearingViewModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string HearingType { get; set; }
        public string HearingTypeName { get; set; }
        public DateTime? Schedule { get; set; }
        public string HearingSchedule { get; set; }
        public string MeetingLink { get; set; }

        //[Display(Name = "HearingStatus", ResourceType = typeof(Hearing))]
        public int? HearingStatus { get; set; }
        public string HearingStatusName { get; set; }

        //[Display(Name = "MeetingPassword", ResourceType = typeof(Hearing))]
        public string MeetingPassword { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public List<SelectListItem> HearingTypeSelectList { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public HttpPostedFileBase[] HearingFiles { get; set; }
        public string UserRoleName { get; set; }
    }

    public class HearingListing
    {
        public List<HearingViewModel> Listing { get; set; }
    }
}