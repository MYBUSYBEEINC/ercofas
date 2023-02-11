using System;
using System.ComponentModel.DataAnnotations;

namespace NetStarter.Models
{
    public class HearingType
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}