using System;
using System.ComponentModel.DataAnnotations;

namespace NetStarter.Models
{
    public class PreFiledRequirementsCertification
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string LguName { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public int CertificationType { get; set; }
        public int FileType { get; set; }
        public string Reason { get; set; }
        public string PreFiledCaseId { get; set; }
        public string LguId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}