# 🚀 Quick Start Guide

Get up and running in 10 minutes!

## ✅ Prerequisites Checklist

- [ ] Visual Studio 2022 installed
- [ ] SQL Server access to ITLHealth database
- [ ] NetEncrypt.dll available from WinForms project

## 📋 5-Minute Setup

### Step 1: Open Project (30 seconds)
```
Double-click: d:\VS_2022Projects\ITLHealthWeb\ITLHealthWeb.sln
```

### Step 2: Add NetEncrypt Reference (2 minutes)
1. Right-click project → **Add → Reference**
2. **Browse** → `d:\VS_2022Projects\SIDITL\bin\Debug\NetEncrypt.dll`
3. Click **Add**

### Step 3: Link DBAccess (1 minute)
1. Right-click project → **Add → Existing Item**
2. Navigate to: `d:\VS_2022Projects\SIDITL\Classes\DBAccess.cs`
3. Click dropdown on "Add" → **Add As Link** ⚠️

### Step 4: Install NuGet Package (1 minute)
1. Right-click project → **Manage NuGet Packages**
2. Search: `System.Data.SqlClient`
3. Click **Install**

### Step 5: Update Connection String (30 seconds)
Open `Web.config`, update line 14-16:
```xml
<add name="DefaultConnection" 
     connectionString="YOUR_CONNECTION_STRING" 
     providerName="System.Data.SqlClient" />
```

### Step 6: Build & Run (1 minute)
1. Press **Ctrl+Shift+B** (Build)
2. Press **F5** (Run)
3. Login page opens in browser!

## 🎯 First Login

1. **URL:** `https://localhost:44314/Pages/Login.aspx`
2. **Username:** Your Employee.UserID from database
3. **Password:** Your decrypted password
4. Click **Login**
5. Redirects to Orders page!

## 📁 Project Structure at a Glance

```
ITLHealthWeb/
├── Pages/          # All .aspx pages
├── Content/        # CSS files
├── App_Code/       # Business logic
├── Site.Master     # Layout template
└── Web.config      # Configuration
```

## 🔑 Key Files

| File | Purpose |
|------|---------|
| `Pages/Login.aspx` | Login page |
| `Pages/Orders.aspx` | Main orders page |
| `Site.Master` | Page layout |
| `Content/Site.css` | Styling |
| `App_Code/UserContext.cs` | Session helper |
| `Web.config` | Configuration |

## 🎨 What You Get

✅ **Professional UI** - Modern gradient design  
✅ **Master Page** - Consistent layout  
✅ **Authentication** - Secure login  
✅ **Error Handling** - Custom error pages  
✅ **Responsive** - Mobile-friendly  

## 🐛 Common Issues

### Build Error: "Cannot find NetEncrypt.dll"
**Solution:** Add reference to NetEncrypt.dll (Step 2)

### Build Error: "Cannot find DBAccess"
**Solution:** Link DBAccess.cs from WinForms (Step 3)

### Login Fails
**Solution:** Check encryption key in Web.config matches WinForms

### Page Not Found
**Solution:** URLs must include `/Pages/` folder

## 📚 Next Steps

1. ✅ Complete setup above
2. 📖 Read full [README.md](README.md)
3. 🔧 Customize Orders.aspx query
4. 🎨 Adjust styling in Site.css
5. ➕ Add new pages as needed

## 💡 Pro Tips

- Use **UserContext** to access session data
- All pages (except Login) use **Site.Master**
- CSS is in **Content/Site.css**
- Business logic goes in **App_Code/**
- Use stored procedures from WinForms

## 🆘 Need Help?

1. Check [README.md](README.md) for detailed docs
2. Review Web.config settings
3. Check Visual Studio Error List
4. Verify database connection

---

**Ready to code!** 🎉

Press F5 and start building!
