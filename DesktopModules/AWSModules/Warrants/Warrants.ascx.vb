Imports System.Reflection
Imports DotNetNuke
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
    Partial Class ViewWarrants
        Inherits ViewStateBase

#Region "Private Members"
        Private objAgencyUser As AgencyUserInfo = Nothing
        Private ctl As New Controller
        Private encrypt As New Encryptor
        Private IsClerk As Boolean


        Private Property CompletedWarrantsThreshold() As Integer
            Get
                If Not ViewState("CompletedWarrantsThreshold") Is Nothing Then
                    Return CType(ViewState("CompletedWarrantsThreshold"), Integer)
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("CompletedWarrantsThreshold") = value
            End Set
        End Property

        Private Property StartDate() As DateTime
            Get
                If Not ViewState("StartDate") Is Nothing Then
                    Return CType(ViewState("StartDate"), DateTime)
                Else
                    Return Now.AddDays(-11)
                End If
            End Get
            Set(ByVal value As DateTime)
                ViewState("StartDate") = value
            End Set
        End Property

        Private Property EndDate() As DateTime
            Get
                If Not ViewState("EndDate") Is Nothing Then
                    Return CType(ViewState("EndDate"), DateTime)
                Else
                    Return Now
                End If
            End Get
            Set(ByVal value As DateTime)
                ViewState("EndDate") = value
            End Set
        End Property

        Private ReadOnly Property IsAdminJudge() As Boolean
            Get
                If AdminJudgeRole <> "" Then
                    Return UserInfo.IsInRole(AdminJudgeRole)
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property AdminJudge As String
            Get
                If Not Settings("AdminJudge") Is Nothing Then
                    Return Settings("AdminJudge").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property AdminJudgeRole As String
            Get
                If Not Settings("AdminJudge") Is Nothing Then
                    Return Settings("AdminJudge").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim objJudge As JudgeInfo = ctl.GetJudge(UserId)
                If Not objJudge Is Nothing Then
                    If AdminJudge <> "" Then
                        Dim judgeId As Integer = Null.NullInteger
                        Try
                            Int32.TryParse(encrypt.QueryStringDecode(objJudge.Approved, AdminJudge), judgeId)
                        Catch
                        End Try
                        Return UserId = judgeId
                    End If
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property JudgeRoleName() As String
            Get
                If Not Settings("JudgeRole") Is Nothing Then
                    Return Settings("JudgeRole").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property JaRoleName() As String
            Get
                If Not Settings("JaRole") Is Nothing Then
                    Return Settings("JaRole").ToString.Trim
                End If
                Return ""
            End Get
        End Property
        Public ReadOnly Property HasSignedWarrantRole() As Boolean
            Get
                If Not Settings("SignedWarrantRole") Is Nothing Then
                    Dim SignedWarrantRole = Settings("SignedWarrantRole").ToString.Trim
                    If UserInfo.IsInRole(SignedWarrantRole) Then
                        Return True
                    End If
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property SergeantRoleName() As String
            Get
                If Not Settings("SergeantRole") Is Nothing Then
                    Return Settings("SergeantRole").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property IsSeargeant As Boolean
            Get
                If SergeantRoleName <> "" Then
                    Return UserInfo.IsInRole(SergeantRoleName)
                End If
                Return False
            End Get

        End Property

        Private ReadOnly Property IsSiteAdmin As Boolean
            Get
                Dim isAdmin As Boolean = UserInfo.IsSuperUser
                If Not isAdmin Then
                    Return UserInfo.IsInRole("Administrators")
                End If
                Return isAdmin
            End Get
        End Property

        Private ReadOnly Property JaPage As String
            Get
                If Not Settings("JaPage") Is Nothing Then
                    Return Settings("JaPage").ToString.Trim
                End If
                Return "0"
            End Get
        End Property

        Private ReadOnly Property ContactPage As String
            Get
                If Not Settings("ContactPage") Is Nothing Then
                    Return Settings("ContactPage").ToString.Trim
                End If
                Return ""
            End Get
        End Property

#End Region

#Region "Public Methods"
        Public Function GetUserInfo(createdByUserID As String) As String
            Dim returnValue As String = ""
            Dim objUser As UserInfo = UserController.GetUserById(PortalId, createdByUserID)
            If Not objUser Is Nothing Then
                Dim cell As String = objUser.Profile.Cell
                returnValue += IIf(cell = "", "No Cell", "" & cell)
                returnValue += "<br />"
                returnValue += objUser.Email
            End If
            Return returnValue
        End Function

#End Region

#Region "private methods"

        Private Sub BindData()
            Dim myStartDate As Date
            Dim myEndDate As Date
            If IsDate(txtStartDate.Text) Then
                myStartDate = DateTime.Parse(txtStartDate.Text)
            End If
            If IsDate(txtEndDate.Text) Then
                myEndDate = DateTime.Parse(txtEndDate.Text)
            End If
            Dim colWarrants As List(Of WarrantsInfo) = New List(Of WarrantsInfo)
            If IsJudge Then
                colWarrants = ctl.ListWarrants(ModuleId, myStartDate, myEndDate.AddDays(1))
                If cmdShowAssigned.CommandArgument = "true" Then
                    colWarrants = colWarrants.Where(Function(w) w.JudgeUserId = UserId).ToList
                End If
                If drpAgency.SelectedIndex > 0 Then
                    colWarrants = colWarrants.Where(Function(w) w.AgencyId = Int32.Parse(drpAgency.SelectedValue)).ToList
                End If

            ElseIf IsSeargeant Then
                colWarrants = ctl.ListWarrantsBySergeant(ModuleId, UserId, myStartDate, myEndDate.AddDays(1))
            Else
                colWarrants = ctl.ListWarrantsByUser(ModuleId, UserId, myStartDate, myEndDate.AddDays(1))
            End If
            If HasSignedWarrantRole Then
                colWarrants.AddRange(ctl.ListWarrantsSigned(ModuleId, myStartDate, myEndDate.AddDays(1)).Where(Function(i) Not colWarrants.Select(Function(c) c.WarrantId).Contains(i.WarrantId)))

            End If

            If txtTitle.Text <> "" Then
                colWarrants = colWarrants.Where(Function(w) w.Title.ToLower.Contains(txtTitle.Text.ToLower)).ToList
            End If
            If txtDefendant.Text <> "" Then
                colWarrants = colWarrants.Where(Function(w) w.Defendant.ToLower.Contains(txtDefendant.Text.ToLower)).ToList
            End If
            If txtID.Text <> "" Then
                colWarrants = colWarrants.Where(Function(w) w.WarrantId = Int32.Parse(txtID.Text)).ToList
            End If
            rptWarrants.DataSource = colWarrants.OrderByDescending(Function(w) w.WarrantId)
            rptWarrants.DataBind()
        End Sub

        Private Sub BindAgencyList()
            Dim ctl As New Controller
            drpAgency.DataTextField = "AgencyName"
            drpAgency.DataValueField = "AgencyId"
            drpAgency.DataSource = ctl.ListAgencies(ModuleId).OrderBy(Function(w) w.AgencyName).ToList
            drpAgency.DataBind()
            drpAgency.Items.Insert(0, New ListItem("< All >", "0"))
        End Sub

        Public Function FormatDate(inDate As DateTime) As String
            If inDate = Null.NullDate Then
                Return ""
            Else
                Return inDate.ToShortDateString & " " & inDate.ToShortTimeString
            End If
        End Function

        Private Function GetIp() As String
            Dim clientIp As String = Page.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If Not String.IsNullOrEmpty(clientIp) Then
                Dim forwardedIps As String() = clientIp.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                clientIp = forwardedIps(forwardedIps.Length - 1)
            Else
                clientIp = Page.Request.ServerVariables("REMOTE_ADDR")
            End If
            Return clientIp
        End Function

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
            If Not IsSiteAdmin And UserInfo.IsInRole(JaRoleName) Then
                Dim tabId As Integer = Null.NullInteger
                Int32.TryParse(JaPage, tabId)
                If tabId > 0 Then
                    Dim ctlTab As New TabController
                    Dim tab As DotNetNuke.Entities.Tabs.TabInfo = ctlTab.GetTab(tabId, PortalId, True)
                    Response.Redirect("/" & tab.TabPath.ToString.Trim("/"))
                Else
                    Response.Redirect("/OutofOfficeNotifications.aspx")
                End If
            End If
            objAgencyUser = ctl.GetUser(ModuleId, UserId)
            If Not objAgencyUser Is Nothing Then
                Dim objAgency As AgencyInfo = ctl.GetAgency(objAgencyUser.AgencyId)
                If Not objAgency Is Nothing Then
                    IsClerk = objAgency.IsClerk
                End If
                'If IsSeargeant Then
                '    Me.ModuleConfiguration.ModuleTitle = "Document List"
                'Else
                '    Me.ModuleConfiguration.ModuleTitle = "Document for " & UserInfo.DisplayName
                'End If
            End If
        End Sub

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
                    If DotNetNuke.Framework.AJAX.IsInstalled() Then
                        DotNetNuke.Framework.AJAX.RegisterScriptManager()
                    End If
                    lnkRefresh.NavigateUrl = NavigateURL()
                    toolbar.AdminJudge = AdminJudge
                    toolbar.IsAdminJudge = IsAdminJudge
                    toolbar.IsClerk = IsClerk
                    toolbar.IsJudge = IsJudge
                    toolbar.IsSiteAdmin = IsSiteAdmin
                    toolbar.JudgeRoleName = JudgeRoleName
                    toolbar.NavAddWarrant = EditUrl("addWarrant")
                    toolbar.NavAdminAgency = EditUrl("adminagency")
                    toolbar.NavAdminCounty = EditUrl("admincounty")
                    toolbar.NavAnnotations = EditUrl("annotations")
                    toolbar.NavJudgeSign = EditUrl("judgesign")
                    toolbar.NavManageUsers = EditUrl("users")
                    toolbar.NavOutofOffice = EditUrl("cover")
                    toolbar.NavStatus = EditUrl("wadmin")
                    toolbar.objAgencyUser = objAgencyUser
                    toolbar.ContactListUrl = ContactPage
                    toolbar.SergeantRoleName = SergeantRoleName
                    txtStartDate.Text = StartDate.ToShortDateString
                    txtEndDate.Text = EndDate.ToShortDateString
                    ltHeader.Text = "<h2>Warrants List</h2>"
                    If JudgeRoleName = "" Or AdminJudge = "" Or SergeantRoleName = "" Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The Application Settings have not been set.  Please contact the site administrator to configure settings.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        rptWarrants.Visible = False
                        dvFilters.Visible = False
                        Exit Sub
                    End If

                    Dim objSettings As WarrantConfigSettings = ctl.GetModuleSettings(ModuleId)
                    If Not objSettings Is Nothing Then
                        CompletedWarrantsThreshold = objSettings.Hours
                        If CompletedWarrantsThreshold > 0 And Not objAgencyUser Is Nothing Then
                            ltMessage.Text = String.Format(ltMessage.Text, objSettings.Hours.ToString)
                        Else
                            ltMessage.Visible = False
                        End If
                    End If
                    Dim _isJudge As Boolean = IsJudge
                    Dim _isSiteAdmin As Boolean = IsSiteAdmin
                    If Not Request.QueryString("msg") Is Nothing Then
                        UI.Skins.Skin.AddModuleMessage(Me, Request.QueryString("msg").ToString.Trim, UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    End If

                    If IsAdminJudge Or _isSiteAdmin Then
                        If _isSiteAdmin Then
                            rptWarrants.Visible = False
                            dvFilters.Visible = False
                        End If
                    Else
                        If objAgencyUser Is Nothing And Not _isJudge Then
                            rptWarrants.Visible = False
                            dvFilters.Visible = False
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You have not been assigned to a law enforcement agency. Please contact <a href=""mailto:" & PortalSettings.Email & """>support</a> to report this error.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                        Else
                            If Not objAgencyUser Is Nothing Then
                                If IsClerk Then
                                    ltHeader.Text = "<h2>Signed Warrants</h2>"
                                Else
                                    ltHeader.Text = "<h2>Warrants for " & UserInfo.DisplayName & "</h2>"
                                    If objAgencyUser.IsAdmin Then
                                        dvFilters.Visible = False
                                        rptWarrants.Visible = False
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If _isJudge Then
                        BindAgencyList()
                        cmdShowAssigned.Visible = True
                    ElseIf IsClerk Then
                        BindAgencyList()
                    Else
                        If HasSignedWarrantRole Then
                            BindAgencyList()
                        Else
                            drpAgency.Enabled = False
                            lblAgency.Enabled = False
                            If Not objAgencyUser Is Nothing Then
                                drpAgency.Items.Add(New ListItem(objAgencyUser.AgencyName))
                            End If
                        End If
                    End If
                    BindData()
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub UpdatePanel_Unload(ByVal sender As Object, ByVal e As EventArgs)
            Dim methodInfo As MethodInfo = GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance).Where(Function(i) i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First()
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {TryCast(sender, UpdatePanel)})
        End Sub

        Private Sub rptWarrants_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptWarrants.ItemCommand
            Dim WarrantId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "release" Then
                Dim objwarrant As WarrantsInfo = ctl.GetWarrants(WarrantId)
                If Not objwarrant Is Nothing Then
                    objwarrant.StatusId = WarrantStatus.NewWarrant
                    ctl.UpdateWarrants(objwarrant)
                    BindData()
                Else
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Error Retrieving Document Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If
            If e.CommandName.ToLower = "sign" Then
                Dim encryptor As New Encryptor
                Dim encryptedValue As String = encryptor.QueryStringEncode(WarrantId, UserInfo.Username)
                Dim url As String = ""
                url = TemplateSourceDirectory & "/judgeView.aspx?ModuleId=" & TabModuleId & "&enc=" & encryptedValue

                Dim objwarrant As WarrantsInfo = ctl.GetWarrants(WarrantId)
                If Not objwarrant Is Nothing Then
                    If objwarrant.StatusId = WarrantStatus.UnderReview And objwarrant.ReviewedByJudgeUserId <> UserId Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "This Document is currently under review by another judge.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        BindData()
                        Exit Sub
                    Else
                        objwarrant.ReviewedByJudgeUserId = UserId
                        objwarrant.StatusId = WarrantStatus.UnderReview
                        ctl.UpdateWarrants(objwarrant)
                    End If

                End If
                Response.Redirect(url, True)
            End If
            If e.CommandName.ToLower = "delete" Then
                Dim objLog As New LogInfo
                objLog.UserId = UserInfo.UserID
                objLog.EventTime = Now()
                objLog.EventDescription = "Deleted Record"
                objLog.WarrantId = WarrantId
                objLog.IP = GetIp()
                ctl.AddEvent(objLog)
                Try
                    ctl.DeleteWarrants(WarrantId)
                    BindData()
                Catch ex As Exception
                    Dim message = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("", ex.Message & "<br />" & ex.StackTrace, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    gridmessage.Controls.Add(message)
                End Try

            End If
        End Sub

        Private Sub rptWarrants_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptWarrants.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim lnkSign As LinkButton = CType(item.FindControl("lnkSign"), LinkButton)
                '  Dim lnkAttach As HyperLink = CType(item.FindControl("lnkAttach"), HyperLink)
                Dim cmdRelease As LinkButton = CType(item.FindControl("cmdRelease"), LinkButton)
                Dim cmdDelete As LinkButton = CType(item.FindControl("cmdDelete"), LinkButton)
                Dim rptAttachments As Repeater = CType(item.FindControl("rptAttachments"), Repeater)
                Dim objWarrant As WarrantsInfo = e.Item.DataItem
                cmdRelease.Visible = False
                ' lnkAttach.Visible = False
                Dim _isJudge = IsJudge

                If objWarrant.StatusId = WarrantStatus.NewWarrant And Not IsJudge And objWarrant.CreatedByUserId = UserId Then
                    cmdDelete.Visible = True
                Else
                    cmdDelete.Visible = False
                End If
                If objWarrant.StatusId = WarrantStatus.Reviewed And Not IsJudge And CompletedWarrantsThreshold > 0 Then
                    Dim tr As System.Web.UI.HtmlControls.HtmlTableRow = CType(e.Item.FindControl("tablerow"), System.Web.UI.HtmlControls.HtmlTableRow)
                    tr.Attributes.Add("style", "background-color:#ffeeba;")
                    lblDeleteNotice.Text = String.Format(lblDeleteNotice.Text, CompletedWarrantsThreshold.ToString)
                    lblDeleteNotice.Visible = True
                End If
                Dim warrantId As Integer = CType(e.Item.DataItem, WarrantsInfo).WarrantId
                Dim judgeId As Integer = objWarrant.JudgeUserId
                Dim statusId As WarrantStatus = objWarrant.StatusId
                'If statusId = WarrantStatus.Signed And Not IsJudge And objWarrant.CreatedByUserId = UserId Then
                '    lnkAttach.NavigateUrl = EditUrl("warrantid", warrantId.ToString, "cLog")
                '    lnkAttach.Visible = True
                'End If
                If statusId <> WarrantStatus.NewWarrant Then
                    lnkSign.Visible = False
                    If judgeId = UserId And statusId = WarrantStatus.UnderReview Then
                        lnkSign.Visible = True
                    End If
                End If
                If _isJudge Then
                    If statusId <> WarrantStatus.NewWarrant Then
                        lnkSign.Visible = False
                        ' lnkAttach.Visible = False
                        If judgeId = UserId And statusId = WarrantStatus.UnderReview Then
                            lnkSign.Visible = True
                            cmdRelease.Visible = True
                        End If
                    Else
                        lnkSign.Visible = True
                        cmdRelease.Visible = False
                    End If
                End If

                Dim lnkWarrant As HyperLink = CType(e.Item.FindControl("lnkWarrant"), HyperLink)
                If Not lnkWarrant Is Nothing Then
                    Dim encryptor As New Encryptor
                    Dim encryptedValue As String = encryptor.QueryStringEncode(objWarrant.FileId, UserInfo.Username)
                    lnkWarrant.NavigateUrl = TemplateSourceDirectory & "/Handlers/PDFViewer.ashx?enc=" & encryptedValue
                    If Not IsJudge Then
                        lnkWarrant.Attributes.Add("data-stat", objWarrant.Status.ToLower)
                    End If

                End If
            End If

        End Sub

        Protected Sub txtEndDate_TextChanged(sender As Object, e As EventArgs)
            EndDate = txtEndDate.Text
            BindData()
        End Sub

        Protected Sub txtStartDate_TextChanged(sender As Object, e As EventArgs)
            StartDate = txtStartDate.Text
            BindData()
        End Sub

        Private Sub rptWarrants_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptWarrants.ItemCreated
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdRelease As LinkButton = DirectCast(item.FindControl("cmdRelease"), LinkButton)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)

                sm.RegisterAsyncPostBackControl(cmdRelease)
                sm.RegisterAsyncPostBackControl(cmdDelete)

            End If

        End Sub

        Private Sub cmdShowAssigned_Click(sender As Object, e As EventArgs) Handles cmdShowAssigned.Click
            If cmdShowAssigned.CommandArgument = "true" Then
                cmdShowAssigned.Text = "<em class=""fa fa-square fa-lg""></em> Show only my warrants"
                cmdShowAssigned.CommandArgument = "false"
            Else
                cmdShowAssigned.Text = "<em class=""fa fa-check-square fa-lg""></em> Show only my warrants"
                cmdShowAssigned.CommandArgument = "true"
            End If

            BindData()
        End Sub
        Protected Sub txtDefendant_TextChanged(sender As Object, e As EventArgs)
            BindData()

        End Sub
        Protected Sub txtTitle_TextChanged(sender As Object, e As EventArgs)
            BindData()

        End Sub

        Protected Sub txtID_TextChanged(sender As Object, e As EventArgs)
            BindData()

        End Sub
        Protected Sub drpAgency_SelectedIndexChanged(sender As Object, e As EventArgs)
            BindData()

        End Sub

#End Region

    End Class

End Namespace
