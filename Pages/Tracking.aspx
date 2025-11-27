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
            <h2>Tracking Form</h2>
            <asp:Label ID="LblMessage" runat="server" CssClass="message-label"></asp:Label>
        </div>
        
        <!-- Main Content Area: Info on Left, Buttons on Right -->
        <div class="tracking-main-content">
            <!-- Left Column: Tracking Information -->
            <div class="tracking-info-section">
                <div class="info-grid">
                    <div class="info-row">
                        <label class="info-label">Lead TrackingNo</label>
                        <asp:TextBox ID="TxtTrackingNo" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">Shiper Acct No</label>
                        <asp:TextBox ID="TxtShipperAcctNo" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">Service Type</label>
                        <asp:TextBox ID="TxtServiceType" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">Est Delivery Date</label>
                        <asp:TextBox ID="TxtDeliveryDate" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">Status</label>
                        <asp:TextBox ID="TxtStatus" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly status-field" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">PO</label>
                        <asp:TextBox ID="TxtPO" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                    
                    <div class="info-row">
                        <label class="info-label">Refl - OrderID</label>
                        <asp:TextBox ID="TxtOrderID" runat="server" ReadOnly="true" 
                            CssClass="form-control-readonly" />
                    </div>
                </div>
                
                <!-- Ship To Address Section -->
                <div class="address-section">
                    <label class="info-label">Ship To Address</label>
                    <asp:TextBox ID="TxtShipToAddress" runat="server" ReadOnly="true" 
                        TextMode="MultiLine" Rows="3" CssClass="form-control-readonly address-box" />
                </div>
                
                <!-- Last Location Section -->
                <div class="address-section">
                    <label class="info-label">Last</label>
                    <asp:TextBox ID="TxtLastLocation" runat="server" ReadOnly="true" 
                        TextMode="MultiLine" Rows="2" CssClass="form-control-readonly address-box" />
                </div>
            </div>
            
            <!-- Right Column: Action Buttons -->
            <div class="tracking-actions">
                <asp:Button ID="BtnWebSite" runat="server" 
                    Text="CanPar Website" 
                    OnClick="BtnWebSite_Click" 
                    CssClass="btn btn-carrier" />
                
                <asp:Button ID="BtnExit" runat="server" 
                    Text="Exit" 
                    OnClientClick="window.close(); return false;" 
                    CssClass="btn btn-exit" />
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