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
            <h2> Package Tracking</h2>
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