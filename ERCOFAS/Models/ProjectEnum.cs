using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NetStarter.Models
{
    //project default enum

    public class ProjectEnum
    {

        public enum ModuleCode
        {
            Dashboard,
            Registration,
            StakeholderRegistration,
            PreFiledCase,
            FiledCase,
            Hearing,
            InitiatoryPleading,
            PleadingWithCaseNumber,
            OtherLetterCorrespondense,
            DisputeResolution,
            Transaction,
            Payment,
            UserStatus,
            UserType,
            CaseType,
            CaseNature,
            UserAttachmentType,
            RoleManagement,
            UserManagement,
            LoginHistory,
            Setting,
            PreRegistration,
            PreFiledRequirements,
            Amendment,
            Security,
            RERClassification
        }

        public enum UserAttachment
        {
            ProfilePicture
        }

        public enum PreFileAttachment
        {            
            ApplicationForm,
            NotaryPresentedId,
            CertificationForum,
            AuthorityCounsel,
            AuthorityAffiant,
            ServiceLgu,
            Publication,
            ExemptionCompetitive,
            Competitive,
            OtherDocument
        }

        public enum Gender
        {
            Female,
            Male,
            Other
        }

        public enum UserStatus
        {
            Registered,
            Validated,
            NotValidated,
            Banned
        }

        public enum EmailTemplate
        {
            ConfirmEmail,
            PasswordResetByAdmin
        }    
        public enum HearingStatus
        {
            Unknown,
            Approved,
            Cancelled,
            Pending,
            Scheduled,
            ForSigning,
            Signed,
            Confirmed
        }

    }

    public static class Constant
    {
        public const string InitialReview_Status_Pending = "Pending";

        public const string InitialReview_Status_Approved = "Approved";

        public const string InitialReview_Status_Rejected = "Rejected";

        public const string PreFileAttachment_Status_Pending = "Pending";

        public const string PreFileAttachment_Status_Approved = "Approved";

        public const string PreFileAttachment_Status_Rejected = "Rejected";

        public const string DocumentUpload_Status_Pending = "Pending";

        public const string DocumentUpload_Status_Approved = "Approved";

        public const string DocumentUpload_Status_Rejected = "Rejected";

        public const string Payment_Status_Pending = "Pending";

        public const string Payment_Status_Approved = "Approved";

        public const string Payment_Status_Rejected = "Rejected";

        public const string Field_Required = "Required";

    }
}