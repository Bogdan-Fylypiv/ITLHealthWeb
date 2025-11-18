# Phase 3: Filter Controls (User Input)

## 🎯 Objective
Add date range filters and action buttons to allow users to specify which orders to display.

## ⏱️ Estimated Time
1 hour

---

## 📋 Prerequisites
- Phase 1: Database verified ✅
- Phase 2: Page layout created ✅

---

## 📝 Task 3.1: Add Filter Controls to Orders.aspx

**Action:** Replace the placeholder in the filter-controls div.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx`

**Find this section:**
```aspx
<div class="filter-controls">
    <!-- Filter controls will be added in Phase 3 -->
    <p class="placeholder-text">Date filters will appear here</p>
</div>
```

**Replace with:**
```aspx
<div class="filter-controls">
    <div class="filter-group">
        <label for="<%= TxtFromDate.ClientID %>">From Date:</label>
        <asp:TextBox ID="TxtFromDate" runat="server" 
            TextMode="Date" 
            CssClass="form-control date-picker"></asp:TextBox>
    </div>
    
    <div class="filter-group">
        <label for="<%= TxtToDate.ClientID %>">To Date:</label>
        <asp:TextBox ID="TxtToDate" runat="server" 
            TextMode="Date" 
            CssClass="form-control date-picker"></asp:TextBox>
    </div>
    
    <div class="filter-group">
        <asp:Button ID="BtnFetch" runat="server" 
            Text="📥 Fetch Orders" 
            OnClick="BtnFetch_Click" 
            CssClass="btn btn-primary" />
    </div>
    
    <div class="filter-group">
        <asp:Button ID="BtnClear" runat="server" 
            Text="🔄 Clear" 
            OnClick="BtnClear_Click" 
            CssClass="btn btn-secondary" />
    </div>
</div>
```

**Checklist:**
- [ ] TxtFromDate added with TextMode="Date"
- [ ] TxtToDate added with TextMode="Date"
- [ ] BtnFetch added with OnClick event
- [ ] BtnClear added with OnClick event
- [ ] All controls have proper CSS classes
- [ ] Labels use ClientID for accessibility

---

## 📝 Task 3.2: Update Orders.aspx.designer.cs

**Action:** Add the new control declarations.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.designer.cs`

**Add after existing controls:**
```csharp
/// <summary>
/// TxtFromDate control.
/// </summary>
/// <remarks>
/// Auto-generated field.
/// To modify move field declaration from designer file to code-behind file.
/// </remarks>
protected global::System.Web.UI.WebControls.TextBox TxtFromDate;

/// <summary>
/// TxtToDate control.
/// </summary>
/// <remarks>
/// Auto-generated field.
/// To modify move field declaration from designer file to code-behind file.
/// </remarks>
protected global::System.Web.UI.WebControls.TextBox TxtToDate;

/// <summary>
/// BtnFetch control.
/// </summary>
/// <remarks>
/// Auto-generated field.
/// To modify move field declaration from designer file to code-behind file.
/// </remarks>
protected global::System.Web.UI.WebControls.Button BtnFetch;

/// <summary>
/// BtnClear control.
/// </summary>
/// <remarks>
/// Auto-generated field.
/// To modify move field declaration from designer file to code-behind file.
/// </remarks>
protected global::System.Web.UI.WebControls.Button BtnClear;
```

**Checklist:**
- [ ] All four controls declared
- [ ] Correct data types used (TextBox, Button)
- [ ] Names match ASPX file exactly
- [ ] Namespace is global::System.Web.UI.WebControls

---

## 📝 Task 3.3: Implement InitializeFilters Method

**Action:** Add method to set default filter values.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method (based on FrmOrders.cs lines 149-150):**
```csharp
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
```

**Checklist:**
- [ ] Method created
- [ ] Default dates set to today
- [ ] Date format is "yyyy-MM-dd" (HTML5 date input format)
- [ ] Welcome message displayed
- [ ] Summary cleared

---

## 📝 Task 3.4: Update Page_Load to Initialize Filters

**Action:** Call InitializeFilters on first page load.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Update the Page_Load method:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        InitializeFilters();
    }
}
```

**Checklist:**
- [ ] InitializeFilters() called
- [ ] Only called on first load (!IsPostBack)
- [ ] Method exists in class

---

## 📝 Task 3.5: Implement BtnFetch_Click Event Handler

**Action:** Add validation and data loading logic.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method:**
```csharp
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

        // Check if date range is too large (optional business rule)
        TimeSpan dateRange = dateTo - dateFrom;
        if (dateRange.TotalDays > 90)
        {
            ShowMessage("⚠️ Date range cannot exceed 90 days. Please select a smaller range.", "error");
            return;
        }

        // Success - show message
        ShowMessage($"✅ Fetching orders from {dateFrom:dd-MMM-yyyy} to {dateTo:dd-MMM-yyyy}...", "success");
        
        // TODO: Phase 4 - Load mock data
        // TODO: Phase 5 - Load real data from database
        // LoadOrders(dateFrom, dateTo);
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ Error: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"BtnFetch_Click Error: {ex.Message}");
    }
}
```

**Checklist:**
- [ ] Method created with correct signature
- [ ] Empty date validation
- [ ] Date parsing validation
- [ ] Date range validation (From <= To)
- [ ] 90-day limit validation
- [ ] Success message shows date range
- [ ] Error handling implemented
- [ ] Debug logging added
- [ ] TODO comments for future phases

---

## 📝 Task 3.6: Implement BtnClear_Click Event Handler

**Action:** Add clear/reset functionality.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method:**
```csharp
/// <summary>
/// Handle Clear button click
/// Resets all filters and clears data
/// </summary>
protected void BtnClear_Click(object sender, EventArgs e)
{
    // Reset filters to defaults
    InitializeFilters();
    
    // Clear any loaded data (will be implemented in Phase 4)
    // GvOrders.DataSource = null;
    // GvOrders.DataBind();
    
    // Clear summary
    LblSummary.Text = string.Empty;
    
    ShowMessage("✅ Filters cleared", "success");
}
```

**Checklist:**
- [ ] Method created
- [ ] InitializeFilters() called
- [ ] Summary cleared
- [ ] Success message displayed
- [ ] TODO comments for grid clearing (Phase 4)

---

## 📝 Task 3.7: Ensure ShowMessage Method Exists

**Action:** Verify or create the ShowMessage helper method.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**If not already present, add this method:**
```csharp
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
```

**Checklist:**
- [ ] Method exists
- [ ] Sets message text
- [ ] Sets CSS class based on type
- [ ] Makes label visible

---

## 📝 Task 3.8: Add Filter Styles to Orders.css

**Action:** Add comprehensive styling for filter controls.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Content\Orders.css`

**Add these styles (if not already present from Phase 2):**
```css
/* ===================================
   Filter Controls Styles
   =================================== */

/* Filter Group Container */
.filter-group {
    display: flex;
    flex-direction: column;
    gap: 6px;
    min-width: 150px;
}

.filter-group label {
    font-weight: 600;
    color: #495057;
    font-size: 0.9em;
    margin-bottom: 2px;
}

/* Form Controls */
.form-control {
    padding: 10px 12px;
    border: 1px solid #ced4da;
    border-radius: 5px;
    font-size: 1em;
    transition: border-color 0.3s, box-shadow 0.3s;
    font-family: inherit;
}

.form-control:focus {
    outline: none;
    border-color: #0066cc;
    box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
}

.form-control:disabled {
    background-color: #e9ecef;
    cursor: not-allowed;
}

.date-picker {
    min-width: 160px;
    cursor: pointer;
}

/* Button Styles */
.btn {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    font-size: 1em;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    margin-top: 22px; /* Align with inputs that have labels */
    white-space: nowrap;
}

.btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.btn:active {
    transform: translateY(0);
}

.btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
    transform: none;
}

.btn-primary {
    background: linear-gradient(135deg, #0066cc 0%, #0052a3 100%);
    color: white;
}

.btn-primary:hover {
    background: linear-gradient(135deg, #0052a3 0%, #003d7a 100%);
}

.btn-secondary {
    background: linear-gradient(135deg, #6c757d 0%, #545b62 100%);
    color: white;
}

.btn-secondary:hover {
    background: linear-gradient(135deg, #545b62 0%, #3d4349 100%);
}

/* Responsive Filters */
@media (max-width: 768px) {
    .filter-controls {
        flex-direction: column;
        align-items: stretch;
    }
    
    .filter-group {
        width: 100%;
        min-width: auto;
    }
    
    .btn {
        margin-top: 0;
        width: 100%;
    }
    
    .date-picker {
        min-width: auto;
        width: 100%;
    }
}

@media (max-width: 480px) {
    .filter-section {
        padding: 15px;
    }
    
    .filter-group label {
        font-size: 0.85em;
    }
    
    .form-control,
    .btn {
        font-size: 0.9em;
        padding: 8px 10px;
    }
}
```

**Checklist:**
- [ ] Filter group styles added
- [ ] Form control styles added
- [ ] Button styles added (primary and secondary)
- [ ] Hover effects implemented
- [ ] Focus states styled
- [ ] Disabled states styled
- [ ] Responsive styles added for mobile
- [ ] Touch-friendly sizing on mobile

---

## ✅ Deliverables

At the end of Phase 3, you should have:

1. **Filter Controls:**
   - [ ] From Date picker (TxtFromDate)
   - [ ] To Date picker (TxtToDate)
   - [ ] Fetch Orders button (BtnFetch)
   - [ ] Clear button (BtnClear)

2. **Functionality:**
   - [ ] Default dates set to today on page load
   - [ ] Date validation (empty, invalid format, range)
   - [ ] 90-day limit validation
   - [ ] Error messages display correctly
   - [ ] Success messages display correctly
   - [ ] Clear button resets everything

3. **Styling:**
   - [ ] Professional button appearance
   - [ ] Hover effects work
   - [ ] Focus states visible
   - [ ] Responsive on mobile
   - [ ] Touch-friendly on tablets/phones

4. **Code Quality:**
   - [ ] Event handlers implemented
   - [ ] Validation logic complete
   - [ ] Error handling in place
   - [ ] Debug logging added
   - [ ] Comments and TODOs for future phases

---

## 🧪 Testing Checklist

### Functional Testing
- [ ] Page loads with today's date in both fields
- [ ] Can select different dates using date picker
- [ ] Can type dates manually
- [ ] Click "Fetch Orders" with valid dates shows success message
- [ ] Click "Clear" resets dates to today
- [ ] Error message shows if From Date is empty
- [ ] Error message shows if To Date is empty
- [ ] Error message shows if From Date is invalid format
- [ ] Error message shows if To Date is invalid format
- [ ] Error message shows if From Date > To Date
- [ ] Error message shows if date range > 90 days
- [ ] Success message shows selected date range

### Visual Testing
- [ ] Date pickers display correctly
- [ ] Buttons are properly aligned with date inputs
- [ ] Labels are readable and properly positioned
- [ ] Hover effects work on buttons
- [ ] Focus states visible on date inputs
- [ ] Message label changes color based on type (success=green, error=red, info=blue)
- [ ] Emojis display in buttons and messages

### Responsive Testing
- [ ] Desktop (1920x1080): Filters in horizontal row
- [ ] Tablet (768px): Filters stack vertically
- [ ] Mobile (375px): Filters full-width, stacked
- [ ] Buttons full-width on mobile
- [ ] Touch targets large enough (min 44px height)
- [ ] No horizontal scrolling
- [ ] Date pickers work on mobile devices

### Browser Testing
- [ ] Works in Chrome
- [ ] Works in Edge
- [ ] Works in Firefox
- [ ] Date picker UI matches browser
- [ ] No console errors
- [ ] Validation messages display correctly

### Accessibility Testing
- [ ] Labels associated with inputs (for attribute)
- [ ] Tab order is logical (From Date → To Date → Fetch → Clear)
- [ ] Can operate with keyboard only
- [ ] Focus indicators visible
- [ ] Error messages announced by screen readers

---

## 🚨 Common Issues & Solutions

### Issue 1: Date Picker Not Showing
**Symptom:** Text input instead of date picker  
**Solution:**
- Verify TextMode="Date" on TextBox
- Check browser supports HTML5 date input (all modern browsers do)
- For older browsers, consider jQuery UI datepicker
- Test in different browsers

### Issue 2: Date Format Mismatch
**Symptom:** "Invalid Date format" error with valid dates  
**Solution:**
- Use "yyyy-MM-dd" format for HTML5 date inputs
- Use DateTime.TryParse() for parsing
- Check server culture settings
- Verify date format in validation

### Issue 3: Buttons Not Aligned
**Symptom:** Buttons appear lower than date inputs  
**Solution:**
- Add `margin-top: 22px` to .btn class
- This accounts for label height above inputs
- Adjust value if labels are different height

### Issue 4: Validation Not Working
**Symptom:** Can submit empty dates  
**Solution:**
- Verify OnClick event is wired up
- Check BtnFetch_Click method exists
- Ensure validation code runs before data loading
- Check for JavaScript errors in console

### Issue 5: Clear Button Doesn't Reset
**Symptom:** Dates don't reset to today  
**Solution:**
- Verify BtnClear_Click calls InitializeFilters()
- Check InitializeFilters() sets Text property
- Ensure format is "yyyy-MM-dd"
- Rebuild solution

### Issue 6: Message Not Displaying
**Symptom:** No feedback when clicking buttons  
**Solution:**
- Verify LblMessage exists in ASPX
- Check ShowMessage() method exists
- Ensure LblMessage.Visible = true
- Check CSS classes are defined
- Clear browser cache

---

## 📊 Validation Rules Summary

| Rule | Validation | Error Message |
|------|------------|---------------|
| From Date Required | !string.IsNullOrEmpty() | Please select both From and To dates |
| To Date Required | !string.IsNullOrEmpty() | Please select both From and To dates |
| From Date Valid | DateTime.TryParse() | Invalid From Date format |
| To Date Valid | DateTime.TryParse() | Invalid To Date format |
| Date Range | dateFrom <= dateTo | From Date cannot be after To Date |
| Max Range | dateTo - dateFrom <= 90 days | Date range cannot exceed 90 days |

---

## 📚 Reference

### From FrmOrders.cs
- **Lines 149-150:** Default date initialization
```csharp
DtpToShipRpt.Value = DateTime.Today;
DtpFromShipRpt.Value = DateTime.Today;
```

### Date Format Conversion
**WinForms DateTimePicker:**
```csharp
DtpFromShipRpt.Value = DateTime.Today;
DateTime date = DtpFromShipRpt.Value;
```

**Web HTML5 Date Input:**
```csharp
TxtFromDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
DateTime date = DateTime.Parse(TxtFromDate.Text);
```

---

## 💡 Best Practices Applied

1. **Validation First:** All validation before any processing
2. **User Feedback:** Clear messages for every action
3. **Error Handling:** Try-catch with logging
4. **Accessibility:** Labels, tab order, keyboard support
5. **Responsive:** Mobile-first design
6. **Progressive Enhancement:** Works without JavaScript
7. **Consistent Styling:** Matches overall site design

---

## ➡️ Next Phase
Once Phase 3 is complete, proceed to **Phase 4: Orders Grid with Mock Data** to create the display grid.
