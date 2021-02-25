using AppForAuthorize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AppForAuthorize
{
    public static class MyIdentity
    {
        private static BaseNoRoleEntities db = new BaseNoRoleEntities();

        public static string FullName(this IIdentity identity)
        {
            string FullName = db.User.Where(u => u.Login == identity.Name).Select(u => u.LName + " " + u.FName + " " + u.MName).FirstOrDefault();
            return FullName;
        }
    }
}