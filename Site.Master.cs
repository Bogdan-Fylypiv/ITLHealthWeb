using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace ITLHealthWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Show user info and navigation only if authenticated
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UserInfoPanel.Visible = true;
                MainNav.Visible = true;
                
                // Display username from session
                if (Session["Username"] != null)
                {
                    LblUsername.Text = Session["Username"].ToString();
                }
                else
                {
                    LblUsername.Text = HttpContext.Current.User.Identity.Name;
                }
            }
            else
            {
                UserInfoPanel.Visible = false;
                MainNav.Visible = false;
            }
        }

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();
            
            // Sign out
            FormsAuthentication.SignOut();
            
            // Redirect to login
            Response.Redirect("~/Pages/Login.aspx");
        }
    }
}
