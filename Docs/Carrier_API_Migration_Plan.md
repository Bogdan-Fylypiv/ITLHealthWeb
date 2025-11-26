# 📦 Carrier API Migration Plan
## Copy Carrier Integration Files from SIDITL to ITLHealthWeb

---

## 🎯 Objective

Remove dependencies on the SIDITL WinForms project by copying all carrier API integration files into the ITLHealthWeb project with proper namespace changes.

---

## 📋 Files to Copy

### 1. UPS Restful API
**Source:** `d:\VS_2022Projects\SIDITL\UPSRestful\`  
**Target:** `d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\UPS\`

**Files:**
- `UPSWeb.cs` (Main UPS API class)
- `Models\UPSTrackingInfoResponseModel.cs` (Tracking response)
- `Models\UPSCredentials.cs` (API credentials)
- `Models\UPSAccessTokenResponseModel.cs` (OAuth token)
- `Models\UPSEnums.cs` (Enumerations)
- All other model files needed for tracking

**Namespace Change:**
- FROM: `namespace SID.UPSRestful`
- TO: `namespace ITLHealthWeb.CarrierAPIs.UPS`

---

### 2. FedEx REST API
**Source:** `d:\VS_2022Projects\SIDITL\FedExRest\`  
**Target:** `d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\FedEx\`

**Files:**
- `FedExWeb.cs` (Main FedEx API class)
- `Models\FedExTrackingRequest.cs`
- `Models\FedExTrackingResponse.cs`
- All supporting model files

**Namespace Change:**
- FROM: `namespace SID.FedExRest`
- TO: `namespace ITLHealthWeb.CarrierAPIs.FedEx`

---

### 3. Purolator Tracking Service
**Source:** `d:\VS_2022Projects\SIDITL\Service References\PurolatorTrackingSvc\`  
**Target:** `d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\Purolator\`

**Files:**
- All service reference files
- OR regenerate service reference in ITLHealthWeb

**Namespace Change:**
- FROM: `namespace SID.PurolatorTrackingSvc`
- TO: `namespace ITLHealthWeb.CarrierAPIs.Purolator`

---

### 4. CanPar Tracking Service
**Source:** `d:\VS_2022Projects\SIDITL\Service References\CanparTracking\`  
**Target:** `d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\CanPar\`

**Files:**
- All service reference files
- OR regenerate service reference in ITLHealthWeb

**Namespace Change:**
- FROM: `namespace SID.CanparTracking`
- TO: `namespace ITLHealthWeb.CarrierAPIs.CanPar`

---

### 5. Scan Class (Helper)
**Source:** `d:\VS_2022Projects\SIDITL\Classes\Scan.cs`  
**Target:** `d:\VS_2022Projects\ITLHealthWeb\App_Code\Scan.cs`

**Namespace Change:**
- FROM: `namespace SID.Classes`
- TO: `namespace ITLHealthWeb.App_Code`

---

## 🏗️ Target Folder Structure

```
ITLHealthWeb/
├── CarrierAPIs/
│   ├── UPS/
│   │   ├── UPSWeb.cs
│   │   └── Models/
│   │       ├── UPSTrackingInfoResponseModel.cs
│   │       ├── UPSCredentials.cs
│   │       ├── UPSAccessTokenResponseModel.cs
│   │       └── ... (other models)
│   ├── FedEx/
│   │   ├── FedExWeb.cs
│   │   └── Models/
│   │       ├── FedExTrackingRequest.cs
│   │       ├── FedExTrackingResponse.cs
│   │       └── ... (other models)
│   ├── Purolator/
│   │   └── (Service reference files)
│   └── CanPar/
│       └── (Service reference files)
├── App_Code/
│   ├── Scan.cs (Helper class)
│   ├── UserContext.cs
│   └── DBAccess.cs
└── Pages/
    └── Tracking.aspx
```

---

## 🔧 Migration Steps

### Step 1: Create Folder Structure
```
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\UPS\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\UPS\Models\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\FedEx\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\FedEx\Models\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\Purolator\
Create: d:\VS_2022Projects\ITLHealthWeb\CarrierAPIs\CanPar\
```

### Step 2: Copy UPS Files
1. Copy `SIDITL\UPSRestful\UPSWeb.cs` → `ITLHealthWeb\CarrierAPIs\UPS\UPSWeb.cs`
2. Copy all model files from `SIDITL\UPSRestful\Models\` → `ITLHealthWeb\CarrierAPIs\UPS\Models\`
3. Update namespace in all files: `SID.UPSRestful` → `ITLHealthWeb.CarrierAPIs.UPS`
4. Update using statements to reference new namespaces

### Step 3: Copy FedEx Files
1. Copy `SIDITL\FedExRest\FedExWeb.cs` → `ITLHealthWeb\CarrierAPIs\FedEx\FedExWeb.cs`
2. Copy all model files from `SIDITL\FedExRest\Models\` → `ITLHealthWeb\CarrierAPIs\FedEx\Models\`
3. Update namespace in all files: `SID.FedExRest` → `ITLHealthWeb.CarrierAPIs.FedEx`
4. Update using statements

### Step 4: Copy Scan Helper Class
1. Copy `SIDITL\Classes\Scan.cs` → `ITLHealthWeb\App_Code\Scan.cs`
2. Update namespace: `SID.Classes` → `ITLHealthWeb.App_Code`
3. Update references to carrier APIs to use new namespaces

### Step 5: Handle Service References (Purolator & CanPar)

**Option A: Copy Service Reference Files**
- Copy entire service reference folders
- Update namespaces
- Update app.config/Web.config bindings

**Option B: Regenerate Service References (Recommended)**
1. In ITLHealthWeb project, right-click → Add → Service Reference
2. For Purolator: Use WSDL URL from SIDITL project
3. For CanPar: Use WSDL URL from SIDITL project
4. Set namespace to `ITLHealthWeb.CarrierAPIs.Purolator` and `ITLHealthWeb.CarrierAPIs.CanPar`

### Step 6: Update Tracking.aspx.cs
Update using statements:
```csharp
// OLD (SIDITL dependencies)
using SID.CanparTracking;
using SID.Classes;
using SID.FedExRest.Models;
using SID.PurolatorTrackingSvc;
using SID.UPSRestful;
using SID.UPSRestful.Models;

// NEW (ITLHealthWeb namespaces)
using ITLHealthWeb.CarrierAPIs.CanPar;
using ITLHealthWeb.App_Code;
using ITLHealthWeb.CarrierAPIs.FedEx.Models;
using ITLHealthWeb.CarrierAPIs.Purolator;
using ITLHealthWeb.CarrierAPIs.UPS;
using ITLHealthWeb.CarrierAPIs.UPS.Models;
```

### Step 7: Update Project File
Add new files to `ITLHealthWeb.csproj`:
```xml
<ItemGroup>
  <Compile Include="CarrierAPIs\UPS\UPSWeb.cs" />
  <Compile Include="CarrierAPIs\UPS\Models\*.cs" />
  <Compile Include="CarrierAPIs\FedEx\FedExWeb.cs" />
  <Compile Include="CarrierAPIs\FedEx\Models\*.cs" />
  <Compile Include="App_Code\Scan.cs" />
</ItemGroup>
```

### Step 8: Update Web.config
Add service endpoint configurations for Purolator and CanPar if needed.

---

## ✅ Verification Checklist

### UPS Integration
- [ ] UPSWeb.cs copied and namespace updated
- [ ] All UPS model files copied
- [ ] UPS tracking method compiles
- [ ] Test UPS tracking with real tracking number

### FedEx Integration
- [ ] FedExWeb.cs copied and namespace updated
- [ ] All FedEx model files copied
- [ ] FedEx tracking method compiles
- [ ] Test FedEx tracking with real tracking number

### Purolator Integration
- [ ] Service reference added/copied
- [ ] Namespace updated
- [ ] Purolator tracking method compiles
- [ ] Test Purolator tracking

### CanPar Integration
- [ ] Service reference added/copied
- [ ] Namespace updated
- [ ] CanPar tracking method compiles
- [ ] Test CanPar tracking

### Scan Helper
- [ ] Scan.cs copied and namespace updated
- [ ] All dependencies resolved
- [ ] Compiles without errors

### Overall
- [ ] No references to SID namespaces remain
- [ ] All carrier methods compile
- [ ] Tracking.aspx.cs compiles
- [ ] All using statements updated
- [ ] Project builds successfully

---

## 🚨 Common Issues & Solutions

### Issue 1: Missing Dependencies
**Problem:** UPSWeb or FedExWeb reference other SID classes  
**Solution:** Copy those dependent classes as well, or refactor to remove dependencies

### Issue 2: WinForms Dependencies
**Problem:** Scan.cs uses System.Windows.Forms.Cursor  
**Solution:** 
```csharp
// Remove WinForms cursor parameter
// OLD: public Scan(Cursor cursor)
// NEW: public Scan()
```

### Issue 3: Service Reference Conflicts
**Problem:** Purolator/CanPar service references don't work  
**Solution:** Regenerate service references in ITLHealthWeb project

### Issue 4: Configuration Missing
**Problem:** API credentials not found  
**Solution:** Add to Web.config:
```xml
<appSettings>
  <add key="UPS_ClientID" value="..." />
  <add key="UPS_ClientSecret" value="..." />
  <add key="FedEx_ClientID" value="..." />
  <add key="FedEx_ClientSecret" value="..." />
</appSettings>
```

---

## 📝 Updated Tracking_CarrierMethods.cs

After migration, update the using statements at the top of the file:

```csharp
// Updated using statements for ITLHealthWeb
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using ITLHealthWeb.CarrierAPIs.UPS;
using ITLHealthWeb.CarrierAPIs.UPS.Models;
using ITLHealthWeb.CarrierAPIs.FedEx.Models;
using ITLHealthWeb.CarrierAPIs.Purolator;
using ITLHealthWeb.CarrierAPIs.CanPar;
using ITLHealthWeb.App_Code;
```

---

## 🎯 Next Steps

**Would you like me to:**
1. ✅ Start copying UPS files first?
2. ✅ Copy all carrier API files at once?
3. ✅ Create a PowerShell script to automate the copying?
4. ✅ Show you which specific files need to be copied?

Let me know and I'll proceed with the migration!
