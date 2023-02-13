using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreFiledSurveyServiceDimension
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string ServiceDimensionQuality { get; set; }
        public string Attribute { get; set; }
        public int SatisfactionRate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        public string PreFiledSurveyInformationId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}