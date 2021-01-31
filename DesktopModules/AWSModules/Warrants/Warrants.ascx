<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ViewWarrants" CodeFile="Warrants.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Warrants/controls/Toolbar.ascx" %>
<asp:Literal ID="ltHeader" runat="server" />
<div id="warrantForm">
    <div style="margin-top: 15px;" id="warrantList">
        <asp:PlaceHolder ID="gridmessage" runat="server" EnableViewState="false" />
    </div>
    <asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
        <ContentTemplate>
            <asp:Literal ID="lblDeleteNotice" runat="server" Visible="false">
        <p class="alert alert-danger"><em class="fa fa-warning"></em> The records highlighted below have been completed and will be automatically deleted {0} hours after viewing.</p>
            </asp:Literal>
            <div class="btn-toolbar " role="toolbar" aria-label="Toolbar Group" style="display: block; margin-bottom: 8px">

                <tb:ToolBar runat="server" ID="toolbar" />
                <div class="btn-group pull-right " role="group" aria-label="Action group">
                    <strong>
                        <asp:LinkButton ID="cmdShowAssigned" Visible="false" runat="server" CssClass="" CommandArgument="true"><em class="fa fa-check-square fa-lg"></em> Show only my documents</asp:LinkButton></strong>
                </div>
            </div>

            <div class="form-inline" id="dvFilters" runat="server">
                <div class="form-group">
                    <asp:Label ID="lblID" CssClass="control-label" runat="server" AssociatedControlID="txtID">ID:</asp:Label>
                    <asp:TextBox ID="txtID" AutoPostBack="true" OnTextChanged="txtID_TextChanged" runat="server" MaxLength="6" CssClass="form-control form-control-sm id-field" />
                </div>

                <div class="form-group">
                    <asp:Label ID="lblAgency" CssClass="control-label" runat="server" AssociatedControlID="drpAgency">Agency:</asp:Label>
                    <asp:DropDownList runat="server" ID="drpAgency" OnSelectedIndexChanged="drpAgency_SelectedIndexChanged" CssClass="form-control form-control-sm agency-select" AutoPostBack="true">
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblDefendant" CssClass="control-label" runat="server" AssociatedControlID="txtDefendant">Party:</asp:Label>
                    <asp:TextBox ID="txtDefendant" AutoPostBack="true" OnTextChanged="txtDefendant_TextChanged" runat="server" MaxLength="50" CssClass="form-control form-control-sm" />
                </div>
                <div class="form-group">
                    <asp:Label ID="lblTitle" CssClass="control-label" runat="server" AssociatedControlID="txtTitle">Title:</asp:Label>
                    <asp:TextBox ID="txtTitle" AutoPostBack="true" OnTextChanged="txtTitle_TextChanged" runat="server" MaxLength="50" CssClass="form-control form-control-sm" />
                </div>
                <div class="form-group">
                    <asp:Label ID="lblStartDate" CssClass="control-label" runat="server" AssociatedControlID="txtStartDate">From:</asp:Label>
                    <asp:TextBox ID="txtStartDate" AutoPostBack="true" OnTextChanged="txtStartDate_TextChanged" runat="server" MaxLength="10" CssClass="form-control form-control-sm date" />
                </div>
                <div class="form-group">
                    <asp:Label ID="lblEndDate" CssClass="control-label" runat="server" AssociatedControlID="txtEndDate">To:</asp:Label>
                    <asp:TextBox ID="txtEndDate" AutoPostBack="true" OnTextChanged="txtEndDate_TextChanged" runat="server" MaxLength="50" CssClass="form-control form-control-sm date" />
                </div><div class="form-group">
                &nbsp;<asp:HyperLink ID="lnkRefresh" runat="server" ToolTip="Refresh Page"><em class="fa fa-refresh fa-lg"></em></asp:HyperLink></div>
            </div>
            <asp:Repeater runat="server" ID="rptWarrants">
                <HeaderTemplate>

                    <table id="warrants" summary="List of Documents" class="table table-striped">
                        <thead class="bg-primary">
                            <tr>
                                <th>&nbsp;</th>
                                <th>ID
                                </th>
                                <th>Title
                                </th>
                                <th>Party</th>
                                <th>Submitted By</th>
                                <th>Agency</th>
                                <th>Judge</th>
                                <th>SA</th>
                                <th>Created</th>
                                <th>Reviewed</th>
                                <th>Status</th>
                                <th class="nowrap">&nbsp;
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr id="tablerow" runat="server">
                        <td class="command command-icon">
                            <asp:LinkButton ID="lnkSign" runat="server" data-toggle="tooltip" data-placement="top" title="Sign Document" Visible="false" CommandName="sign" CommandArgument='<%#Eval("WarrantId") %>'>
                                <em class="fa fa-edit">&nbsp;</em>
                            </asp:LinkButton>
<%--                            <asp:Hyperlink ID="lnkAttach" data-toggle="tooltip" data-placement="top" title="Attach Return of Service" runat="server" Visible="false">
                                <em class='fa fa-paperclip'></em>
                            </asp:Hyperlink>--%>
                        </td>
                        <td>
                            <%#DataBinder.Eval(Container.DataItem, "WarrantId")%>
                        </td>
                        <td>
                            <asp:HyperLink ID="lnkWarrant" CssClass="warrantLink" Target="_blank" runat="server"><%#Eval("title")%></asp:HyperLink>
                        </td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Defendant")%></td>
                        <td><span class="toolTip" data-toggle="tooltip" data-placement="top" data-html="true" title='<%#GetUserInfo(Eval("CreatedByUserId"))%>'><%#DataBinder.Eval(Container.DataItem, "CreatedByName")%></span></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "AgencyName")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "JudgeName")%></td>
                        <td><%#IIf(Eval("saApproved"), Eval("saName"), "Not Approved")%></td>
                        <td><%#FormatDate(Eval("CreatedDate"))%></td>
                        <td><%#FormatDate(Eval("ReviewedDate"))%>
                        </td>
                        <td><span data-toggle="tooltip" class="toolTip" data-placement="top" title='<%#Eval("statusTooltip")%>'><%#DataBinder.Eval(Container.DataItem, "Status")%></span></td>
                        <td class="command command-icon">
                            <asp:LinkButton ID="cmdRelease" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "WarrantId").ToString() %>' CommandName="Release"><em class="fa fa-lock" data-toggle="tooltip" data-placement="top" title="Release Document"></em></asp:LinkButton>
                            <asp:LinkButton ID="cmdDelete" runat="server" ToolTip="Delete Record" CommandName="Delete" OnClientClick="return confirm('Delete this Record?')" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "WarrantId").ToString() %>'><em class="fa fa-trash"></em></asp:LinkButton>
                        </td>

                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
            </asp:Repeater>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdShowAssigned" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="txtID" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="drpAgency" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtTitle" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtDefendant" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtStartDate" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtEndDate" EventName="TextChanged" />

        </Triggers>
    </asp:UpdatePanel>

    <asp:Literal ID="ltMessage" runat="server">
    <p class="alert alert-info" style="margin-top: 20px"><em class="fa fa-info-circle"></em> <strong>Please Note:</strong> Documents will be automatically deleted {0} hours after you or another agency member view a signed or rejected document.</p></asp:Literal>


</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />

<script type="text/javascript">
    function pageLoad(sender, args) {
        $(document).ready(function () {
            $('#warrantForm label').addClass("control-label");
            $("#<%= txtStartDate.ClientID%>").datepicker();
            $("#<%= txtEndDate.ClientID%>").datepicker();

            $('[data-toggle="tooltip"]').tooltip()
            $(".warrantLink").on("click", function (e) {
                var docLink = $(this);
                var status = docLink.data("stat");
                if (status == "s") {
                    e.preventDefault();
                    window.open(docLink.attr("href"));
                    setTimeout(refreshGrid(), 1000);
                }
            });
            bindDataTable();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable); // bind data table on every UpdatePanel refresh
        });
    }

    function bindDataTable() {

        var warrants = $('#warrants').DataTable({
            "searching": false,
            "aoColumns": [
                { "bSortable": false },
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false,
            order: [[1, 'desc']]
        });
        $('#warrants_wrapper label').css("font-weight:700");

    }
    function refreshGrid() {
        window.location.replace("/");
    }
</script>
