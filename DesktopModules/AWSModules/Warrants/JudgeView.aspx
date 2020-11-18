<%@ Page Language="VB" AutoEventWireup="false" CodeFile="JudgeView.aspx.vb" Inherits="AWS.Modules.Warrants.JudgeView" %>

<%@ Register Assembly="Atalasoft.dotImage.WebControls" Namespace="Atalasoft.Imaging.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="bzb" TagName="Zooming" Src="~/DesktopModules/AWSModules/Warrants/controls/ZoomingControl.ascx" %>
<%@ Register TagPrefix="nb" TagName="Navigation" Src="~/DesktopModules/AWSModules/Warrants/controls/NavigationControl.ascx" %>
<%@ Register Assembly="Atalasoft.dotImage.WebControls" Namespace="Atalasoft.Imaging.WebControls.Annotations" TagPrefix="cc2" %>


<!DOCTYPE html >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<%--    <link href="/Portals/_default/default.css" type="text/css" rel="stylesheet" />--%>
    <link rel="stylesheet" type="text/css" href="css/JudeView.min.css" />
    <link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/themes/smoothness/jquery-ui.css" type="text/css" />
    <!--[if lte IE 8]>
<script src="js/html5.js" type="text/javascript"></script>
        <link rel="stylesheet" type="text/css" href="css/ie.css" />
<![endif]-->

    <title>Review Warrants</title>
</head>
<body>
    <form id="Form1" runat="server">
        <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="js/jquery.form.js"></script>
        <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.js"></script>
        <asp:PlaceHolder ID="phMessage" runat="server" />
        <div id="container" runat="server">
            <header>
                <hgroup>
                    <h1>
                        <asp:Literal ID="ltHeader" runat="server"></asp:Literal></h1>
                </hgroup>
                <menu type="toolbar" class="main">
                    <ul class="main_ul">

                        <li>
                            <nb:Navigation runat="server" />
                        </li>

                        <li>
                            <bzb:Zooming runat="server" />
                        </li>

                        <li>
                            <menu class="sub" label="Annotations"> <asp:ImageButton class="button" ID="Button_Freehand" runat="server" ImageUrl="~/images/Pen.png"
                                ToolTip="Creates freehand annotations" Height="24px" Width="24px" OnClientClick="CreateAnnotation('FreehandData', 'DefaultFreehand'); return false;" />
                                <asp:ImageButton class="button" ID="Button_Redaction" runat="server" ImageUrl="~/images/Redact.png"
                                    ToolTip="Creates redaction annotations." Height="24px" Width="24px" OnClientClick="CreateAnnotation('RectangleData', 'DefaultRedaction'); return false;" />
                                <asp:ImageButton CssClass="button" ID="Button_Delete" runat="server" ImageUrl="~/images/Delete-blue.png"
                                    ToolTip="Deletes the Selected Annotation from the Image" Height="24px" Width="24px"
                                    OnClientClick="Delete(); return false;" />
                            </menu>
                            <a>Annotations</a>
                        </li>
                        <li>
                            <menu class="sub" label="Sign">
                                <asp:ImageButton CssClass="button" ID="Button_SignStamp" runat="server" ImageUrl="~/images/rubber stamp.png"
                                    ToolTip="Creates signature annotation." Height="24px" Width="24px" OnClientClick="CreateAnnotation('EmbeddedImageData', 'DefaultSignature'); return false;" />
                            </menu>
                            <a>Sign</a>
                        </li>
                        <li>
                            <menu class="sub" label="Initial">
                                <asp:ImageButton CssClass="button" ID="Button_InitialStamp" runat="server" ImageUrl="~/images/rubber stamp-i.png"
                                    ToolTip="Creates Initials annotation." Height="24px" Width="24px" OnClientClick="CreateAnnotation('EmbeddedImageData', 'DefaultInitial'); return false;" />
                            </menu>
                            <a>Initial</a>
                        </li>
                        <li>
                            <menu class="sub" label="CheckMark">
                                <asp:ImageButton CssClass="button" ID="Button_CheckMark" runat="server" ImageUrl="~/images/check.png"
                                    ToolTip="Creates Check Mark annotation." Height="24px" Width="24px" OnClientClick="CreateAnnotation('EmbeddedImageData', 'DefaultCheckMark'); return false;" />

                            </menu>
                            <a>Check</a>
                        </li>

                        <li>
                            <menu class="sub" label="Stamp">
                                <asp:ImageButton CssClass="button" ID="Button_Date" runat="server" ImageUrl="~/images/calendar.png"
                                    ToolTip="Creates Date annotation." Height="24px" Width="24px" OnClientClick="CreateAnnotation('EmbeddedImageData', 'DefaultDateStamp'); return false;" />

                            </menu>
                            <a>Date</a>
                        </li>
                        <li>
                            <menu class="sub" label="Stamp">
                                <span style="display: inline-block; margin-top: 2px; float: right;">
                                    <input type="text" id="txtStampText" name="lname" maxlength="50" style="width: 100px" /><asp:Button ID="btnText" OnClientClick="SetText(); return false;" runat="server" Text="Insert" /></span>

                                <asp:ImageButton CssClass="button" ID="Button_PS" runat="server" ImageUrl="~/images/ps.png"
                                    ToolTip="Creates Per Schedule annotation." Height="24px" Width="24px" OnClientClick="CreateAnnotation('EmbeddedImageData', 'DefaultPerscheduleStamp'); return false;" />
                            </menu>
                            <a>Per Schedule</a>
                        </li>
                        <li>
                            <menu class="sub" label="Save Text">
                                <asp:ImageButton CssClass="button" ID="Button_Text" runat="server" ImageUrl="~/images/Text.png"
                                    ToolTip="Creates text annotations." Height="24px" Width="24px" OnClientClick="CreateAnnotation('TextData', 'DefaultTextAnnotation'); return false;" />

                                <asp:ImageButton CssClass="button" ID="SaveText" runat="server" ImageUrl="~/images/SaveText.png"
                                    ToolTip="Save text annotations" Height="24px" Width="24px" OnClientClick="GetTextFromAnnotation(); return false;" />
                                <asp:ImageButton CssClass="button" ClientIDMode="Static" ID="showCustomTextList" runat="server" ImageUrl="~/images/add-list.png"
                                    ToolTip="Insert custom text" Height="24px" Width="24px" />

                            </menu>
                            <a>Custom Text</a>
                        </li>

                    </ul>
                </menu>
            </header>
            <section id="awsViewer">
                <ul>
                    <li id="wtv">
                        <cc1:WebThumbnailViewer ID="WebThumbnailViewer1" runat="server" BackColor="Gainsboro"
                            Height="100%" Width="100%" ViewerID="WebAnnotationViewer1" Centered="True" ShowAnnotations="true"
                            AllowDragDrop="false" ThumbStyleSelected-BackColor="DimGray"
                            ThumbStyleSelectedHover-BackColor="LightSlateGray" ThumbSize="100,100" />
                    </li>
                    <li id="wav">
                        <cc2:WebAnnotationViewer ID="WebAnnotationViewer1" runat="server" BackColor="Gainsboro"
                            Height="100%" Width="100%" Centered="True" AntialiasDisplay="ReductionOnly" AutoLinkThumbnailViewer="True"
                            InteractMode="Modify" AuthorMode="Single" />
                    </li>

                </ul>
            </section>
            <footer id="status" class="dnnForm dnnClear">
                <ul class="dnnActions dnnClear">
                    <li id="sb">
                        <asp:Button ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" Text="Approve" OnClientClick="ApproveWarrant(); return false;" /></li>
                    <li id="rb">
                        <input id="txtReason" type="text" name="Reason" maxlength="250" placeholder="Reject Reason" />
                        <asp:Button ID="cmdReject" CssClass="dnnSecondaryAction" runat="server" CausesValidation="true" Text="Reject" OnClientClick="RejectWarrant(); return false;" /></li>
                    <li id="cb">
                        <asp:Button ID="cmdCancel" CssClass="dnnSecondaryAction" runat="server" CausesValidation="false" Text="Return to Signing Queue without Action" OnClientClick="CancelWarrant(); return false;" /></li>
                </ul>

            </footer>
            <div id="dialog" title="Alert message" style="display: none">
                <div class="ui-dialog-content ui-widget-content">
                    <p>
                        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0"></span>
                        <label id="lblMessage">
                        </label>
                    </p>
                </div>
            </div>

        </div>
        <asp:HiddenField ID="hdtoCancel" runat="server" />

    </form>
    <div id="dialog_customText" style="display:none;">
        <ul id="customTextList">
            <li><span class="listItem">Per Schedule</span></li>
        </ul>
    </div>
    <script type="text/javascript" src="js/HTMLAdvDocViewer.min.js?ver=08-16-19"></script>
    <script type="text/javascript">
        function NavigateManageText() {
            location.replace("<%=NavigateUrl%>");
            return false;
        }
    </script>
</body>

</html>
