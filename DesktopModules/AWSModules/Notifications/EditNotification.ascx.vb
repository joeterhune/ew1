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
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.


Imports DotNetNuke
Imports System.IO
Imports Telerik.Web.UI

Namespace AWS.Modules.Notifications

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDynamicModule class is used to manage content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditNotification
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private ctl As New Controller
        Public ReadOnly Property JudgeRole() As String
            Get
                If Not Settings("JudgeRole") Is Nothing Then
                    Return Settings("JudgeRole")
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property JARole() As String
            Get
                If Not Settings("JARole") Is Nothing Then
                    Return Settings("JARole")
                Else
                    Return ""
                End If
            End Get
        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim nCtl As New AWS.Modules.Warrants.Controller
                Dim objJudge As AWS.Modules.Warrants.JudgeInfo = nCtl.GetJudge(UserId)
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
            Dim judges = Security.Roles.RoleController.Instance.GetUsersByRole(PortalId, JudgeRole)
            For Each u As UserInfo In judges
                drpOOOJudge.Items.Add(New ListItem(u.DisplayName.Replace("&nbsp;", " "), u.UserID))
                drpCoverJudge.Items.Add(New ListItem(u.DisplayName.Replace("&nbsp;", " "), u.UserID))
            Next

            drpCoverJudge.Items.Insert(0, New ListItem("< Select Cover Judge >", ""))
            drpOOOJudge.Items.Insert(0, New ListItem("< Select Judge >", ""))
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
                    If JARole = "" Or Not UserInfo.IsInRole(JARole) Then
                        Response.Redirect("/")
                    End If
                    BindJudges()
                    lnkCancel.NavigateUrl = NavigateURL()
                End If

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
                If drpCoverJudge.SelectedIndex <= 0 And txtMessage.Text.Trim() = "" Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You must select a Judge to cover you or enter a notification message", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    Exit Sub
                End If
                Dim requestingJudge As Integer = Null.NullInteger
                Int32.TryParse(drpOOOJudge.SelectedValue, requestingJudge)

                Dim objNotification As New NotificationInfo
                With objNotification
                    If drpCoverJudge.SelectedIndex > 0 Then
                        .CoveringJudgeId = CType(drpCoverJudge.SelectedValue, Integer)
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

                    .MessageText = txtMessage.Text
                    .RequestingJudgeId = requestingJudge
                End With
                Dim colNotifications As List(Of NotificationInfo) = ctl.ListConflictingNotifications(requestingJudge, objNotification.StartDateTime, objNotification.EndDateTime)
                If colNotifications.Count > 0 Then
                    Dim strMessage As New StringBuilder
                    strMessage.Append("<strong>The selected dates conflict with other Notifications scheduled for the following dates:<strong><ul>")
                    For Each n As NotificationInfo In colNotifications
                        strMessage.Append("<li>")
                        strMessage.Append(UserController.GetUserById(PortalId, n.RequestingJudgeId).DisplayName)
                        strMessage.Append(":&nbsp;")
                        strMessage.Append(n.StartDateTime.ToShortDateString)
                        strMessage.Append(" - ")
                        strMessage.Append(n.EndDateTime.ToShortDateString)
                        strMessage.Append("</li>")
                    Next
                    strMessage.Append("</ul>")

                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, strMessage.ToString, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    Exit Sub
                End If
                ctl.AddNotification(objNotification)
                ' refresh cache
                Entities.Modules.ModuleController.SynchronizeModule(ModuleId)
                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
