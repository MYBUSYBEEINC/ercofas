using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ERCOFAS.Models
{
    public class PreFiledCaseRequirements
    {
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        public DateTime? PreFilingDate { get; set; }
        public string ApplicantName { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public int HardCopyNumber { get; set; }
        public int SoftCopyNumber { get; set; }
        public DateTime? VerificationDate { get; set; }
        public bool VerificationNotarized { get; set; }
        public string VerificationPresentedId { get; set; }
        public DateTime? CertificationForumDate { get; set; }
        public bool CertificationForumNotarized { get; set; }
        public string CertificationForumPresentedId { get; set; }
        public bool CounselBoardResolution { get; set; }
        public bool CounselSecretaryCertification { get; set; }
        public bool AffiantBoardResolution { get; set; }
        public bool AffiantSecretaryCertification { get; set; }
        public string PreFiledCaseId { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class PreFiledCaseRequirementsViewModel
    {
        public DateTime? PreFilingDate { get; set; }
        public string ApplicantName { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string Application { get; set; }
        public int HardCopyNumber { get; set; }
        public int SoftCopyNumber { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Verification { get; set; }
        public bool VerificationNotarized { get; set; }
        public string VerificationIdPresented { get; set; }
        public DateTime? CertificationForumDate { get; set; }
        public string CertificationForum { get; set; }
        public bool CertificationForumNotarized { get; set; }
        public string CertificationForumIdPresented { get; set; }
        public bool CounselBoardResolution { get; set; }
        public bool CounselSecretaryCertification { get; set; }
        public bool AffiantBoardResolution { get; set; }
        public bool AffiantSecretaryCertification { get; set; }
        public string PreFileCaseId { get; set; }
        //public string Remarks { get; set; }
        public List<SelectListItem> CertificationLguList { get; set; }
        public List<SelectListItem> AbsenceCertificationLguList { get; set; }
        public List<SelectListItem> ReasonCertificationLguList { get; set; }
        public int CertificationType { get; set; }
        public int FileType { get; set; }
        //public List<HttpPostedFileBase> Documents { get; set; }
        //public string DocumentFileName { get; set; }
        public HttpPostedFileBase[] AffidavitServiceFiles { get; set; }
        public HttpPostedFileBase[] ApplicationStampFiles { get; set; }
        public HttpPostedFileBase[] ProofRefusalFiles { get; set; }

        [DataType(DataType.MultilineText)]
        public string ReasonCertificationRemarks { get; set; }
        public HttpPostedFileBase[] AffidavitPublicationFiles { get; set; }
        public string NewsPaperName { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string NewsPaperIssuePublication { get; set; }
        public string ReviewRate { get; set; }

        [DataType(DataType.MultilineText)]
        public string OtherRemarks { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public bool SystemDefault { get; set; }
        //public CreatedAndModifiedViewModel CreatedAndModified { get; set; }
    }
}