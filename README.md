# ITL Health Web - Order Tracking System

Modern ASP.NET Web Forms application for order tracking and management.

## 🏗️ Project Structure (Industry Standard)

```
ITLHealthWeb/
├── Pages/                      # All web pages
│   ├── Login.aspx             # Authentication page
│   ├── Orders.aspx            # Main orders page
│   ├── Error.aspx             # Error page
│   └── 404.aspx               # Not found page
├── Content/                    # Stylesheets
│   └── Site.css               # Main stylesheet
├── Scripts/                    # JavaScript files (ready for use)
├── lib/                        # Third-party DLLs (included in Git)
│   └── Encryptor.dll          # NetEncrypt library
├── App_Code/                   # Business logic
│   └── UserContext.cs         # Session management helper
├── App_Data/                   # Local data storage
├── Properties/                 # Assembly info
├── Site.Master                 # Master page layout
├── Web.config                  # Configuration
└── Global.asax                 # Application events
```

## ✨ Features

### Architecture
- ✅ **Master Page System** - Consistent layout across all pages
- ✅ **External CSS** - Professional styling with modern gradients
- ✅ **Forms Authentication** - Secure login system
- ✅ **Session Management** - User context tracking
- ✅ **Error Handling** - Global error handler with custom error pages
- ✅ **Responsive Design** - Mobile-friendly layout

### Security
- ✅ **Encrypted Passwords** - Using NetEncrypt library
- ✅ **Session Timeout** - 8 hours (configurable)
- ✅ **Anonymous Access Control** - Proper authorization rules
- ✅ **SQL Injection Protection** - Parameterized queries

### User Experience
- ✅ **Professional UI** - Modern gradient design
- ✅ **Validation** - Client and server-side validation
- ✅ **Error Messages** - User-friendly error display
- ✅ **Navigation** - Consistent header and navigation

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022
- .NET Framework 4.8 or higher
- SQL Server database (ITLHealth)

### Setup Steps

#### 1. Open Solution
```
Double-click: d:\VS_2022Projects\ITLHealthWeb\ITLHealthWeb.sln
```

#### 2. Restore NuGet Packages
Visual Studio will automatically restore NuGet packages on first build:
- `Microsoft.CodeDom.Providers.DotNetCompilerPlatform`
- `System.Data.SqlClient`

Or manually restore:
```
Right-click Solution → Restore NuGet Packages
```

#### 3. Configure Database Connection

Update `Web.config`:
```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="YOUR_CONNECTION_STRING" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

#### 4. Build and Run
1. **Build → Build Solution** (Ctrl+Shift+B)
2. **Press F5** to run
3. Browser opens to: `https://localhost:44314/Pages/Login.aspx`

## 🔐 Authentication

### How It Works
1. User enters username and password
2. Password is decrypted from database using NetEncrypt
3. User info loaded from `[dbo].[GetUserInfo]` stored procedure
4. Session variables set:
   - `EmpGID`, `BusID`, `SiteID`, `EmpRole`, `Username`
5. Redirect to Orders page

### Access UserContext
```csharp
using ITLHealthWeb;

int busID = UserContext.BusID;
Guid empGID = UserContext.EmpGID;
string username = UserContext.Username;
bool isAuth = UserContext.IsAuthenticated;
```

## 📄 Pages

### Login.aspx
- Standalone page (no master page)
- Username/password authentication
- Validation controls
- Error message display

### Orders.aspx
- Uses Site.Master
- Search and filter functionality
- GridView with pagination
- Date range filtering

### Error.aspx
- Global error handler
- User-friendly error message
- "Go to Home" button

### 404.aspx
- Page not found
- Custom 404 page

## 🎨 Styling

### Site.css
Professional stylesheet with:
- Modern gradient color scheme (purple/blue)
- Responsive design
- Button styles
- Form controls
- Grid styling
- Navigation
- Error messages

### Color Palette
- Primary: `#667eea` to `#764ba2` (gradient)
- Background: `#f5f5f5`
- Text: `#333`
- Borders: `#e0e0e0`

## ⚙️ Configuration

### Web.config Key Settings

**Authentication**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Pages/Login.aspx" 
         timeout="480" 
         defaultUrl="~/Pages/Orders.aspx" />
</authentication>
```

**Session**
```xml
<sessionState mode="InProc" timeout="480" />
```

**Error Handling**
```xml
<customErrors mode="On" defaultRedirect="~/Pages/Error.aspx">
  <error statusCode="404" redirect="~/Pages/404.aspx" />
</customErrors>
```

## 🔧 Development

### Adding New Pages

1. **Create in Pages folder**
```aspx
<%@ Page Title="MyPage" Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="MyPage.aspx.cs" 
    Inherits="ITLHealthWeb.Pages.MyPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Your content here -->
</asp:Content>
```

2. **Add to navigation** (Site.Master)
```html
<li><a href="~/Pages/MyPage.aspx" runat="server">My Page</a></li>
```

### Using Stored Procedures

```csharp
string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
DBAccess _DB = new DBAccess(encryptionKey);

using (SqlConnection con = _DB.GetConnection())
{
    using (SqlCommand cmd = new SqlCommand("[dbo].[YourStoredProc]", con))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = _DB.CommandTimeout;
        
        cmd.Parameters.AddWithValue("@Param1", value1);
        
        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            ad.Fill(dt);
            // Use data
        }
    }
}
```

## 📊 Database Requirements

### Tables
- `Employee` - User accounts (UserID, Password, IsActive)
- `Orders` - Order data
- `Business`, `Customer`, `Warehouse` - Related tables

### Stored Procedures
- `[dbo].[GetUserInfo]` - User authentication
- `[dbo].[GetOrdersFrmLoad]` - Load orders (from WinForms)

## 🐛 Troubleshooting

### Build Errors

**"Could not find stored procedure"**
- Verify database connection string in `Web.config`
- Check stored procedure exists in database

**"NuGet packages missing"**
- Right-click Solution → Restore NuGet Packages
- Or run: `dotnet restore`

### Runtime Errors

**Login fails with correct credentials**
- Check encryption key matches WinForms
- Verify Employee table has UserID and Password columns

**"Object reference not set to an instance"**
- Check session variables are set after login
- Use UserContext helper class

**Pages not loading**
- Check authentication is configured
- Verify anonymous access for login page

## 📝 Best Practices

### Code Organization
- ✅ Keep business logic in App_Code
- ✅ Use stored procedures, not inline SQL
- ✅ Use UserContext for session access
- ✅ Handle errors gracefully

### Security
- ✅ Never store passwords in plain text
- ✅ Use parameterized queries
- ✅ Validate all user input
- ✅ Set appropriate session timeouts

### Performance
- ✅ Use paging for large datasets
- ✅ Cache static data
- ✅ Optimize database queries
- ✅ Minimize ViewState

## 🔄 Migration from WinForms

### Reusable Components
- ✅ **DBAccess.cs** - Database access (linked)
- ✅ **Scan.cs** - Business logic (can be linked)
- ✅ **Carrier APIs** - FedEx, UPS, CanPar (can be linked)
- ✅ **Stored Procedures** - All database logic
- ✅ **Encryption** - NetEncrypt library

### Not Reusable
- ❌ UI Controls (WinForms → Web Forms)
- ❌ Event handlers (different model)
- ❌ Threading (different approach)

## 📞 Support

For issues or questions:
1. Check this README
2. Review Web.config settings
3. Check Visual Studio Error List
4. Review Application Event Log

## 📄 License

© 2025 ITL Health. All rights reserved.

---

**Version:** 1.0  
**Last Updated:** November 5, 2025  
**Framework:** ASP.NET Web Forms 4.7.2
