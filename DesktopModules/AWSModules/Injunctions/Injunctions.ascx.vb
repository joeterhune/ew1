' 
' DotNetNuke® - http:'www.dotnetnuke.com
' Copyright (c) 2002-2011
' by DotNetNuke Corporation
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT InjunctionY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE InjunctionIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.

Imports DotNetNuke
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection
Imports Telerik.Web.UI

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
    Partial Class ViewInjunctions
        Inherits ViewStateBase

#Region "Private Members"
        Private objAgencyUser As AgencyUserInfo = Nothing
        Private ctl As New Controller
        Private encrypt As New Encryptor
        Private IsClerk As Boolean

        Private Property AgencySelected() As Integer
            Get
                If Not ViewState("AgencySelected") Is Nothing Then
                    Return CType(ViewState("AgencySelected"), Integer)
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("AgencySelected") = value
            End Set
        End Property

        Private Property AgencyId() As Integer
            Get
                If Not ViewState("AgencyId") Is Nothing Then
                    Return CType(ViewState("AgencyId"), Integer)
                Else
                    Return Null.NullInteger
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("AgencyId") = value
            End Set
        End Property

        Private Property TitleText() As String
            Get
                If Not ViewState("TitleText") Is Nothing Then
                    Return CType(ViewState("TitleText"), String)
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                ViewState("TitleText") = value
            End Set
        End Property

        Private Property DefendantText() As String
            Get
                If Not ViewState("DefendantText") Is Nothing Then
                    Return CType(ViewState("DefendantText"), String)
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                ViewState("DefendantText") = value
            End Set
        End Property

        Private Property CompletedInjunctionsThreshold() As Integer
            Get
                If Not ViewState("CompletedInjunctionsThreshold") Is Nothing Then
                    Return CType(ViewState("CompletedInjunctionsThreshold"), Integer)
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("CompletedInjunctionsThreshold") = value
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

        Private ReadOnly Property AdminJudgeRole As String
            Get
                If Not Settings("AdminJudge") Is Nothing Then
                    Return Settings("AdminJudge").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Public ReadOnly Property HasSignedInjunctionRole() As Boolean
            Get
                If Not Settings("SignedInjunctionRole") Is Nothing Then
                    Dim SignedInjunctionRole = Settings("SignedInjunctionRole").ToString.Trim
                    If UserInfo.IsInRole(SignedInjunctionRole) Then
                        Return True
                    End If
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim objJudge As JudgeInfo = ctl.GetJudge(UserId)
                If Not objJudge Is Nothing Then
                    If AdminJudgeRole <> "" Then
                        Dim judgeId As Integer = Null.NullInteger
                        Try
                            Int32.TryParse(encrypt.QueryStringDecode(objJudge.Approved, AdminJudgeRole), judgeId)
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

#End Region

#Region "Public Methods"
        Public Function GetUserInfo(createdByUserID As String) As String
            Dim returnValue As String = ""
            Dim objUser As UserInfo = UserController.GetUserById(PortalId, createdByUserID)
            If Not objUser Is Nothing Then
                Dim cell As String = objUser.Profile.Cell
                returnValue += IIf(cell = "", "<br />No Cell", "" & cell)
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
            Dim colInjunctions As List(Of InjunctionsInfo) = New List(Of InjunctionsInfo)

            If IsJudge Then
                colInjunctions = ctl.ListInjunctions(ModuleId, myStartDate, myEndDate.AddDays(1))
                If cmdShowAssigned.CommandArgument = "true" Then
                    colInjunctions = colInjunctions.Where(Function(w) w.JudgeUserId = UserId).ToList
                End If
                If drpAgency.SelectedIndex > 0 Then
                    colInjunctions = colInjunctions.Where(Function(w) w.AgencyId = Int32.Parse(drpAgency.SelectedValue)).ToList
                End If

            ElseIf IsSeargeant Then
                colInjunctions = ctl.ListInjunctionsBySergeant(ModuleId, UserId, myStartDate, myEndDate.AddDays(1))
            Else
                colInjunctions = ctl.ListInjunctionsByUser(ModuleId, UserId, myStartDate, myEndDate.AddDays(1))
            End If
            If HasSignedInjunctionRole Then
                colInjunctions.AddRange(ctl.ListInjunctionsSigned(ModuleId, myStartDate, myEndDate.AddDays(1)).Where(Function(i) Not colInjunctions.Select(Function(c) c.InjunctionId).Contains(i.InjunctionId)))
            End If

            If txtTitle.Text <> "" Then
                colInjunctions = colInjunctions.Where(Function(w) w.Title.ToLower.Contains(txtTitle.Text.ToLower)).ToList
            End If
            If txtDefendant.Text <> "" Then
                colInjunctions = colInjunctions.Where(Function(w) w.Defendant.ToLower.Contains(txtDefendant.Text.ToLower)).ToList
            End If
            If txtID.Text <> "" Then
                colInjunctions = colInjunctions.Where(Function(w) w.InjunctionId = Int32.Parse(txtID.Text)).ToList
            End If
            If drpAgency.Items.Count > 1 And drpAgency.SelectedValue <> "0" Then
                colInjunctions = colInjunctions.Where(Function(w) w.AgencyId = Int32.Parse(drpAgency.SelectedValue)).ToList
            End If
            rptInjunctions.DataSource = colInjunctions.OrderByDescending(Function(i) i.InjunctionId)
            rptInjunctions.DataBind()

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
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "JA Judge Notification Page has not been created. Please notify the site administrator", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If
            objAgencyUser = ctl.GetUser(ModuleId, UserId)
            If Not objAgencyUser Is Nothing Then
                Dim objAgency As AgencyInfo = ctl.GetAgency(objAgencyUser.AgencyId)
                If Not objAgency Is Nothing Then
                    IsClerk = objAgency.IsClerk
                End If
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
                    toolbar.AdminJudge = AdminJudgeRole
                    toolbar.IsAdminJudge = IsAdminJudge
                    toolbar.IsClerk = IsClerk
                    toolbar.IsJudge = IsJudge
                    toolbar.IsSiteAdmin = IsSiteAdmin
                    toolbar.JudgeRoleName = JudgeRoleName
                    toolbar.NavAddInjunction = EditUrl("addInjunction")
                    toolbar.NavAdminAgency = EditUrl("adminagency")
                    toolbar.NavAdminCounty = EditUrl("admincounty")
                    toolbar.NavAnnotations = EditUrl("annotations")
                    toolbar.NavJudgeSign = EditUrl("judgesign")
                    toolbar.NavManageUsers = EditUrl("users")
                    toolbar.NavOutofOffice = EditUrl("cover")
                    toolbar.NavStatus = EditUrl("iadmin")
                    toolbar.objAgencyUser = objAgencyUser
                    toolbar.ContactListUrl = ContactPage
                    toolbar.SergeantRoleName = SergeantRoleName

                    txtStartDate.Text = StartDate.ToShortDateString
                    txtEndDate.Text = EndDate.ToShortDateString
                    ltHeader.Text = "<h2>Injunction List</h2>"

                    If JudgeRoleName = "" Or AdminJudgeRole = "" Or SergeantRoleName = "" Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The Application Settings have not been set.  Please contact the site administrator to configure settings.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        rptInjunctions.Visible = False
                        dvFilters.Visible = False
                        Exit Sub
                    End If
                    Dim objSettings As InjunctionConfigSettings = ctl.GetModuleSettings(ModuleId)
                    If Not objSettings Is Nothing Then
                        CompletedInjunctionsThreshold = objSettings.Hours
                        If CompletedInjunctionsThreshold > 0 Then
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
                            rptInjunctions.Visible = False
                            dvFilters.Visible = False

                        End If
                    Else

                        If objAgencyUser Is Nothing And Not _isJudge Then
                            rptInjunctions.Visible = False
                            dvFilters.Visible = False
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You have not been assigned to an agency. Please contact <a href=""mailto:" & PortalSettings.Email & """>support</a> to report this error.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                        Else
                            If Not objAgencyUser Is Nothing Then
                                If IsClerk Then
                                    ltHeader.Text = "<h2>Signed Injunctions</h2>"
                                Else
                                    ltHeader.Text = "<h2>Injunctions for " & UserInfo.DisplayName & "</h2>"
                                    If objAgencyUser.IsAdmin Then
                                        rptInjunctions.Visible = False
                                        dvFilters.Visible = False

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
                        If HasSignedInjunctionRole Then
                            BindAgencyList()
                        Else
                            drpAgency.Enabled = False
                            lblAgency.Enabled = False
                            If Not objAgencyUser Is Nothing Then
                                drpAgency.DataValueField = "AgencyId"
                                drpAgency.DataTextField = "AgencyName"
                                drpAgency.DataSource = ctl.ListUserAgencies(ModuleId, UserId)
                                drpAgency.DataBind()
                                drpAgency.SelectedValue = objAgencyUser.AgencyId
                                If drpAgency.Items.Count > 1 Then
                                    drpAgency.Enabled = True
                                    drpAgency.Items.Insert(0, New ListItem("All", "0"))
                                    drpAgency.SelectedValue = "0"
                                End If
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

        Private Sub rptInjunctions_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptInjunctions.ItemCommand
            Dim InjunctionId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "release" Then
                Dim objInjunction As InjunctionsInfo = ctl.GetInjunctions(InjunctionId)
                If Not objInjunction Is Nothing Then
                    objInjunction.StatusId = InjunctionStatus.NewInjunction
                    ctl.UpdateInjunctions(objInjunction)
                    BindData()
                Else
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Error Retrieving Document Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If
            If e.CommandName.ToLower = "sign" Then
                Dim encryptor As New Encryptor
                Dim encryptedValue As String = encryptor.QueryStringEncode(InjunctionId, UserInfo.Username)
                Dim url As String = ""
                url = TemplateSourceDirectory & "/judgeView.aspx?ModuleId=" & TabModuleId & "&enc=" & encryptedValue

                Dim objInjunction As InjunctionsInfo = ctl.GetInjunctions(InjunctionId)
                If Not objInjunction Is Nothing Then
                    If objInjunction.StatusId = InjunctionStatus.UnderReview And objInjunction.ReviewedByJudgeUserId <> UserId Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "This Document is currently under review by another judge.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        BindData()
                        Exit Sub
                    Else
                        objInjunction.ReviewedByJudgeUserId = UserId
                        objInjunction.StatusId = InjunctionStatus.UnderReview
                        ctl.UpdateInjunctions(objInjunction)
                    End If

                End If
                Response.Redirect(url, True)
            End If
            If e.CommandName.ToLower = "delete" Then
                Dim objLog As New LogInfo
                objLog.UserId = UserInfo.UserID
                objLog.EventTime = Now()
                objLog.EventDescription = "Deleted Record"
                objLog.InjunctionId = InjunctionId
                objLog.IP = GetIp()
                ctl.AddEvent(objLog)
                Try
                    ctl.DeleteInjunctions(InjunctionId)
                    BindData()
                Catch ex As Exception
                    Dim message = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("", ex.Message & "<br />" & ex.StackTrace, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    gridmessage.Controls.Add(message)
                End Try

            End If
        End Sub

        Protected Sub rptInjunctions_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptInjunctions.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim lnkSign As LinkButton = CType(item.FindControl("lnkSign"), LinkButton)
                Dim cmdRelease As LinkButton = CType(item.FindControl("cmdRelease"), LinkButton)
                Dim cmdDelete As LinkButton = CType(item.FindControl("cmdDelete"), LinkButton)
                Dim objInjunction As InjunctionsInfo = e.Item.DataItem
                lnkSign.ToolTip = "Sign Injunction"
                cmdRelease.Visible = False
                If objInjunction.StatusId = InjunctionStatus.NewInjunction And Not IsJudge Then
                    cmdDelete.Visible = True
                Else
                    cmdDelete.Visible = False
                End If
                If objInjunction.StatusId = InjunctionStatus.Reviewed And Not IsJudge And CompletedInjunctionsThreshold > 0 Then
                    Dim tr As System.Web.UI.HtmlControls.HtmlTableRow = CType(e.Item.FindControl("tablerow"), System.Web.UI.HtmlControls.HtmlTableRow)
                    tr.Attributes.Add("style", "background-color:#ffeeba;")
                    lblDeleteNotice.Text = String.Format("<div class='dnnFormMessage dnnFormError dnnFormInfo' style='margin:20px 0;'>The injunctions highlighted below have been completed and will be automatically deleted {0} hours after viewing.</div>", CompletedInjunctionsThreshold.ToString)
                End If
                Dim InjunctionId As Integer = CType(e.Item.DataItem, InjunctionsInfo).InjunctionId
                Dim judgeId As Integer = objInjunction.JudgeUserId
                Dim statusId As InjunctionStatus = objInjunction.StatusId

                If statusId <> InjunctionStatus.NewInjunction Then
                    lnkSign.Visible = False
                    If judgeId = UserId And statusId = InjunctionStatus.UnderReview Then
                        lnkSign.Visible = True
                    End If
                Else
                    If IsJudge Then
                        lnkSign.Visible = True
                        cmdRelease.Visible = False
                    End If
                End If

                Dim lnkInjunction As HyperLink = CType(e.Item.FindControl("lnkInjunction"), HyperLink)
                If Not lnkInjunction Is Nothing Then
                    Dim encryptor As New Encryptor
                    Dim encryptedValue As String = encryptor.QueryStringEncode(objInjunction.FileId, UserInfo.Username)
                    lnkInjunction.NavigateUrl = TemplateSourceDirectory & "/Handlers/PDFViewer.ashx?enc=" & encryptedValue
                    If Not IsJudge Then
                        lnkInjunction.Attributes.Add("data-stat", objInjunction.Status.ToLower)
                    End If
                End If

            End If
        End Sub

        Private Sub rptInjunctions_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptInjunctions.ItemCreated
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

        Protected Sub txtEndDate_TextChanged(sender As Object, e As EventArgs)
            EndDate = txtEndDate.Text
            BindData()
        End Sub

        Protected Sub txtStartDate_TextChanged(sender As Object, e As EventArgs)
            StartDate = txtStartDate.Text
            BindData()
        End Sub

#End Region



    End Class

End Namespace
