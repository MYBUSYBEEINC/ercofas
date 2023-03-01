using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class ProgressReportAttachment
    {
        [Key]
        public long Id { get; set; }
        public long ProgressReportId { get; set; }
        public string DocumentName { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}