<%@ Control Language="vb" Inherits="AWS.Modules.Notifications.EditNotification" CodeFile="EditNotification.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear Notification" id="oooNotificationForm">
    <div class="dnnFormItem dnnFormHelp dnnFormInfo dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>
    <p class="alert alert-info">

        <em class="fa fa-info-circle"></em>&nbsp;Please enter the date range including times that the judge will be unavailable.  You must select a cover judge or enter a notification message for law enforcement.
    </p>
    <asp:Literal ID="ltMessage" runat="server"></asp:Literal>

    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblOOOJudge" runat="server" ControlName="drpOOOJudge" Text="Judge:"></dnn:Label>
        <asp:DropDownList ID="drpOOOJudge" runat="server"></asp:DropDownList>
        <asp:RequiredFieldValidator ID="valOOOJudge" ControlToValidate="drpOOOJudge"
            CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Please Select a Judge from the list" runat="server" />

    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblStartDate" runat="server" ControlName="rdPickerStart" Text="Start Date:" CssClass="dnnFormRequired"></dnn:Label>
        <asp:TextBox ID="txtStartDate" runat="server" MaxLength="10" CssClass="date" placeholder="mm/dd/yyyy" />
        <asp:TextBox ID="txtStartTime" runat="server" MaxLength="10" CssClass="time" placeholder="h:mm pm" />
        <asp:RequiredFieldValidator ID="valStartDate" ControlToValidate="txtStartDate"
            CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter a Start Date" runat="server" />
        <asp:RequiredFieldValidator ID="valStartTime" ControlToValidate="txtStartTime"
            CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter a Start Time" runat="server" />

    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblEndDate" runat="server" ControlName="rdPickerEnd" Text="End Date:" CssClass="dnnFormRequired"></dnn:Label>
        <asp:TextBox ID="txtEndDate" runat="server" MaxLength="10" CssClass="date" placeholder="mm/dd/yyyy" />
        <asp:TextBox ID="txtEndTime" runat="server" MaxLength="10" CssClass="time" placeholder="h:mm pm" />
        <asp:RequiredFieldValidator ID="valEndDate" ControlToValidate="txtEndDate"
            CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter an End Date" runat="server" />
        <asp:RequiredFieldValidator ID="valEndTime" ControlToValidate="txtEndTime"
            CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter an End Time" runat="server" />
    </div>

    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblCoverJudge" runat="server" ControlName="drpCoverJudge" Text="Select a Cover Judge:"></dnn:Label>
        <asp:DropDownList ID="drpCoverJudge" runat="server"></asp:DropDownList>

    </div>
    <div class="dnnFormItem"><span class="dnnLabel" style="font-weight: bold; margin-bottom: 18px;">OR</span></div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblMessage" runat="server" ControlName="txtMessage" Text="Send Notification Message:"></dnn:Label>
        <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" MaxLength="2500" placeholder="I am currently unavailable, but will review warrants immediately upon my return.  If this is an emergency please contact me by phone at (555) 555-5555"></asp:TextBox>
    </div>

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Save" CssClass="btn btn-primary" /></li>
        <li>
            <asp:hyperlink ID="lnkCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" />
        </li>
    </ul>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />

<script type="text/javascript">
    jQuery(function ($) {
        $("#<%= txtStartDate.ClientID%>").datepicker();
        $("#<%= txtEndDate.ClientID%>").datepicker();
        $("#<%= txtStartTime.ClientID%>").timepicker({});
        $("#<%= txtEndTime.ClientID%>").timepicker({});
    });
    // -- Begin disable message box or cover judge dropdown 
    var $message = $('[id*=txtMessage]'), $coverjudgelist = $('[id*=drpCoverJudge]'), $judgelist = $('[id*=drpOOOJudge]');
    $message.change(function () {
        if ($message.val() == '') {
            $coverjudgelist.removeAttr('disabled');
            $coverjudgelist.css("background-color", "");
        } else {
            $coverjudgelist.attr('disabled', 'disabled').val('');
            $coverjudgelist.css("background-color", "#dddddd");
        }
    }).trigger('change');

    $coverjudgelist.change(function () {
        if ($coverjudgelist.val() == '') {
            $message.removeAttr('disabled').val('');
            $message.css("background-color", "");
        } else {
            $message.attr('disabled', 'disabled').val('');
            $message.css("background-color", "#dddddd");
        }
    }).trigger('change'); // added trigger to calculate initial state

    // --  End disable message box or cover judge dropdown

    $judgelist.change(function () {
        $coverjudgelist.children('option').show();
        $coverjudgelist.children("option[value='" + $(this).val() + "']").hide()
    }).trigger('change');

</script>
