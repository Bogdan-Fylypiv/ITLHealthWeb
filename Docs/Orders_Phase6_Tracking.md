# Phase 6: Tracking Functionality (Package Tracking)

## 🎯 Objective
Implement a dedicated Tracking.aspx page that displays detailed package tracking information:
1. Create Tracking.aspx page (web version of SIDITL\Forms\frmTracking.cs)
2. Update Orders.aspx "Track" link to navigate to Tracking.aspx with tracking number
3. Display tracking details: status, delivery date, shipping address, tracking history
4. Call carrier APIs (UPS, FedEx, Purolator, CanPar, etc.) for real-time tracking data
5. Show hierarchical tracking events in expandable grid/list

## ⏱️ Estimated Time
4-5 hours

---

## 📋 Prerequisites
- Phase 1: Database verified ✅
- Phase 2: Page layout created ✅
- Phase 3: Filter controls implemented ✅
- Phase 4: Mock grid working ✅
- Phase 5: Real data integrated ✅
- Carrier API integration classes available (UPSWeb, FedExRest, etc.)

---

## 📦 Implementation Overview

### Files to Create/Modify

| File | Action | Description |
|------|--------|-------------|
| **Orders.aspx** | Modify | Change Track LinkButton to HyperLink |
| **Tracking.aspx** | Create | New tracking page UI |
| **Tracking.aspx.cs** | Create | Tracking page code-behind |
| **Tracking.css** | Create | Tracking page styles |
| **Tracking_CarrierMethods.cs** | Reference | Complete carrier method implementations |

### Architecture
**Orders.aspx** → (Track link with TrackingNo) → **Tracking.aspx** → (Display tracking details)

### Key Components
1. **Tracking.aspx** - UI page with tracking information display
2. **Tracking.aspx.cs** - Code-behind with carrier API integration
3. **Orders.aspx** - Modified Track link to navigate to Tracking.aspx
4. **Database SP** - GetLabelByTrackingNo (already exists in SIDITL)

### Data Flow
1. User clicks "Track" link in Orders.aspx grid
2. Navigate to Tracking.aspx?trackingNo=XXX
3. Tracking.aspx loads tracking number from query string
4. Call GetLabelByTrackingNo SP to get order details (BusID, SiteID, DelMode)
5. Based on DelMode, call appropriate carrier API
6. Display tracking information and history

---

## 📝 Task 6.1: Update Orders.aspx Track Link

**Action:** Modify the Track link to navigate to Tracking.aspx page instead of opening carrier website.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx`

**Current implementation (lines 110-120):**
```aspx
<asp:TemplateField HeaderText="Actions" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
    <ItemTemplate>
        <asp:LinkButton ID="LnkTrack" runat="server" 
            Text="Track" 
            CommandArgument='<%# Eval("TrackingNo") %>'
            OnClick="LnkTrack_Click"
            CssClass="track-link"
            ToolTip='<%# "Track package: " + Eval("TrackingNo") %>'
            Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>' />
    </ItemTemplate>
</asp:TemplateField>
```

**Replace with:**
```aspx
<asp:TemplateField HeaderText="Actions" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
    <ItemTemplate>
        <asp:HyperLink ID="LnkTrack" runat="server" 
            Text="🔍 Track" 
            NavigateUrl='<%# "~/Pages/Tracking.aspx?trackingNo=" + Server.UrlEncode(Eval("TrackingNo").ToString()) %>'
            Target="_blank"
            CssClass="track-link"
            ToolTip='<%# "Track package: " + Eval("TrackingNo") %>'
            Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>' />
    </ItemTemplate>
</asp:TemplateField>
```

**Changes:**
- Changed from `LinkButton` to `HyperLink` (no server-side click needed)
- Set `NavigateUrl` to Tracking.aspx with trackingNo query parameter
- Added `Target="_blank"` to open in new tab
- Added emoji icon for visual appeal
- URL-encoded tracking number for safety

**Checklist:**
- [ ] Changed LinkButton to HyperLink
- [ ] NavigateUrl points to Tracking.aspx
- [ ] TrackingNo passed as query parameter
- [ ] Opens in new tab (Target="_blank")
- [ ] Link only visible when TrackingNo exists

---

## 📝 Task 6.2: Create Tracking.aspx Page

**Action:** Create the main tracking page UI.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Tracking.aspx`

**Create new file with this content (based on frmTracking.cs UI):**
```aspx
<%@ Page Title="Package Tracking" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Tracking.aspx.cs" 
    Inherits="ITLHealthWeb.Pages.Tracking" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Content/Tracking.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="tracking-container">
        <!-- Page Header -->
        <div class="page-header">
            <h2>📦 Package Tracking</h2>
            <asp:Label ID="LblMessage" runat="server" CssClass="message-label"></asp:Label>
        </div>
        
        <!-- Tracking Information Section -->
        <div class="tracking-info-section">
            <div class="info-row">
                <div class="info-group">
                    <label>Tracking Number:</label>
                    <asp:TextBox ID="TxtTrackingNo" runat="server" ReadOnly="true" 
                        CssClass="form-control" />
                </div>
                
                <div class="info-group">
                    <label>Service Type:</label>
                    <asp:TextBox ID="TxtServiceType" runat="server" ReadOnly="true" 
                        CssClass="form-control" />
                </div>
                
                <div class="info-group">
                    <label>Status:</label>
                    <asp:TextBox ID="TxtStatus" runat="server" ReadOnly="true" 
                        CssClass="form-control status-field" />
                </div>
            </div>
            
            <div class="info-row">
                <div class="info-group">
                    <label>Delivery Date:</label>
                    <asp:TextBox ID="TxtDeliveryDate" runat="server" ReadOnly="true" 
                        CssClass="form-control" />
                </div>
                
                <div class="info-group">
                    <label>Order ID:</label>
                    <asp:TextBox ID="TxtOrderID" runat="server" ReadOnly="true" 
                        CssClass="form-control" />
                </div>
                
                <div class="info-group">
                    <label>PO Number:</label>
                    <asp:TextBox ID="TxtPO" runat="server" ReadOnly="true" 
                        CssClass="form-control" />
                </div>
            </div>
            
            <div class="info-row">
                <div class="info-group full-width">
                    <label>Ship To Address:</label>
                    <asp:TextBox ID="TxtShipToAddress" runat="server" ReadOnly="true" 
                        TextMode="MultiLine" Rows="3" CssClass="form-control" />
                </div>
            </div>
            
            <div class="info-row">
                <div class="info-group full-width">
                    <label>Last Location:</label>
                    <asp:TextBox ID="TxtLastLocation" runat="server" ReadOnly="true" 
                        TextMode="MultiLine" Rows="2" CssClass="form-control" />
                </div>
            </div>
            
            <div class="info-row">
                <div class="info-group">
                    <asp:Button ID="BtnWebSite" runat="server" 
                        Text="View on Carrier Website" 
                        OnClick="BtnWebSite_Click" 
                        CssClass="btn btn-primary" />
                    <asp:Button ID="BtnClose" runat="server" 
                        Text="Close" 
                        OnClientClick="window.close(); return false;" 
                        CssClass="btn btn-secondary" />
                </div>
            </div>
        </div>
        
        <!-- Tracking Activity Section -->
        <div class="tracking-activity-section">
            <h3>Tracking Activity</h3>
            <asp:GridView ID="GvActivity" runat="server" 
                AutoGenerateColumns="False"
                CssClass="tracking-grid"
                EmptyDataText="No tracking activity available.">
                
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="#" 
                        ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" />
                    
                    <asp:BoundField DataField="TrackingNo" HeaderText="Tracking No" 
                        ItemStyle-Width="180px" />
                    
                    <asp:BoundField DataField="Status" HeaderText="Status" 
                        ItemStyle-Width="250px" />
                    
                    <asp:BoundField DataField="ActivityDate" HeaderText="Date/Time" 
                        DataFormatString="{0:dd-MMM-yyyy HH:mm}" 
                        ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                    
                    <asp:BoundField DataField="Notes" HeaderText="Notes" 
                        ItemStyle-Width="300px" />
                </Columns>
                
                <HeaderStyle BackColor="#0066cc" ForeColor="White" Font-Bold="True" Height="40px" />
                <RowStyle BackColor="White" Height="35px" />
                <AlternatingRowStyle BackColor="#f8f9fa" />
                <EmptyDataRowStyle BackColor="#fff3cd" ForeColor="#856404" HorizontalAlign="Center" Height="50px" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
```

**Checklist:**
- [ ] Page created with Site.Master
- [ ] ScriptManager added
- [ ] All tracking info fields added (matching frmTracking.cs)
- [ ] GridView for tracking activity
- [ ] Buttons for carrier website and close
- [ ] CSS reference added

---

## 📝 Task 6.3: Create Tracking.aspx.cs Code-Behind

**Action:** Implement the tracking logic and carrier API integration.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Tracking.aspx.cs`

**Create new file with this content (based on frmTracking.cs logic):**
```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using SID.Classes;
using SID.UPSRestful;
using SID.UPSRestful.Models;
using SID.FedExRest.Models;
using SID.PurolatorTrackingSvc;
using SID.CanparTracking;

namespace ITLHealthWeb.Pages
{
    public partial class Tracking : System.Web.UI.Page
    {
        internal DBAccess _DB = new DBAccess(Program.Key);
        internal DataTable _dtTrack;
        internal DataTable _dtItems;
        internal string _sTrackingNumber = "";
        internal int _iBusID = 1;
        internal int _iSiteID = 1;
        internal int _iDelMode = 4; // Default to UPS
        internal Guid _gOrdID;
        internal SID.Classes.Scan _Scan;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Get tracking number from query string
                    _sTrackingNumber = Request.QueryString["trackingNo"];
                    
                    if (string.IsNullOrEmpty(_sTrackingNumber))
                    {
                        ShowMessage("⚠️ No tracking number provided.", "error");
                        return;
                    }
                    
                    // Initialize data tables
                    InitializeDataTables();
                    
                    // Initialize Scan object for carrier API calls
                    _Scan = new SID.Classes.Scan(System.Windows.Forms.Cursors.Default);
                    
                    // Load tracking data
                    SetData(_sTrackingNumber);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error loading tracking: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Page_Load Error: {ex.Message}");
            }
        }

        private void InitializeDataTables()
        {
            _dtTrack = new DataTable("Track");
            _dtTrack.Columns.Add("ID", typeof(int));
            _dtTrack.Columns.Add("TrackingNo", typeof(string));
            _dtTrack.Columns.Add("Status", typeof(string));
            _dtTrack.Columns.Add("ActivityDate", typeof(DateTime));
            _dtTrack.Columns.Add("Notes", typeof(string));
            _dtTrack.Columns.Add("Level", typeof(int));
            _dtTrack.AcceptChanges();
            
            _dtItems = new DataTable("Item");
            _dtItems.Columns.Add("ID", typeof(int));
            _dtItems.Columns.Add("TrackingNo", typeof(string));
            _dtItems.Columns.Add("Status", typeof(string));
            _dtItems.Columns.Add("ActivityDate", typeof(DateTime));
            _dtItems.Columns.Add("Notes", typeof(string));
            _dtItems.Columns.Add("Level", typeof(int));
            _dtItems.AcceptChanges();
        }

        /// <summary>
        /// Load tracking data based on tracking number
        /// Based on frmTracking.cs SetData method (lines 76-170)
        /// </summary>
        private void SetData(string sTrackingNumber)
        {
            DataTable dt = new DataTable();
            try
            {
                _sTrackingNumber = sTrackingNumber;
                
                // Clear existing data
                if (_dtTrack.Rows.Count > 0)
                {
                    _dtItems.Rows.Clear();
                    _dtTrack.Rows.Clear();
                }
                
                // Get label information from database
                using (SqlConnection con = _DB.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[GetLabelByTrackingNo]", con)
                    {
                        CommandTimeout = _DB.CommandTimeout,
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        cmd.Parameters.Add(new SqlParameter() 
                        { 
                            ParameterName = "@TrackingNo", 
                            SqlDbType = SqlDbType.VarChar, 
                            Size = 25, 
                            Direction = ParameterDirection.Input, 
                            Value = _sTrackingNumber 
                        });
                        
                        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(dt);
                        }
                    }
                }
                
                if (dt.Rows.Count > 0)
                {
                    _iBusID = 1;
                    _iSiteID = Convert.ToInt32(dt.Rows[0]["SiteID"]);
                    _iDelMode = Convert.ToInt32(dt.Rows[0]["DelMode"]);
                    _gOrdID = (Guid)dt.Rows[0]["OrdID"];
                }
                
                // Set tracking number
                TxtTrackingNo.Text = _sTrackingNumber;
                
                // Call appropriate carrier tracking method based on DelMode
                // DelMode values: 4=UPS, 8=FedEx, 64=Purolator, 512=CanadaPost, 2048=CanPar, 4096=Loomis, 32=USPS, 2=CPU
                if ((_iDelMode & 4) == 4) // UPS
                {
                    BtnWebSite.Text = "View on UPS Website";
                    GetUPSTrackInfo();
                }
                else if (_iDelMode == 8) // FedEx
                {
                    BtnWebSite.Text = "View on FedEx Website";
                    GetFedExTrackInfo(dt);
                }
                else if (_iDelMode == 64) // Purolator
                {
                    BtnWebSite.Text = "View on Purolator Website";
                    GetPurolatorTrackInfo(dt);
                }
                else if (_iDelMode == 512) // Canada Post
                {
                    BtnWebSite.Text = "View on Canada Post Website";
                    GetCaPostTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
                }
                else if (_iDelMode == 2048) // CanPar
                {
                    BtnWebSite.Text = "View on CanPar Website";
                    GetCanparTrackInfo(dt);
                }
                else if (_iDelMode == 4096) // Loomis
                {
                    BtnWebSite.Text = "View on Loomis Website";
                    GetLoomisTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
                }
                else if (_iDelMode == 32) // USPS
                {
                    BtnWebSite.Text = "View on USPS Website";
                    GetUSPSTrackInfo(_sTrackingNumber, _iBusID, _iSiteID, _iDelMode);
                }
                else if (_iDelMode == 2) // CPU
                {
                    BtnWebSite.Text = "CPU";
                    GetCPU(dt);
                }
                
                // Bind tracking activity to grid
                GvActivity.DataSource = _dtItems;
                GvActivity.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"SetData Error: {ex.Message}");
            }
        }

        // Add carrier-specific tracking methods (stubs - implement based on frmTracking.cs)
        // See frmTracking.cs lines 664-794 for GetUPSTrackInfo implementation
        // See frmTracking.cs lines 529-662 for GetFedExTrackInfo implementation
        // See frmTracking.cs lines 319-381 for GetPurolatorTrackInfo implementation
        // See frmTracking.cs lines 413-503 for GetCanparTrackInfo implementation
        
        private void GetUPSTrackInfo() { /* Implement from frmTracking.cs lines 664-794 */ }
        private void GetFedExTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 529-662 */ }
        private void GetPurolatorTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 319-381 */ }
        private void GetCanparTrackInfo(DataTable dt) { /* Implement from frmTracking.cs lines 413-503 */ }
        private void GetCaPostTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
        private void GetLoomisTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
        private void GetUSPSTrackInfo(string sTrackingNumber, int iBusID, int iSiteID, int iDelmode) { }
        private void GetCPU(DataTable dt) { /* Implement from frmTracking.cs lines 382-400 */ }
        
        /// <summary>
        /// Handle website button click - opens carrier tracking page
        /// Based on frmTracking.cs BtnExWebSite_Click (lines 874-906)
        /// </summary>
        protected void BtnWebSite_Click(object sender, EventArgs e)
        {
            string sURL = "";
            try
            {
                switch (_iDelMode)
                {
                    case 4: // UPS
                        sURL = $"https://www.ups.com/track?loc=en_US&tracknum={TxtTrackingNo.Text}";
                        break;
                    case 8: // FedEx
                        sURL = $"https://www.fedex.com/apps/fedextrack/?action=track&trackingnumber={TxtTrackingNo.Text}";
                        break;
                    case 64: // Purolator
                        sURL = $"https://www.purolator.com/en/shipping/tracker?pins={TxtTrackingNo.Text}";
                        break;
                    case 2048: // CanPar
                        sURL = $"https://www.canpar.com/en/tracking/track.htm?barcode={TxtTrackingNo.Text}";
                        break;
                    case 512: // Canada Post
                        sURL = $"https://www.canadapost-postescanada.ca/track-reperage/en#/search?searchFor={TxtTrackingNo.Text}";
                        break;
                    case 32: // USPS
                        sURL = $"https://tools.usps.com/go/TrackConfirmAction?tLabels={TxtTrackingNo.Text}";
                        break;
                    default:
                        sURL = "";
                        break;
                }
                
                if (sURL.Length > 0)
                {
                    // Open in new window
                    string script = $"window.open('{sURL}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenCarrierSite", script, true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error: {ex.Message}", "error");
            }
        }
        
        /// <summary>
        /// Show message to user
        /// </summary>
        private void ShowMessage(string message, string type)
        {
            LblMessage.Text = message;
            LblMessage.CssClass = $"message-label {type}";
            LblMessage.Visible = true;
        }
    }
}
```

**Note:** The carrier-specific methods are fully implemented in the separate file:
**`d:\VS_2022Projects\ITLHealthWeb\Docs\Tracking_CarrierMethods.cs`**

This file contains complete implementations for:
- ✅ **GetUPSTrackInfo()** - UPS tracking with full activity history
- ✅ **GetFedExTrackInfo(DataTable dt)** - FedEx tracking with scan events
- ✅ **GetPurolatorTrackInfo(DataTable dt)** - Purolator tracking with depot info
- ✅ **GetCanparTrackInfo(DataTable dt)** - CanPar tracking with events
- ✅ **GetCPU(DataTable dt)** - CPU basic info
- ⚠️ **GetCaPostTrackInfo()** - Placeholder (API pending)
- ⚠️ **GetLoomisTrackInfo()** - Placeholder (API pending)
- ⚠️ **GetUSPSTrackInfo()** - Placeholder (API pending)

**Copy all methods from `Tracking_CarrierMethods.cs` into your `Tracking.aspx.cs` file after the SetData method.**

**Checklist:**
- [ ] Page_Load implemented
- [ ] SetData method implemented
- [ ] InitializeDataTables implemented
- [ ] BtnWebSite_Click implemented
- [ ] ShowMessage helper method
- [ ] **Carrier methods copied from Tracking_CarrierMethods.cs**
- [ ] All using statements added

---

## 📝 Task 6.4: Create Tracking.css Stylesheet

**Action:** Create CSS for tracking page styling.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Content\Tracking.css`

**Create new file:**
```css
/* Tracking Page Styles */
.tracking-container {
    max-width: 1400px;
    margin: 20px auto;
    padding: 20px;
    background-color: #ffffff;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.page-header {
    margin-bottom: 30px;
    padding-bottom: 15px;
    border-bottom: 2px solid #0066cc;
}

.page-header h2 {
    color: #0066cc;
    margin: 0 0 10px 0;
}

.message-label {
    display: block;
    padding: 10px 15px;
    margin: 10px 0;
    border-radius: 4px;
    font-weight: bold;
}

.message-label.success {
    background-color: #d4edda;
    color: #155724;
    border: 1px solid #c3e6cb;
}

.message-label.error {
    background-color: #f8d7da;
    color: #721c24;
    border: 1px solid #f5c6cb;
}

/* Tracking Info Section */
.tracking-info-section {
    background-color: #f8f9fa;
    padding: 20px;
    border-radius: 6px;
    margin-bottom: 30px;
}

.info-row {
    display: flex;
    gap: 20px;
    margin-bottom: 15px;
    flex-wrap: wrap;
}

.info-group {
    flex: 1;
    min-width: 250px;
}

.info-group.full-width {
    flex: 100%;
}

.info-group label {
    display: block;
    font-weight: bold;
    color: #333;
    margin-bottom: 5px;
    font-size: 14px;
}

.info-group .form-control {
    width: 100%;
    padding: 8px 12px;
    border: 1px solid #ced4da;
    border-radius: 4px;
    font-size: 14px;
    background-color: #e9ecef;
}

.info-group .status-field {
    font-weight: bold;
    color: #0066cc;
}

/* Buttons */
.btn {
    padding: 10px 20px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    font-weight: bold;
    margin-right: 10px;
    transition: background-color 0.3s;
}

.btn-primary {
    background-color: #0066cc;
    color: white;
}

.btn-primary:hover {
    background-color: #0052a3;
}

.btn-secondary {
    background-color: #6c757d;
    color: white;
}

.btn-secondary:hover {
    background-color: #5a6268;
}

/* Tracking Activity Section */
.tracking-activity-section {
    margin-top: 30px;
}

.tracking-activity-section h3 {
    color: #0066cc;
    margin-bottom: 15px;
    padding-bottom: 10px;
    border-bottom: 2px solid #e9ecef;
}

.tracking-grid {
    width: 100%;
    border-collapse: collapse;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.tracking-grid th {
    background-color: #0066cc;
    color: white;
    padding: 12px;
    text-align: left;
    font-weight: bold;
}

.tracking-grid td {
    padding: 10px 12px;
    border-bottom: 1px solid #dee2e6;
}

.tracking-grid tr:hover {
    background-color: #f1f3f5;
}
```

**Checklist:**
- [ ] File created in Content folder
- [ ] Container styles added
- [ ] Info section styles added
- [ ] Button styles added
- [ ] Grid styles added
- [ ] Responsive design considered

---

## 📝 Task 6.5: Remove Old LnkTrack_Click from Orders.aspx.cs

**Action:** Since we changed to HyperLink, remove the old server-side click handler.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Remove this method if it exists:**
```csharp
protected void LnkTrack_Click(object sender, EventArgs e)
{
    // Remove this entire method - no longer needed
}
```

**Checklist:**
- [ ] Old LnkTrack_Click method removed
- [ ] No compilation errors

---

## ✅ Deliverables

At the end of Phase 6, you should have:

1. **Orders.aspx Updated:**
   - [ ] Track link changed to HyperLink
   - [ ] Links to Tracking.aspx with trackingNo parameter
   - [ ] Opens in new tab

2. **Tracking.aspx Created:**
   - [ ] Page UI with all tracking fields
   - [ ] GridView for tracking activity
   - [ ] Buttons for carrier website and close
   - [ ] CSS styling applied

3. **Tracking.aspx.cs Created:**
   - [ ] Page_Load gets tracking number from query string
   - [ ] SetData method calls GetLabelByTrackingNo SP
   - [ ] Carrier detection based on DelMode
   - [ ] BtnWebSite_Click opens carrier site
   - [ ] Carrier API methods (stubs or implemented)

4. **Tracking.css Created:**
   - [ ] Professional styling
   - [ ] Responsive layout
   - [ ] Consistent with Orders.aspx design

---

## 🧪 Testing Checklist

### Navigation Testing
- [ ] Click Track link in Orders.aspx → Opens Tracking.aspx in new tab
- [ ] Tracking number passed correctly in URL
- [ ] Tracking.aspx loads without errors

### Data Display Testing
- [ ] Tracking number displays correctly
- [ ] Order details load from database
- [ ] Carrier detected correctly based on DelMode
- [ ] Website button text updates based on carrier

### Carrier Website Testing
- [ ] Click "View on Carrier Website" → Opens correct carrier site
- [ ] UPS tracking opens UPS.com
- [ ] FedEx tracking opens FedEx.com
- [ ] CanPar tracking opens CanPar.com
- [ ] Other carriers open correct sites

### Error Handling Testing
- [ ] Invalid tracking number → Shows error message
- [ ] Missing tracking number → Shows error message
- [ ] Database connection error → Handled gracefully

---

## 🚨 Common Issues & Solutions

### Issue 1: Tracking.aspx Not Found
**Symptom:** 404 error when clicking Track link  
**Solution:**
- Verify Tracking.aspx exists in Pages folder
- Check NavigateUrl path is correct: `~/Pages/Tracking.aspx`
- Rebuild solution

### Issue 2: Tracking Number Not Passed
**Symptom:** Tracking page loads but no data  
**Solution:**
- Check query string parameter name matches: `trackingNo`
- Verify URL encoding: `Server.UrlEncode()`
- Check Request.QueryString["trackingNo"] in code-behind

### Issue 3: Carrier API Not Working
**Symptom:** Tracking info doesn't load  
**Solution:**
- Verify carrier API classes are referenced
- Check API credentials in database
- Implement error handling in carrier methods
- Start with BtnWebSite_Click (opens carrier site) before implementing APIs

### Issue 4: GetLabelByTrackingNo SP Missing
**Symptom:** Database error when loading tracking  
**Solution:**
- Verify SP exists in database
- Check SP returns required columns: SiteID, DelMode, OrdID
- Test SP directly in SQL Server Management Studio

---

## 📚 Reference

### From frmTracking.cs (SIDITL Project)
- **Lines 44-75:** Form initialization and data table setup
- **Lines 76-170:** SetData method - main tracking logic
- **Lines 319-381:** GetPurolatorTrackInfo - Purolator API integration
- **Lines 413-503:** GetCanparTrackInfo - CanPar API integration
- **Lines 529-662:** GetFedExTrackInfo - FedEx API integration
- **Lines 664-794:** GetUPSTrackInfo - UPS API integration
- **Lines 874-906:** BtnExWebSite_Click - Open carrier website

### DelMode Values (Carrier Identification)
| DelMode | Carrier | API Class |
|---------|---------|-----------|
| 2 | CPU | N/A |
| 4 | UPS | UPSWeb |
| 8 | FedEx | FedExRest |
| 32 | USPS | N/A |
| 64 | Purolator | PurolatorTrackingSvc |
| 512 | Canada Post | N/A |
| 2048 | CanPar | CanparTracking |
| 4096 | Loomis | N/A |

---

## 💡 Implementation Strategy

### Phase 1: Basic Navigation (30 min)
1. Update Orders.aspx Track link to HyperLink
2. Create Tracking.aspx with basic UI
3. Create Tracking.aspx.cs with Page_Load
4. Test navigation and query string passing

### Phase 2: Database Integration (1 hour)
1. Implement SetData method
2. Call GetLabelByTrackingNo SP
3. Display basic tracking info (tracking number, order ID, PO)
4. Test with real tracking numbers

### Phase 3: Carrier Website Links (30 min)
1. Implement BtnWebSite_Click
2. Test opening carrier websites
3. Verify correct carrier detected

### Phase 4: Carrier API Integration (2-3 hours)
1. Start with one carrier (e.g., UPS)
2. Implement GetUPSTrackInfo method
3. Parse API response and populate fields
4. Bind tracking events to grid
5. Repeat for other carriers as needed

---

## ➡️ Next Phase
Once Phase 6 is complete, proceed to **Phase 7: End of Day Summary** to display summary statistics.
- [ ] Debug logging added

---

## 📝 Task 6.3: Add ScriptManager to Orders.aspx (If Not Present)

**Action:** Ensure ScriptManager exists for JavaScript execution.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx`

**Check if ScriptManager exists in Site.Master or add to Orders.aspx:**

**Option 1: If Site.Master has ScriptManager** (preferred)
- No action needed, skip this task

**Option 2: If no ScriptManager exists, add to Orders.aspx:**
```aspx
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    
    <div class="orders-container">
        <!-- Rest of content -->
    </div>
</asp:Content>
```

**Checklist:**
- [ ] ScriptManager exists (either in Site.Master or Orders.aspx)
- [ ] Only one ScriptManager per page
- [ ] ScriptManager placed before any UpdatePanels or script registration

---

## 📝 Task 6.4: Add Carrier Detection Helper Method (Optional)

**Action:** Add method to identify carrier name for display.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this optional helper method:**
```csharp
/// <summary>
/// Detect carrier name from tracking number
/// Useful for display purposes
/// </summary>
/// <param name="trackingNo">Tracking number</param>
/// <returns>Carrier name</returns>
private string DetectCarrier(string trackingNo)
{
    if (string.IsNullOrEmpty(trackingNo))
    {
        return "Unknown";
    }
    
    trackingNo = trackingNo.Trim();
    
    if (trackingNo.StartsWith("1Z", StringComparison.OrdinalIgnoreCase) && trackingNo.Length == 18)
        return "UPS";
    
    if ((trackingNo.Length == 12 || trackingNo.Length == 15 || trackingNo.Length == 20) && 
        trackingNo.All(char.IsDigit))
        return "FedEx";
    
    if ((trackingNo.StartsWith("D", StringComparison.OrdinalIgnoreCase) || 
         trackingNo.StartsWith("C", StringComparison.OrdinalIgnoreCase)) && 
        trackingNo.Length >= 11)
        return "CanPar";
    
    if (trackingNo.Length == 12 && trackingNo.All(char.IsDigit))
        return "Purolator";
    
    if (trackingNo.Length == 16)
        return "Canada Post";
    
    if (trackingNo.Length == 20 || trackingNo.Length == 22)
        return "USPS";
    
    return "Unknown";
}
```

**Usage Example (optional enhancement):**
```csharp
// In LnkTrack_Click, you could display carrier name:
string carrier = DetectCarrier(trackingNo);
ShowMessage($"🔍 Opening {carrier} tracking for: {trackingNo}", "success");
```

**Checklist:**
- [ ] Method created (optional)
- [ ] Returns carrier name string
- [ ] Matches same logic as GetTrackingUrl()

---

## 📝 Task 6.5: Enhance Track Link Display (Optional)

**Action:** Add carrier icon or tooltip to track link.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx`

**Option 1: Add tooltip to track link:**
```aspx
<asp:TemplateField HeaderText="Actions" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
    <ItemTemplate>
        <asp:LinkButton ID="LnkTrack" runat="server" 
            Text="🔍 Track" 
            CommandArgument='<%# Eval("TrackingNo") %>'
            OnClick="LnkTrack_Click"
            CssClass="track-link"
            ToolTip='<%# "Track package: " + Eval("TrackingNo") %>'
            Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>' />
    </ItemTemplate>
</asp:TemplateField>
```

**Option 2: Show carrier icon (advanced):**
```aspx
<ItemTemplate>
    <asp:LinkButton ID="LnkTrack" runat="server" 
        CommandArgument='<%# Eval("TrackingNo") %>'
        OnClick="LnkTrack_Click"
        CssClass="track-link"
        Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>'>
        <span class='<%# "carrier-icon " + GetCarrierClass(Eval("TrackingNo").ToString()) %>'></span>
        Track
    </asp:LinkButton>
</ItemTemplate>
```

**Checklist:**
- [ ] Tooltip added (optional)
- [ ] Carrier icon added (optional)
- [ ] Track link still functional

---

## 📝 Task 6.6: Test Tracking with Different Carriers

**Action:** Verify tracking works for all carrier types.

### Test Cases

**UPS Tracking:**
```
Tracking Number: 1Z999AA10123456784
Expected: Opens UPS tracking page
URL: https://www.ups.com/track?loc=en_US&tracknum=1Z999AA10123456784
```

**FedEx Tracking:**
```
Tracking Number: 123456789012
Expected: Opens FedEx tracking page
URL: https://www.fedex.com/fedextrack/?trknbr=123456789012
```

**CanPar Tracking:**
```
Tracking Number: D42510139000001525001
Expected: Opens CanPar tracking page
URL: https://www.canpar.com/en/track/TrackingAction.do?reference=D42510139000001525001
```

**Unknown Format:**
```
Tracking Number: ABC123XYZ
Expected: Opens Google search
URL: https://www.google.com/search?q=ABC123XYZ+tracking
```

**Checklist:**
- [ ] UPS tracking opens correct page
- [ ] FedEx tracking opens correct page
- [ ] CanPar tracking opens correct page
- [ ] Unknown format opens Google search
- [ ] New window opens (not same tab)
- [ ] Confirmation message displays

---

## ✅ Deliverables

At the end of Phase 6, you should have:

1. **Tracking URL Generation:**
   - [ ] GetTrackingUrl() method implemented
   - [ ] Supports UPS, FedEx, CanPar, Purolator, Canada Post, USPS
   - [ ] Fallback to Google search
   - [ ] Carrier detection logic

2. **Track Link Functionality:**
   - [ ] LnkTrack_Click opens tracking page
   - [ ] Opens in new window
   - [ ] Shows confirmation message
   - [ ] Error handling for missing tracking numbers

3. **User Experience:**
   - [ ] Track links only show for orders with tracking numbers
   - [ ] Clicking track opens carrier website
   - [ ] User feedback messages
   - [ ] Tooltip shows tracking number (optional)

4. **Testing:**
   - [ ] Tested with real tracking numbers
   - [ ] All carriers work correctly
   - [ ] New window opens properly
   - [ ] Error cases handled

---

## 🧪 Testing Checklist

### Functional Testing
- [ ] Click track link for UPS order → opens UPS site
- [ ] Click track link for FedEx order → opens FedEx site
- [ ] Click track link for CanPar order → opens CanPar site
- [ ] Click track link for unknown format → opens Google
- [ ] Track link only appears for orders with tracking numbers
- [ ] Track link hidden for orders without tracking numbers
- [ ] Confirmation message displays after clicking
- [ ] New window opens (not replacing current page)

### Carrier Detection Testing
- [ ] UPS: 1Z999AA10123456784 → Detected as UPS
- [ ] FedEx: 123456789012 → Detected as FedEx
- [ ] FedEx: 123456789012345 → Detected as FedEx
- [ ] CanPar: D42510139000001525001 → Detected as CanPar
- [ ] Purolator: 123456789012 → Detected correctly
- [ ] Canada Post: 1234567890123456 → Detected as Canada Post
- [ ] Unknown: ABC123 → Falls back to Google

### Browser Testing
- [ ] Works in Chrome
- [ ] Works in Edge
- [ ] Works in Firefox
- [ ] New window opens in all browsers
- [ ] Pop-up blocker doesn't interfere

### Error Handling Testing
- [ ] Empty tracking number → Shows error message
- [ ] Null tracking number → Shows error message
- [ ] Invalid tracking number → Opens Google search
- [ ] JavaScript disabled → Graceful degradation

---

## 🚨 Common Issues & Solutions

### Issue 1: Pop-up Blocked
**Symptom:** New window doesn't open  
**Solution:**
- Browser pop-up blocker is active
- User must allow pop-ups for your site
- Add message: "Please allow pop-ups to view tracking"
- Alternative: Use target="_blank" in anchor tag

### Issue 2: ScriptManager Error
**Symptom:** "Only one instance of ScriptManager can be added to the page"  
**Solution:**
- Check if Site.Master already has ScriptManager
- Remove duplicate ScriptManager from Orders.aspx
- Use ScriptManager.RegisterStartupScript only if ScriptManager exists

### Issue 3: Tracking URL Not Opening
**Symptom:** Click track but nothing happens  
**Solution:**
- Check browser console for JavaScript errors
- Verify GetTrackingUrl() returns valid URL
- Check ScriptManager.RegisterStartupScript syntax
- Ensure script parameter is true (adds script tags)

### Issue 4: Wrong Carrier Detected
**Symptom:** Opens wrong tracking site  
**Solution:**
- Review tracking number format
- Check carrier detection logic order
- Some formats overlap (e.g., FedEx vs Purolator both 12 digits)
- Add more specific detection rules
- Use database Courier field if available

### Issue 5: Track Link Not Visible
**Symptom:** Track link doesn't appear  
**Solution:**
- Check Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>'
- Verify TrackingNo column has data
- Check DataBind() is called
- Verify column name matches database

---

## 📊 Carrier URL Reference

| Carrier | Format | Length | Example | URL Pattern |
|---------|--------|--------|---------|-------------|
| UPS | 1Z + 16 chars | 18 | 1Z999AA10123456784 | ups.com/track |
| FedEx | Numeric | 12, 15, 20 | 123456789012 | fedex.com/fedextrack |
| CanPar | D/C + digits | 11+ | D42510139000001 | canpar.com/track |
| Purolator | Numeric | 12 | 123456789012 | purolator.com/track |
| Canada Post | Alphanumeric | 16 | 1234567890ABCDEF | canadapost.ca/track |
| USPS | Alphanumeric | 20, 22 | 12345678901234567890 | usps.com/track |

---

## 📚 Reference

### From FrmOrders.cs
- **Lines 1495-1520:** UPS tracking API integration
- **Lines 1594-1630:** FedEx tracking API integration
- **Line 3027-3028:** Tracking number vs OrderID detection logic

### Simplified for Web
**WinForms (API calls):**
```csharp
UPSWeb ups = new UPSWeb(1, 1);
object response = ups.UPSGetTrackingInfo(orderId, trackingNo);
// Process API response
```

**Web (Direct URLs):**
```csharp
string url = $"https://www.ups.com/track?tracknum={trackingNo}";
window.open(url, '_blank');
```

**Why Simplified:**
- No API credentials needed
- No API rate limits
- Faster implementation
- User sees carrier's official tracking page
- Can add API integration later if needed

---

## 💡 Future Enhancements (Optional)

### Phase 8 Considerations:
1. **Tracking Status in Grid:**
   - Call carrier APIs to get status
   - Display status in grid (Delivered, In Transit, etc.)
   - Color code by status

2. **Tracking History:**
   - Show tracking events in modal
   - Display delivery date
   - Show current location

3. **Bulk Tracking:**
   - Select multiple orders
   - Open all tracking pages
   - Export tracking numbers

4. **Tracking Notifications:**
   - Email when delivered
   - Alert for exceptions
   - Daily tracking report

---

## ➡️ Next Phase
Once Phase 6 is complete, proceed to **Phase 7: End of Day Summary** to display summary statistics.
