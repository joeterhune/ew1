<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ExistingUser" CodeFile="ExistingUser.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblExists" runat="server" Text="Existing Username"
            ControlName="txtExistingUser" />
        <asp:TextBox ID="txtExistingUser" runat="server" MaxLength="50"></asp:TextBox>&nbsp;&nbsp;
    </div>

    <ul class="dnnActions dnnClear">
    <li>
        <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" /></li>
    <li>
        <asp:hyperlink ID="lnkCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" />
    </li>
</ul>
</div>
