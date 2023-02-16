using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ERCOFAS.Controllers.GeneralController;

namespace ERCOFAS.Models
{
    public class PreFiledCaseLogs
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string PreFiledCaseId { get; set; }
        public string Remarks { get; set; }
    }
}
