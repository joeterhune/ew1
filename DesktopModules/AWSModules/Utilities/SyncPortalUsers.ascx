<%@ Control Language="vb" Inherits="AWS.Modules.Utilities.SyncPortalUsers" CodeFile="SyncPortalUsers.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">
    <fieldset>
        <div class="dnnFormItem labelnowidth">
            <dnn:Label ID="lblPortals" runat="server" ControlName="drpPortals" Text="Select Site:"></dnn:Label>
            <asp:DropDownList ID="drpPortals" runat="server"></asp:DropDownList>

        </div>
    </fieldset>
    
    <asp:GridView ID="GridView1" runat="server" EnableTheming="true" SkinID="Professional" DataKeyNames="UserID" AutoGenerateColumns="false">
    <Columns>
  <asp:BoundField DataField="username" HeaderText="Username" />
<asp:BoundField DataField="firstname" HeaderText="First Name" />
<asp:BoundField DataField="lastname" HeaderText="Last Name" />
        <asp:ButtonField CommandName="add" ButtonType="button"  CausesValidation="false" text="add" ShowHeader="false" />
    </Columns>
</asp:GridView>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Add All Users" CssClass="dnnPrimaryAction" OnClientClick="return confirm('Are you sure you want to add all users?');" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="dnnSecondaryAction" />

        </li>
    </ul>
</div>


