<%@ Control Language="vb" Inherits="AWS.Modules.Injunctions.EditInjunctions" CodeFile="EditInjunction.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">
    <div class="dnnFormItem dnnFormHelp dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>
    <asp:Panel ID="pnlInjunction" runat="server">
        <div class="filebox">
            <div id="upload-overlay" class="overlay" style="display: none;">
                <div class="spinner"></div>
            </div>
            <p class="alert alert-info" style="margin-top: 10px;"><em class="fa fa-info-circle"></em>&nbsp;To upload a document for review, click "<strong>Choose File</strong>" and browse for the file on your computer.</p>
            <div class="dnnFormItem clearfix">
                <asp:FileUpload ID="uplWarrant" runat="server" CssClass="fileUpload" accept=".pdf,.tif,.tiff" onchange='check_extension(this.value);' ToolTip="Select File to Upload" />
                <asp:Button ID="cmdUpload" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
                <span id="fileInputWarning" class="dnnFormMessage dnnFormError" style="display: none; right: auto; left: 25%;">Please Choose File to Upload</span>

            </div>
            <p class="info clearfix"></p>
        </div>
        <h2 id="dnnPanel-FileInfo" class="dnnFormSectionHead">Enter Document Information</h2>
        <div class="dnnFormItem" id="agencyDiv" runat="server" visible="false">
            <dnn:Label ID="lblAgency" runat="server" ControlName="drpAgency" Text="Agency" Suffix=":" CssClass="dnnFormRequired" HelpText="Select the Associated Agency"></dnn:Label>
            <asp:DropDownList ID="drpAgency" runat="server" onchange="FillJudges()" TabIndex="0"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="valAgency" ControlToValidate="txtTitle"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Please select an agency" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTitle" runat="server" ControlName="txtTitle" Text="Title" Suffix=":" CssClass="dnnFormRequired" HelpText="Enter Document Title"></dnn:Label>
            <asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="firstText NormalTextBox" TabIndex="1" />
            <asp:RequiredFieldValidator ID="valTitle" ControlToValidate="txtTitle"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Please enter a descriptive title" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDefendantName" runat="server" ControlName="txtDefendant" Text="Defendant" Suffix=":" CssClass="dnnFormRequired" HelpText="Enter Defendant Name or unknown defendant."></dnn:Label>
            <asp:TextBox ID="txtDefendant" runat="server" CssClass="NormalTextBox" MaxLength="250" placeholder='Enter "unknown defandant" where applicable.' TabIndex="2" />
            <asp:RequiredFieldValidator ID="valDefendant" ControlToValidate="txtDefendant"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Please enter the Defendant's Name" runat="server" />
        </div>
        <div id="divCounty" runat="server" class="dnnFormItem awsToggleCountyList">
            <dnn:Label ID="lblCounty" runat="server" ControlName="drpCounty" Text="County" Suffix=":" CssClass="dnnFormRequired" HelpText="Select County"></dnn:Label>
            <asp:DropDownList ID="drpCounty" runat="server" onchange="FillJudges()" TabIndex="3"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="valCounty" ControlToValidate="drpCounty"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="You must select a county" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblOffenseType" runat="server" ControlName="rblOffenseType" Text="Type" Suffix=":" CssClass="dnnFormRequired" HelpText="Select Division Type"></dnn:Label>
            <asp:RadioButtonList ID="rblOffenseType" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons" TabIndex="4" RepeatLayout="Flow"></asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="valOffenseType" ControlToValidate="rblOffenseType"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="You must select an Division Type" runat="server" />
        </div>
        <div id="JudgeContainer" class="dnnFormItem awsToggleJudgeList">
            <dnn:Label ID="lblJudge" runat="server" ControlName="drpJudge" Text="Judge" Suffix=":" CssClass="dnnFormRequired" HelpText="Select a Judge from the List"></dnn:Label>
            <asp:DropDownList ID="drpJudge" runat="server" onchange="SetJudgeId()" TabIndex="5"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="valJudge" ControlToValidate="drpJudge"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="You must select a judge" runat="server" />
        </div>
        <div class="dnnFormItem awsToggleMessage">
            <p style="color: red; font-weight: bold; margin-left: 360px;">
                No Judges exits for the selected division.<br />
                Please select a different division or contact the site administrator.
            </p>
        </div>
        <div class="dnnFormItem" id="dvEmergency">
            <dnn:Label ID="lblEmergency" runat="server" ControlName="rblEmergency" CssClass="dnnFormRequired" Text="Is this an emergency?" Suffix=":" HelpText="If this is an emergency the Judge will be notified immediately otherwise the notification will be sent in the next day" />
            <asp:RadioButtonList ID="rblEmergency" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" TabIndex="11" CssClass="dnnFormRadioButtons">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
            <asp:CustomValidator ID="valEmergency" runat="server" ClientValidationFunction="rblEmergency_ClientValidate" OnServerValidate="valEmergency_ServerValidate" ErrorMessage="You must select Yes or No" Display="Dynamic" CssClass="dnnFormMessage dnnFormError"></asp:CustomValidator>
            <p class="dnnClear dnnFormMessage dnnFormInfo" style="margin-left: 35%; width: 47%; max-width: 445px">Selecting Yes will send the Document notification immediately to the Judge. Selecting No will postpone the notification until <span id="judgeTime"></span>.</p>
        </div>
        <asp:HiddenField ID="hdFileId" runat="server" />
        <asp:HiddenField ID="hdJudgeId" runat="server" />
    </asp:Panel>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:Button ID="cmdUpdate" runat="server" Text="Update" OnClientClick="doSubmit(this)" CssClass="btn btn-primary submitbutton" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" Text="Cancel" CausesValidation="false" CssClass="btn btn-danger" />
        </li>
    </ul>
</div>
<script type="text/javascript">
    function doSubmit(cmdUpdate) {
        if (typeof (Page_ClientValidate) == 'function' && Page_ClientValidate() == false) {
            return false;
        }
        cmdUpdate.disabled = 'disabled';
        cmdUpdate.value = 'Processing ...';
    <%= Page.ClientScript.GetPostBackEventReference(cmdUpdate, String.Empty) %>;
    }
    var extensionHash = {
        'pdf': 1,
        'tif': 1,
        'tiff': 1,
    };

    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        var submitEl = document.getElementById('<%=cmdUpload.ClientID%>');
        if (extensionHash[ext]) {
            $(".info").html("");
            $("#<%=cmdUpload.ClientID%>").trigger("click");
            submitEl.disabled = false;
            return true;
        } else {
            $(".info").html("<span class='NormalRed'>Invalid File Type, please choose a Tiff or PDF file!</span>");
            submitEl.disabled = true;
            return false;
        }
    }

    $(document).ready(function () {
        $("#<%=cmdUpload.ClientID%>").click(function (evt) {
            $("#upload-overlay").show();
            var fileUpload = $("#<%=uplWarrant.ClientID%>").get(0);
            var files = fileUpload.files;
            if (files.length == 0) {
                $(".info").html("<span class='NormalRed'>Please Choose a File!</span>");
                return false;
            }
            var data = new FormData();
            data.append(files[0].name, files[0]);
            var options = {};
            options.url = "<%=uploadHandler%>";
            options.type = "POST";
            options.data = data;
            options.contentType = false;
            options.processData = false;
            options.success = function (fileId) {
                $("#upload-overlay").hide();
                $("#<%=hdFileId.ClientID%>").val(fileId);
                WriteMessage(fileId);
            };
            options.error = function (err) {
                $("#upload-overlay").hide();
                alert(err.statusText);
            };

            $.ajax(options);

            evt.preventDefault();
        });

        $(".awsToggleDisplay").hide();
        $(".awsToggleMessage").hide();
        $(".awsToggleJudgeList").hide();
        $("#dvEmergency").hide();
        $(".awsShowText").click(function (e) { $(".awsToggleDisplay").toggle(); });

        if ($("#<%=drpCounty.ClientID%> option").length <= 2) {
            $(".awsToggleCountyList").hide()
            $("#<%=drpCounty.ClientID%> option:eq(1)").prop("selected", true);
        }
        $(".submitbutton").click(function () {
            if ($("#<%=hdFileId.ClientID%>").val() === "") {
                $("#fileInputWarning").show();
                return false;
            }
            return true;
        });

        FillJudges();
    });


    function rblEmergency_ClientValidate(sender, e) {
        if ($("#dvEmergency").is(":visible")) {
            var rbl = $("#<%=rblEmergency.ClientID %> input:checked").length;
            if (rbl == 0) { e.IsValid = false } else { e.IsValid = true }

        } else {
            e.IsValid = true;
        }
    }

    function SetJudgeId() {
        var judgeId = $("#<%=drpJudge.ClientID%> option:selected").val();
        $('#<%=hdJudgeId.ClientID%>').val(judgeId);
        var Url = "/DesktopModules/AWSModules/Injunctions/api/HttpData/notifyJudge/" + judgeId + "/";
        var result = "";
        $.ajax({
            type: "GET",
            dataType: "json",
            url: Url,
            success: function (j) {
                if (j.Available === false) {
                    $("#dvEmergency").show();
                    $("#judgeTime").text(j.StartOfDay);
                }
            },
            error: function () {
                $(".awsToggleMessage p").html("Error retrieving judge availability.<br />Please contact the site administrator with this message.");
                $(".awsToggleMessage").show();
            }
        });
    }

    function FillJudges() {
        countyName = $("#<%=drpCounty.ClientID%> option:selected").val();
        judgeTypeId = $('#<%= rblOffenseType.ClientID%> input:checked').val();
        judgeList = $('#<%=drpJudge.ClientID%>');
        var ModuleId = '<%=Moduleid%>';
        var Url = "/DesktopModules/AWSModules/Injunctions/api/HttpData/judges/" + ModuleId + "/";
        var choices = '';
        if (countyName != "" && judgeTypeId != undefined) {
            Url += countyName + "/" + judgeTypeId;
            $.ajax({
                type: "GET",
                dataType: "json",
                url: Url,
                success: function (j) {
                    if (j.length == 0) {
                        $(".awsToggleMessage").show();
                        $(".awsToggleJudgeList").hide();
                        choices += '<option value="">No Judges Found</option>';
                        judgeList.html(choices);
                    } else {
                        $(".awsToggleMessage").hide();
                        $(".awsToggleJudgeList").show();
                        choices += '<option value="">< Select Judge ></option>';
                        for (var i = 0; i < j.length; i++) {
                            c = j[i];
                            choices += '<option value="' + c.Value + '">' + c.Text + '</option>';
                        }
                        judgeList.html(choices);
                    }
                },
                error: function () {
                    $(".awsToggleMessage p").html("Error retrieving judge names.<br />Please contact the site administrator with this message.");
                    $(".awsToggleMessage").show();
                }
            });
        } else {
            choices += '<option value="">No Judges for Selected Options</option>';
            judgeList.html(choices);
        }
    }

    function WriteMessage(fileId) {
        if (fileId == "0") {
            $(".info").html("<span class='NormalRed'>Unable to upload file. Please make sure the file is in Tiff or PDF format.</span>");
        } else {
            $("#fileInputWarning").fadeOut();
            $(".info").html("<span class='NormalRed'>Warrant File Captured.</span>");
            $(".firstText").focus();
        }
    }

</script>
