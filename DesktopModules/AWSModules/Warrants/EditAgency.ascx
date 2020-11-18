<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.EditAgency" CodeFile="EditAgency.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">
    <div class="dnnFormItem dnnFormHelp dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>

    <h2 id="dnnPanel-Personal" class="dnnFormSectionHead">Personal Information</h2>
    <fieldset>
        <legend></legend>
        <div class="dnnFormItem">
            <dnn:Label ID="lblAgencyName" runat="server" Text="Agency Name" ControlName="txtAgencyName"  CssClass="dnnFormRequired"/>
            <asp:TextBox ID="txtAgencyName" runat="server" Columns="40" MaxLength="250" />
            <asp:RequiredFieldValidator ID="valAgencyName" runat="server" Display="dynamic" SetFocusOnError="true"
                CssClass="dnnFormMessage dnnFormError" ErrorMessage="Agency Name is Required" ControlToValidate="txtAgencyName" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblParentAgency" runat="server" Text="Parent Agency"
                ControlName="txtParentAgency" />
            <asp:TextBox ID="txtParentAgency" runat="server"  Columns="40" MaxLength="250" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblCounties" runat="server" Text="County Jurisdiction" CssClass="dnnFormRequired"
                ControlName="lstSelectedCounties" />
            <asp:CheckBoxList ID="chkCounties" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
            </asp:CheckBoxList>
            <asp:CustomValidator ID="valCounties" Display="Dynamic" SetFocusOnError="true" ErrorMessage="Please select at least one county."
                 CssClass="dnnFormMessage dnnFormError" ClientValidationFunction="ValidateCheckBoxList" runat="server" />
        </div>
        <div class="dnnFormItem">

            <dnn:Label ID="lblEmailAddress" runat="server" Text="E-mail Address" ControlName="txtEmail" CssClass="dnnFormRequired" />
            <asp:TextBox ID="txtEmail" runat="server" Columns="30" MaxLength="256" />
            <asp:RequiredFieldValidator ID="valEmail" runat="server" Display="dynamic" SetFocusOnError="true"
                CssClass="dnnFormMessage dnnFormError" ErrorMessage="E-mail address is Required" ControlToValidate="txtEmail" />
            <asp:RegularExpressionValidator ID="valIsEmail" runat="server" Display="dynamic"
                SetFocusOnError="true" CssClass="dnnFormMessage dnnFormError" ErrorMessage="The value entered is not a valid E-mail address"
                ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
        </div>
        <div class="dnnFormItem">
            <asp:CheckBox Text="Is This Clerk's Office" runat="server" id="chkIsClerk"/>
        </div>
    </fieldset>

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" /></li>
        <li>
            <asp:HyperLink ID="lnkCancel" runat="server" class="btn btn-danger" Text="Return" /></li>
    </ul>
</div>
<script type="text/javascript">
    function onClientTransferredHandler(sender, e) {
       var destinationList = e.get_destinationListBox();
       var items = destinationList.get_items();
       var item = destinationList.getItem(items.get_count() - 1);
       item.select();
    }
    function ValidateCheckBoxList(sender, args) {
        var checkBoxList = document.getElementById("<%=chkCounties.ClientID %>");
        var checkboxes = checkBoxList.getElementsByTagName("input");
        var isValid = false;
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                isValid = true;
                break;
            }
        }
        args.IsValid = isValid;
    }

</script>