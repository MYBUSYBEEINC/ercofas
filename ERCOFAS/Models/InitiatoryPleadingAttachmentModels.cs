using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class InitiatoryPleadingAttachment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string InitiatoryPleadingId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}