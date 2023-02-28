using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class SealingAndAcceptance
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string RequestDescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Details { get; set; }
        public string Remarks { get; set; }
        public string NotifyBilling { get; set; }
        public string ReviewStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string AssignedNumber { get; set; }
        public string AssignedPersonnel { get; set; }
        public string AssignedPersonnelStatus { get; set; }
        public string Supervisor { get; set; }
        public string SupervisorStatus { get; set; }
        public string DivisionChief { get; set; }
        public string DivisionChiefStatus { get; set; }
        public string Director { get; set; }
        public string DirectorAssignedNumber { get; set; }
        public string OED { get; set; }
        public string TravelAuthorityStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string Stakeholder { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class SealingAndAcceptanceViewModel
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Stakeholder { get; set; }
        public string RequestDescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Details { get; set; }
        public string Remarks { get; set; }
        public string NotifyBilling { get; set; }
        public string ReviewStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string AssignedNumber { get; set; }
        public string AssignedPersonnel { get; set; }
        public string AssignedPersonnelStatus { get; set; }
        public string Supervisor { get; set; }
        public string SupervisorStatus { get; set; }
        public string DivisionChief { get; set; }
        public string DivisionChiefStatus { get; set; }
        public string Director { get; set; }
        public string DirectorAssignedNumber { get; set; }
        public string OED { get; set; }
        public string TravelAuthorityStatus { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}