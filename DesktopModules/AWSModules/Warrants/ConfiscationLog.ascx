<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.ConfiscationLog" CodeFile="ConfiscationLog.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<asp:Panel CssClass="dnnForm dnnClear" ID="dvForm" runat="server">
    <div class="filebox">
        <h2 id="dnnPanel-Warrants" class="dnnFormSectionHead">Select Return of Service to Upload (PDF Only)</h2>
        <p class="instructions">To upload a Return of Service, click select and browse for the pdf file on your computer.</p>
        <div class="filebox">
            <div id="upload-overlay" class="overlay" style="display: none;">
                <div class="spinner"></div>
            </div>
            <p class="alert alert-info" style="margin-top: 10px;"><em class="fa fa-info-circle"></em>&nbsp;To upload a document for review, click "<strong>Choose File</strong>" and browse for the file on your computer.</p>
            <div class="dnnFormItem clearfix">
                <span class="dnnInputFileWrapper dnnSecondaryAction">Choose File
                <asp:FileUpload ID="uplReturnService" runat="server" CssClass="fileUpload" accept=".pdf" onchange='check_extension(this.value);' ToolTip="Select File to Upload" />
                </span>
                <asp:Button ID="cmdUpload" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
                <span id="fileInputWarning" class="dnnFormMessage dnnFormError" style="display: none; right: auto; left: 25%;">Please Choose File to Upload</span>
            </div>
            <p id="info" style="margin-top: 15px"><asp:Literal ID="ltmessage" runat="server"></asp:Literal></p>
        </div>
    </div>

    <ul class="dnnActions dnnClear">
        <li>
            <asp:HyperLink ID="lnkCancel" runat="server" Text="Return" CssClass="btn btn-danger" />
        </li>
    </ul>
</asp:Panel>
<asp:HiddenField ID="hdFileId" runat="server" />

<script type="text/javascript">
    var extensionHash = {
        'pdf': 1,
    };

    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        var submitEl = document.getElementById('<%=cmdUpload.ClientID%>');
        $("#info").removeClass();
        if (extensionHash[ext]) {
            $("#info").html("");
            $("#<%=cmdUpload.ClientID%>").trigger("click");
            submitEl.disabled = false;
            return true;
        } else {
            $("#info").html("Invalid File Type, please choose a PDF file!");
            $("#info").addClass("alert alert-danger clearfix");
            submitEl.disabled = true;
            return false;
        }
    }
    $(document).ready(function () {
        $("#<%=cmdUpload.ClientID%>").click(function (evt) {
            $("#upload-overlay").show();
            var fileUpload = $("#<%=uplReturnService.ClientID%>").get(0);
            var files = fileUpload.files;
            if (files.length == 0) {
                $("#info").removeClass();
                $("#info").html("Please Choose a File!");
                $("#info").addClass("alert alert-danger clearfix");
                return false;
            }
            var data = new FormData();
            data.append(files[0].name, files[0]);
            data.append('wid', '<%=warrantId%>');
            var options = {};
            options.url = "<%=uploadHandler%>";
            options.type = "POST";
            options.data = data;
            options.contentType = false;
            options.processData = false;
            options.success = function (result) {
                $("#upload-overlay").hide();
                WriteMessage(result);
                setTimeout(function () { location.replace("/") }, 3000);
            };
            options.error = function (err) {
                $("#upload-overlay").hide();
                alert(err.statusText);
            };

            $.ajax(options);

            evt.preventDefault();
        });
    });
    function WriteMessage(result) {
        $("#info").removeClass();
        if (result.startsWith("<strong")) {
            $("#info").html(result);
            $("#info").addClass("alert alert-danger clearfix");
        } else {
            $("#fileInputWarning").fadeOut();
            $("#info").html(result);
            $("#info").addClass("alert alert-success clearfix");
        }
    }

</script>
