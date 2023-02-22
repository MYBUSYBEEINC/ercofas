using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERCOFAS.Models
{
    public class SurveyQuestion
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }       
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class SurveyQuestionModel
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Rate { get; set; }
        public int RateVerySatisfied { get; set; }
        public int RateSatisfied { get; set; }
        public int RateNeitherSatisfied { get; set; }
        public int RateDissatisfied { get; set; }
        public int RateVeryDissatisfied { get; set; }
    }
}