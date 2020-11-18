<%@ Control Language="vb" Inherits="AWS.Modules.Notifications.NotificationSettings" CodeFile="NotificationSettings.ascx.vb" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" ControlName="drpJudgeRole" Suffix=":"></dnn:Label>
        <asp:DropDownList ID="drpJudgeRole" AutoPostBack="true" runat="server"></asp:DropDownList>
    </div>
    
     <div class="dnnFormItem">
        <dnn:Label ID="lblJARole" runat="server" ControlName="drpJARole" Suffix=":"></dnn:Label>
         <asp:DropDownList ID="drpJARole" AutoPostBack="true" runat="server"></asp:DropDownList>
    </div>
</div>
