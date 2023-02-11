using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreFiledRequirementsPublication
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string NewsPaperName { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string NewsPaperPublication { get; set; }
        public string Application { get; set; }
        public string Verification { get; set; }
        public string CertificationForum { get; set; }
        public string ReviewRate { get; set; }
        public string PreFiledCaseId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}