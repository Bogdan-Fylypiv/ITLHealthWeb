# Phase 1: Database & Stored Procedures (Foundation)

## 🎯 Objective
Verify database access and ensure all required stored procedures are available and working.

## ⏱️ Estimated Time
1 hour

---

## 📋 Required Stored Procedures

### 1. `[dbo].[GetOrders]`
**Purpose:** Main orders retrieval  
**Used in FrmOrders.cs:** Lines 1244-1256  
**Parameters:**
- `@DateFrom` (DateTime) - Start date
- `@DateTo` (DateTime) - End date

**Returns:** Order data with columns (based on screenshot):
- ID (int) - Order number
- Courier (string) - Courier code (CA, etc.)
- BillTo (string) - Bill To / Customer Name
- ShipTo (string) - Ship To / Customer Name, Prov, Country
- ShipVia (string) - Ship Via (Track, CPU, etc.)
- Qty (decimal/int) - Quantity
- ProductCode (string) - Product Code(s)
- TrackingNo (string) - Tracking number
- Status (string) - Order status (On Hold, Ready to..., No Invoi...)
- Completed (DateTime) - Completed date
- ShipOn (DateTime) - Ship On date
- ShipFrom (string) - Ship From location (Duff CA, etc.)
- InvID (string) - Invoice ID
- *(Verify actual column names with your database)*

**WinForms Implementation Reference:**
```csharp
// From FrmOrders.cs lines 1244-1256
using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandTimeout = _DB.CommandTimeout;
    cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dtmFromShipRpt });
    cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dtmToShipRpt });
    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
    {
        ad.Fill(_dsOrd);
    }
}
_dsOrd.Tables[0].TableName = "Ord";
```

### 2. `[dbo].[GetOrdersEndOfDay]`
**Purpose:** Summary statistics  
**Used in FrmOrders.cs:** Lines 1311-1319  
**Parameters:**
- `@DateFrom` (DateTime) - Start date
- `@DateTo` (DateTime) - End date

**Returns:** Aggregate data (document actual columns after testing)

**WinForms Implementation Reference:**
```csharp
// From FrmOrders.cs lines 1311-1319
using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrdersEndOfDay]", con))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandTimeout = _DB.CommandTimeout;
    cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dtmFromShipRpt });
    cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dtmToShipRpt });
    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
    {
        ad.Fill(_dsOrdEoD);
    }
}
```

### 3. `[dbo].[GetOrdersFrmLoad]` *(For future filter dropdowns)*
**Purpose:** Load form initialization data  
**Used in FrmOrders.cs:** Lines 162-173  
**Parameters:**
- `@BusID` (int) - Business ID

**Returns 4 Tables:**
- Table[0]: OrdStatus - Order status lookup
- Table[1]: Warehouse - Warehouse/site list
- Table[2]: Business - Business list
- Table[3]: DelMode - Delivery mode lookup

**Note:** Use this later for warehouse/business filter dropdowns

### 4. `[dbo].[GetOrdersPkg]` *(Optional - for drill-down)*
**Purpose:** Package details for an order  
**Used in FrmOrders.cs:** Line 669  
**Note:** For future hierarchical display

### 5. `[dbo].[GetOrdersOrdItems]` *(Optional - for drill-down)*
**Purpose:** Order line items  
**Parameters:** Order-specific

### 5. `[dbo].[GetOrdersPkgItems]` *(Optional for Phase 1)*
**Purpose:** Items within packages  
**Parameters:** Package-specific

---

## 📝 Tasks

### Task 1.1: Verify Stored Procedures Exist

**Action:** Run this query in SQL Server Management Studio (SSMS):

```sql
-- Check if all required stored procedures exist
SELECT name, create_date, modify_date
FROM sys.procedures 
WHERE name IN (
    'GetOrders',
    'GetOrdersEndOfDay',
    'GetOrdersPkg',
    'GetOrdersOrdItems',
    'GetOrdersPkgItems'
)
ORDER BY name
```

**Expected Result:** All 5 procedures should be listed.

**Checklist:**
- [ ] GetOrders exists
- [ ] GetOrdersEndOfDay exists
- [ ] GetOrdersPkg exists (optional)
- [ ] GetOrdersOrdItems exists (optional)
- [ ] GetOrdersPkgItems exists (optional)

---

### Task 1.2: Test GetOrders Stored Procedure

**Action:** Execute with sample dates:

```sql
-- Test GetOrders with today's date
DECLARE @Today DATE = GETDATE()

EXEC [dbo].[GetOrders] 
    @DateFrom = @Today, 
    @DateTo = @Today

-- Test with date range
EXEC [dbo].[GetOrders] 
    @DateFrom = '2025-11-01', 
    @DateTo = '2025-11-08'
```

**Expected Result:** Returns rows with order data.

**Document the Results:**
```
Column Name          | Data Type | Sample Value
---------------------|-----------|------------------
ID                   | int       | 6688
Courier              | varchar   | CA
CustomerName         | varchar   | James Gallo
ShipTo               | varchar   | James Gallo, Risby ON, CA
Status               | varchar   | Ready to Ship
ShipOn               | datetime  | 2025-11-08
TrackingNo           | varchar   | 1Z999AA10123456784
PkgCnt               | int       | 2
```

**Checklist:**
- [ ] Procedure executes without errors
- [ ] Returns data for valid date range
- [ ] Column names documented
- [ ] Data types documented
- [ ] Sample values recorded

---

### Task 1.3: Test GetOrdersEndOfDay Stored Procedure

**Action:** Execute with sample dates:

```sql
-- Test GetOrdersEndOfDay
EXEC [dbo].[GetOrdersEndOfDay] 
    @DateFrom = '2025-11-01', 
    @DateTo = '2025-11-08'
```

**Expected Result:** Returns summary statistics.

**Document the Results:**
```
Column Name     | Data Type | Sample Value
----------------|-----------|-------------
TotalOrders     | int       | 156
Shipped         | int       | 142
Pending         | int       | 14
OnHold          | int       | 3
```

**Checklist:**
- [ ] Procedure executes without errors
- [ ] Returns summary data
- [ ] Column names documented
- [ ] Understand what each metric means

---

### Task 1.4: Verify Database Connection from Web App

**Action:** Test DBAccess class connectivity:

Create a test page temporarily:

**File:** `Pages/TestDB.aspx.cs`

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    try
    {
        string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        DBAccess _DB = new DBAccess(encryptionKey);
        
        using (SqlConnection con = _DB.GetConnection())
        {
            Response.Write($"<h2>Database Connection Test</h2>");
            Response.Write($"<p>Connection State: {con.State}</p>");
            Response.Write($"<p>Server: {_DB.ServerName}</p>");
            Response.Write($"<p>Database: {_DB.DatabaseName}</p>");
            Response.Write($"<p style='color:green;'>✅ Connection Successful!</p>");
        }
    }
    catch (Exception ex)
    {
        Response.Write($"<p style='color:red;'>❌ Connection Failed: {ex.Message}</p>");
    }
}
```

**Expected Result:** Page displays "Connection Successful!"

**Checklist:**
- [ ] Web.config has correct connection string
- [ ] Encryption key is configured
- [ ] DBAccess class connects successfully
- [ ] Can query database from web app

---

### Task 1.5: Test Stored Procedure from Web App

**Action:** Execute GetOrders from code:

```csharp
protected void TestGetOrders()
{
    try
    {
        string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        DBAccess _DB = new DBAccess(encryptionKey);
        
        DataSet ds = new DataSet();
        using (SqlConnection con = _DB.GetConnection())
        {
            using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _DB.CommandTimeout;
                cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) 
                    { Value = DateTime.Today });
                cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) 
                    { Value = DateTime.Today });
                
                using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                {
                    ad.Fill(ds);
                }
            }
        }
        
        Response.Write($"<h3>GetOrders Test</h3>");
        Response.Write($"<p>Rows returned: {ds.Tables[0].Rows.Count}</p>");
        Response.Write($"<p>Columns: {ds.Tables[0].Columns.Count}</p>");
        
        // List column names
        Response.Write("<h4>Column Names:</h4><ul>");
        foreach (DataColumn col in ds.Tables[0].Columns)
        {
            Response.Write($"<li>{col.ColumnName} ({col.DataType.Name})</li>");
        }
        Response.Write("</ul>");
    }
    catch (Exception ex)
    {
        Response.Write($"<p style='color:red;'>Error: {ex.Message}</p>");
    }
}
```

**Expected Result:** Displays row count and column names.

**Checklist:**
- [ ] Stored procedure executes from web app
- [ ] Data is returned successfully
- [ ] Column names match expectations
- [ ] No timeout errors

---

## ✅ Deliverables

At the end of Phase 1, you should have:

1. **Documentation:**
   - [ ] List of all stored procedures with parameters
   - [ ] Column names and data types from GetOrders
   - [ ] Summary fields from GetOrdersEndOfDay
   - [ ] Sample data for reference

2. **Verification:**
   - [ ] All stored procedures exist in database
   - [ ] Stored procedures execute successfully
   - [ ] Web app can connect to database
   - [ ] Web app can call stored procedures

3. **Test Results:**
   - [ ] Screenshot of SSMS query results
   - [ ] Test page showing successful connection
   - [ ] Test page showing data retrieval

---

## 🧪 Testing Checklist

- [ ] Can execute GetOrders in SSMS
- [ ] Can execute GetOrdersEndOfDay in SSMS
- [ ] DBAccess class connects from web app
- [ ] Can call GetOrders from web app
- [ ] Data structure documented
- [ ] No permission errors
- [ ] No timeout errors

---

## 🚨 Common Issues & Solutions

### Issue 1: Stored Procedure Not Found
**Error:** `Could not find stored procedure 'dbo.GetOrders'`  
**Solution:** 
- Verify procedure exists: `SELECT * FROM sys.procedures WHERE name = 'GetOrders'`
- Check spelling and schema (dbo)
- Ensure user has EXECUTE permission

### Issue 2: Connection String Error
**Error:** `Login failed for user`  
**Solution:**
- Verify connection string in Web.config
- Check SQL Server authentication mode
- Verify user credentials

### Issue 3: Timeout Error
**Error:** `Timeout expired`  
**Solution:**
- Increase CommandTimeout in DBAccess
- Check if stored procedure is slow
- Verify database server is responsive

### Issue 4: Encryption Key Missing
**Error:** `EncryptionKey not found in configuration`  
**Solution:**
- Add to Web.config: `<add key="EncryptionKey" value="YourKeyHere"/>`
- Verify key matches WinForms app

---

## 📚 Reference

### From FrmOrders.cs
```csharp
// Line 1244-1256: How WinForms calls GetOrders
using (SqlCommand cmd = new SqlCommand("[dbo].[GetOrders]", con)
{
    CommandTimeout = _DB.CommandTimeout,
    CommandType = CommandType.StoredProcedure
})
{
    cmd.Parameters.Add(new SqlParameter() 
        { ParameterName = "@DateFrom", SqlDbType = SqlDbType.DateTime, 
          Direction = ParameterDirection.Input, Value = dtmFromShipRpt });
    cmd.Parameters.Add(new SqlParameter() 
        { ParameterName = "@DateTo", SqlDbType = SqlDbType.DateTime, 
          Direction = ParameterDirection.Input, Value = dtmToShipRpt });
    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
    {
        ad.Fill(_dsOrd);
    }
}
```

---

## ➡️ Next Phase
Once Phase 1 is complete, proceed to **Phase 2: Basic Page Layout**
