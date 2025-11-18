using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using NetEncrypt;

namespace ITLHealthWeb.Pages
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any existing authentication
                if (User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                }
            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter username and password");
                return;
            }

            if (ValidateUser(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                LoadUserSession(username);
                Response.Redirect("~/Pages/Orders.aspx");
            }
            else
            {
                ShowError("Invalid username or password");
            }
        }

        private bool ValidateUser(string username, string password)
        {
            try
            {
                string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
                DBAccess _DB = new DBAccess(encryptionKey);
                Encrypter en = new Encrypter(encryptionKey);

                using (SqlConnection con = _DB.GetConnection())
                {
                    // Query to get employee by UserID and validate password
                    string query = @"
                        SELECT E.GID, U.Password, E.IsDel
                        FROM Employee E 
                        INNER JOIN EmployeeUserID U ON E.GID = U.EmpID
                        WHERE U.UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 50) { Value = username });

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Check if employee is active (IsDel = 0 means active)
                                bool isDeleted = reader["IsDel"] != DBNull.Value && Convert.ToBoolean(reader["IsDel"]);
                                if (isDeleted)
                                {
                                    ShowError("This account is inactive.");
                                    return false;
                                }

                                // Validate password
                                object passwordObj = reader["Password"];
                                if (passwordObj != null && passwordObj != DBNull.Value)
                                {
                                    string encryptedPasswordFromDB = passwordObj.ToString();
                                    string decryptedPassword = en.Decrypt(encryptedPasswordFromDB);

                                    return decryptedPassword == password;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error (implement logging in production)
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                ShowError("An error occurred. Please try again.");
            }

            return false;
        }

        private void LoadUserSession(string username)
        {
            try
            {
                string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
                DBAccess _DB = new DBAccess(encryptionKey);

                using (SqlConnection con = _DB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[GetUserInfo]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", username);
                        cmd.Parameters.AddWithValue("@use_like", false);

                        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                Session["EmpGID"] = dt.Rows[0]["GID"];
                                Session["BusID"] = dt.Rows[0]["BusID"];
                                Session["SiteID"] = dt.Rows[0]["Site"];
                                Session["EmpRole"] = dt.Rows[0]["Role"];
                                Session["Username"] = username;
                                Session["UserID"] = username;
                                Session["gBusID"] = dt.Rows[0]["gBusID"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUserSession error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            LblError.Text = message;
            LblError.Visible = true;
        }
   }
}
