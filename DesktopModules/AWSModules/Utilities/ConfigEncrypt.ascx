<%@ Control Language="vb" Inherits="AWS.Modules.Utilities.ConfigEncryption" CodeFile="ConfigEncrypt.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<div class="dnnFormMessage dnnFormInfo">
<p><strong><asp:HyperLink ID="lnkImportUsers" runat="server">Import Users</asp:HyperLink></strong></p>
    <p><strong><asp:HyperLink ID="lnkExportUsers" runat="server">Export Users</asp:HyperLink></strong></p>
    <p><strong><asp:hyperlink ID="lnkSynUsers" runat="server">Sync Portal Users</asp:hyperlink></strong></p>
<p><strong><asp:hyperlink ID="lnkArchive" runat="server">Archive Files</asp:hyperlink></strong></p>
    </div>
<div class="dnnFormMessage dnnFormInfo">
    Select Protect to Encrypt the appsettings and connection string sections of your webconfig file. Select UnProtect to decrypt the sections for editing.
</div>
<fieldset>
        <legend></legend>
        <div class="dnnFormItem">
    <asp:LinkButton ID="cmdProtect" runat="server" CssClass="dnnPrimaryAction">Protect</asp:LinkButton>

    <asp:LinkButton ID="cmdUnProtect" runat="server" CssClass="dnnPrimaryAction"> UnProtect</asp:LinkButton>
</div></fieldset>


