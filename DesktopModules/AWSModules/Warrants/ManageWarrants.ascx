<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ManageWarrants" CodeFile="ManageWarrants.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Warrants/controls/Toolbar.ascx" %>
<div class="btn-toolbar" role="toolbar" aria-label="Toolbar Group">
    <tb:ToolBar runat="server" ID="toolbar" />
</div>
<div class="dnnForm dnnClear">
    <h2 id="dnnPanel-WarrantStatus" class="dnnFormSectionHead">Find Document Record</h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lblWarrantId" runat="server" ControlName="txtWarrantId" Text="Document Id" Suffix=":" />

            <asp:TextBox ID="txtWarrantId" runat="server" MaxLength="20"></asp:TextBox> <asp:LinkButton ID="cmdFindWarrant" runat="server" ValidationGroup="warrant" class="btn btn-primary" Text="Find" />
            <asp:RequiredFieldValidator ID="valId" runat="server" ControlToValidate="txtWarrantId" ErrorMessage="Document Id is Required" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ValidationGroup="warrant"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="valNum" runat="server" ControlToValidate="txtWarrantId" ErrorMessage="Document Id must be an Integer" Type="Integer" Operator="DataTypeCheck" ValidationGroup="warrant" CssClass="dnnFormMessage dnnFormError" Display="Dynamic"></asp:CompareValidator>
        </div>
        <asp:Literal ID="ltWarrantInfo" runat="server" />
        <asp:Panel ID="pnlStatus" CssClass="dnnFormItem" runat="server" Visible="false">
            <dnn:label ID="lblStatus" runat="server" ControlName="drpStatus" Text="Status" Suffix=":" />
            <asp:DropDownList ID="drpStatus" runat="server">
                <asp:ListItem Text="New" Value="1"></asp:ListItem>
                <asp:ListItem Text="Under Review" Value="2"></asp:ListItem>
                <asp:ListItem Text="Signed" Value="3"></asp:ListItem>
                <asp:ListItem Text="Rejected" Value="4"></asp:ListItem>
                <asp:ListItem Text="Reviewed" Value="5"></asp:ListItem>
<%--                <asp:ListItem Text="Return Service" Value="6"></asp:ListItem>--%>
            </asp:DropDownList>
            <asp:LinkButton ID="btnSubmit" runat="server" ValidationGroup="warrant" class="btn btn-primary" Text="Update Status" />
        </asp:Panel>
        <ul class="dnnActions dnnClear">
            <li></li>
            <li>
                <asp:HyperLink ID="lnkCancel" runat="server" class="btn btn-danger" Text="Return" /></li>
        </ul>

    </fieldset>



</div>
