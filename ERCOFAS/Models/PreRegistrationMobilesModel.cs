using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreRegistrationMobiles
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public long PreRegistrationId { get; set; }
        public string CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string OneTimePassword { get; set; }
        public int? Order { get; set; }
        public bool IsVerified { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
    }
}