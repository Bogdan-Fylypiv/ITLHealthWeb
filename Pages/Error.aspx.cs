using System;

namespace ITLHealthWeb.Pages
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Log error details if available
            if (Session["LastError"] != null)
            {
                Exception ex = Session["LastError"] as Exception;
                // Log the exception (implement logging in production)
                System.Diagnostics.Debug.WriteLine($"Error: {ex?.Message}");
            }
        }

        protected void BtnGoHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Orders.aspx");
        }
    }
}
