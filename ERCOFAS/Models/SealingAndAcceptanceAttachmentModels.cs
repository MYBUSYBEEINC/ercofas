using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class SealingAndAcceptanceAttachment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string SealingAndAcceptanceId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public string FileTypeId { get; set; }
        public string FileTypeName { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}