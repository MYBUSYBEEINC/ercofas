using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreFiledSurveyInformation
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public DateTime SurveyDateTimeIn { get; set; }
        public DateTime SurveyDateTimeOut { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string OfficeVisited { get; set; }
        public string PurposeVisit { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public class PreFiledSurveyInformationModel
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public DateTime SurveyDateTimeIn { get; set; }
        public DateTime SurveyDateTimeOut { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string OfficeVisited { get; set; }
        public string PurposeVisit { get; set; }   
        public IList<PreFiledSurveyServiceDimension> PreFiledSurveyServiceDimensions { get; set; }
    }
}