using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class RequiredDocuments
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        [MaxLength(128)]
        public string RequirementsId { get; set; }
        [MaxLength(128)]
        public string RERType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class RequiredDocumentsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RERType { get; set; }
    }
}