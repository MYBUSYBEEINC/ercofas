﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class TransactionAttachment
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}