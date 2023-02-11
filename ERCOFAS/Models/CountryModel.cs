using System;
using System.ComponentModel.DataAnnotations;

namespace ERCOFAS.Models
{
    public class Country
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}