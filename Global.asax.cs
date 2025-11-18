using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ITLHealthWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Disable unobtrusive validation (or configure jQuery)
            System.Web.UI.ValidationSettings.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the exception
            Exception ex = Server.GetLastError();
            
            if (ex != null)
            {
                // Log the error (implement logging in production)
                System.Diagnostics.Debug.WriteLine($"Application Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Store in session for error page
                if (Session != null)
                {
                    Session["LastError"] = ex;
                }
                
                // Clear the error
                Server.ClearError();
                
                // Redirect to error page
                Response.Redirect("~/Pages/Error.aspx", false);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Session start logic
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Session end logic - cleanup
        }
    }
}