<%@ Page Title="Orders" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Orders.aspx.cs" 
    Inherits="ITLHealthWeb.Pages.Orders" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Content/Orders.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="orders-container">
        <!-- Page Header -->
        <div class="page-header">
            <h2>Order Fulfillment</h2>
            <asp:Label ID="LblMessage" runat="server" CssClass="message-label"></asp:Label>
        </div>
        
        <!-- Filter Section -->
        <div class="filter-section">
            <h3>Filters</h3>
                <div class="filter-controls">
                    <div class="filter-group">
                        <label for="<%= TxtFromDate.ClientID %>">From Date:</label>
                        <asp:TextBox ID="TxtFromDate" runat="server" 
                            TextMode="Date" 
                            CssClass="form-control date-picker"></asp:TextBox>
                    </div>
                    
                    <div class="filter-group">
                        <label for="<%= TxtToDate.ClientID %>">To Date:</label>
                        <asp:TextBox ID="TxtToDate" runat="server" 
                            TextMode="Date" 
                            CssClass="form-control date-picker"></asp:TextBox>
                    </div>
                    
                    <div class="filter-group">
                        <asp:Button ID="BtnFetch" runat="server" 
                            Text="Fetch Orders" 
                            OnClick="BtnFetch_Click" 
                            CssClass="btn btn-primary" />
                    </div>
                    
                    <div class="filter-group">
                        <asp:Button ID="BtnClear" runat="server" 
                            Text="Clear" 
                            OnClick="BtnClear_Click" 
                            CssClass="btn btn-secondary" />
                    </div>
                </div>
        </div>
        
        <div class="orders-grid-section">
            <h3>Orders</h3>
                <div class="grid-container">
                    <asp:GridView ID="GvOrders" runat="server" 
                        AutoGenerateColumns="False"
                        CssClass="orders-grid"
                        EmptyDataText="No orders found for the selected date range."
                        OnRowDataBound="GvOrders_RowDataBound"
                        AllowPaging="True"
                        PageSize="50"
                        OnPageIndexChanging="GvOrders_PageIndexChanging">
        
                        <Columns>
                            <asp:TemplateField HeaderText="OrdID" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# Eval("OrdID").ToString().Length >= 4 ? Eval("OrdID").ToString().Substring(0, 4) : Eval("OrdID").ToString() %>
                                </ItemTemplate>
                            </asp:TemplateField>
            
                            <asp:BoundField DataField="BTCustName" HeaderText="Bill To / Customer Name" 
                                ItemStyle-Width="200px" />
            
                            <asp:BoundField DataField="STCustName" HeaderText="Ship To / Customer Name, Prov, Country" 
                                ItemStyle-Width="250px" />
            
                            <asp:BoundField DataField="ShipVia" HeaderText="Ship Via" 
                                ItemStyle-Width="100px" 
                                ItemStyle-HorizontalAlign="Center" />
            
                            <asp:BoundField DataField="Quantity" HeaderText="Qty" 
                                ItemStyle-Width="50px" 
                                ItemStyle-HorizontalAlign="Center" />
            
                            <asp:BoundField DataField="PCode" HeaderText="Product Code" 
                                ItemStyle-Width="150px" />
            
                            <asp:BoundField DataField="TrackingNo" HeaderText="Tracking No" 
                                ItemStyle-Width="180px" 
                                ItemStyle-Font-Size="Small" />
            
                            <asp:BoundField DataField="Status" HeaderText="Status" 
                                ItemStyle-Width="100px" />
            
                            <asp:BoundField DataField="ShipDate" HeaderText="Completed" 
                                DataFormatString="{0:dd-MMM-yyyy}" 
                                ItemStyle-Width="90px" 
                                ItemStyle-HorizontalAlign="Center" />
            
                            <asp:BoundField DataField="ShippedDate" HeaderText="Ship On" 
                                DataFormatString="{0:dd-MMM-yyyy}" 
                                ItemStyle-Width="90px" 
                                ItemStyle-HorizontalAlign="Center" />
            
                            <asp:BoundField DataField="ShipFrom" HeaderText="ShipFrom" 
                                ItemStyle-Width="80px" 
                                ItemStyle-HorizontalAlign="Center" />
            
                            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LnkTrack" runat="server" 
                                        Text="Track" 
                                        CommandArgument='<%# Eval("TrackingNo") %>'
                                        OnClick="LnkTrack_Click"
                                        CssClass="track-link"
                                        Visible='<%# !string.IsNullOrEmpty(Eval("TrackingNo").ToString()) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
        
                        <HeaderStyle BackColor="#0066cc" ForeColor="White" Font-Bold="True" Height="40px" />
                        <RowStyle BackColor="White" Height="35px" />
                        <AlternatingRowStyle BackColor="#f8f9fa" />
                        <PagerStyle BackColor="#0066cc" ForeColor="White" HorizontalAlign="Center" Height="35px" />
                        <EmptyDataRowStyle BackColor="#fff3cd" ForeColor="#856404" HorizontalAlign="Center" Height="50px" />
                    </asp:GridView>
                </div>
        </div>
        
        <div class="summary-section">
            <h3>End of Day Summary</h3>
            <asp:Label ID="LblSummary" runat="server" CssClass="summary-text">
                Summary statistics will appear here
            </asp:Label>
        </div>
    </div>
</asp:Content>