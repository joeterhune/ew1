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
Imports System.Reflection

Namespace AWS.Modules.Injunctions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDynamicModule class is used to manage content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditCoverSchedule
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private ctl As New Controller
        Private nCtl As New Modules.Notifications.Controller

        Public ReadOnly Property JudgeRole() As String
            Get
                If Not Settings("JudgeRole") Is Nothing Then
                    Return Settings("JudgeRole")
                Else
                    Return ""
                End If
            End Get
        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim objJudge As JudgeInfo = ctl.GetJudge(UserId)
                If Not objJudge Is Nothing Then
                    Return True
                End If
                Return False
            End Get
        End Property

#End Region

#Region "Methods"
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

        Private Sub BindJudges()
            Dim rolectl As New DotNetNuke.Security.Roles.RoleController
            Dim judges = rolectl.GetUsersByRole(PortalId, JudgeRole)
            For Each u As UserInfo In judges
                drpJudge.Items.Add(New ListItem(u.DisplayName.Replace("&nbsp;", " "), u.UserID))
            Next
            drpJudge.Items.Insert(0, New ListItem("< Select Judge >", ""))
        End Sub

        Public Function FormatDateTime(inDate As DateTime) As String
            If inDate = Null.NullDate Then
                Return ""
            Else
                Return inDate.ToShortDateString & " " & inDate.ToShortTimeString
            End If
        End Function

        Private Sub BindData()
            Dim colNotifications As List(Of AWS.Modules.Notifications.NotificationInfo) = nCtl.ListMyNotifications(UserId)
            rptNotifications.DataSource = colNotifications
            rptNotifications.DataBind()

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
                    If Not IsJudge Then
                        Response.Redirect(NavigateURL())
                    End If
                    BindJudges()
                    BindData()

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try

                Response.Redirect(NavigateURL, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdReset_Click(sender As Object, e As EventArgs) Handles cmdReset.Click
            Try

                Response.Redirect(EditUrl("cover"), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                Dim objNotification As New AWS.Modules.Notifications.NotificationInfo
                With objNotification
                    If drpJudge.SelectedIndex > 0 Then
                        .CoveringJudgeId = CType(drpJudge.SelectedValue, Integer)
                    Else
                        .CoveringJudgeId = Null.NullInteger
                    End If
                    Dim startDate As String = txtStartDate.Text + " " + txtStartTime.Text
                    Dim endDate As String = txtEndDate.Text + " " + txtEndTime.Text
                    If Not IsDate(startDate) Or Not IsDate(endDate) Then
                        ltMessage.Text = "<p class='alert alert-danger'><em class='fa fa-warning'></em>&nbsp;You must enter a valid Start and End Date.  Please check your entries and try again.</p>"
                        Exit Sub
                    Else
                        Dim sDate As DateTime = DateTime.Parse(startDate)
                        Dim eDate As DateTime = DateTime.Parse(endDate)
                        If sDate >= eDate Then
                            ltMessage.Text = "<p class='alert alert-danger'><em class='fa fa-warning'></em>&nbsp;The Start date and time must not be equal to or less than the End date and time.</p>"
                            Exit Sub
                        End If
                        .StartDateTime = sDate
                        .EndDateTime = eDate
                    End If
                    If drpJudge.SelectedIndex <= 0 And txtMessage.Text.Trim() = "" Then
                        ltMessage.Text = "<p class='alert alert-danger'><em class='fa fa-warning'></em>&nbsp;You must select a Judge to cover you or enter a notification message</p>"
                        Exit Sub
                    End If

                    .MessageText = txtMessage.Text
                    .RequestingJudgeId = UserId
                End With
                Dim colNotifications As List(Of AWS.Modules.Notifications.NotificationInfo) = nCtl.ListConflictingNotifications(UserId, objNotification.StartDateTime, objNotification.EndDateTime)
                If colNotifications.Count > 0 Then
                    Dim strMessage As New StringBuilder
                    strMessage.Append("<div class='alert alert-danger'><em class='fa fa-warning'></em><strong>The selected dates conflict with other Notifications scheduled for the following dates:<strong><ul>")
                    For Each n As AWS.Modules.Notifications.NotificationInfo In colNotifications
                        strMessage.Append("<li>")
                        strMessage.Append(n.StartDateTime.ToShortDateString)
                        strMessage.Append(" - ")
                        strMessage.Append(n.EndDateTime.ToShortDateString)
                        strMessage.Append("</li>")
                    Next
                    strMessage.Append("</ul></div>")
                    ltMessage.Text = strMessage.ToString
                    Exit Sub
                End If
                nCtl.AddNotification(objNotification)
                ' refresh cache
                Entities.Modules.ModuleController.SynchronizeModule(ModuleId)
                ' Redirect back to the portal home page
                Response.Redirect(EditUrl("cover"), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub rptNotifications_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptNotifications.ItemCommand
            Dim ScheduleId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "delete" Then
                nCtl.DeleteNotification(ScheduleId)
                BindData()
            End If
        End Sub

        Private Sub rptNotifications_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptNotifications.ItemCreated
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
            End If

        End Sub

        Protected Sub UpdatePanel_Unload(ByVal sender As Object, ByVal e As EventArgs)
            Dim methodInfo As MethodInfo = GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance).Where(Function(i) i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First()
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {TryCast(sender, UpdatePanel)})
        End Sub


#End Region

    End Class

End Namespace
