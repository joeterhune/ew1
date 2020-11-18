<%@ Control Language="vb" Inherits="AWS.Modules.Utilities.AutoAddUser" CodeFile="AutoAddUser.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Import Users" CssClass="dnnPrimaryAction" OnClientClick="return confirm('Are you sure you want to add users?');" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="dnnSecondaryAction" />

        </li>
    </ul>
</div>
<hr />
<asp:GridView ID="GridView1" runat="server" EnableTheming="true" SkinID="Professional" autogeneratecolumns="false">

    <Columns>
        <asp:BoundField DataField="username" HeaderText="Username" />
        <asp:BoundField DataField="firstname" HeaderText="First Name" />
        <asp:BoundField DataField="lastname" HeaderText="Last Name" />
        <asp:BoundField DataField="email" HeaderText="Email" />
    </Columns>

</asp:GridView>
