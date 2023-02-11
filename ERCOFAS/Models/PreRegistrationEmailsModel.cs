using System;
using System.ComponentModel.DataAnnotations;

namespace NetStarter.Models
{
    public class PreRegistrationEmails
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string PreRegistrationId { get; set; }
        public string EmailAddress { get; set; }
        public int? Order { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
    }
}