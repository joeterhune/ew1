
Namespace AWS.Modules.Warrants

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageWarrants
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
            toolbar.NavAddWarrant = EditUrl("addWarrant")
            toolbar.HiddenMenu = "Status"
            toolbar.NavAdminAgency = EditUrl("adminagency")
            toolbar.NavAdminCounty = EditUrl("admincounty")
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavStatus = ""
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.NavManageUsers = EditUrl("users")
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
            Dim warrantId As Integer = Int32.Parse(txtWarrantId.Text)
            Dim ctl As New Controller
            Dim objwarrant As WarrantsInfo = ctl.GetWarrants(warrantId)
            If Not objwarrant Is Nothing Then
                'If objwarrant.StatusId >= 3 Then
                '    If drpStatus.SelectedIndex < 2 Then
                '        Dim success As Boolean = True
                '        Dim fileId As Integer = Null.NullInteger
                '        fileId = ctl.FileExists(objwarrant.FileId)
                '        'If fileId <= 0 Then
                '        '    Dim pdfFileName As String = ConfigurationManager.AppSettings("CompletedWarrants")
                '        '    success = ImageProcessing.InsertFile(objwarrant, pdfFileName)
                '        'End If
                '        If Not success Then
                '            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Unable to Process Warrant File.  Please check that the warrant file exists in the archive", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                '        End If
                '    End If
                'End If
                objwarrant.StatusId = drpStatus.SelectedValue
                ctl.UpdateWarrants(objwarrant)
                pnlStatus.Visible = False
                txtWarrantId.Text = ""
                ltWarrantInfo.Text = ""
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Warrant Status Updated", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "No Matching Warrant Found", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)

            End If

        End Sub

        Protected Sub cmdFindWarrant_Click(sender As Object, e As EventArgs) Handles cmdFindWarrant.Click
            If Not IsNumeric(txtWarrantId.Text) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Please Enter a Valid Number", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Exit Sub
            End If
            Dim warrantId As Integer = Int32.Parse(txtWarrantId.Text)
            Dim ctl As New Controller
            Dim objwarrant As WarrantsInfo = ctl.GetWarrants(warrantId)
            If Not objwarrant Is Nothing Then
                Dim sb As New StringBuilder
                sb.Append("<table class='dnnTableDisplay '><tbody><tr>")
                sb.Append("<td><strong>Title:</strong></td><td>")
                sb.Append(objwarrant.Title)
                sb.Append("</td><td><strong>Defendant:</strong></td><td>")
                sb.Append(objwarrant.Defendant)
                sb.Append("</td><td><strong>Agency:</strong></td><td>")
                sb.Append(objwarrant.AgencyName)
                sb.Append("</td><td><strong>Warrant Type:</strong></td><td>")
                sb.Append(GetWarrantTypeName(objwarrant.WarrantType))
                sb.Append("</td></tr><tr><td><strong>Judge:</strong></td><td>")
                sb.Append(UserController.GetUserById(PortalId, objwarrant.JudgeUserId).DisplayName)
                sb.Append("</td><td><strong>Current Status:</strong></td><td>")
                sb.Append(GetWarrantStatusName(objwarrant.StatusId))
                sb.Append("</td><td><strong>Created By:</strong></td><td>")
                sb.Append(UserController.GetUserById(PortalId, objwarrant.CreatedByUserId).DisplayName)
                sb.Append("</td><td><strong>Created Date:</strong></td><td>")
                sb.Append(objwarrant.CreatedDate.ToShortDateString)
                sb.Append("</td></tr></tbody></table>")
                ltWarrantInfo.Text = sb.ToString
                pnlStatus.Visible = True
            Else
                ltWarrantInfo.Text = "<strong style='color:red'>No Matching Warrant Found!</strong>"
            End If
        End Sub

        Private Function GetWarrantStatusName(warrantStatusId As Integer) As String
            Dim statusName As String = ""
            Select Case warrantStatusId
                Case 1
                    statusName = "New Warrant"
                Case 2
                    statusName = "Under Review"
                Case 3
                    statusName = "Signed"
                Case 4
                    statusName = "Rejected"
                Case 5
                    statusName = "Reviewed"
                Case 6
                    statusName = "Return Service"
            End Select
            Return statusName
        End Function

        Private Function GetWarrantTypeName(warrantTypeId As Integer) As String
            Dim typeName As String = ""
            Select Case warrantTypeId
                Case 1
                    typeName = "Arrest"
                Case 2
                    typeName = "Search"
            End Select
            Return typeName
        End Function



#End Region

    End Class

End Namespace
