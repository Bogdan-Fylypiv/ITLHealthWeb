using System;
using System.Web;

namespace ITLHealthWeb
{
    /// <summary>
    /// Helper class to access user session data
    /// Replaces Program.m_User, Program.m_EmpGID, etc. from WinForms
    /// </summary>
    public static class UserContext
    {
        public static Guid EmpGID
        {
            get { return GetSessionValue<Guid>("EmpGID", Guid.Empty); }
        }

        public static int BusID
        {
            get { return GetSessionValue<int>("BusID", 0); }
        }

        public static Guid gBusID
        {
            get { return GetSessionValue<Guid>("gBusID", Guid.Empty); }
        }

        public static int SiteID
        {
            get { return GetSessionValue<int>("SiteID", 1); }
        }

        public static int EmpRole
        {
            get { return GetSessionValue<int>("EmpRole", 0); }
        }

        public static string Username
        {
            get { return GetSessionValue<string>("Username", string.Empty); }
        }

        public static string UserID
        {
            get { return GetSessionValue<string>("UserID", string.Empty); }
        }

        public static bool IsAuthenticated
        {
            get 
            { 
                return HttpContext.Current != null && 
                       HttpContext.Current.User != null && 
                       HttpContext.Current.User.Identity.IsAuthenticated; 
            }
        }

        private static T GetSessionValue<T>(string key, T defaultValue)
        {
            if (HttpContext.Current?.Session?[key] != null)
            {
                try
                {
                    return (T)HttpContext.Current.Session[key];
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}
