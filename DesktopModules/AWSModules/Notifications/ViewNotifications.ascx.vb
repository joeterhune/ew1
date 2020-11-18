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
Imports System.Reflection
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
    Partial Class ViewNotifications
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

        Public ReadOnly Property AssistantRole() As String
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
        Public Function GetJudge(requestingJudgeId As Integer) As String

            Dim objJudge As UserInfo = UserController.GetUserById(PortalId, requestingJudgeId)
            Return objJudge.DisplayName
        End Function

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

        Public Function FormatDateTime(inDate As DateTime) As String
            If inDate = Null.NullDate Then
                Return ""
            Else
                Return inDate.ToShortDateString & " " & inDate.ToShortTimeString
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        Private Sub BindData()

            Dim colNotifications As List(Of NotificationInfo) = ctl.ListCurrentNotifications
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
                    If AssistantRole <> "" Then
                        lnkAddNotification.NavigateUrl = EditUrl()

                        If Not UserInfo.IsInRole(AssistantRole) Then
                            Response.Redirect("/")
                        End If
                        BindData()
                    Else
                        dptUpdate.Visible = False
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Please Configure this Module", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub rptNotifications_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptNotifications.ItemCommand
            Dim ScheduleId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "delete" Then
                ctl.DeleteNotification(ScheduleId)
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


