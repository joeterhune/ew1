Imports DotNetNuke
Imports System.Reflection

Namespace AWS.Modules.Injunctions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageInjunctions
        Inherits Entities.Modules.PortalModuleBase

#Region "Methods"
        Private Sub InitializeAdminToolbar()
            Dim AdminJudge As String = ""
            Dim JudgeRoleName As String = ""
            Dim SergeantRole As String = ""
            If Not Settings("AdminJudge") Is Nothing Then
                AdminJudge = Settings("AdminJudge").ToString.Trim
            End If
            If AdminJudge <> "" Then
                toolbar.IsAdminJudge = (AdminJudge = UserInfo.Username)
            End If
            If Not Settings("JudgeRole") Is Nothing Then
                JudgeRoleName = Settings("JudgeRole").ToString.Trim
            End If
            If Not Settings("SergeantRole") Is Nothing Then
                SergeantRole = Settings("SergeantRole").ToString.Trim
            End If
            toolbar.AdminJudge = AdminJudge
            toolbar.IsClerk = False
            toolbar.IsJudge = False
            toolbar.IsSiteAdmin = (UserInfo.IsSuperUser Or UserInfo.IsInRole("Administrators"))
            toolbar.JudgeRoleName = JudgeRoleName
            toolbar.NavAddInjunction = EditUrl("addInjunction")
            toolbar.HiddenMenu = "Status"
            toolbar.NavAdminAgency = EditUrl("adminagency")
            toolbar.NavAdminCounty = EditUrl("admincounty")
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.NavManageUsers = EditUrl("users")
            toolbar.NavStatus = ""
            toolbar.NavOutofOffice = EditUrl("cover")
            toolbar.objAgencyUser = Nothing
            toolbar.SergeantRoleName = SergeantRole
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    InitializeAdminToolbar()
                    lnkCancel.NavigateUrl = NavigateURL()
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
            Dim injunctionId As Integer = Int32.Parse(txtInjunctionId.Text)
            Dim ctl As New Controller
            Dim objinjunction As InjunctionsInfo = ctl.GetInjunctions(injunctionId)
            If Not objinjunction Is Nothing Then
                If objinjunction.StatusId >= 3 Then
                    If drpStatus.SelectedIndex < 2 Then
                        Dim success As Boolean = True
                        Dim fileId As Integer = Null.NullInteger
                        fileId = ctl.FileExists(objinjunction.FileId)
                        If fileId <= 0 Then
                            Dim pdfFileName As String = ConfigurationManager.AppSettings("CompletedInjunctions")
                            success = ImageProcessing.InsertFile(objinjunction, pdfFileName)
                        End If
                        If Not success Then
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Unable to Proccess Document File", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If
                    End If
                End If
                objinjunction.StatusId = drpStatus.SelectedValue
                ctl.UpdateInjunctions(objinjunction)
                pnlStatus.Visible = False
                txtInjunctionId.Text = ""
                ltInjunctionInfo.Text = ""
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Injuction Status Updated", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "No Matching Injunction Found", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Sub

        Protected Sub cmdFindInjunction_Click(sender As Object, e As EventArgs) Handles cmdFindInjunction.Click
            If Not IsNumeric(txtInjunctionId.Text) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Please Enter a Valid Number", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Exit Sub
            End If

            Dim injunctionId As Integer = Int32.Parse(txtInjunctionId.Text)
            Dim ctl As New Controller
            Dim objinjunction As InjunctionsInfo = ctl.GetInjunctions(injunctionId)
            If Not objinjunction Is Nothing Then
                Dim sb As New StringBuilder
                sb.Append("<table class='dnnTableDisplay '><tbody><tr>")
                sb.Append("<td><strong>Title:</strong></td><td>")
                sb.Append(objinjunction.Title)
                sb.Append("</td><td><strong>Defendant:</strong></td><td>")
                sb.Append(objinjunction.Defendant)
                sb.Append("</td><td><strong>Agency:</strong></td><td colspan='3'>")
                sb.Append(objinjunction.AgencyName)
                sb.Append("</td></tr><tr><td><strong>Judge:</strong></td><td>")
                sb.Append(UserController.GetUserById(PortalId, objinjunction.JudgeUserId).DisplayName)
                sb.Append("</td><td><strong>Current Status:</strong></td><td>")
                sb.Append(GetInjunctionStatusName(objinjunction.StatusId))
                sb.Append("</td><td><strong>Created By:</strong></td><td>")
                sb.Append(UserController.GetUserById(PortalId, objinjunction.CreatedByUserId).DisplayName)
                sb.Append("</td><td><strong>Created Date:</strong></td><td>")
                sb.Append(objinjunction.CreatedDate.ToShortDateString)
                sb.Append("</td></tr></tbody></table>")
                ltInjunctionInfo.Text = sb.ToString
                pnlStatus.Visible = True
            Else
                ltInjunctionInfo.Text = "<strong style='color:red'>No Matching Injunction Found!</strong>"
            End If
        End Sub

        Private Function GetInjunctionStatusName(injunctionStatusId As Integer) As String
            Dim statusName As String = ""
            Select Case injunctionStatusId
                Case 1
                    statusName = "New Injunction"
                Case 2
                    statusName = "Under Review"
                Case 3
                    statusName = "Signed"
                Case 4
                    statusName = "Rejected"
                Case 5
                    statusName = "Reviewed"
            End Select
            Return statusName
        End Function
#End Region

    End Class

End Namespace
