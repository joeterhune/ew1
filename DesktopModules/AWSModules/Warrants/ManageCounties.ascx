<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ManageCounties" CodeFile="ManageCounties.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="tb" TagName="ToolBar" Src="~/DesktopModules/AwsModules/Warrants/controls/Toolbar.ascx" %>
<div class="btn-toolbar" role="toolbar" aria-label="Toolbar Group" style="margin-bottom:10px">
    <tb:ToolBar runat="server" ID="toolbar" />
</div>


<div id="listsForm">
    <asp:UpdatePanel ID="dptUpdate" runat="server" RenderMode="Block" OnUnload="UpdatePanel_Unload">
        <ContentTemplate>
            <ul class="nav nav-tabs mt-lg">
                <li class="active"><a data-toggle="tab" href="#nav-county">Manage Counties</a></li>
                <li><a data-toggle="tab" href="#nav-division">Manage Division Types</a></li>
            </ul>
            <div class="tab-content">
                <div class="tab-pane fade in active" id="nav-county" role="tabpanel" aria-labelledby="nav-county-tab">

                    <div class="form-inline">
                        <div class="form-group col-auto">
                            <asp:Label ID="lblCounty" CssClass="control-label" runat="server" AssociatedControlID="txtCounty">County Name:</asp:Label>
                            <asp:TextBox ID="txtCounty" runat="server" MaxLength="50" CssClass="form-control" />
                            <asp:HiddenField ID="hdCountyId" runat="server" />
                        </div>
                        <div class="form-group col-auto">
                            <asp:Button ID="cmdSaveCounty" runat="server" CssClass="btn btn-primary" Text="Add County" />
                            <asp:HyperLink ID="lnkCancelCounty" Visible="false" runat="server" CssClass="btn btn-danger" Text="Cancel" />
                        </div>
                    </div>
                    <asp:Repeater runat="server" ID="rptCounties">
                        <HeaderTemplate>

                            <table id="counties" summary="List of Counties" class="table table-striped">
                                <thead class="bg-primary">
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>County Name
                                        </th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td class="command command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CommandArgument='<%#Eval("countyId") %>'>
                            <em class="fa fa-pencil">&nbsp;</em>
                                    </asp:LinkButton>
                                </td>

                                <td><%#DataBinder.Eval(Container.DataItem, "CountyName")%></td>
                                <td class="command command-icon">
                                    <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this County?');" CommandArgument='<%#Eval("countyId") %>'>
                            <em class="fa fa-trash">&nbsp;</em>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>
                <div class="tab-pane fade" id="nav-division" role="tabpanel" aria-labelledby="nav-division-tab">
                    <div class="form-inline">
                        <div class="form-group col-auto">
                            <asp:Label ID="lblDivisionType" CssClass="control-label" runat="server" AssociatedControlID="txtDivisionType">Division Type:</asp:Label>
                            <asp:TextBox ID="txtDivisionType" runat="server" MaxLength="50" CssClass="form-control" />
                            <asp:HiddenField ID="hdDivisionTypeId" runat="server" />
                        </div>
                        <div class="form-group col-auto">
                            <asp:Button ID="cmdAddDivisionType" runat="server" CssClass="btn btn-primary" Text="Add Division Type" />
                            <asp:HyperLink ID="lnkCancelDivisionType" Visible="false" runat="server" CssClass="btn btn-danger" Text="Cancel" />
                        </div>
                    </div>
                    <asp:Repeater runat="server" ID="rptTypes">
                        <HeaderTemplate>

                            <table id="types" summary="List of Division Types" class="table table-striped">
                                <thead class="bg-primary">
                                    <tr>
                                        <th>&nbsp;
                                        </th>
                                        <th>Division Type
                                        </th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td class="command command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CommandArgument='<%#Eval("JudgeTypeId") %>'>
                            <em class="fa fa-pencil">&nbsp;</em>
                                    </asp:LinkButton>
                                </td>

                                <td><%#DataBinder.Eval(Container.DataItem, "JudgeType")%></td>
                                <td class="command command-icon">
                                    <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this Division Type?');" CommandArgument='<%#Eval("JudgeTypeId") %>'>
                            <em class="fa fa-trash">&nbsp;</em>
                                    </asp:LinkButton>
                                </td>

                            </tr>

                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <asp:HiddenField ID="TabName" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdSaveCounty" />
            <asp:AsyncPostBackTrigger ControlID="cmdAddDivisionType" />
        </Triggers>
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
            $('[data-toggle="tooltip"]').tooltip()
            bindDataTable();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable); // bind data table on every UpdatePanel refresh
        });
    }

    function bindDataTable() {
        var counties = $('#counties').DataTable({
            "aoColumns": [
                { "bSortable": false },
                null,
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });

        var types = $('#types').DataTable({
            "aoColumns": [
                { "bSortable": false },
                null,
                { "bSortable": false }
            ],
            "bStateSave": true,
            retrieve: true,
            paging: false
        });

        $('#users_wrapper label').css("font-weight:700");

        var tabName = $("[id*=TabName]").val() != "" ? $("[id*=TabName]").val() : "nav-county";
        $('#listsForm a[href="#' + tabName + '"]').tab('show');
        $("#listsForm a").click(function () {
            $("[id*=TabName]").val($(this).attr("href").replace("#", ""));
        });
    }

</script>
