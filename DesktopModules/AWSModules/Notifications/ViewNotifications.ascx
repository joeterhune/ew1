<%@ Control Language="vb" Inherits="AWS.Modules.Notifications.ViewNotifications" CodeFile="ViewNotifications.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear Notification" id="oooNotificationForm">
    <p class="alert alert-info">
        <em class="fa fa-info-circle"></em>&nbsp;The list below displays active notifications. To delete a notification click the trash can icon in the last column.
    </p>
    <asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
        <ContentTemplate>
            <asp:Literal ID="ltGridMessage" runat="server" />
            <asp:HyperLink ID="lnkAddNotification" runat="server" CssClass="btn btn-primary"><em class="fa fa-plus-circle"></em> Add Notification</asp:HyperLink>

            <asp:Repeater runat="server" ID="rptNotifications">
                <HeaderTemplate>

                    <table id="notifications" summary="List of Notifications" class="table table-striped">
                        <thead class="bg-primary">
                            <tr>
                                <th>Judge</th>
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
                        <td><%#GetJudge(Eval("RequestingJudgeId"))%>
                        </td>
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
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />

<script type="text/javascript">

    jQuery(function ($) {
        bindDataTable();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable);
    });
    function bindDataTable() {
        var notifications = $('#notifications').DataTable({
            "aoColumns": [
                null,
                null,
                null,
                null,
                { "bSortable": false },
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });
        $('#notifications_wrapper label').css("font-weight:700");
    }

</script>
