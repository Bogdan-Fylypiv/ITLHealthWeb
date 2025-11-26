using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITLHealthWeb.Pages
{
    public partial class Orders : System.Web.UI.Page
    {
        private DataTable _dtOrdStatus = new DataTable();
        private DataTable _dtBusiness = new DataTable();
        private DataTable _dtWarehouse = new DataTable();
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
            
            // Load business filter, status lookup, and warehouse lookup
            LoadBusinessFilter();
            LoadStatusLookup();
            LoadWarehouseLookup();
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

         // DON'T clear selections - keep all businesses selected by default
         // (LoadBusinessFilter already selected all)

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
      /// Load business filter from database
      /// Based on FrmOrders.cs lines 159-182
      /// </summary>
      private void LoadBusinessFilter()
      {
         try
         {
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            DBAccess _DB = new DBAccess(encryptionKey);

            DataSet ds = new DataSet();
            using (SqlConnection con = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrdersFrmLoad]", con))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandTimeout = _DB.CommandTimeout;
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@BusID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 3 });

                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(ds);
                  }
               }
            }

            // Tables: 0=OrdStatus, 1=Warehouse, 2=Business, 3=DelMode
            if (ds.Tables.Count >= 3)
            {
               ds.Tables[2].TableName = "Business";
               _dtBusiness = ds.Tables[2];

               // Bind to CheckBoxList
               CblBusiness.DataSource = _dtBusiness;
               CblBusiness.DataTextField = "BusName";
               CblBusiness.DataValueField = "BusID";
               CblBusiness.DataBind();

               // Select all businesses by default
               foreach (ListItem item in CblBusiness.Items)
               {
                  item.Selected = true;
               }
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"LoadBusinessFilter Error: {ex.Message}");
         }
      }

      /// <summary>
      /// Load status lookup table from database
      /// Based on FrmOrders.cs lines 175-179
      /// </summary>
      private void LoadStatusLookup()
      {
         try
         {
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            DBAccess _DB = new DBAccess(encryptionKey);

            DataSet ds = new DataSet();
            using (SqlConnection con = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrdersFrmLoad]", con))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandTimeout = _DB.CommandTimeout;
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@BusID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 3 });

                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(ds);
                  }
               }
            }

            // Tables: 0=OrdStatus, 1=Warehouse, 2=Business, 3=DelMode
            if (ds.Tables.Count >= 1)
            {
               ds.Tables[0].TableName = "OrdStatus";
               _dtOrdStatus = ds.Tables[0];

               // Store in ViewState for later use
               ViewState["OrdStatus"] = _dtOrdStatus;
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"LoadStatusLookup Error: {ex.Message}");
         }
      }

      /// <summary>
      /// Load warehouse lookup table from database
      /// Based on FrmOrders.cs lines 1169-1194 (ColShipFrom.AspectGetter)
      /// </summary>
      private void LoadWarehouseLookup()
      {
         try
         {
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            DBAccess _DB = new DBAccess(encryptionKey);

            DataSet ds = new DataSet();
            using (SqlConnection con = _DB.GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrdersFrmLoad]", con))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandTimeout = _DB.CommandTimeout;
                  cmd.Parameters.Add(new SqlParameter() { ParameterName = "@BusID", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 3 });

                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(ds);
                  }
               }
            }

            // Tables: 0=OrdStatus, 1=Warehouse, 2=Business, 3=DelMode
            if (ds.Tables.Count >= 2)
            {
               ds.Tables[1].TableName = "Warehouse";
               _dtWarehouse = ds.Tables[1];

               // Store in ViewState for later use
               ViewState["Warehouse"] = _dtWarehouse;
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"LoadWarehouseLookup Error: {ex.Message}");
         }
      }

      /// <summary>
      /// Get status text from status ID
      /// Based on FrmOrders.cs lines 1001-1027
      /// </summary>
      protected string GetStatusText(object statusValue)
      {
         try
         {
            if (statusValue == null || statusValue == DBNull.Value)
               return "";

            // Retrieve status lookup table from ViewState
            DataTable dtStatus = ViewState["OrdStatus"] as DataTable;
            if (dtStatus == null)
            {
               // If not in ViewState, return the raw value
               return statusValue.ToString();
            }

            // Look up status text
            int statusId = Convert.ToInt32(statusValue);
            DataRow[] drSel = dtStatus.Select($"ID={statusId}");
            if (drSel.Length == 1)
            {
               return drSel[0]["EN"].ToString();
            }
            else
            {
               return statusValue.ToString();
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"GetStatusText Error: {ex.Message}");
            return statusValue?.ToString() ?? "";
         }
      }

      /// <summary>
      /// Get warehouse name from ShipFrom ID
      /// Based on FrmOrders.cs lines 1169-1194 (ColShipFrom.AspectGetter)
      /// </summary>
      protected string GetWarehouseName(object shipFromValue)
      {
         try
         {
            if (shipFromValue == null || shipFromValue == DBNull.Value)
               return "";

            // Retrieve warehouse lookup table from ViewState
            DataTable dtWarehouse = ViewState["Warehouse"] as DataTable;
            if (dtWarehouse == null)
            {
               // If not in ViewState, return the raw value
               return shipFromValue.ToString();
            }

            // Look up warehouse name
            int warehouseId = Convert.ToInt32(shipFromValue);
            DataRow[] drSel = dtWarehouse.Select($"WarehouseID={warehouseId}");
            if (drSel.Length == 1)
            {
               return drSel[0]["Name"].ToString();
            }
            else
            {
               // If not found, return the raw value
               return shipFromValue.ToString();
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"GetWarehouseName Error: {ex.Message}");
            return shipFromValue?.ToString() ?? "";
         }
      }

      /// <summary>
      /// Handle Fetch Orders button click
      /// Validates date inputs and loads orders
      /// Based on FrmOrders.cs business filter logic
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

            // Get selected business IDs (based on FrmOrders.cs lines 104-130)
            string sBusID_IN = GetSelectedBusinessIDs();

            // Success - show message
            ShowMessage($"Fetching orders from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy}...", "success");

            LoadOrders(dateFrom, dateTo, sBusID_IN);
            //LoadMockOrders();
         }
         catch (Exception ex)
         {
            ShowMessage($"Error: {ex.Message}", "error");
            System.Diagnostics.Debug.WriteLine($"BtnFetch_Click Error: {ex.Message}");
         }
      }

      /// <summary>
      /// Get selected business IDs as comma-separated string
      /// Based on FrmOrders.cs lines 104-130
      /// </summary>
      private string GetSelectedBusinessIDs()
      {
         var selectedIDs = new System.Collections.Generic.List<string>();

         System.Diagnostics.Debug.WriteLine($"GetSelectedBusinessIDs - Total items: {CblBusiness.Items.Count}");
         
         foreach (ListItem item in CblBusiness.Items)
         {
            System.Diagnostics.Debug.WriteLine($"  Item: {item.Text}, Value: {item.Value}, Selected: {item.Selected}");
            if (item.Selected)
            {
               selectedIDs.Add(item.Value);
            }
         }

         System.Diagnostics.Debug.WriteLine($"GetSelectedBusinessIDs - Selected count: {selectedIDs.Count}, Result: {string.Join(",", selectedIDs)}");

         // If no selection, return default (1,3) based on FrmOrders.cs line 48
         if (selectedIDs.Count == 0)
         {
            System.Diagnostics.Debug.WriteLine("GetSelectedBusinessIDs - No selections, returning default: 1,3");
            return "1,3";
         }

         return string.Join(",", selectedIDs);
      }

      /// <summary>
      /// Apply business filter to DataTable
      /// Based on FrmOrders.cs SetFilter method lines 4640-4657
      /// </summary>
      private DataRow[] ApplyBusinessFilter(DataTable dtOrd, string sBusID_IN)
      {
         string sFilter = "";

         // Build filter string matching FrmOrders.cs logic
         if (!string.IsNullOrEmpty(sBusID_IN))
         {
            sFilter = $"BusID IN ({sBusID_IN})";
         }

         // Apply filter and return filtered rows (matching FrmOrders.cs lines 4702-4709)
         DataRow[] filteredRows;
         if (!string.IsNullOrEmpty(sFilter))
         {
            filteredRows = dtOrd.Select(sFilter, "OrderID ASC");
         }
         else
         {
            filteredRows = dtOrd.Select("", "OrderID ASC");
         }

         return filteredRows;
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
      /// <param name="sBusID_IN">Business IDs filter</param>
      private void LoadOrders(DateTime dateFrom, DateTime dateTo, string sBusID_IN)
      {
         try
         {
            // Show loading message
            ShowMessage("Loading orders from database...", "info");

            // Fetch data from database
            DataSet ds = GetOrdersData(dateFrom, dateTo, sBusID_IN);

            if (ds.Tables.Contains("Ord") && ds.Tables["Ord"].Rows.Count > 0)
            {
               // Apply business filter (matching FrmOrders.cs SetFilter logic)
               DataRow[] filteredRows = ApplyBusinessFilter(ds.Tables["Ord"], sBusID_IN);

               // Bind to GridView
               if (filteredRows.Length > 0)
               {
                  GvOrders.DataSource = filteredRows.CopyToDataTable();
               }
               else
               {
                  GvOrders.DataSource = null;
               }
               GvOrders.DataBind();

               // Show success message
               ShowMessage($"Loaded {filteredRows.Length} order(s) from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy} (Total: {ds.Tables["Ord"].Rows.Count})", "success");

               // Store dataset in ViewState for paging (keep original unfiltered data)
               ViewState["OrdersDataSet"] = ds;
               ViewState["DateFrom"] = dateFrom;
               ViewState["DateTo"] = dateTo;
               ViewState["BusID_IN"] = sBusID_IN;
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
      /// <param name="sBusID_IN">Business IDs filter (applied after retrieval)</param>
      /// <returns>DataSet with Ord table</returns>
      private DataSet GetOrdersData(DateTime dateFrom, DateTime dateTo, string sBusID_IN)
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

                  // Add parameters - ONLY @DateFrom and @DateTo (matching FrmOrders.cs lines 1250-1251)
                  cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime)
                  {
                     Value = dateFrom
                  });
                  cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime)
                  {
                     Value = dateTo
                  });

                  // Debug logging
                  ShowMessage($"GetOrdersData - Input dates: From={dateFrom:yyyy-MM-dd HH:mm:ss}, To={dateTo:yyyy-MM-dd HH:mm:ss}", "info");
                  ShowMessage($"GetOrdersData - SQL params: @DateFrom={dateFrom:yyyy-MM-dd HH:mm:ss}, @DateTo={dateTo:yyyy-MM-dd HH:mm:ss}", "info");

                  // Fill dataset
                  using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                  {
                     ad.Fill(ds);
                     ShowMessage($"GetOrdersData - Rows returned: {(ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0)}", "info");
                  }
               }
            }

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
            string sBusID_IN = ViewState["BusID_IN"] as string ?? "1,3";
            
            if (ds.Tables.Contains("Ord"))
            {
               // Apply business filter before binding
               DataRow[] filteredRows = ApplyBusinessFilter(ds.Tables["Ord"], sBusID_IN);
               
               if (filteredRows.Length > 0)
               {
                  GvOrders.DataSource = filteredRows.CopyToDataTable();
               }
               else
               {
                  GvOrders.DataSource = null;
               }
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
               string sBusID_IN = ViewState["BusID_IN"] as string ?? "1,3";
               LoadOrders(dateFrom, dateTo, sBusID_IN);
            }
         }
      }

   }
}
