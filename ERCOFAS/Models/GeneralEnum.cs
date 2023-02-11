using System.ComponentModel;

namespace NetStarter.Models
{
    //project default enum

    public class GeneralEnum
    {
        public enum Type
        {
            [Description("UPDATE")]
            Update,
            [Description("CREATE")]
            Create,
            [Description("LOGIN")]
            Login,
            [Description("LOGOFF")]
            Logoff,
            [Description("DELETE")]
            Delete,
            [Description("VIEW")]
            View
        }
        public enum ModuleId
        {
            [Description("B4E801DA-B661-4923-B74C-42E38DD1DF68")]
            Dashboard,
            [Description("595F9ECE-F081-46DF-A7BF-412DB5FEA348")]
            PreFiledCase,
            [Description("6720E14A-0640-4F36-9EBC-E21D1319B4DA")]
            FiledCase,
            [Description("0645D42C-F244-48CB-A11B-AE15C3A1E534")]
            Hearing,
            [Description("CA768487-A357-4897-B631-9C01C7B26CCB")]
            InitiatoryPleading,
            [Description("F112215D-7C3C-4DF5-BC5B-0415E8DA051C")]
            PleadingWithCaseNumber,
            [Description("5E1EB5A9-8F8E-4D81-992E-7C3FEBC0C2D6")]
            OtherLetterCorrespondense,
            [Description("1D659134-25C3-4FE0-B1C7-DB3092C3E811")]
            DisputeResolution,
            [Description("A343BD5F-68D9-4720-9694-9E89C26E2D37")]
            Transaction,
            [Description("1E4DF1B2-839A-43BA-909D-DE3ED9A35604")]
            Payment,
            [Description("10A4FED3-D179-4E09-85A1-AEFDBAD46B89")]
            UserStatus,
            [Description("D44BA4BA-A2AC-404B-87B0-8B2A8BB29246")]
            UserType,
            [Description("A0980D3A-E253-4995-A395-7DCC2CA99997")]
            CaseType,
            [Description("70D5F9B4-4359-441A-87B6-4B5FFFED9D44")]
            CaseNature,
            [Description("ED9A9D57-7917-4BEB-AAD2-9446A64532FF")]
            UserAttachmentType,
            [Description("3113A195-9260-44FF-9138-1AB5C64983B4")]
            RoleManagement,
            [Description("96DEF15B-7534-4485-84AD-476D97A14825")]
            UserManagement,
            [Description("1767BCEE-AE05-448D-8348-6EACAC4463DD")]
            LoginHistory,
            [Description("G221315E-5T1V-4DF8-EE0D-0415G8HJ613B")]
            Setting,
            [Description("2565DEHH-DH12-771R-6G13-6HSVAE652F33")]
            AuditLog,
        }

        public enum ActionTakenView
        {
            [Description("Dashboard")]
            Dashboard,
            [Description("Prefiled case")]
            PreFiledCase,
            [Description("Filed case")]
            FiledCase,
            [Description("Hearing")]
            Hearing,
            [Description("Initiatory pleading")]
            InitiatoryPleading,
            [Description("Pleading with case number")]
            PleadingWithCaseNumber,
            [Description("Other letter correspondense")]
            OtherLetterCorrespondense,
            [Description("Dispute resolution")]
            DisputeResolution,
            [Description("Transaction")]
            Transaction,
            [Description("Payment")]
            Payment,
            [Description("User status")]
            UserStatus,
            [Description("User type")]
            UserType,
            [Description("Case type")]
            CaseType,
            [Description("Case nature")]
            CaseNature,
            [Description("User attachment type")]
            UserAttachmentType,
            [Description("Role management")]
            RoleManagement,
            [Description("User management")]
            UserManagement,
            [Description("Login history")]
            LoginHistory,
            [Description("Setting")]
            Setting,
            [Description("Audit logs")]
            AuditLog,
        }
        public enum GeneralActionTaken
        {
            [Description("Logged in.")]
            Login,
            [Description("Logged out.")]
            Logoff,
            [Description(" is been deleted.")]
            Deleted,
            [Description("User type {value} is been created")]
            NewUserTypeCreated,
            [Description("User status {value} is been created")]
            NewUserStatusCreated,
            [Description("Case type {value} is been created")]
            NewCaseTypeCreated,
            [Description("Case nature {value} is been created")]
            NewCaseNatureCreated,
            [Description("Role {value} is been created")]
            NewRoleCreated,
            [Description("User {value} is been created")]
            NewUserCreated,
            [Description("Prefile case {value} is been created")]
            NewPrefiledCaseCreated,
            [Description("File case {value} is been created")]
            NewFiledCaseCreated,
            [Description("Hearing  {value} is been created")]
            NewHearingCreated,
            [Description("Initiatory pleading {value} is been created")]
            NewInitiatoryPleadingCreated,
            [Description("Pleading with case number {value} is been created")]
            NewPleadingWithCaseNumberCreated,
        }

    }

}