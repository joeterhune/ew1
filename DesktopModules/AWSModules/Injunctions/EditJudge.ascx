<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.EditJudge" CodeFile="EditJudge.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="dnnForm dnnClear">
    <div class="dnnFormItem dnnFormHelp dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblJudge" runat="server" ControlName="drpJudge" Text="Judge:" CssClass="dnnFormRequired"></dnn:Label>
        <asp:DropDownList ID="drpJudge" runat="server"></asp:DropDownList>
        <asp:RequiredFieldValidator ID="valJudge" ControlToValidate="drpJudge"
            CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="You must select a judge" runat="server" />
    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblOffenseType" runat="server" ControlName="rblOffenseType" Text="Division Type" Suffix=":" CssClass="dnnFormRequired"></dnn:Label>
        <asp:CheckBoxList ID="cklOffenseType" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons"></asp:CheckBoxList>

    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblCounty" runat="server" ControlName="drpCounty" Text="Counties:" CssClass="dnnFormRequired"></dnn:Label>
        <asp:CheckBoxList ID="cklCounties" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons"></asp:CheckBoxList>
        <asp:CustomValidator runat="server" ID="cvmodulelist" ClientValidationFunction="ValidateCountyList" ErrorMessage="Please Select At least one County"></asp:CustomValidator>
    </div>
    <div class="dnnFormItem labelnowidth">
        <dnn:Label ID="lblDayStart" runat="server" ControlName="tpDayStart" Text="Daily Availability:" CssClass="dnnFormRequired"></dnn:Label>
        <asp:TextBox ID="txtDayStart" runat="server" MaxLength="10" CssClass="date" /><span> to </span>
        <asp:TextBox ID="txtDayEnd" runat="server" MaxLength="10" CssClass="date" />
    </div>
    <div class="dnnFormItem clearfix">

        <dnn:Label ID="lblupload" runat="server" ControlName="signatureUpload" Text="Upload Signatures" Suffix=":" HelpText="Select the Image Type and then select the file to upload" />
        <div style="position: relative;">
            <div id="sign-overlay" class="overlay" style="display: none;">
                <div class="spinner"></div>
            </div>
            <asp:DropDownList ID="drpSignatureType" runat="server" CssClass="pull-left smallDropdown">
                <asp:ListItem Text="<Select Image Type>" Value="" />
                <asp:ListItem Value="s">Signature</asp:ListItem>
                <asp:ListItem Value="i">Initial</asp:ListItem>
            </asp:DropDownList>
            <asp:FileUpload ID="uplSignatures" runat="server" Enabled="false" CssClass="fileUpload" accept=".jpeg,.png,.jpg" onchange='check_signature(this.value);' ToolTip="Select Image Type First" />
            <asp:Button ID="cmdAddSignature" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
            <span id="fileSignatureWarning" class="dnnFormMessage dnnFormError" style="display: none; right: auto; left: 25%;">Please Choose File to Upload</span>
        </div>
    </div>
    <div class="dnnFormItem">
        <div class="formFieldAdjust">
            <span class="signatureInfo"></span>
            <ul id="signatureList" class="signatures">
            </ul>
        </div>
    </div>
    <asp:HiddenField ID="hdSignatureIds" runat="server" />

    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" runat="server" Text="Save" CssClass="btn btn-primary" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="btn btn-danger" />
        </li>
    </ul>
    <asp:Panel ID="pnlSignatures" runat="server" Visible="false">
        <h2 id="dnnPanel-Signatures" class="dnnFormSectionHead">Signatures</h2>
        <div class="row">
            <div class="col-md-6">
                <strong>Signature</strong>
                <br />
                <asp:Image ID="imgSignature" runat="server" CssClass="img-fluid img-thumbnail" />
            </div>
            <div class="col-md-6">
                <strong>Initial</strong>
                <br />
                <asp:Image ID="imgInitial" runat="server" CssClass="img-fluid img-thumbnail"  />
            </div>
        </div>

    </asp:Panel>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/jquery-timepicker/1.10.0/jquery.timepicker.min.js" />

<script type="text/javascript">
    var extensionHash = {
        '.jpeg': 1,
        '.png': 1,
        '.jpg': 1,
    };

    function check_signature(filename) {
        var re = /\..+$/;
        var ext = filename.match(re);
        var submitEl = document.getElementById('<%=cmdAddSignature.ClientID%>');
        if (extensionHash[ext]) {
            $(".info").html("");
            $( "#<%=cmdAddSignature.ClientID%>").trigger("click");
            submitEl.disabled = false;
            return true;
        } else {
            $(".signatureInfo").html("<span class='NormalRed'>Invalid Image Type, please choose a JPEG or PNG!</span>");
            submitEl.disabled = true;
            return false;
        }
    }
    function DeleteSignature(fileId) {
        var data = new FormData();
        data.append("fileId", fileId);
        var options = {};
        options.url = "<%=signatureHandler%>";
        options.type = "POST";
        options.data = data;
        options.contentType = false;
        options.processData = false;
        options.success = function (result) {
            var listItem = $("li[data-fileid='" + fileId + "']");
            listItem.remove();
            var fileList = "";
            $('li[data-fileid]').each(function () {
                var id = $(this).data("fileid");
                var type = $(this).data("type");
                if (fileList == "") {
                    fileList = type + ":" + id;
                } else {
                    fileList = fileList + "," + type + ":" + id;
                }
            });
            $("#<%=hdSignatureIds.ClientID%>").val(fileList);
        };
        options.error = function (err) { alert(err.statusText); };
        $.ajax(options);

        return false;
    }

    $(document).ready(function () {
        //Disable the signature Button
        setTimeout(function () {
            $("#<%=uplSignatures.ClientID%>").parent().addClass("disabledWrapper");
        }, 300);
        $(".date").timepicker({});;


        //send signature async
        $("#<%=cmdAddSignature.ClientID%>").click(function (evt) {
            $("#sign-overlay").show();

            var upload = $("#<%=uplSignatures.ClientID%>");

            if (upload.is(':enabled')) {
                var selectedSignatureType = $("#<%=drpSignatureType.ClientID%>").val();
                var selectedText = $( "#<%=drpSignatureType.ClientID%> option:selected").text();
                var fileUpload = $("#<%=uplSignatures.ClientID%>").get(0);
                var files = fileUpload.files;
                if (files.length == 0) {
                    $(".signatureInfo").html("<span class='NormalRed'>Please Choose a Signature Image!</span>");
                    return false;
                }
                var filename = files[0].name;
                if (selectedSignatureType === "") {
                    $(".signatureInfo").html("<span class='NormalRed'>Please Choose a Signature Type!</span>");
                    return false;
                }
                var data = new FormData();
                data.append(filename, files[0]);
                data.append("signatureType", selectedSignatureType);
                var options = {};
                options.url = "<%=signatureHandler%>";
                options.type = "POST";
                options.data = data;
                options.contentType = false;
                options.processData = false;
                options.success = function (fileId) {
                    $("#sign-overlay").hide();
                    var fileIdList = $("#<%=hdSignatureIds.ClientID%>").val();
                    var valueSet = selectedSignatureType + ":" + fileId;
                    if (fileIdList.length == 0) {
                        $("#<%=hdSignatureIds.ClientID%>").val(valueSet);
                    } else {
                        $("#<%=hdSignatureIds.ClientID%>").val(fileIdList + "," + valueSet);
                    }
                    $("#signatureList").append("<li data-type='" + selectedSignatureType + "' data-fileId='" + fileId + "'><span class='fileType'>" + selectedText + "</span>&nbsp;<a onclick=\"DeleteSignature('" + fileId + "')\"><em class='fa fa-trash'></em></></li>");
                    WritesignatureMessage(filename);
                };
                options.error = function (err) {
                    alert(err.statusText);
                    setTimeout(function () {
                        $("#sign-overlay").hide();
                        upload.parent().addClass("disabledWrapper");
                        //upload.parent().text("Choose File");
                        upload.attr("disabled", true);
                        upload.attr("title", "Select Signature Type First");
                        $(".signatureInfo").html('');
                    }, 1000);

                    $("#<%=drpsignatureType.ClientID%>").val('');

                };

                $.ajax(options);

                evt.preventDefault();
            }
        });

        //check signature type and enable upload if file type selected
        $("#<%=drpsignatureType.ClientID%>").change(function () {
            var upload = $("#<%=uplSignatures.ClientID%>");
            if ($(this).val() != "") {
                upload.attr("disabled", false);
                upload.parent().removeClass("disabledWrapper");
                upload.attr("title", "Select File to Upload");
            } else {
                upload.attr("disabled", true);
                upload.parent().addClass("disabledWrapper");
                upload.attr("title", "Select Signature Type First");
            }
        });
    });

    function ValidateCountyList(source, args) {
        var cklCounties = document.getElementById('<%= cklCounties.ClientID %>');
        var chkListinputs = cklCounties.getElementsByTagName("input");
        for (var i = 0; i < chkListinputs.length; i++) {
            if (chkListinputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }

    function WritesignatureMessage(filename) {
        if (filename == "") {
            $(".signatureInfo").html("<span class='NormalRed'>Unable to upload file. Please make sure the file JPEG or PNG.</span>");
        } else {
            $("#fileSignatureWarning").fadeOut();
            $(".signatureInfo").html("<span class='NormalRed'>Signature Captured.</span>");
        }
        var upload = $("#<%=uplSignatures.ClientID%>");
        upload.parent().addClass("disabledWrapper");
        var html = upload.parent().html();
        upload.parent().html(html.replace(filename, "Choose File"));
        //upload.parent().text("Choose File");
        upload.attr("disabled", true);
        upload.attr("title", "Select signature Type First");
        $(".signatureInfo").html('');
        $("#<%=drpSignatureType.ClientID%>").val('');
    }
</script>
