using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class HearingDocuments
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int? DocumentType { get; set; }

        public string HearingId { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    public class HearingDocumentsViewModel
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int? DocumentType { get; set; }

        public string HearingId { get; set; }

        public bool SystemDefault { get; set; }

        public List<SelectListItem> PersonnelSelectList { get; set; }

        public HttpPostedFileBase[] Files { get; set; }

        public HttpPostedFileBase[] HearingFiles { get; set; }

        public string Personnel { get; set; }
    }
}