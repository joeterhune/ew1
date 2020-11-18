<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.JudgeList" CodeFile="JudgeList.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Injunctions/controls/Toolbar.ascx" %>
<div class="btn-toolbar" role="toolbar" aria-label="Toolbar group">
    <div class="btn-group mr-2" role="group" aria-label="Action group">
        <asp:HyperLink ID="lnkAddJudge" runat="server" Visible="false" CssClass="btn btn-success"><em class="fa fa-plus-circle"></em> Add New Judge</asp:HyperLink>
    </div>
    <tb:ToolBar runat="server" ID="toolbar" />
</div>

<asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
    <ContentTemplate>

        <asp:Repeater runat="server" ID="rptJudges">
            <HeaderTemplate>

                <table id="judges" summary="List of Judges" class="table table-striped">
                    <thead class="bg-primary">
                        <tr>
                            <th>&nbsp;</th>
                            <th>Name
                            </th>
                            <th>Divisions
                            </th>
                            <th>Counties
                            </th>
                            <th class="text-center">Signature</th>
                            <th class="text-center">Initial</th>
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
                    <td><%#CType(Eval("user"), UserInfo).DisplayName%></td>
                    <td><%#GetDivision(Eval("JudgeId")) %></td>
                    <td><%#GetCounties(Eval("JudgeId")) %></td>
                    <td class="command command-icon">
                        <asp:Label ID="lblSignature" runat="server"></asp:Label>

                    </td>
                    <td class="command command-icon">
                        <asp:Label ID="lblInitial" runat="server"></asp:Label>

                    </td>
                </tr>

            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater runat="server" ID="rptAdminJudges" Visible="false">
            <HeaderTemplate>

                <table id="adminJudges" summary="List of Judges" class="table table-striped">
                    <thead class="bg-primary">
                        <tr>
                            <th>Name
                            </th>
                            <th>Divisions
                            </th>
                            <th>Counties
                            </th>
                            <th class="text-center">Signature</th>
                            <th class="text-center">Initial</th>
                            <th class="text-center">Approved</th>
                            <th>&nbsp;</th>

                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>

            <ItemTemplate>
                <tr>
                    <td><%#CType(Eval("user"), UserInfo).DisplayName%></td>
                    <td><%#GetDivision(Eval("JudgeId")) %></td>
                    <td><%#GetCounties(Eval("JudgeId")) %></td>
                    <td class="command command-icon">
                        <asp:Label ID="lblSignature" runat="server"></asp:Label>

                    </td>
                    <td class="command command-icon">
                        <asp:Label ID="lblInitial" runat="server"></asp:Label>

                    </td>
                    <td class="command command-icon" id="approveField" runat="server">
                        <asp:LinkButton ID="cmdApprove" runat="server" CommandName="approve" CommandArgument='<%#Eval("JudgeId") %>'>
                                <em class="fa fa-square">&nbsp;</em>
                        </asp:LinkButton>

                    </td>

                    <td class="command command-icon" id="deleteField" runat="server">
                        <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this User?');" CommandArgument='<%#Eval("JudgeId") %>'>
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
        <asp:HyperLink ID="lnkCancel" runat="server" Text="Return" CssClass="btn btn-danger" />
    </li>
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
        var judges = $('#judges').DataTable({
            "aoColumns": [
                { "bSortable": false },
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
        $('#judges_wrapper label').css("font-weight:700");

        var adminJudges = $('#adminJudges').DataTable({
            "aoColumns": [
                null,
                null,
                null,
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });
        $('#adminJudges_wrapper label').css("font-weight:700");
    }

</script>
