<%@ Control Language="vb" Inherits="AWS.Modules.Utilities.ArchiveFiles" CodeFile="ArchiveFiles.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormItem dnnFormHelp dnnClear">
    This utility will archive files from the selected storage area to the directory specified.
    <br />
    <strong>Please Note:</strong> the directory specified must grant modify rights to the IIS App Pool of the current website.
</div>
    <div class="dnnFormItem dnnFormHelp dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>

<div class="dnnForm dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblSource" runat="server" ControlName="drpSource" Text="Source" Suffix=":" CssClass="dnnFormRequired" HelpText="Select the storage area to archive"></dnn:Label>
        <asp:DropDownList ID="drpSource" runat="server">
            <asp:ListItem Text="Injunctions" Value="1" />
            <asp:ListItem Text="Warrants" Value="2" />
        </asp:DropDownList>

    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDate" runat="server" ControlName="txtDate" Text="Cutoff Date" Suffix=":" CssClass="dnnFormRequired" HelpText="Records reviewed before this date will be archived"></dnn:Label>
        <asp:TextBox ID="txtDate" runat="server" MaxLength="2000" CssClass="NormalTextBox" />
        <asp:RequiredFieldValidator ID="valDate" runat="server" SetFocusOnError="true" Display="Dynamic"
            ErrorMessage="Cutoff Date is Required" ControlToValidate="txtDate"  CssClass="dnnFormMessage dnnFormError" />
        <asp:CompareValidator ID="valIsDate" runat="server" ControlToValidate="txtDate"  CssClass="dnnFormMessage dnnFormError"
            Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Display="Dynamic" ErrorMessage="Cutoff date is not in a valid date format " />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblFileLocation" runat="server" ControlName="txtFileLocation" Text="Output Path" Suffix=":" CssClass="dnnFormRequired" HelpText="Enter the directory path to save the files to"></dnn:Label>
        <asp:TextBox ID="txtFileLocation" runat="server" MaxLength="2000" CssClass="NormalTextBox" />
        <asp:RequiredFieldValidator ID="valLocation" runat="server" SetFocusOnError="true" Display="Dynamic"
            ErrorMessage="Output Path is Required" ControlToValidate="txtFileLocation"  CssClass="dnnFormMessage dnnFormError" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeleteRecords" runat="server" ControlName="chkDeleteRecords" Text="Delete Database Record" Suffix=":" HelpText="Checking this box will delete the associated database records"></dnn:Label>
        <asp:CheckBox ID="chkDeleteRecords" runat="server" Checked="true" />

    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeleteRejected" runat="server" ControlName="chkDeleteRejected" Text="Archive Rejected Files?" Suffix=":" HelpText="Unchecking this checkbox will delete rejected files instead of archiving."></dnn:Label>
        <asp:CheckBox ID="chkDeleteRejected" runat="server" Checked="false" />

    </div>

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Archive Files" CssClass="dnnPrimaryAction" OnClientClick="return confirm('Are you sure you want to proceed?');" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="dnnSecondaryAction" />

        </li>
    </ul>
</div>
<hr />
<p>
    <asp:LinkButton ID="cmdShowRecords" runat="server" Text="Show Records to be Archived" CausesValidation="false" CssClass="dnnSecondaryAction" />
</p>
<asp:GridView ID="GridView1" runat="server" EnableTheming="true" SkinID="Professional" AutoGenerateColumns="false" EmptyDataText="No Records to Display">

    <Columns>

        <asp:BoundField DataField="Title" HeaderText="Title" />
        <asp:BoundField DataField="AgencyName" HeaderText="Agency Name" />
        <asp:BoundField DataField="ReviewedDate" HeaderText="ReviewedDate" />
        <asp:BoundField DataField="JudgeName" HeaderText="Judge Name" />
    </Columns>

</asp:GridView>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.css" />
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $("#<%=txtDate.ClientID%>").datepicker();
    }(jQuery, window.Sys));

</script>
