Imports System.Reflection
Imports DotNetNuke

Namespace AWS.Modules.Warrants

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDynamicModule class is used to manage content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class JudgeList
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private JudgeRole As String = "judge"
        Private ctl As New Controller
#End Region

#Region "properties"
        Public ReadOnly Property ChiefJudge() As String
            Get
                If Settings("AdminJudge") Is Nothing Then
                    Return ""
                Else
                    Return Settings("AdminJudge")
                End If
            End Get
        End Property

        Public ReadOnly Property IsAdminJudge() As Boolean
            Get
                Return UserInfo.IsInRole(ChiefJudge)
            End Get
        End Property

#End Region

#Region "Methods"
        Public Function GetDivision(judgeId As Integer) As String
            Dim divisions As String = ""
            Dim ctl As New Controller
            Dim colJudgeTypes As New List(Of JudgeTypeInfo)
            colJudgeTypes = ctl.ListJudgeTypesByJudge(judgeId, ModuleId)
            For Each jt As JudgeTypeInfo In colJudgeTypes
                divisions += jt.JudgeType + ", "
            Next
            Return divisions.ToString.Trim.TrimEnd(",")
        End Function

        Public Function GetCounties(judgeId As Integer) As String
            Dim counties As String = ""
            Dim ctl As New Controller
            Dim colCounties As New List(Of CountyInfo)
            colCounties = ctl.ListJudgeCounties(judgeId, ModuleId)
            For Each c As CountyInfo In colCounties
                counties += c.CountyCode + ", "
            Next
            Return counties.ToString.Trim.TrimEnd(",")
        End Function

        Private Function IsApproved(encryptedText As String, judgeId As String) As Boolean
            Dim approved As Boolean = False
            Dim judgeInfo As UserInfo = UserController.GetUserById(PortalId, judgeId)
            If Not judgeInfo Is Nothing Then
                Dim tempJudgeId As String = ""
                Dim encrypt As New Encryptor
                Try
                    tempJudgeId = encrypt.QueryStringDecode(encryptedText, ChiefJudge)
                Catch
                End Try
                If tempJudgeId = judgeId Then
                    approved = True
                End If
            End If
            Return approved
        End Function

        Private Sub BindData()
            rptJudges.DataSource = ctl.ListJudges(ModuleId)
            rptJudges.DataBind()
        End Sub

        Private Sub BindAdminData()
            rptAdminJudges.DataSource = ctl.ListJudges(ModuleId)
            rptAdminJudges.DataBind()
        End Sub
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
            toolbar.HiddenMenu = "ManageJudges"
            toolbar.NavAdminAgency = EditUrl("adminagency")
            toolbar.NavAdminCounty = EditUrl("admincounty")
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.NavManageUsers = EditUrl("users")
            toolbar.NavOutofOffice = EditUrl("cover")
            toolbar.NavStatus = EditUrl("wadmin")
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
                If Page.IsPostBack = False Then
                    InitializeAdminToolbar()
                    lnkCancel.NavigateUrl = NavigateURL()
                    If IsAdminJudge Then
                        rptAdminJudges.Visible = True
                        rptJudges.Visible = False
                        lnkAddJudge.NavigateUrl = EditUrl("EditJudge")
                        If UserInfo.IsInRole("Administrator") Then
                            lnkAddJudge.NavigateUrl = EditUrl("EditJudge")
                            lnkAddJudge.Visible = True
                            rptAdminJudges.Visible = False
                            rptJudges.Visible = True
                            BindData()
                        Else
                            BindAdminData()
                        End If
                    Else
                        'Security Redirect Not Chief Judge or Admin
                        Response.Redirect(NavigateURL)
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub UpdatePanel_Unload(ByVal sender As Object, ByVal e As EventArgs)
            Dim methodInfo As MethodInfo = GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance).Where(Function(i) i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First()
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {TryCast(sender, UpdatePanel)})
        End Sub

        Private Sub rptAdminJudges_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptAdminJudges.ItemCreated

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                Dim cmdApprove As LinkButton = DirectCast(item.FindControl("cmdApprove"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
                sm.RegisterAsyncPostBackControl(cmdApprove)
            End If

        End Sub

        Private Sub rptAdminJudges_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptAdminJudges.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim objJudge As JudgeInfo = e.Item.DataItem
                Dim lblSignature As Label = DirectCast(item.FindControl("lblSignature"), Label)
                Dim lblInitial As Label = DirectCast(item.FindControl("lblInitial"), Label)
                Dim cmdApprove As LinkButton = DirectCast(item.FindControl("cmdApprove"), LinkButton)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                cmdApprove.Text = "<em class='fa fa-square'>&nbsp;</em>"
                cmdApprove.CommandName = "approve"
                If objJudge.Approved <> "" Then
                    If IsApproved(objJudge.Approved, objJudge.JudgeId) Then
                        cmdApprove.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                        cmdApprove.CommandName = "deny"
                    End If
                End If
                lblSignature.Text = "<em class='fa fa-square'>&nbsp;</em>"
                lblSignature.Enabled = False
                lblInitial.Text = "<em class='fa fa-square'>&nbsp;</em>"
                lblInitial.Enabled = False
                If objJudge.HasSignature Then
                    lblSignature.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                End If
                If objJudge.HasInitial Then
                    lblInitial.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                End If
            End If
        End Sub


        Private Sub rptAdminJudges_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptAdminJudges.ItemCommand
            Dim _judgeId As Integer = CType(e.CommandArgument, Integer)

            If e.CommandName.ToLower = "approve" Then
                Dim encrypt As New Encryptor
                Dim approvalCode As String = encrypt.StringEncode(_judgeId, ChiefJudge)
                ctl.ApproveJudge(_judgeId, approvalCode)
                BindAdminData()
            End If

            If e.CommandName.ToLower = "deny" Then
                ctl.ApproveJudge(_judgeId, "")
                BindAdminData()
            End If

            If e.CommandName.ToLower = "delete" Then
                ctl.DeleteJudge(_judgeId)
                BindAdminData()
            End If

        End Sub

        Private Sub rptJudges_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptJudges.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim objJudge As JudgeInfo = e.Item.DataItem
                Dim lnkEdit As HyperLink = DirectCast(item.FindControl("lnkEdit"), HyperLink)
                Dim lblSignature As Label = DirectCast(item.FindControl("lblSignature"), Label)
                Dim lblInitial As Label = DirectCast(item.FindControl("lblInitial"), Label)
                lblSignature.Text = "<em class='fa fa-square'>&nbsp;</em>"
                lblSignature.Enabled = False
                lblInitial.Text = "<em class='fa fa-square'>&nbsp;</em>"
                lblInitial.Enabled = False
                If objJudge.HasSignature Then
                    lblSignature.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                End If
                If objJudge.HasInitial Then
                    lblInitial.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                End If
                lnkEdit.NavigateUrl = EditUrl("JudgeId", objJudge.JudgeId.ToString, "EditJudge")
            End If
        End Sub


#End Region
    End Class

End Namespace
