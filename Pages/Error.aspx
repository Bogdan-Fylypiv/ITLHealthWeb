<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ITLHealthWeb.Pages.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-wrapper">
        <div class="empty-state">
            <div class="empty-state-icon">⚠️</div>
            <h2>Oops! Something went wrong</h2>
            <p class="empty-state-text">
                An unexpected error has occurred. Please try again or contact support if the problem persists.
            </p>
            <asp:Button ID="BtnGoHome" runat="server" Text="Go to Home" CssClass="btn btn-primary" OnClick="BtnGoHome_Click" />
        </div>
    </div>
</asp:Content>
