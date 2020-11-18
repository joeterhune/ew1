<%@ Control Language="vb" Inherits="AWS.Modules.Utilities.ExportUsers" CodeFile="ExportUsers.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>


    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdExport" runat="server" Text="Export Users" CssClass="dnnPrimaryAction" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="dnnSecondaryAction" />
            
        </li>
    </ul>
