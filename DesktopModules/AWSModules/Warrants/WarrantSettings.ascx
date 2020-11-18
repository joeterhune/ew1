<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.Settings" CodeFile="WarrantSettings.ascx.vb" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="dnnForm dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblDemoMode" runat="server" ControlName="chkDemoMode" Suffix=":"></dnn:Label>
        <asp:CheckBox ID="chkDemoMode" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSignedWarrantRole" runat="server" ControlName="drpSignedWarrantRole" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpSignedWarrantRole" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAgencySupervisor" runat="server" ControlName="drpAgencySupervisor" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpAgencySupervisor" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" ControlName="drpJudgeRole" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpJudgeRole" AutoPostBack="true" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminJudge" runat="server" ControlName="drpAdminJudge" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpAdminJudge" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJaRole" runat="server" ControlName="drpJARole" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpJARole" AutoPostBack="true" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblNotificationEmail" runat="server" ControlName="txtNotificationEmail" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtNotificationEmail" runat="server" MaxLength="250"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSenderEmail" runat="server" ControlName="txtSenderEmail" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtSenderEmail" runat="server" MaxLength="250"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeleteThreshold" runat="server" ControlName="txtDeleteThreshold" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtDeleteThreshold" Text="0" runat="server" MaxLength="2" Width="50"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSignedThreshold" runat="server" ControlName="txtSignedThreshold" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtSignedThreshold" Text="0" runat="server" MaxLength="2" Width="50"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblClaimedThreshold" runat="server" ControlName="txtClaimedThreshold" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtClaimedThreshold" Text="0" runat="server" MaxLength="2" Width="50"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCompletedThreshold" runat="server" ControlName="txtCompletedThreshold" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtCompletedThreshold" runat="server" MaxLength="2" Text="0" Width="50"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJAPage" runat="server" ControlName="cboJAPage" Suffix=":" Text="JA Notification Page"></dnn:Label>
        <asp:DropDownList ID="cboJAPage" runat="server" CssClass="NormalTextBox" Width="200" DataValueField="TabID" DataTextField="IndentedTabName" AutoPostBack="false" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblContactListUrl" runat="server" ControlName="cboContactList" Suffix=":"></dnn:Label>
        <asp:dropdownlist id="cboContactList" runat="server" cssclass="NormalTextBox" Width="200" datavaluefield="TabID" datatextfield="IndentedTabName" autopostback="false" />
    </div>  
</div>
