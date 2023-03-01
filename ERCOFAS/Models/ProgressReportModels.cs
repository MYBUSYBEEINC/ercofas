using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ERCOFAS.Models
{
    public class ProgressReport
    {
        [Key]
        public long Id { get; set; }
        public string Description { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime DateSubmitted { get; set; }
    }

    public class ProgressReportViewModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime DateSubmitted { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
    }
}