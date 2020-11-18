<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.ManageAgencies" CodeFile="ManageAgencies.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Injunctions/controls/Toolbar.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="btn-toolbar" role="toolbar" aria-label="Toolbar group">
    <div class="btn-group mr-2" role="group" aria-label="Action group">
        <asp:HyperLink ID="lnkAddAgency" runat="server" CssClass="btn btn-success"><em class="fa fa-plus-circle"></em> Add New Agency</asp:HyperLink>
    </div>
    <tb:ToolBar runat="server" ID="toolbar" />
</div>
<asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
    <ContentTemplate>

        <asp:Repeater runat="server" ID="rptAgencies">
            <HeaderTemplate>
                <table id="agencies" summary="List of Agencies" class="table table-striped">
                    <thead class="bg-primary">
                        <tr>
                            <th>&nbsp;</th>
                            <th>&nbsp;</th>
                            <th>Agency Name</th>
                            <th>Parent Agency</th>
                            <th>County</th>
                            <th>Email</th>
                            <th class="text-center">Is&nbsp;Clerk?</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>

            <ItemTemplate>
                <tr>
                    <td class="command command-icon">
                        <asp:HyperLink ID="lnkEdit" runat="server">
                            <em class="fa fa-pencil">&nbsp;</em>
                        </asp:HyperLink>
                    </td>
                    <td class="command command-icon">
                        <asp:HyperLink ID="lnkUsers" runat="server">
                            <em class="fa fa-users">&nbsp;</em>
                        </asp:HyperLink>
                    </td>
                    <td><%#DataBinder.Eval(Container.DataItem, "AgencyName")%></td>
                    <td><%#DataBinder.Eval(Container.DataItem, "ParentAgency")%></td>
                    <td><%#DataBinder.Eval(Container.DataItem, "CountyList")%></td>
                    <td><a href='mailto:<%#DataBinder.Eval(Container.DataItem, "EmailAddress")%>'><%#DataBinder.Eval(Container.DataItem, "EmailAddress")%></a></td>
                    <td class="command command-icon text-primary"><%#IIf(Ctype(DataBinder.Eval(Container.DataItem, "IsClerk"),Boolean),"<em class='fa fa-check-square'></em>","<em class='fa fa-square'></em>")%></td>
                    <td class="command command-icon text-nowrap">
                        <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this Agency?');" CommandArgument='<%#Eval("agencyId") %>'>
                                <em class="fa fa-trash">&nbsp;</em>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
</asp:UpdatePanel>


<ul class="dnnActions dnnClear">
    <li>
        <asp:HyperLink ID="lnkCancel" runat="server" class="btn btn-danger" Text="Return" /></li>
</ul>

<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />


<script type="text/javascript">
    function pageLoad(sender, args) {
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip()
            bindDataTable();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable); // bind data table on every UpdatePanel refresh
        });
    }

    function bindDataTable() {
        var agencies = $('#agencies').DataTable({
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": false },
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
        $('#agencies_wrapper label').css("font-weight:700");
    }

</script>
