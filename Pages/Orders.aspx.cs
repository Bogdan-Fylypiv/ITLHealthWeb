using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITLHealthWeb.Pages
{
    public partial class Orders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
               InitializeFilters();
            }
        }

        private void InitializePage()
        {
            // Set default dates
            TxtFromDate.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            TxtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            // Set welcome message
            LblMessage.Text = "Welcome to Order Fulfillment - Select date range to view orders";
            LblMessage.CssClass = "message-label";

            // Set welcome message
            ShowMessage("Select date range and click 'Fetch Orders' to view data", "info");
            
            // Clear summary
            LblSummary.Text = "No data loaded yet";
            
            // Load initial data
            //LoadOrders();
        }

      /// <summary>
      /// Initialize filter controls with default values
      /// Based on FrmOrders.cs lines 149-150
      /// </summary>
      private void InitializeFilters()
      {
         // Set default dates to today (same as FrmOrders.cs)
         TxtFromDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
         TxtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

         // Set welcome message
         ShowMessage("Select date range and click 'Fetch Orders' to view data", "info");

         // Clear summary
         LblSummary.Text = "No data loaded yet";
      }

      /// <summary>
      /// Display message to user with appropriate styling
      /// </summary>
      /// <param name="message">Message text</param>
      /// <param name="type">Message type: success, error, info</param>
      private void ShowMessage(string message, string type)
      {
         LblMessage.Text = message;
         LblMessage.CssClass = $"message-label {type}";
         LblMessage.Visible = true;
      }

      /// <summary>
      /// Handle Fetch Orders button click
      /// Validates date inputs and loads orders
      /// </summary>
      protected void BtnFetch_Click(object sender, EventArgs e)
      {
         try
         {
            // Validate date inputs
            if (string.IsNullOrEmpty(TxtFromDate.Text) || string.IsNullOrEmpty(TxtToDate.Text))
            {
               ShowMessage("Please select both From and To dates.", "error");
               return;
            }

            // Parse dates
            DateTime dateFrom;
            DateTime dateTo;

            if (!DateTime.TryParse(TxtFromDate.Text, out dateFrom))
            {
               ShowMessage("Invalid From Date format.", "error");
               return;
            }

            if (!DateTime.TryParse(TxtToDate.Text, out dateTo))
            {
               ShowMessage("Invalid To Date format.", "error");
               return;
            }

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"BtnFetch_Click - Parsed dates: From={dateFrom:yyyy-MM-dd HH:mm:ss}, To={dateTo:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"BtnFetch_Click - Input text: FromText='{TxtFromDate.Text}', ToText='{TxtToDate.Text}'");

            // Validate date range
            if (dateFrom > dateTo)
            {
               ShowMessage("From Date cannot be after To Date.", "error");
               return;
            }

            // Check if date range is too large (optional business rule)
            TimeSpan dateRange = dateTo - dateFrom;
            if (dateRange.TotalDays > 90)
            {
               ShowMessage("Date range cannot exceed 90 days. Please select a smaller range.", "error");
               return;
            }

            // Success - show message
            ShowMessage($"Fetching orders from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy}...", "success");

            LoadOrders(dateFrom, dateTo);
            //LoadMockOrders();
         }
         catch (Exception ex)
         {
            ShowMessage($"Error: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"BtnFetch_Click Error: {ex.Message}");
         }
      }

      //protected void BtnFetch_Click(object sender, EventArgs e)
      //{
      //   LoadOrders();
      //}

      /// <summary>
      /// Handle Clear button click
      /// Resets all filters and clears data
      /// </summary>
      protected void BtnClear_Click(object sender, EventArgs e)
      {
         // Reset filters to defaults
         InitializeFilters();

         // Clear any loaded data (will be implemented in Phase 4)
         GvOrders.DataSource = null;
         GvOrders.DataBind();

         // Clear summary
         LblSummary.Text = string.Empty;

         ShowMessage("Filters cleared", "success");
      }

      /// <summary>
      /// Load mock data for testing grid layout
      /// Based on FrmOrders.cs screenshot data
      /// </summary>
      private void LoadMockOrders()
      {
         DataTable dt = new DataTable();

         dt.Columns.Add("ID", typeof(int));
         dt.Columns.Add("BillTo", typeof(string));
         dt.Columns.Add("ShipTo", typeof(string));
         dt.Columns.Add("ShipVia", typeof(string));  // Carrier names: FedEx, UPS, CanPar
         dt.Columns.Add("Qty", typeof(int));
         dt.Columns.Add("ProductCode", typeof(string));  // Product codes
         dt.Columns.Add("TrackingNo", typeof(string));  // Actual tracking numbers
         dt.Columns.Add("Status", typeof(string));
         dt.Columns.Add("Completed", typeof(DateTime));
         dt.Columns.Add("ShipOn", typeof(DateTime));
         dt.Columns.Add("ShipFrom", typeof(string));

         dt.Rows.Add(5668, "Bill To/ Customer Name", "Jordan Clarke, Jodan Clarke ON, CA", "FedEx", 96, "CA1100T, CA1102T, CA...", "1Z999AA10123456784", "On Hold", DBNull.Value, DateTime.Parse("31-Oct-2025"), "Duff CA");
         dt.Rows.Add(5687, "Joanne Esrange", "Joanne Esrange, Joanne Esrange ON, CA", "UPS", 96, "CA1103B, CA1103ZA, C...", "1Z999AA10123456785", "On Hold", DBNull.Value, DateTime.Parse("29-Oct-2025"), "Duff CA");
         dt.Rows.Add(6312, "Sarah Esrange", "Suzette Ye BC, CA", "CanPar", 297, "CA1103B, CA1103ZA, C...", "D42510139000001525001", "", DBNull.Value, DateTime.Parse("21-Oct-2025"), "Duff CA");
         dt.Rows.Add(6332, "ITL Health Ltd (F)", "Bella Wellness 400, BC", "FedEx", 96, "UK1152H", "123456789012", "Ready to...", DateTime.Parse("31-Oct-2025"), DateTime.Parse("29-Oct-2025"), "Duff CA");
         dt.Rows.Add(6333, "ITL Health Ltd (F)", "Eurobase House :E", "UPS", 203, "CA1210ZA, E12015, E...", "1Z999AA10123456786", "Ready to...", DateTime.Parse("31-Oct-2025"), DateTime.Parse("29-Oct-2025"), "Duff CA");
         dt.Rows.Add(6334, "ITL Health Ltd (F)", "Eurobase House :E", "FedEx", 22, "CA0002T, CA1102T", "123456789013", "Ready to...", DateTime.Parse("31-Oct-2025"), DateTime.Parse("23-Oct-2025"), "Duff CA");
         dt.Rows.Add(6344, "Purity Life Health Produ...", "Purity Life Health Products, Purity Life ON, CA", "", 488, "CA1Z13M", "", "No Invoi...", DBNull.Value, DateTime.Parse("23-Oct-2025"), "Duff CA");
         dt.Rows.Add(6374, "Nadine Kroner", "Nadine Kroner, Nadine Kroner BC, CA", "CanPar", 24, "CA1104AB", "D42510139000001525002", "Ready to...", DBNull.Value, DateTime.Parse("28-Oct-2025"), "Duff CA");
         dt.Rows.Add(6394, "Brad Bouchard", "Brad Bouchard", "UPS", 210, "CA1103B, CA1103ZA, C...", "1Z999AA10123456787", "Ready to...", DBNull.Value, DateTime.Parse("29-Oct-2025"), "Duff CA");
         dt.Rows.Add(6408, "Leanne Wright", "Swan Lake Market & Garden BC, CA", "FedEx", 167, "CA1100TTCA, CA1103S...", "123456789014", "Ready to...", DBNull.Value, DateTime.Parse("29-Oct-2025"), "Duff CA");
         dt.Rows.Add(6421, "The Water Jug Health F...", "The Water Bug Health Food Store Ltd ON, CA", "UPS", 174, "CA1101M, CA1102TA", "1Z999AA10123456788", "Ready to...", DBNull.Value, DateTime.Parse("30-Oct-2025"), "Duff CA");

         // Bind to GridView
         GvOrders.DataSource = dt;
         GvOrders.DataBind();

         // Show success message
         ShowMessage($"Loaded {dt.Rows.Count} mock orders for testing", "success");
      }

      /// <summary>
      /// Load orders from database and bind to grid
      /// </summary>
      /// <param name="dateFrom">Start date</param>
      /// <param name="dateTo">End date</param>
      private void LoadOrders(DateTime dateFrom, DateTime dateTo)
      {
         try
         {
            // Show loading message
            ShowMessage("Loading orders from database...", "info");

            // Fetch data from database
            DataSet ds = GetOrdersData(dateFrom, dateTo);

            if (ds.Tables.Contains("Ord") && ds.Tables["Ord"].Rows.Count > 0)
            {
               // Bind to GridView
               GvOrders.DataSource = ds.Tables["Ord"];
               GvOrders.DataBind();

               // Show success message
               int rowCount = ds.Tables["Ord"].Rows.Count;
               ShowMessage($"Loaded {rowCount} order(s) from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy}", "success");

               // Store dataset in ViewState for paging
               ViewState["OrdersDataSet"] = ds;
               ViewState["DateFrom"] = dateFrom;
               ViewState["DateTo"] = dateTo;
            }
            else
            {
               // No data found
               GvOrders.DataSource = null;
               GvOrders.DataBind();
               ShowMessage($"No orders found for the selected date range ({dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy})", "info");

               // Clear ViewState
               ViewState["OrdersDataSet"] = null;
            }
         }
         catch (SqlException sqlEx)
         {
            // Database-specific error
            ShowMessage($"Database Error: {sqlEx.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"LoadOrders SQL Error: {sqlEx.Message}\nStack: {sqlEx.StackTrace}");

            // Clear grid
            GvOrders.DataSource = null;
            GvOrders.DataBind();
         }
         catch (Exception ex)
         {
            // General error
            ShowMessage($"Error loading orders: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"LoadOrders Error: {ex.Message}\nStack: {ex.StackTrace}");

            // Clear grid
            GvOrders.DataSource = null;
            GvOrders.DataBind();
         }
      }

      /// <summary>
      /// Get orders data from database
      /// Based on FrmOrders.cs lines 1244-1256
      /// </summary>
      /// <param name="dateFrom">Start date</param>
      /// <param name="dateTo">End date</param>
      /// <returns>DataSet with Ord table</returns>
      private DataSet GetOrdersData(DateTime dateFrom, DateTime dateTo)
      {
         string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
         DBAccess _DB = new DBAccess(encryptionKey);

         DataSet ds = new DataSet();

         try
         {
            using (SqlConnection con = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandTimeout = _DB.CommandTimeout;

                  // Add parameters - use Date type to match string date behavior
                  // When you pass '2025-11-01' as a string, SQL treats it as a date
                  // Using SqlDbType.Date ensures the same behavior
                  cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.Date)
                  {
                     Value = dateFrom.Date
                  });
                  cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.Date)
                  {
                     Value = dateTo.Date
                  });

                  // Debug logging
                  ShowMessage($"GetOrdersData - Input dates: From={dateFrom:yyyy-MM-dd HH:mm:ss}, To={dateTo:yyyy-MM-dd HH:mm:ss}", "info");
                  ShowMessage($"GetOrdersData - SQL params: @DateFrom={dateFrom.Date:yyyy-MM-dd}, @DateTo={dateTo.Date:yyyy-MM-dd}", "info");

                  // Fill dataset
                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(ds);
                     ShowMessage($"GetOrdersData - Rows returned: {(ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0)}", "info");
                  }
               }
            }

            // Name the table for easy reference (same as FrmOrders.cs line 1258)
            if (ds.Tables.Count > 0)
            {
               ds.Tables[0].TableName = "Ord";
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"GetOrdersData Error: {ex.Message}");
            throw; // Re-throw to be handled by calling method
         }

         return ds;
      }

      /// <summary>
      /// Handle row data binding for custom styling
      /// Based on FrmOrders.cs row coloring logic
      /// </summary>
      protected void GvOrders_RowDataBound(object sender, GridViewRowEventArgs e)
      {
         if (e.Row.RowType == DataControlRowType.DataRow)
         {
            // Add hover effect
            e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#e6f2ff'";
            e.Row.Attributes["onmouseout"] = "this.style.backgroundColor=''";

            // Get status for conditional formatting
            string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

            // Color code by status
            if (status.Contains("On Hold"))
            {
               e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff3cd"); // Yellow
               e.Row.ForeColor = System.Drawing.ColorTranslator.FromHtml("#856404");
            }
            else if (status.Contains("No Invoice"))
            {
               e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#f8d7da"); // Light red
               e.Row.ForeColor = System.Drawing.ColorTranslator.FromHtml("#721c24");
            }
            else if (status.Contains("Open Process"))
            {
               e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#d1ecf1"); // Light blue
               e.Row.ForeColor = System.Drawing.ColorTranslator.FromHtml("#0c5460");
            }
            // Ready to Ship stays default (white/alternating)
         }
      }

      /// <summary>
      /// Handle paging
      /// </summary>
      protected void GvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
      {
         GvOrders.PageIndex = e.NewPageIndex;

         // Reload data from ViewState (avoids re-querying database)
         if (ViewState["OrdersDataSet"] != null)
         {
            DataSet ds = (DataSet)ViewState["OrdersDataSet"];
            if (ds.Tables.Contains("Ord"))
            {
               GvOrders.DataSource = ds.Tables["Ord"];
               GvOrders.DataBind();
            }
         }
         else
         {
            // No data in ViewState, reload from database
            if (ViewState["DateFrom"] != null && ViewState["DateTo"] != null)
            {
               DateTime dateFrom = (DateTime)ViewState["DateFrom"];
               DateTime dateTo = (DateTime)ViewState["DateTo"];
               LoadOrders(dateFrom, dateTo);
            }
         }
      }

      /// <summary>
      /// Handle track link click (placeholder for Phase 6)
      /// </summary>
      protected void LnkTrack_Click(object sender, EventArgs e)
      {
         LinkButton btn = (LinkButton)sender;
         string trackingNo = btn.CommandArgument;

         ShowMessage($"Track clicked for: {trackingNo}", "info");
      }
   }
}
