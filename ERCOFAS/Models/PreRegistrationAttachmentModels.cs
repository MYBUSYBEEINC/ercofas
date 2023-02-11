using NetStarter.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NetStarter.Models
{
    public class PreRegistrationAttachment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string PreRegistrationId { get; set; }
        public string DocumentName { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}