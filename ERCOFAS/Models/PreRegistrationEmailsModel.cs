using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class PreRegistrationEmails
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public long PreRegistrationId { get; set; }
        public string EmailAddress { get; set; }
        public int? Order { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
    }
}