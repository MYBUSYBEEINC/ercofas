﻿using ERCOFAS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class GlobalOptionSet
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public int? OptionOrder { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool SystemDefault { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }
    }

    public class GlobalOptionSetViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [Required]
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$" , ErrorMessageResourceName = "OnlyLettersAndNumbers", ErrorMessageResourceType = typeof(Resource))]
        public string DisplayName { get; set; }
        public string ReferenceId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        [Required]
        [Display(Name = "DisplayOrder", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceName = "OnlyNumbers", ErrorMessageResourceType = typeof(Resource))]
        public int? OptionOrder { get; set; }
        public List<SelectListItem> ReferenceSelectList { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public bool SystemDefault { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }

    }

    public class RERClassificationViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Code", ResourceType = typeof(Resource))]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string DisplayName { get; set; }
        public string ReferenceId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int? OptionOrder { get; set; }
        public List<SelectListItem> ReferenceSelectList { get; set; }
        public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
        public bool SystemDefault { get; set; }
        public string IsoUtcCreatedOn { get; set; }
        public string IsoUtcModifiedOn { get; set; }

        [MaxLength(128)]
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public DateTime? CreatedOn { get; set; }
        [MaxLength(128)]
        [Display(Name = "ModifiedBy", ResourceType = typeof(Resource))]
        public string ModifiedBy { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public DateTime? ModifiedOn { get; set; }

    }

    public class RERClassificationListing
    {
        public List<RERClassificationViewModel> Listing { get; set; }
    }

public class UserStatusListing
    {
        public List<GlobalOptionSetViewModel> Listing { get; set; }
    }

    public class UserTypeListing
    {
        public List<GlobalOptionSetViewModel> Listing { get; set; }
    }

    public class CaseTypeListing
    {
        public List<GlobalOptionSetViewModel> Listing { get; set; }
    }

    public class CaseNatureListing
    {
        public List<GlobalOptionSetViewModel> Listing { get; set; }
    }

    public enum UserStatus
    {
        Registered,
        Validated,
        NotValidated,
        Banned
    }

    public class UserAttachmentTypeListing
    {
        public List<GlobalOptionSetViewModel> Listing { get; set; }
    }
}