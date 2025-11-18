# Phase 4: Orders Grid with Mock Data (Display Foundation)

## 🎯 Objective
Create the GridView structure and populate it with mock data to verify layout and functionality before connecting to the database.

## ⏱️ Estimated Time
2 hours

---

## 📋 Prerequisites
- Phase 1: Database verified ✅
- Phase 2: Page layout created ✅
- Phase 3: Filter controls implemented ✅

---

## 📝 Task 4.1: Add GridView to Orders.aspx

**Action:** Replace the placeholder in the grid-container div.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx`

**Find this section:**
```aspx
<div class="grid-container">
    <!-- GridView will be added in Phase 4 -->
    <p class="placeholder-text">Orders grid will appear here</p>
</div>
```

**Replace with:**
```aspx
<div class="grid-container">
    <asp:GridView ID="GvOrders" runat="server" 
        AutoGenerateColumns="False"
        CssClass="orders-grid"
        EmptyDataText="No orders found for the selected date range."
        OnRowDataBound="GvOrders_RowDataBound"
        AllowPaging="True"
        PageSize="50"
        OnPageIndexChanging="GvOrders_PageIndexChanging">
        
        <Columns>
            <!-- ID / Order Number -->
            <asp:BoundField DataField="ID" HeaderText="ID" 
                ItemStyle-Width="60px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Bill To / Customer Name -->
            <asp:BoundField DataField="BillTo" HeaderText="Bill To / Customer Name" 
                ItemStyle-Width="200px" />
            
            <!-- Ship To (Customer Name, Prov, Country) -->
            <asp:BoundField DataField="ShipTo" HeaderText="Ship To / Customer Name, Prov, Country" 
                ItemStyle-Width="250px" />
            
            <!-- Ship Via (Carrier: FedEx, UPS, CanPar, etc.) -->
            <asp:BoundField DataField="ShipVia" HeaderText="Ship Via" 
                ItemStyle-Width="100px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Quantity -->
            <asp:BoundField DataField="Qty" HeaderText="Qty" 
                ItemStyle-Width="50px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Product Code -->
            <asp:BoundField DataField="ProductCode" HeaderText="Product Code" 
                ItemStyle-Width="150px" />
            
            <!-- Tracking Number -->
            <asp:BoundField DataField="TrackingNo" HeaderText="Tracking No" 
                ItemStyle-Width="180px" 
                ItemStyle-Font-Size="Small" />
            
            <!-- Status -->
            <asp:BoundField DataField="Status" HeaderText="Status" 
                ItemStyle-Width="100px" />
            
            <!-- Completed Date -->
            <asp:BoundField DataField="Completed" HeaderText="Completed" 
                DataFormatString="{0:dd-MMM-yyyy}" 
                ItemStyle-Width="90px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Ship On Date -->
            <asp:BoundField DataField="ShipOn" HeaderText="Ship On" 
                DataFormatString="{0:dd-MMM-yyyy}" 
                ItemStyle-Width="90px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Ship From -->
            <asp:BoundField DataField="ShipFrom" HeaderText="ShipFrom" 
                ItemStyle-Width="80px" 
                ItemStyle-HorizontalAlign="Center" />
            
            <!-- Actions Column -->
            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:LinkButton ID="LnkTrack" runat="server" 
                        Text="Track" 
                        CommandArgument='<%# Eval("TrackingNo") %>'
                        OnClick="LnkTrack_Click"
                        CssClass="track-link"
                        Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        
        <!-- Grid Styling -->
        <HeaderStyle BackColor="#0066cc" ForeColor="White" Font-Bold="True" Height="40px" />
        <RowStyle BackColor="White" Height="35px" />
        <AlternatingRowStyle BackColor="#f8f9fa" />
        <PagerStyle BackColor="#0066cc" ForeColor="White" HorizontalAlign="Center" Height="35px" />
        <EmptyDataRowStyle BackColor="#fff3cd" ForeColor="#856404" HorizontalAlign="Center" Height="50px" />
    </asp:GridView>
</div>
```

**Checklist:**
- [ ] GridView added with ID="GvOrders"
- [ ] AutoGenerateColumns="False"
- [ ] All 12 columns defined (ID, BillTo, ShipTo, ShipVia, Qty, ProductCode, TrackingNo, Status, Completed, ShipOn, ShipFrom, Actions)
- [ ] Courier/Business column NOT displayed (exists in DB but hidden)
- [ ] InvID column NOT displayed (exists in DB but hidden)
- [ ] Track link uses TemplateField
- [ ] Paging enabled (PageSize="50")
- [ ] Styling applied (HeaderStyle, RowStyle, etc.)

---

## 📝 Task 4.2: Update Orders.aspx.designer.cs

**Action:** Add GridView control declaration.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.designer.cs`

**Add after existing controls:**
```csharp
/// <summary>
/// GvOrders control.
/// </summary>
/// <remarks>
/// Auto-generated field.
/// To modify move field declaration from designer file to code-behind file.
/// </remarks>
protected global::System.Web.UI.WebControls.GridView GvOrders;
```

**Checklist:**
- [ ] GvOrders declared
- [ ] Type is GridView
- [ ] Namespace is global::System.Web.UI.WebControls

---

## 📝 Task 4.3: Create Mock Data Method

**Action:** Add method to generate sample order data.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method to the Orders class:**
```csharp
/// <summary>
/// Load mock data for testing grid layout
/// Based on FrmOrders.cs screenshot data
/// </summary>
private void LoadMockOrders()
{
    DataTable dt = new DataTable();
    
    // Define columns matching GetOrders stored procedure and screenshot
    // Note: Courier and InvID exist in DB but are NOT displayed in GridView
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

    // Add mock data rows (from screenshot)
    // ID, BillTo, ShipTo, ShipVia (Carrier), Qty, ProductCode, TrackingNo, Status, Completed, ShipOn, ShipFrom
    // Note: ShipVia = carrier name (FedEx/UPS/CanPar), TrackingNo = actual tracking numbers
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
    ShowMessage($"✅ Loaded {dt.Rows.Count} mock orders for testing", "success");
}
```

**Checklist:**
- [ ] Method created with correct signature
- [ ] DataTable columns match GridView columns
- [ ] At least 15 mock rows added
- [ ] Mix of statuses (Ready to Ship, On Hold, No Invoice, Open Process)
- [ ] Some rows have tracking numbers, some don't
- [ ] Different dates used
- [ ] DataSource and DataBind called

---

## 📝 Task 4.4: Implement GridView Event Handlers

**Action:** Add event handlers for row styling and paging.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add these methods:**
```csharp
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
    LoadMockOrders(); // Reload data for new page
}

/// <summary>
/// Handle track link click (placeholder for Phase 6)
/// </summary>
protected void LnkTrack_Click(object sender, EventArgs e)
{
    LinkButton btn = (LinkButton)sender;
    string trackingNo = btn.CommandArgument;
    
    // Placeholder - will be implemented in Phase 6
    ShowMessage($"🔍 Track clicked for: {trackingNo} (Tracking will be implemented in Phase 6)", "info");
}
```

**Checklist:**
- [ ] GvOrders_RowDataBound implemented
- [ ] Hover effects added
- [ ] Status-based color coding implemented
- [ ] GvOrders_PageIndexChanging implemented
- [ ] LnkTrack_Click placeholder added

---

## 📝 Task 4.5: Update BtnFetch_Click to Load Mock Data

**Action:** Modify the Fetch button to load mock orders.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Find the BtnFetch_Click method and update:**
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

        // Check if date range is too large (optional)
        TimeSpan dateRange = dateTo - dateFrom;
        if (dateRange.TotalDays > 90)
        {
            ShowMessage("⚠️ Date range cannot exceed 90 days.", "error");
            return;
        }

        // Load mock data (Phase 4)
        // In Phase 5, this will be replaced with: LoadOrders(dateFrom, dateTo);
        LoadMockOrders();
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ Error: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"BtnFetch_Click Error: {ex.Message}");
    }
}
```

**Checklist:**
- [ ] Date validation still works
- [ ] LoadMockOrders() called after validation
- [ ] Comment added for Phase 5 replacement
- [ ] Error handling in place

---

## 📝 Task 4.6: Update BtnClear_Click to Clear Grid

**Action:** Add grid clearing to the Clear button.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Update the BtnClear_Click method:**
```csharp
protected void BtnClear_Click(object sender, EventArgs e)
{
    // Reset filters
    InitializeFilters();
    
    // Clear grid
    GvOrders.DataSource = null;
    GvOrders.DataBind();
    
    // Clear summary
    LblSummary.Text = string.Empty;
    
    ShowMessage("✅ Filters cleared", "success");
}
```

**Checklist:**
- [ ] Grid cleared when Clear clicked
- [ ] Summary cleared
- [ ] Filters reset

---

## 📝 Task 4.7: Add GridView Styles to Orders.css

**Action:** Add comprehensive grid styling.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Content\Orders.css`

**Add these styles at the end:**
```css
/* ===================================
   GridView Styles
   =================================== */

.orders-grid {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9em;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.orders-grid th {
    padding: 12px 8px;
    text-align: left;
    white-space: nowrap;
    font-weight: 600;
}

.orders-grid td {
    padding: 10px 8px;
    border-bottom: 1px solid #dee2e6;
    vertical-align: middle;
}

.orders-grid tr:hover {
    background-color: #e6f2ff !important;
    cursor: pointer;
}

/* Track Link */
.track-link {
    color: #0066cc;
    text-decoration: none;
    font-weight: 600;
    padding: 5px 10px;
    border-radius: 4px;
    display: inline-block;
    transition: all 0.3s;
}

.track-link:hover {
    background-color: #0066cc;
    color: white;
    text-decoration: none;
}

/* Empty Data Message */
.orders-grid td[colspan] {
    text-align: center;
    padding: 30px;
    color: #6c757d;
    font-style: italic;
    font-size: 1.1em;
}

/* Pager Styles */
.orders-grid .pager {
    padding: 10px;
}

.orders-grid .pager a {
    color: white;
    padding: 5px 10px;
    margin: 0 2px;
    text-decoration: none;
    border-radius: 3px;
}

.orders-grid .pager a:hover {
    background-color: #0052a3;
}

.orders-grid .pager span {
    color: white;
    padding: 5px 10px;
    margin: 0 2px;
    background-color: #0052a3;
    border-radius: 3px;
    font-weight: bold;
}

/* Responsive Grid */
@media (max-width: 1200px) {
    .orders-grid {
        font-size: 0.85em;
    }
    
    .orders-grid th,
    .orders-grid td {
        padding: 8px 5px;
    }
}

@media (max-width: 768px) {
    .grid-container {
        overflow-x: auto;
        -webkit-overflow-scrolling: touch;
    }
    
    .orders-grid {
        min-width: 1000px;
    }
}
```

**Checklist:**
- [ ] Grid table styles added
- [ ] Track link styled
- [ ] Hover effects defined
- [ ] Pager styled
- [ ] Responsive styles added
- [ ] Empty data message styled

---

## ✅ Deliverables

At the end of Phase 4, you should have:

1. **GridView Structure:**
   - [ ] 8 columns defined
   - [ ] Paging enabled
   - [ ] Styling applied
   - [ ] Track link in Actions column

2. **Mock Data:**
   - [ ] LoadMockOrders() method created
   - [ ] 15+ sample rows
   - [ ] Various statuses represented
   - [ ] Mix of orders with/without tracking

3. **Event Handlers:**
   - [ ] Row data bound for styling
   - [ ] Paging handler
   - [ ] Track link placeholder

4. **Integration:**
   - [ ] Fetch button loads mock data
   - [ ] Clear button clears grid
   - [ ] Grid displays in page layout

5. **Styling:**
   - [ ] Professional grid appearance
   - [ ] Status-based color coding
   - [ ] Hover effects
   - [ ] Responsive design

---

## 🧪 Testing Checklist

### Functional Testing
- [ ] Click "Fetch Orders" displays mock data
- [ ] Grid shows all 15 rows
- [ ] All columns display correctly
- [ ] Paging controls appear if >50 rows (add more mock data to test)
- [ ] Click page numbers changes displayed rows
- [ ] Track links appear only for orders with tracking numbers
- [ ] Track links show placeholder message when clicked
- [ ] Click "Clear" empties the grid

### Visual Testing
- [ ] Grid header is blue with white text
- [ ] Rows alternate white/light gray
- [ ] Hover changes row to light blue
- [ ] "On Hold" orders have yellow background
- [ ] "No Invoice" orders have light red background
- [ ] "Open Process" orders have light blue background
- [ ] Track links are blue and underlined
- [ ] Track links turn white on blue background when hovered
- [ ] Empty message displays when no data

### Responsive Testing
- [ ] Desktop (1920x1080): All columns visible
- [ ] Tablet (768px): Horizontal scroll appears
- [ ] Mobile (375px): Grid scrolls horizontally
- [ ] Touch scrolling works smoothly

### Browser Testing
- [ ] Works in Chrome
- [ ] Works in Edge
- [ ] Works in Firefox
- [ ] No console errors

---

## 🚨 Common Issues & Solutions

### Issue 1: GridView Not Displaying
**Symptom:** Grid area is blank  
**Solution:**
- Check GvOrders is declared in designer.cs
- Verify DataSource is set before DataBind()
- Check for exceptions in Debug output
- Rebuild solution

### Issue 2: Columns Not Showing
**Symptom:** Grid shows but no columns  
**Solution:**
- Verify AutoGenerateColumns="False"
- Check all BoundField DataField names match DataTable columns
- Ensure DataTable has data before binding

### Issue 3: Paging Not Working
**Symptom:** Pager shows but clicking doesn't change pages  
**Solution:**
- Verify OnPageIndexChanging event is wired up
- Check GvOrders_PageIndexChanging method exists
- Ensure LoadMockOrders() is called in paging handler

### Issue 4: Track Links Not Appearing
**Symptom:** Actions column is empty  
**Solution:**
- Check Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>'
- Verify TrackingNo column has data
- Check LnkTrack_Click method exists

### Issue 5: Styling Not Applied
**Symptom:** Grid looks plain  
**Solution:**
- Verify Orders.css is loaded
- Check CssClass="orders-grid" on GridView
- Clear browser cache (Ctrl+F5)
- Check for CSS conflicts with Site.css

---

## 📸 Expected Result

After completing Phase 4, your page should look like:

**Grid Display:**
- Professional table with blue header
- 15 rows of mock order data
- Alternating row colors
- Track links in Actions column
- Paging controls at bottom

**Interactions:**
- Hover over rows highlights them
- Status-based color coding visible
- Track links clickable (show placeholder message)
- Paging works (if you add >50 rows)

---

## 📚 Reference

### From FrmOrders.cs
- Lines 975-1000: Column definitions (AspectGetter patterns)
- Lines 1267-1280: Grid display logic
- Screenshot: Order grid layout and styling

### Column Mapping
| FrmOrders.cs | Web GridView | DataField |
|--------------|--------------|-----------|
| ID | Order# | ID |
| Courier | Courier | Courier |
| Customer Name | Customer | CustomerName |
| Ship To | Ship To | ShipTo |
| Status | Status | Status |
| ShipOn | Ship Date | ShipOn |
| TrackingNo | Tracking# | TrackingNo |
| Context Menu | Actions | (Template) |

---

## ➡️ Next Phase
Once Phase 4 is complete, proceed to **Phase 5: Real Data Integration** to replace mock data with actual database queries.
