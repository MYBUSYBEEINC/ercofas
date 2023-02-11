using ERCOFAS.Models;
using System.Linq;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for the user.
    /// </summary>
    public static class UserHelpers
    {
        private static readonly DefaultDBContext _db = new DefaultDBContext();

        public static string GetFullName(string userName)
        {
            var aspNetUser = _db.AspNetUsers.FirstOrDefault(x => x.UserName == userName);
            var profile = _db.UserProfiles.FirstOrDefault(x => x.AspNetUserId == aspNetUser.Id);

            return string.Format("{0} {1}", profile.FirstName, profile.LastName);
        }

        public static string GetRoleName(string userName)
        {
            var aspNetUser = _db.AspNetUsers.FirstOrDefault(x => x.UserName == userName);
            var userTypesId = _db.AspNetUserTypes.Where(x => x.UserId == aspNetUser.Id).ToList();
            if (userTypesId == null)
                return string.Empty;

            string userTypeId = userTypesId.Select(x => x.UserTypeId).FirstOrDefault();

            if (userTypeId == "43B863FB-3E6A-4490-A28F-11E21570436C")
                return "Stakeholder";

            return _db.GlobalOptionSets.FirstOrDefault(x => x.Type == "UserType" && x.Id == userTypeId).DisplayName;
        }
    }
}