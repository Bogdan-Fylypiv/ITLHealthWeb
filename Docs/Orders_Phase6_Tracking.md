# Phase 6: Tracking Functionality (Package Tracking)

## 🎯 Objective
Implement tracking functionality that opens carrier tracking pages based on tracking number format.

## ⏱️ Estimated Time
1 hour

---

## 📋 Prerequisites
- Phase 1: Database verified ✅
- Phase 2: Page layout created ✅
- Phase 3: Filter controls implemented ✅
- Phase 4: Mock grid working ✅
- Phase 5: Real data integrated ✅

---

## 📝 Task 6.1: Implement GetTrackingUrl Method

**Action:** Create method to generate tracking URLs based on carrier detection.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Add this method (based on FrmOrders.cs tracking logic lines 1495-1630):**
```csharp
/// <summary>
/// Get tracking URL based on tracking number format
/// Detects carrier from tracking number pattern
/// Based on FrmOrders.cs tracking integration (simplified for web)
/// </summary>
/// <param name="trackingNo">Tracking number</param>
/// <returns>Carrier tracking URL</returns>
private string GetTrackingUrl(string trackingNo)
{
    if (string.IsNullOrEmpty(trackingNo))
    {
        return string.Empty;
    }
    
    // Remove any whitespace
    trackingNo = trackingNo.Trim();
    
    // UPS: Starts with "1Z" (18 characters)
    // Example: 1Z999AA10123456784
    if (trackingNo.StartsWith("1Z", StringComparison.OrdinalIgnoreCase) && trackingNo.Length == 18)
    {
        return $"https://www.ups.com/track?loc=en_US&tracknum={trackingNo}";
    }
    
    // FedEx: 12 or 15 digits
    // Example: 123456789012 or 123456789012345
    if (trackingNo.Length == 12 || trackingNo.Length == 15)
    {
        // Check if all numeric
        if (trackingNo.All(char.IsDigit))
        {
            return $"https://www.fedex.com/fedextrack/?trknbr={trackingNo}";
        }
    }
    
    // FedEx: Can also be 20 digits
    if (trackingNo.Length == 20 && trackingNo.All(char.IsDigit))
    {
        return $"https://www.fedex.com/fedextrack/?trknbr={trackingNo}";
    }
    
    // CanPar: Starts with "D" or "C" (11 characters)
    // Example: D42510139000001525001
    if ((trackingNo.StartsWith("D", StringComparison.OrdinalIgnoreCase) || 
         trackingNo.StartsWith("C", StringComparison.OrdinalIgnoreCase)) && 
        trackingNo.Length >= 11)
    {
        return $"https://www.canpar.com/en/track/TrackingAction.do?reference={trackingNo}";
    }
    
    // Purolator: 12 digits starting with specific patterns
    if (trackingNo.Length == 12 && trackingNo.All(char.IsDigit))
    {
        return $"https://www.purolator.com/en/ship-track/tracking-details.page?pin={trackingNo}";
    }
    
    // Canada Post: 16 characters alphanumeric
    if (trackingNo.Length == 16)
    {
        return $"https://www.canadapost-postescanada.ca/track-reperage/en#/search?searchFor={trackingNo}";
    }
    
    // USPS: Various formats
    if (trackingNo.Length == 20 || trackingNo.Length == 22)
    {
        return $"https://tools.usps.com/go/TrackConfirmAction?tLabels={trackingNo}";
    }
    
    // Default: Google search for tracking number
    return $"https://www.google.com/search?q={trackingNo}+tracking";
}
```

**Checklist:**
- [ ] Method created with correct signature
- [ ] UPS detection (starts with "1Z", 18 chars)
- [ ] FedEx detection (12, 15, or 20 digits)
- [ ] CanPar detection (starts with "D" or "C")
- [ ] Purolator detection (12 digits)
- [ ] Canada Post detection (16 chars)
- [ ] USPS detection (20 or 22 chars)
- [ ] Fallback to Google search
- [ ] Returns empty string for null/empty input

---

## 📝 Task 6.2: Update LnkTrack_Click Event Handler

**Action:** Replace placeholder with actual tracking functionality.

**File:** `d:\VS_2022Projects\ITLHealthWeb\Pages\Orders.aspx.cs`

**Find and replace the LnkTrack_Click method:**
```csharp
/// <summary>
/// Handle track link click
/// Opens carrier tracking page in new window
/// </summary>
protected void LnkTrack_Click(object sender, EventArgs e)
{
    try
    {
        LinkButton btn = (LinkButton)sender;
        string trackingNo = btn.CommandArgument;
        
        if (string.IsNullOrEmpty(trackingNo))
        {
            ShowMessage("⚠️ No tracking number available for this order.", "error");
            return;
        }
        
        // Get tracking URL
        string trackingUrl = GetTrackingUrl(trackingNo);
        
        if (string.IsNullOrEmpty(trackingUrl))
        {
            ShowMessage("⚠️ Unable to generate tracking URL.", "error");
            return;
        }
        
        // Open tracking page in new window using JavaScript
        string script = $"window.open('{trackingUrl}', '_blank', 'width=1200,height=800,scrollbars=yes,resizable=yes');";
        ScriptManager.RegisterStartupScript(this, GetType(), "TrackPackage", script, true);
        
        // Show confirmation message
        ShowMessage($"🔍 Opening tracking for: {trackingNo}", "success");
    }
    catch (Exception ex)
    {
        ShowMessage($"❌ Error opening tracking: {ex.Message}", "error");
        System.Diagnostics.Debug.WriteLine($"LnkTrack_Click Error: {ex.Message}");
    }
}
```

**Checklist:**
- [ ] Gets tracking number from CommandArgument
- [ ] Validates tracking number exists
- [ ] Calls GetTrackingUrl()
- [ ] Opens URL in new window with JavaScript
- [ ] Shows confirmation message
- [ ] Error handling implemented
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
