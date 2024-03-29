﻿using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class PreRegistrationAttachment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public long PreRegistrationId { get; set; }
        public string DocumentName { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}