namespace ERCOFAS.Enumeration
{
    public class UserTypeEnum
    {
        public string Value { get; private set; }

        private UserTypeEnum(string value) 
        { 
            Value = value; 
        }

        public static UserTypeEnum ProcessOwner { get { return new UserTypeEnum("26424E5F-8CA1-45CF-9398-4FC2633FB865"); } }
        public static UserTypeEnum HandlingLawyer { get { return new UserTypeEnum("2C3785E9-2F9A-45AF-90C9-F548672B6F60"); } }
        public static UserTypeEnum Client { get { return new UserTypeEnum("43B863FB-3E6A-4490-A28F-11E21570436C"); } }
        public static UserTypeEnum Admin { get { return new UserTypeEnum("50675BF5-4A94-4BAD-92AF-1C0CA31D4CD4"); } }
        public static UserTypeEnum Biller { get { return new UserTypeEnum("5497E228-2969-40E3-8E91-19A6F5E508FE"); } }
        public static UserTypeEnum ApplicationApprover { get { return new UserTypeEnum("5ADE0342-7EFD-422F-8153-9BE611C13CFE"); } }
        public static UserTypeEnum SuperAdmin { get { return new UserTypeEnum("A32743D6-02AC-49FE-B715-3A685D8E2AF7"); } }
        public static UserTypeEnum SystemAuditors { get { return new UserTypeEnum("C569DE62-49B5-4BF0-A6A1-37B86398DBE8"); } }
        public static UserTypeEnum DocketOfficer { get { return new UserTypeEnum("CB77889D-CBAE-46EE-8D72-1F18D5E4DD7B"); } }

        public override string ToString()
        {
            return Value;
        }
    }
}