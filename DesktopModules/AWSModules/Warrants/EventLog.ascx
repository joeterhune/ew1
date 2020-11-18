<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.EventLog" CodeFile="EventLog.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <%--<div class="dnnFormItem">
        <dnn:Label ID="lblReport" runat="server" ControlName="drpReport" Text="Select Report:"></dnn:Label>
        <asp:DropDownList ID="drpReport" runat="server"></asp:DropDownList>
    </div>--%>
    <div class="dnnFormItem">
        <dnn:Label ID="lblStartDate" runat="server" ControlName="txtStartDate" Text="Start Date:"></dnn:Label>
        <asp:TextBox ID="txtStartDate" runat="server" CssClass="NormalTextBox" MaxLength="12"></asp:TextBox>
        <asp:RequiredFieldValidator ID="valStartEmpty" ControlToValidate="txtStartDate"
            CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Start Date is Required" runat="server" />

        <asp:CompareValidator ID="valStartDate" runat="server" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Incorrect Date Format [01/01/2012]" Type="Date" ControlToValidate="txtStartDate" Operator="DataTypeCheck"></asp:CompareValidator>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblEndDate" runat="server" ControlName="txtEndDate" Text="End Date:"></dnn:Label>
        <asp:TextBox ID="txtEndDate" runat="server" CssClass="NormalTextBox" MaxLength="12"></asp:TextBox>
        <asp:RequiredFieldValidator ID="valEndEmpty" ControlToValidate="txtEndDate"
            CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="End Date is Required" runat="server" />
        <asp:CompareValidator ID="valEndDate" runat="server" CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Incorrect Date Format [01/01/2012]" Type="Date" ControlToValidate="txtEndDate" Operator="DataTypeCheck"></asp:CompareValidator>
        <%--        <asp:CompareValidator ID="valEndStartComp" runat="server"  CssClass="dnnFormMessage dnnFormError" Display="Dynamic ErrorMessage="End Date must be larger than Start Date" ControlToValidate="txtEndDate" ControlToCompare="txtStartDate" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>--%>
    </div>
</div>
<div style="margin-top: 15px;">
    <telerik:RadGrid ID="rgEvents" runat="server" CellSpacing="0" GridLines="None">
        <MasterTableView AutoGenerateColumns="True" NoMasterRecordsText="No Events Found" CommandItemDisplay="None">
            <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>

            <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>

            <Columns>
            </Columns>
        </MasterTableView>

    </telerik:RadGrid>
</div>

