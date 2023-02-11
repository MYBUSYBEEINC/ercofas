using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERCOFAS.Resources;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERCOFAS.Models
{
    public class AspNetUserTypes
    {
        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        [MaxLength(128)]
        public string UserTypeId { get; set; }
    }
}