using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreFiledSurveyFeedback
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string SurveyQuestion { get; set; }
        public string Description { get; set; }
        public int SatisfactionRate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        public string SurveyType { get; set; }
        public string PreFiledSurveyInformationId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}