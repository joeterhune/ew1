<%@ Control Language="vb" Inherits="AWS.Modules.Warrants.EditWarrants" CodeFile="EditWarrants.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">

    <div class="dnnFormItem dnnFormHelp dnnClear">
        <span class="dnnFormRequired"><%=Localization.GetString("RequiredFields", Localization.SharedResourceFile)%></span>
    </div>
    <asp:Panel ID="pnlWarrant" runat="server">
        <h2 id="dnnPanel-Warrants" class="dnnFormSectionHead">Select File to Upload</h2>

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
            <dnn:Label ID="lblDefendantName" runat="server" ControlName="txtDefendant" Text="Party" Suffix=":" CssClass="dnnFormRequired" HelpText="Enter Defendant Name or unknown defendant."></dnn:Label>
            <asp:TextBox ID="txtDefendant" runat="server" CssClass="NormalTextBox" MaxLength="250" placeholder='Enter "unknown" where applicable.' TabIndex="2" />
            <asp:RequiredFieldValidator ID="valDefendant" ControlToValidate="txtDefendant"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="Please enter the Party's Name" runat="server" />
        </div>
        <div id="divCounty" runat="server" class="dnnFormItem awsToggleCountyList">
            <dnn:Label ID="lblCounty" runat="server" ControlName="drpCounty" Text="County" Suffix=":" CssClass="dnnFormRequired" HelpText="Select County"></dnn:Label>
            <asp:DropDownList ID="drpCounty" runat="server" onchange="FillJudges()" TabIndex="3"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="valCounty" ControlToValidate="drpCounty"
                CssClass="dnnFormMessage dnnFormError" Display="Dynamic" ErrorMessage="You must select a county" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblOffenseType" runat="server" ControlName="rblOffenseType" Text="Division Type" Suffix=":" CssClass="dnnFormRequired" HelpText="Select Division Type"></dnn:Label>
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

        <div class="dnnFormMessage dnnFormError awsToggleMessage" style="margin-left: 34.7%; max-width: 427px; clear: both">
            No Judges exits for the selected division.<br />
            Please select a different division or contact the site administrator.
        </div>
        <div class="dnnFormItem warrant-type">
            <dnn:Label ID="lblWarrantType" runat="server" ControlName="rblWarrantType" Text="Document Type" Suffix=":" CssClass="dnnFormRequired" HelpText="Select Document Type"></dnn:Label>
            <asp:RadioButtonList runat="server" ID="rblWarrantType" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons" TabIndex="6" RepeatLayout="Flow">
                <asp:ListItem Value="1" Text="Arrest Warrant" />
                <asp:ListItem Value="2" Text="Search Warrant" />
                <asp:ListItem Value="3" Text="Pickup Order" />
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="valWarrantType" runat="server" ControlToValidate="rblWarrantType" CssClass="dnnFormMessage dnnFormError"
                Display="Dynamic" ErrorMessage="Document Type is Required" />
        </div>
        <div class="validation">
            <div class="dnnFormItem">
                <dnn:Label ID="lblWarrant" runat="server" ControlName="chkWarrant" Text="Warrant Included?" Suffix=":" CssClass="dnnFormRequired" HelpText="Check to Confirm that the Warrant is Included in the Download" />
                <asp:CheckBox ID="chkWarrant" runat="server" CssClass="warrant-chk" TabIndex="7" />
                <asp:CustomValidator runat="server" ID="valWarrantCheck" EnableClientScript="true"
                    OnServerValidate="chkWarrantRequired_ServerValidate" CssClass="dnnFormMessage dnnFormError"
                    ClientValidationFunction="chkWarrantRequired_ClientValidate">Warrant must be included</asp:CustomValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblAffidavit" runat="server" ControlName="chkAffidavit" Text="Affidavit Included?" Suffix=":" CssClass="dnnFormRequired" HelpText="Check to Confirm that the Affidavit is Included in the Download" />
                <asp:CheckBox ID="chkAffidavit" runat="server" CssClass="affidavit-chk" TabIndex="8" />
                <asp:CustomValidator runat="server" ID="valAffidavitCheck" EnableClientScript="true" CssClass="dnnFormMessage dnnFormError"
                    OnServerValidate="chkAffidavitRequired_ServerValidate"
                    ClientValidationFunction="chkAffidavitRequired_ClientValidate">Affidavit must be included</asp:CustomValidator>
            </div>
            <div class="dnnFormItem pca">
                <dnn:Label ID="lblPCA" runat="server" ControlName="chkPCA" Text="PCA Included?" Suffix=":" CssClass="dnnFormRequired warrantLabel" HelpText="Check to Confirm that the PCA is Included in the Upload" />
                <asp:CheckBox ID="chkPCA" runat="server" CssClass="pca-chk checkbox" Text=" " />
                <asp:CustomValidator runat="server" ID="valPCACheck" EnableClientScript="true"
                    OnServerValidate="valPCACheck_ServerValidate" CssClass="dnnFormMessage dnnFormError"
                    ClientValidationFunction="chkPCARequired_ClientValidate">PCA must be included</asp:CustomValidator>
            </div>

            <div class="dnnFormItem sa-approve">
                <dnn:Label ID="lblsaApproved" runat="server" ControlName="chkSaApproved" Text="State Attorney Approved?" Suffix=":" HelpText="Check to Confirm that the State Attorney has Approved the Document" />
                <asp:CheckBox ID="chkSaApproved" runat="server" CssClass="awsShowText sa-chk" Text="( Check box if Yes )" TabIndex="9" />
            </div>
            <div class="dnnFormItem awsToggleDisplay">
                <dnn:Label ID="lblsaName" runat="server" ControlName="txtSaName" Text="State Attorney Name" Suffix=":" />
                <asp:TextBox ID="txtSaName" runat="server" MaxLength="100" TabIndex="10" />
                <asp:CustomValidator ID="valSAName" runat="server" ErrorMessage="State Attorney Name is Required" ClientValidationFunction="txtSaName_ClientValidate" CssClass="dnnFormMessage dnnFormError"
                    Display="Dynamic"></asp:CustomValidator>
            </div>
            <div class="dnnFormItem" id="dvEmergency">
                <dnn:Label ID="lblEmergency" runat="server" ControlName="rblEmergency" CssClass="dnnFormRequired" Text="Is this an emergency?" Suffix=":" HelpText="If this is an emergency the Judge will be notified immediately otherwise the notification will be sent in the next day" />
                <asp:RadioButtonList ID="rblEmergency" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" TabIndex="11" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Text="Yes" Value="1" />
                    <asp:ListItem Text="No" Value="0" />
                </asp:RadioButtonList>
                <asp:CustomValidator ID="valEmergency" runat="server" ClientValidationFunction="rblEmergency_ClientValidate" OnServerValidate="valEmergency_ServerValidate" ErrorMessage="You must select Yes or No" Display="Dynamic" CssClass="dnnFormMessage dnnFormError"></asp:CustomValidator>
                <p class="dnnClear dnnFormMessage dnnFormInfo" style="margin-left: 35%; width: 47%; max-width: 445px">Selecting Yes will send the document notification immediately to the Judge. Selecting No will postpone the notification until <span id="judgeTime"></span>.</p>
            </div>
        </div>
        <asp:HiddenField ID="hdFileId" runat="server" />
        <asp:HiddenField ID="hdJudgeId" runat="server" />
    </asp:Panel>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:Button ID="cmdUpdate" runat="server" Text="Update" OnClientClick="return doSubmit(this)" CssClass="btn btn-primary submitbutton" /></li>
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
        if ($("#<%=hdFileId.ClientID%>").val() === "") {
            $(".val-image").show();
            return false;
        }
        cmdUpdate.disabled = 'disabled';
        cmdUpdate.value = 'Processing ...';

    <%= Page.ClientScript.GetPostBackEventReference(cmdUpdate, String.Empty) %>;
    }
    //---file upload handler
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
    // ---end file upload handler
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

        $(".validation").hide();
        $(".pca").hide();
        $(".sa-approve").hide();
        $(".warrant-type").hide();
        $(".awsToggleDisplay").hide();
        $(".awsToggleMessage").hide();
        $(".awsToggleJudgeList").hide();
        $("#dvEmergency").hide();
        $(".submitbutton").click(function () {
            if ($("#<%=hdFileId.ClientID%>").val() === "") {
                $("#fileInputWarning").show();
                return false;
            }
            return true;
        });
        $("#<%=chkWarrant.ClientID%>").css("visibility", "visible").css("opacity", "0");
        $("#<%=chkAffidavit.ClientID%>").css("visibility", "visible").css("opacity", "0");
        $("#<%=chkPCA.ClientID%>").css("visibility", "visible").css("opacity", "0");
        $("#<%=chkSaApproved.ClientID%>").css("visibility", "visible").css("opacity", "0");
        $(".dnnFormRadioButtons input").css("visibility", "visible").css("opacity", "0");
        var pickupOrder = $("[id*=rblWarrantType] input[value=" + 3 + "]");
        var arrestWarrant = $("[id*=rblWarrantType] input[value=" + 1 + "]");

        $(".awsShowText").click(function (e) { $(".awsToggleDisplay").toggle(); });
        if ($("#<%=drpCounty.ClientID%> option").length <= 2) {
            $(".awsToggleCountyList").hide()
            $("#<%=drpCounty.ClientID%> option:eq(1)").prop("selected", true);
        }

        //Division Type Selection
        $("#<%=rblOffenseType.ClientID%> input").click(function () {
            $('#<%= rblWarrantType.ClientID%> input:checked').prop("checked", false);
            $(".validation").hide();
            var divsionDrop = $("#<%=rblOffenseType.ClientID%> input");
            var selectedIndex = divsionDrop.index(this);
            var selectedText = divsionDrop[selectedIndex].labels[0].outerText;
            if (selectedText.toLowerCase() === "other") {
                $(".pca-chk input").prop("checked", true);
                $(".warrant-chk input").prop("checked", true);
                $(".affidavit-chk input").prop("checked", true);
                $("#<%=rblWarrantType.ClientID%> input")[0].checked = true;
                $("#<%=rblWarrantType.ClientID%> input")[0].value = "0";
            }
            else {
                if (selectedIndex < 2) {
                    pickupOrder.next().hide().next().hide();
                    pickupOrder.hide();
                    arrestWarrant.next().fadeIn("slow").next().fadeIn("slow");
                    arrestWarrant.show();
                    $(".pca").hide();
                    $(".sa-approve").fadeIn("slow");
                    $(".pca-chk input").prop("checked", true);
                    $("#<%=lblWarrant.ClientID%>_lblLabel").text("Warrant Included?");
                    $("#<%=valWarrantCheck.ClientID%>").text("Warrant must be included");
                } else {
                    pickupOrder.next().fadeIn("slow").next().fadeIn("slow");
                    pickupOrder.show();
                    arrestWarrant.next().hide().next().hide();
                    arrestWarrant.hide();
                }
                $(".warrant-type").fadeIn("slow");
            }
        });

        // Warrant Type Selection
        $("#<%=rblWarrantType.ClientID%> input").click(function () {
            $(".validation").fadeIn("slow");
            var index = $("#<%=rblWarrantType.ClientID%> input").index(this);
            if (index <= 1) {
                $(".pca-chk input").prop("checked", true);
                $(".pca").hide();
                $("#<%=lblWarrant.ClientID%>_lblLabel").text("Warrant Included?");
                $("#<%=valWarrantCheck.ClientID%>").text("Warrant must be included");

                $(".sa-approve").fadeIn("slow");
            } else {
                $(".pca").fadeIn("slow");
                $(".sa-approve").hide();
                $(".pca-chk input").prop("checked", false);
                $("#<%=lblWarrant.ClientID%>_lblLabel").text("Pickup Order Included?");
                $("#<%=valWarrantCheck.ClientID%>").text("Pickup Order must be included");
            }
        });
        FillJudges();
    });

    function WriteMessage(fileId) {
        if (fileId == "0") {
            $(".info").html("<span class='NormalRed'>Unable to upload file. Please make sure the file is in Tiff or PDF format.</span>");
        } else {
            $("#fileInputWarning").fadeOut();
            $(".info").html("<span class='NormalRed'>Document Captured.</span>");
            $(".firstText").focus();
        }
    }

    function chkWarrantRequired_ClientValidate(sender, e) {
        e.IsValid = $(".warrant-chk input:checkbox").is(':checked');
    }

    function chkAffidavitRequired_ClientValidate(sender, e) {
        e.IsValid = $(".affidavit-chk input:checkbox").is(':checked');
    }

    function chkPCARequired_ClientValidate(sender, e) {
        e.IsValid = $(".pca-chk input:checkbox").is(':checked');
    }

    function rblEmergency_ClientValidate(sender, e) {
        if ($("#dvEmergency").is(":visible")) {
            var rbl = $("#<%=rblEmergency.ClientID %> input:checked").length;
            if (rbl == 0) { e.IsValid = false } else { e.IsValid = true }

        } else {
            e.IsValid = true;
        }
    }

    function txtSaName_ClientValidate(sender, e) {
        if ($("#<%=chkSaApproved.ClientID%> input:checkbox").is(':checked')) {

            if ($("#<%=txtSaName.ClientID%>").val() == "") {
                e.IsValid = false
            } else {
                e.IsValid = true
            }
        } else {
            e.IsValid = true;
        }
    }

    function SetJudgeId() {
        var judgeId = $("#<%=drpJudge.ClientID%> option:selected").val();
        $('#<%=hdJudgeId.ClientID%>').val(judgeId);
        var Url = "/DesktopModules/AWSModules/Warrants/api/HttpData/notifyJudge/" + judgeId + "/";
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
        var Url = "/DesktopModules/AWSModules/Warrants/api/HttpData/judges/" + ModuleId + "/";
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


</script>
