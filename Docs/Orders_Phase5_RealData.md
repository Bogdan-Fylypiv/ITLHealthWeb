# Phase 5: Real Data Integration (Database Connection)

## 🎯 Objective
Replace mock data with real database queries using the `[dbo].[GetOrders]` stored procedure.

## ⏱️ Estimated Time
2 hours

---

## 📋 Prerequisites
- Phase 1: Database verified ✅
- Phase 2: Page layout created ✅
- Phase 3: Filter controls implemented ✅
- Phase 4: Mock grid working ✅

---

## 📝 Task 5.1: Create Data Access Method

**Action:** Add method to call GetOrders stored procedure.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method (based on FrmOrders.cs lines 1241-1258):**
```csharp
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
                
                // Add parameters (same as FrmOrders.cs)
                cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) 
                { 
                    Value = dateFrom 
                });
                cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) 
                { 
                    Value = dateTo 
                });
                
                // Fill dataset
                using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                {
                    ad.Fill(ds);
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
```

**Checklist:**
- [ ] Method signature matches requirements
- [ ] Uses DBAccess class
- [ ] Calls [dbo].[GetOrders] stored procedure
- [ ] Parameters added correctly
- [ ] DataSet table named "Ord"
- [ ] Error handling implemented
- [ ] Debug logging added

---

## 📝 Task 5.2: Create LoadOrders Method

**Action:** Add method to load and bind real orders data.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method:**
```csharp
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
        ShowMessage("⏳ Loading orders from database...", "info");
        
        // Fetch data from database
        DataSet ds = GetOrdersData(dateFrom, dateTo);
        
        if (ds.Tables.Contains("Ord") && ds.Tables["Ord"].Rows.Count > 0)
        {
            // Bind to GridView
            GvOrders.DataSource = ds.Tables["Ord"];
            GvOrders.DataBind();
            
            // Show success message
            int rowCount = ds.Tables["Ord"].Rows.Count;
            ShowMessage($"✅ Loaded {rowCount} order(s) from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy}", "success");
            
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
            ShowMessage($"ℹ️ No orders found for the selected date range ({dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy})", "info");
            
            // Clear ViewState
            ViewState["OrdersDataSet"] = null;
        }
    }
    catch (SqlException sqlEx)
    {
        // Database-specific error
        ShowMessage($"❌ Database Error: {sqlEx.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"LoadOrders SQL Error: {sqlEx.Message}\nStack: {sqlEx.StackTrace}");
        
        // Clear grid
        GvOrders.DataSource = null;
        GvOrders.DataBind();
    }
    catch (Exception ex)
    {
        // General error
        ShowMessage($"❌ Error loading orders: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"LoadOrders Error: {ex.Message}\nStack: {ex.StackTrace}");
        
        // Clear grid
        GvOrders.DataSource = null;
        GvOrders.DataBind();
    }
}
```

**Checklist:**
- [ ] Method created with date parameters
- [ ] Calls GetOrdersData()
- [ ] Binds data to GridView
- [ ] Handles empty results
- [ ] Stores DataSet in ViewState for paging
- [ ] Comprehensive error handling
- [ ] User-friendly messages

---

## 📝 Task 5.3: Update BtnFetch_Click to Use Real Data

**Action:** Replace LoadMockOrders() with LoadOrders().

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Find and update BtnFetch_Click:**
```csharp
protected void BtnFetch_Click(object sender, EventArgs e)
{
    try
    {
        // Validate date inputs
        if (string.IsNullOrEmpty(TxtFromDate.Text) || string.IsNullOrEmpty(TxtToDate.Text))
        {
            ShowMessage("⚠️ Please select both From and To dates.", "error");
            return;
        }

        // Parse dates
        DateTime dateFrom;
        DateTime dateTo;
        
        if (!DateTime.TryParse(TxtFromDate.Text, out dateFrom))
        {
            ShowMessage("⚠️ Invalid From Date format.", "error");
            return;
        }
        
        if (!DateTime.TryParse(TxtToDate.Text, out dateTo))
        {
            ShowMessage("⚠️ Invalid To Date format.", "error");
            return;
        }

        // Validate date range
        if (dateFrom > dateTo)
        {
            ShowMessage("⚠️ From Date cannot be after To Date.", "error");
            return;
        }

        // Check if date range is too large
        TimeSpan dateRange = dateTo - dateFrom;
        if (dateRange.TotalDays > 90)
        {
            ShowMessage("⚠️ Date range cannot exceed 90 days. Please select a smaller range.", "error");
            return;
        }

        // Load real data from database (Phase 5)
        LoadOrders(dateFrom, dateTo);
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ Error: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"BtnFetch_Click Error: {ex.Message}");
    }
}
```

**Checklist:**
- [ ] LoadMockOrders() removed
- [ ] LoadOrders(dateFrom, dateTo) called
- [ ] All validation still in place
- [ ] Error handling maintained

---

## 📝 Task 5.4: Update Paging Handler for Real Data

**Action:** Modify paging to reload from ViewState.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Update GvOrders_PageIndexChanging:**
```csharp
/// <summary>
/// Handle paging with real data from ViewState
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
```

**Checklist:**
- [ ] Uses ViewState to avoid re-querying
- [ ] Falls back to database if ViewState empty
- [ ] Page index updated correctly

---

## 📝 Task 5.5: Add Required Using Statements

**Action:** Ensure all necessary namespaces are imported.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add at the top of the file:**
```csharp
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
```

**Checklist:**
- [ ] System.Configuration added (for ConfigurationManager)
- [ ] System.Data added (for DataSet, DataTable)
- [ ] System.Data.SqlClient added (for SqlConnection, SqlCommand, SqlException)
- [ ] System.Web.UI.WebControls added (for GridView events)

---

## 📝 Task 5.6: Test with Database

**Action:** Verify database connectivity and data retrieval.

### Step 1: Verify Connection String
**File:** `d:\VS_2022Projects\ITLHealthWeb\Web.config`

**Ensure this exists:**
```xml
<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=your-server;Database=your-database;..." 
         providerName="System.Data.SqlClient"/>
</connectionStrings>
```

### Step 2: Verify Encryption Key
**File:** `d:\VS_2022Projects\ITLHealthWeb\Web.config`

**Ensure this exists:**
```xml
<appSettings>
    <add key="EncryptionKey" value="YourEncryptionKeyHere"/>
</appSettings>
```

### Step 3: Test the Page
1. Press F5 to run the application
2. Navigate to `/Pages/Orders.aspx`
3. Select today's date in both From and To fields
4. Click "Fetch Orders"
5. Verify orders display from database

**Checklist:**
- [ ] Connection string configured
- [ ] Encryption key configured
- [ ] Page loads without errors
- [ ] Fetch button queries database
- [ ] Orders display in grid
- [ ] Row count message shows
- [ ] No console errors

---

## 📝 Task 5.7: Handle Column Mismatches

**Action:** If database columns don't match GridView, update BoundFields.

**Common Column Name Differences:**

| GridView DataField | Possible DB Column Names | Notes |
|--------------------|--------------------------|-------|
| ID | ID, OrderID, OrdID | |
| BillTo | BillTo, BillToName, CustomerName | |
| ShipTo | ShipTo, ShipToAddress, ShipToName | |
| ShipVia | ShipVia, Carrier, CarrierName | Should contain: FedEx, UPS, CanPar, etc. |
| Qty | Qty, Quantity, TotalQty | |
| ProductCode | ProductCode, Products, ItemCode | |
| TrackingNo | TrackingNo, LeadTrackingNo, TrackNo | Should contain actual tracking numbers |
| Status | Status, OrdStatus, StatusDesc | |
| Completed | Completed, CompletedDate, dtmCompleted | |
| ShipOn | ShipOn, ShipDate, dtmShip | |
| ShipFrom | ShipFrom, Warehouse, Location | |

**Note:** Courier/Business and InvID columns may exist in database but are NOT displayed in the GridView.

**If you get "Column 'X' does not belong to table" error:**

1. Run this query to see actual column names:
```sql
EXEC [dbo].[GetOrders] 
    @DateFrom = '2025-11-01', 
    @DateTo = '2025-11-08'
```

2. Update GridView BoundField DataField values to match

**Example Fix:**
```aspx
<!-- If database returns "OrderID" instead of "ID" -->
<asp:BoundField DataField="OrderID" HeaderText="ID" ... />

<!-- If database returns "CustomerName" instead of "BillTo" -->
<asp:BoundField DataField="CustomerName" HeaderText="Bill To / Customer Name" ... />

<!-- If database returns "Carrier" instead of "ShipVia" -->
<asp:BoundField DataField="Carrier" HeaderText="Ship Via" ... />

<!-- If database returns "LeadTrackingNo" instead of "TrackingNo" -->
<asp:BoundField DataField="LeadTrackingNo" HeaderText="Tracking No" ... />

<!-- If database returns "Warehouse" instead of "ShipFrom" -->
<asp:BoundField DataField="Warehouse" HeaderText="ShipFrom" ... />
```

**Important Notes:**
- **ShipVia** should contain carrier names like "FedEx", "UPS", "CanPar" (not "Track" or "CPU")
- **TrackingNo** should contain actual tracking numbers (e.g., "1Z999AA10123456784", "D42510139000001525001")
- **ProductCode** column shows product codes (e.g., "CA1100T, CA1102T, CA...")
- Do NOT bind Courier/Business or InvID columns to GridView (even if they exist in database)

**Checklist:**
- [ ] Verified actual column names from database
- [ ] Updated BoundField DataField values if needed
- [ ] Grid displays without column errors

---

## ✅ Deliverables

At the end of Phase 5, you should have:

1. **Database Integration:**
   - [ ] GetOrdersData() method created
   - [ ] LoadOrders() method created
   - [ ] Calls [dbo].[GetOrders] stored procedure
   - [ ] Handles database errors gracefully

2. **Data Display:**
   - [ ] Real orders display from database
   - [ ] Row count shows actual number
   - [ ] Date range filtering works
   - [ ] Empty results handled properly

3. **Performance:**
   - [ ] ViewState used for paging (no re-query)
   - [ ] Loading messages provide feedback
   - [ ] Errors logged to Debug output

4. **Testing:**
   - [ ] Successfully queries database
   - [ ] Displays orders correctly
   - [ ] Paging works with real data
   - [ ] Error handling works

---

## 🧪 Testing Checklist

### Database Connection Testing
- [ ] Page loads without connection errors
- [ ] Can query database successfully
- [ ] Connection string is correct
- [ ] Encryption key is correct
- [ ] DBAccess class works

### Data Retrieval Testing
- [ ] Select today's date, click Fetch → shows today's orders
- [ ] Select date range → shows orders in range
- [ ] Select future date → shows "no orders" message
- [ ] Select very old date → shows "no orders" or old orders
- [ ] Date range >90 days → shows validation error

### Grid Display Testing
- [ ] All columns display correctly
- [ ] Data matches database
- [ ] Row count is accurate
- [ ] Status colors still work
- [ ] Track links appear for orders with tracking numbers

### Paging Testing
- [ ] If >50 orders, paging controls appear
- [ ] Click page 2 → shows next 50 orders
- [ ] Click page 1 → returns to first 50
- [ ] Paging doesn't re-query database (check Debug output)

### Error Handling Testing
- [ ] Stop SQL Server → shows database error message
- [ ] Invalid connection string → shows connection error
- [ ] Missing encryption key → shows configuration error
- [ ] Stored procedure doesn't exist → shows SQL error
- [ ] All errors logged to Debug output

---

## 🚨 Common Issues & Solutions

### Issue 1: Connection String Error
**Symptom:** "Login failed for user" or "Cannot open database"  
**Solution:**
- Verify connection string in Web.config
- Check SQL Server is running
- Verify database name is correct
- Test connection in SSMS first
- Check firewall settings

### Issue 2: Encryption Key Missing
**Symptom:** "EncryptionKey not found in configuration"  
**Solution:**
- Add to Web.config appSettings:
  ```xml
  <add key="EncryptionKey" value="YourKeyHere"/>
  ```
- Use same key as WinForms app
- Verify key is correct

### Issue 3: Column Does Not Belong to Table
**Symptom:** "Column 'X' does not belong to table"  
**Solution:**
- Run GetOrders in SSMS to see actual columns
- Update GridView BoundField DataField values
- Check for typos in column names
- Verify stored procedure returns expected columns

### Issue 4: No Data Displayed
**Symptom:** Grid is empty but no error  
**Solution:**
- Check if database has orders for selected dates
- Verify stored procedure returns data in SSMS
- Check Debug output for errors
- Verify DataBind() is called
- Check ViewState isn't corrupted

### Issue 5: Timeout Error
**Symptom:** "Timeout expired" error  
**Solution:**
- Increase CommandTimeout in DBAccess
- Check if stored procedure is slow
- Verify database server is responsive
- Reduce date range
- Check for missing indexes on Order table

### Issue 6: Paging Re-queries Database
**Symptom:** Slow paging, multiple DB calls in Debug output  
**Solution:**
- Verify ViewState["OrdersDataSet"] is set
- Check ViewState is enabled on page
- Ensure DataSet is serializable
- Consider Session instead of ViewState for large datasets

---

## 📊 Performance Considerations

### ViewState vs Session
**Current:** Using ViewState to store DataSet  
**Pros:** Automatic per-user storage  
**Cons:** Increases page size, limited to ~4MB

**If dataset is large (>1000 rows):**
```csharp
// Use Session instead
Session["OrdersDataSet"] = ds;

// In paging handler
if (Session["OrdersDataSet"] != null)
{
    DataSet ds = (DataSet)Session["OrdersDataSet"];
    // ...
}
```

### Query Optimization
- Keep date ranges reasonable (<90 days)
- Add indexes on Order table (ShipOn, Status columns)
- Consider pagination at database level for very large datasets

---

## 📚 Reference

### From FrmOrders.cs
- **Lines 1241-1256:** GetOrders stored procedure call
- **Lines 1258:** Table naming convention
- **Lines 1267-1280:** Grid binding and refresh

### Code Comparison

**WinForms (FrmOrders.cs):**
```csharp
_dsOrd = new DataSet();
using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dtmFromShipRpt });
    cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dtmToShipRpt });
    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
    {
        ad.Fill(_dsOrd);
    }
}
_dsOrd.Tables[0].TableName = "Ord";
```

**Web (Orders.aspx.cs):**
```csharp
DataSet ds = new DataSet();
using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dateFrom });
    cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dateTo });
    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
    {
        ad.Fill(ds);
    }
}
ds.Tables[0].TableName = "Ord";
```

**✅ Nearly identical!** Only difference is variable names.

---

## ➡️ Next Phase
Once Phase 5 is complete, proceed to **Phase 6: Tracking Functionality** to implement the Track button.
