<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.EditCoverSchedule" CodeFile="EditCoverSchedule.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<ul class="nav nav-tabs">
    <li class="active">
        <a href="#nform" data-toggle="tab">Create Notification</a>
    </li>
    <li><a href="#list" data-toggle="tab">List of Existing Notifications</a>
    </li>
</ul>

<div class="tab-content">
    <div class="tab-pane active" id="nform">

        <div class="dnnFormItem dnnFormHelp dnnFormInfo dnnClear">
            <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
        </div>
        <p class="alert alert-info">

            <em class="fa fa-info-circle"></em>&nbsp;Please enter the date range including times that you will be unavailable.  You must select a judge to cover for you or enter a notification message.
        </p>
        <asp:Literal ID="ltMessage" runat="server"></asp:Literal>

        <div class="dnnFormItem labelnowidth">
            <dnn:Label ID="lblStartDate" runat="server" ControlName="rdPickerStart" Text="Start Date:" CssClass="dnnFormRequired"></dnn:Label>
            <asp:TextBox ID="txtStartDate" runat="server" MaxLength="10" CssClass="date" placeholder="mm/dd/yyyy" autocomplete="off"/>
            <asp:TextBox ID="txtStartTime" runat="server" MaxLength="10" CssClass="time" placeholder="h:mm pm"  autocomplete="off"/>
            <asp:RequiredFieldValidator ID="valStartDate" ControlToValidate="txtStartDate"
                CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter a Start Date" runat="server" />
            <asp:RequiredFieldValidator ID="valStartTime" ControlToValidate="txtStartTime"
                CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter a Start Time" runat="server" />

        </div>
        <div class="dnnFormItem labelnowidth">
            <dnn:Label ID="lblEndDate" runat="server" ControlName="rdPickerEnd" Text="End Date:" CssClass="dnnFormRequired"></dnn:Label>
            <asp:TextBox ID="txtEndDate" runat="server" MaxLength="10" CssClass="date" placeholder="mm/dd/yyyy" autocomplete="off"/>
            <asp:TextBox ID="txtEndTime" runat="server" MaxLength="10" CssClass="time" placeholder="h:mm pm"  autocomplete="off"/>
            <asp:RequiredFieldValidator ID="valEndDate" ControlToValidate="txtEndDate"
                CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter an End Date" runat="server" />
            <asp:RequiredFieldValidator ID="valEndTime" ControlToValidate="txtEndTime"
                CssClass="dnnFormMessage dnnFormError inline-error" Display="Dynamic" ErrorMessage="Please Enter an End Time" runat="server" />
        </div>

        <div class="dnnFormItem labelnowidth">
            <dnn:Label ID="lblJudge" runat="server" ControlName="drpJudge" Text="Select a Judge to Cover for you:"></dnn:Label>
            <asp:DropDownList ID="drpJudge" runat="server"></asp:DropDownList>

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
                <asp:LinkButton ID="cmdCancel" runat="server" Text="Close" CausesValidation="false" CssClass="btn btn-danger" />
            </li>
            <li>
                <asp:LinkButton ID="cmdReset" runat="server" Text="Reset" CausesValidation="false" CssClass="btn btn-success pull-right" />
            </li>
        </ul>
    </div>
    <div class="tab-pane" id="list">
        <p class="alert alert-info">

            <em class="fa fa-info-circle"></em>&nbsp;The list below displays your active notifications. To delete a notification click the trash can icon in the last column.
        </p>
        <asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
            <ContentTemplate>
                <asp:Literal ID="ltGridMessage" runat="server" />

                <asp:Repeater runat="server" ID="rptNotifications">
                    <HeaderTemplate>

                        <table id="notifications" summary="List of Notifications" class="table table-striped">
                            <thead class="bg-primary">
                                <tr>
                                    <th>Start Date</th>
                                    <th>End Date
                                    </th>
                                    <th>Covering Judge
                                    </th>
                                    <th>Notification Text</th>
                                    <th class="nowrap">&nbsp;
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td>
                                <%#FormatDateTime(DataBinder.Eval(Container.DataItem, "StartDateTime"))%>
                            </td>
                            <td>
                                <%#FormatDateTime(DataBinder.Eval(Container.DataItem, "EndDateTime"))%> 
                            </td>
                            <td><%#DataBinder.Eval(Container.DataItem, "CoveringJudgeName")%></td>
                            <td><%#DataBinder.Eval(Container.DataItem, "MessageText")%></td>
                            <td class="command command-icon">
                                <asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="false" ToolTip="Delete this Notification" CommandName="delete" OnClientClick="return confirm('Delete this Notification?')" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ScheduleId").ToString() %>'><em class="fa fa-trash"></em></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />

<script type="text/javascript">
    var $message = $('[id*=txtMessage]'), $judgelist = $('[id*=drpJudge]');
    $message.change(function () {
        if ($message.val() == '') {
            $judgelist.removeAttr('disabled');
            $judgelist.css("background-color", "");
        } else {
            $judgelist.attr('disabled', 'disabled').val('');
            $judgelist.css("background-color", "#dddddd");
        }
    }).trigger('change'); // added trigger to calculate initial state

    $judgelist.change(function () {
        if ($judgelist.val() == '') {
            $message.removeAttr('disabled').val('');
            $message.css("background-color", "");
        } else {
            $message.attr('disabled', 'disabled').val('');
            $message.css("background-color", "#dddddd");
        }
    }).trigger('change'); // added trigger to calculate initial state

    jQuery(function ($) {
        $("#<%= txtStartDate.ClientID%>").datepicker();
        $("#<%= txtEndDate.ClientID%>").datepicker();
        $("#<%= txtStartTime.ClientID%>").timepicker({});
        $("#<%= txtEndTime.ClientID%>").timepicker({});

        bindDataTable();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable);
    });
    function bindDataTable() {
        var warrants = $('#notifications').DataTable({
            "aoColumns": [
                null,
                null,
                null,
                null,
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });
        $('#notifications_wrapper label').css("font-weight:700");
    }

</script>
