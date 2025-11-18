using System;

namespace ITLHealthWeb.Pages
{
    public partial class NotFound : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 404;
        }

        protected void BtnGoHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Orders.aspx");
        }
    }
}
