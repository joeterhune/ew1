<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ManageUsers" CodeFile="ManageUsers.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Warrants/controls/Toolbar.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="btn-toolbar" role="toolbar" aria-label="Toolbar group">
    <div class="btn-group mr-2" role="group" aria-label="Action group">
        <asp:HyperLink ID="lnkAddUser" runat="server" ToolTip="Add New User to this Agency" CssClass="btn btn-success"><em class="fa fa-plus-circle"></em> New User</asp:HyperLink>
        <asp:LinkButton ID="btnExistingUser" runat="server" ToolTip="Add an Existing User to this Agency" Text="<em class='fa fa-user'></em> Existing User" CausesValidation="false" CssClass="btn btn-warning" />
    </div>
    <tb:ToolBar runat="server" ID="toolbar" />
</div>
<div class="dnnForm dnnClear">
    <asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
        <ContentTemplate>

            <asp:Repeater runat="server" ID="rptUsers">
                <HeaderTemplate>

                    <table id="users" summary="List of Users" class="table table-striped">
                        <thead class="bg-primary">
                            <tr>
                                <th>Display Name
                                </th>
                                <th>Username
                                </th>
                                <th class="text-center">Is Admin</th>
                                <th class="text-center">Delete</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr>
                        <td><%#DataBinder.Eval(Container.DataItem, "DisplayName")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Username")%></td>
                        <td class="command command-icon">
                            <asp:LinkButton ID="cmdSetAdmin" runat="server" CommandName="setadmin" CommandArgument='<%#Eval("userId") %>'>
                                <em class="fa fa-square">&nbsp;</em>
                            </asp:LinkButton>

                        </td>
                        <td class="command command-icon">
                            <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this User?');" CommandArgument='<%#Eval("userId") %>'>
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
</div>
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />


<script type="text/javascript">
    function pageLoad(sender, args) {
        $(document).ready(function () {
            $('.TitleH2').html('<%=ModuleTitle%>');
            $('[data-toggle="tooltip"]').tooltip()
            bindDataTable();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable); // bind data table on every UpdatePanel refresh
        });
    }

    function bindDataTable() {
        var users = $('#users').DataTable({
            "aoColumns": [
                null,
                null,
                { "bSortable": false },
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });
        $('#users_wrapper label').css("font-weight:700");
    }

</script>
