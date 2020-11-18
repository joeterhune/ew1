<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.ManageInjunctions" CodeFile="ManageInjunctions.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Injunctions/controls/Toolbar.ascx" %>
<tb:ToolBar runat="server" ID="toolbar" />

<div class="dnnForm dnnClear">
    <h2 id="dnnPanel-InjunctionStatus" class="dnnFormSectionHead">Find Injunction</h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lblInjunctionId" runat="server" ControlName="txtInjunctionId" Text="Injunction Id" Suffix=":" />

            <asp:TextBox ID="txtInjunctionId" runat="server" MaxLength="20"></asp:TextBox> 
            <asp:LinkButton ID="cmdFindInjunction" runat="server" ValidationGroup="injunction" class="btn btn-primary" Text="Find" />
            <asp:RequiredFieldValidator ID="valId" runat="server" ControlToValidate="txtInjunctionId" ErrorMessage="Injunction Id is Required" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ValidationGroup="injunction"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="valNum" runat="server" ControlToValidate="txtInjunctionId" ErrorMessage="Injunction Id must be an Integer" Type="Integer" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" Operator="DataTypeCheck" ValidationGroup="injunction"></asp:CompareValidator>
        </div>
        <asp:Literal ID="ltInjunctionInfo" runat="server" />
        <asp:Panel ID="pnlStatus" CssClass="dnnFormItem" runat="server" Visible="false">
            <dnn:label ID="lblStatus" runat="server" ControlName="drpStatus" Text="Status" Suffix=":" />
            <asp:DropDownList ID="drpStatus" runat="server">
                <asp:ListItem Text="New" Value="1"></asp:ListItem>
                <asp:ListItem Text="Under Review" Value="2"></asp:ListItem>
                <asp:ListItem Text="Signed" Value="3"></asp:ListItem>
                <asp:ListItem Text="Rejected" Value="4"></asp:ListItem>
                <asp:ListItem Text="Reviewed" Value="5"></asp:ListItem>
            </asp:DropDownList>
            <asp:LinkButton ID="btnSubmit" runat="server" ValidationGroup="injunction" class="btn btn-primary" Text="Update Status" />
        </asp:Panel>
        <ul class="dnnActions dnnClear">
            <li></li>
            <li>
                <asp:HyperLink ID="lnkCancel" runat="server" class="btn btn-danger" Text="Return" /></li>
        </ul>

    </fieldset>

</div>
