<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Toolbar.ascx.vb" Inherits="AWS.Modules.Injunctions.Toolbar" %>
 <div id="rtoolbar" runat="server" class="btn-group" role="group" aria-label="Toolbar">
     <asp:HyperLink ToolTip="Manage Judges" CssClass="btn btn-primary" Visible="false" ID="lnkApprove" runat="server"><em class='fa fa-gavel'></em> Manage Judges</asp:HyperLink>
      <asp:HyperLink ToolTip="Manage Agencies" CssClass="btn btn-primary" Visible="false" ID="lnkAgency" runat="server"><em class='fa fa-star'></em> Manage Agencies</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" ToolTip="Manage Counties and Division Types" Visible="false" ID="lnkCounty" runat="server"><em class='fa fa-list'></em> Manage Lists</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" ToolTip="Change the Status of an Injunction" Visible="false" ID="lnkStatus" runat="server"><em class='fa fa-bar-chart'></em> Change Status</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" ToolTip="Manage Agency Users" Visible="false" ID="lnkManage" runat="server"><em class='fa fa-users'></em> Manage Users</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" ToolTip="Manage Custom Text Values for Signing Injunctions" Visible="false" ID="lnkAnnotations" runat="server"><em class='fa fa-keyboard-o'></em> Manage Custom Text</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-success" ID="lnkUpload" ToolTip="Upload a New Injunction" runat="server"><em class='fa fa-upload'></em> Upload Injunction</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" Visible="false" ToolTip="Schedule notifications for when your out of the office" ID="lnkCover" runat="server"><em class='fa fa-calendar'></em> Out of Office Notice</asp:HyperLink>
     <asp:HyperLink CssClass="btn btn-primary" ID="lnkContacts" ToolTip="Contact infomation for the Judiciary" runat="server"><em class='fa fa-phone'></em> Contact Information</asp:HyperLink>

 </div>