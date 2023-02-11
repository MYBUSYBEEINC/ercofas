using NetStarter.Enumeration;
using NetStarter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetStarter.Helpers
{
    /// <summary>
    /// The helper class for role.
    /// </summary>
    public static class RoleHelpers
    {
        /// <summary>
        /// Gets the user main role.
        /// </summary>
        public static string GetMainRole()
        {
            if (!(HttpContext.Current.Session["UserTypes"] is List<AspNetUserTypes> userTypes))
                return string.Empty;

            if (userTypes.Any(x => x.UserTypeId == UserTypeEnum.SuperAdmin.ToString()))
                return UserTypeEnum.SuperAdmin.ToString();
            else
                return userTypes.Select(x => x.UserTypeId).FirstOrDefault();
        }
    }
}