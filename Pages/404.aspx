<%@ Page Title="Page Not Found" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="ITLHealthWeb.Pages.NotFound" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-wrapper">
        <div class="empty-state">
            <div class="empty-state-icon">🔍</div>
            <h2>404 - Page Not Found</h2>
            <p class="empty-state-text">
                The page you're looking for doesn't exist or has been moved.
            </p>
            <asp:Button ID="BtnGoHome" runat="server" Text="Go to Home" CssClass="btn btn-primary" OnClick="BtnGoHome_Click" />
        </div>
    </div>
</asp:Content>
