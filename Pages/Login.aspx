<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ITLHealthWeb.Pages.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - ITL Health Order Tracking</title>
    <link href="~/Content/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <h2>Order Tracking System</h2>
            
            <asp:Label ID="LblError" runat="server" CssClass="error-message" Visible="false" />
            
            <div class="form-group">
                <label for="TxtUsername">Username</label>
                <asp:TextBox ID="TxtUsername" runat="server" CssClass="form-control" placeholder="Enter your username" />
                <asp:RequiredFieldValidator ID="RfvUsername" runat="server" 
                    ControlToValidate="TxtUsername" 
                    ErrorMessage="Username is required" 
                    CssClass="error-message" 
                    Display="Dynamic" />
            </div>
            
            <div class="form-group">
                <label for="TxtPassword">Password</label>
                <asp:TextBox ID="TxtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" />
                <asp:RequiredFieldValidator ID="RfvPassword" runat="server" 
                    ControlToValidate="TxtPassword" 
                    ErrorMessage="Password is required" 
                    CssClass="error-message" 
                    Display="Dynamic" />
            </div>
            
            <asp:Button ID="BtnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="BtnLogin_Click" />
        </div>
    </form>
</body>
</html>
