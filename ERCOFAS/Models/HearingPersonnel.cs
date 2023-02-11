using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class HearingPersonnel
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        public string HearingId { get; set; }

        public string UserId { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    public class HearingPersonnelViewModel
    {
        public string Id { get; set; }

        public string HearingId { get; set; }

        public string StakeHolderUserId { get; set; }

        public string ServiceUserId { get; set; }

        public bool SystemDefault { get; set; }

        public List<SelectListItem> StakeHolderSelectList { get; set; }

        public List<SelectListItem> ServiceSelectList { get; set; }

        public string Name1 { get; set; }

        public string Name2 { get; set; }

        public string Name3 { get; set; }

        public string Name4 { get; set; }

        public string Name5 { get; set; }

        public string EmailAddress1 { get; set; }

        public string EmailAddress2 { get; set; }

        public string EmailAddress3 { get; set; }

        public string EmailAddress4 { get; set; }

        public string EmailAddress5 { get; set; }
    }
}