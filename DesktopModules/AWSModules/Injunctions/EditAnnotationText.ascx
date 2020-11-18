<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.EditAnnotationText" CodeFile="EditAnnotationText.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormMessage dnnFormInfo">To add a custom text item, enter your text in the Custom Text field and click the save button.  To remove a custom text Item, click the delete button next to the item you wish to delete.</div>
<asp:Repeater ID="rptAnnotations" runat="server" EnableViewState="true">
    <HeaderTemplate>
        <div class="listAnnotations">
            <h6>Custom Text List</h6>
            <ul id="customTextList">
    </HeaderTemplate>
    <FooterTemplate></ul></div><hr />
    </FooterTemplate>
    <ItemTemplate>
        <li><span class="listItem"><%#Eval("AnnotationText") %></span>
            <asp:LinkButton ID="cmdDelete" ToolTip="Delete" runat="server" OnClientClick="return confirm('Delete this Custom Text?');" OnClick="cmdDelete_Click" CausesValidation="false" CommandArgument='<%#Eval("AnnotationId") %>' CommandName="Delete">
<em class="fa fa-trash"></em>
            </asp:LinkButton>

        </li>
    </ItemTemplate>
</asp:Repeater>

<div class="dnnForm dnnClear">
    <div class="form-group">
        <asp:Label ID="lblAnnotation" runat="server" CssClass="dnnFormRequired" AssociatedControlID="txtAnnotation">Custom Text <span class="text-danger">*</span></asp:Label>
        <asp:TextBox ID="txtAnnotation" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
        <asp:RequiredFieldValidator ID="valJudge" ControlToValidate="txtAnnotation"
            CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="No Text Entered" runat="server" />

    </div>

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Save" CssClass="btn btn-primary" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Close" CausesValidation="false" CssClass="btn btn-danger" />
        </li>
    </ul>
</div>

