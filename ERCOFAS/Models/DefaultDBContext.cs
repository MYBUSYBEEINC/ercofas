using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace NetStarter.Models
{
    public class DefaultDBContext : DbContext
    {
        public DefaultDBContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        //used by asp.net identity
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }

        //other tables
        public DbSet<GlobalOptionSet> GlobalOptionSets { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModulePermission> RoleModulePermissions { get; set; }
        public DbSet<UserAttachment> UserAttachments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<AspNetUserTypes> AspNetUserTypes { get; set; }
        public DbSet<PreFiledCases> PreFiledCases { get; set; }
        public DbSet<PreRegistration> PreRegistration { get; set; }
        public DbSet<Hearing> Hearings { get; set; }
        public DbSet<HearingType> HearingTypes { get; set; }
        public DbSet<HearingDocuments> HearingDocuments { get; set; }
        public DbSet<HearingPersonnel> HearingPersonnels { get; set; }
        public DbSet<InitiatoryPleading> InitiatoryPleadings { get; set; }
        public DbSet<InitiatoryPleadingAttachment> InitiatoryPleadingAttachments { get; set; }
        public DbSet<PreFiledAttachment> PreFiledAttachments { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Requirements> Requirements { get; set; }
        public DbSet<RequiredDocuments> RequiredDocuments { get; set; }
        public DbSet<PreRegistrationEmails> PreRegistrationEmails { get; set; }
        public DbSet<PreRegistrationMobiles> PreRegistrationMobiles { get; set; }
        public DbSet<TransactionAttachment> TransactionAttachments { get; set; }
        public DbSet<Amendment> Amendment { get; set; }
        public DbSet<AmendmentAttachment> AmendmentAttachments { get; set; }
        public DbSet<PreRegistrationAttachment> PreRegistrationAttachments { get; set; }
        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<PreFiledCaseRequirements> PreFiledRequirementsCases { get; set; }
        public DbSet<PreFiledRequirementsCertification> PreFiledRequirementsCertifications { get; set; }
        public DbSet<PreFiledRequirementsPublication> PreFiledRequirementsPublications { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}