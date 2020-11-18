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
    Partial Class AgencyUsers
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private objAgency As AgencyInfo = Nothing
        Private ctl As New Controller
        Private objAgencyUser As AgencyUserInfo = Nothing
        Public ModuleTitle As String

        Public Property AgencyId() As Integer
            Get
                If Not ViewState("AgencyId") Is Nothing Then
                    Return Int32.Parse(ViewState("AgencyId"))
                Else
                    Return Null.NullInteger
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("AgencyId") = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Sub BindData()
            rptUsers.DataSource = ctl.ListAgencyUsers(AgencyId)
            rptUsers.DataBind()
        End Sub
#End Region

#Region "Event Handlers"
        Private Sub AgencyUsers_Init(sender As Object, e As EventArgs) Handles Me.Init
            objAgencyUser = ctl.GetUser(ModuleId, UserId)
            If Not objAgencyUser Is Nothing Then
                AgencyId = objAgencyUser.AgencyId
                objAgency = ctl.GetAgency(AgencyId)
                ModuleTitle = "Users for <strong class=""text-primary"">" & objAgency.AgencyName & "</strong>"
            End If

        End Sub


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then

                    lnkCancel.NavigateUrl = NavigateURL()
                    If Not objAgencyUser Is Nothing Then
                        If objAgencyUser.IsAdmin Then
                            lnkAddUser.NavigateUrl = EditUrl("agencyId", AgencyId.ToString, "adduser")
                            BindData()

                            If objAgency Is Nothing Then
                                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Unable to determine assigned agency.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            End If
                        Else
                            'not admin
                            Response.Redirect(NavigateURL)
                        End If
                    Else
                        'notuser
                        Response.Redirect(NavigateURL)
                    End If


                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub UpdatePanel_Unload(ByVal sender As Object, ByVal e As EventArgs)
            Dim methodInfo As MethodInfo = GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance).Where(Function(i) i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First()
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {TryCast(sender, UpdatePanel)})
        End Sub

        Private Sub rptUsers_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptUsers.ItemCommand

            Dim _userId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "setadmin" Then
                Dim userid As Integer = e.CommandArgument
                Dim objAgencyUser As AgencyUserInfo = ctl.GetAgencyUser(AgencyId, userid)
                If objAgencyUser.IsAdmin Then
                    objAgencyUser.IsAdmin = False
                Else
                    Dim agencyUsers As List(Of AgencyUserInfo) = ctl.ListAgencyUsers(AgencyId)
                    Dim admins As Integer = agencyUsers.Where(Function(au) au.IsAdmin = True).Count
                    If admins >= 3 Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You cannot assign more than three admins.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        Exit Sub
                    Else
                        objAgencyUser.IsAdmin = True
                    End If
                End If
                ctl.UpdateAgencyUser(objAgencyUser)
                BindData()
            End If
            If e.CommandName = "reset" Then
                Dim objUser As UserInfo = UserController.GetUserById(PortalId, _userId)
                If Not objUser Is Nothing Then
                    DotNetNuke.Entities.Users.UserController.ResetPasswordToken(objUser)

                    DotNetNuke.Services.Mail.Mail.SendMail(objUser, Services.Mail.MessageType.PasswordReminder, PortalSettings)
                    Dim password As String = UserController.ResetPassword(objUser, "")
                    UI.Skins.Skin.AddModuleMessage(Me, "A password reset link was sent to " & objUser.DisplayName, UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, "Unable to retrieve user information", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If
            If e.CommandName = "email" Then
                Dim url As String = EditUrl("AgencyId", AgencyId.ToString, "EditEmail", "userid=" & _userId)
                Response.Redirect(url, True)
            End If

            If e.CommandName.ToLower = "delete" Then
                If _userId = UserId Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You cannot delete yourself.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    Exit Sub
                End If
                Dim objUser As UserInfo = UserController.GetUserById(PortalId, _userId)
                ctl.DeleteAgencyUser(AgencyId, _userId)
                UserController.DeleteUser(objUser, False, True)
            End If

        End Sub

        Private Sub rptUsers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUsers.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim objUser As AgencyUserInfo = e.Item.DataItem
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                Dim cmdPasswordReset As LinkButton = DirectCast(item.FindControl("cmdPasswordReset"), LinkButton)
                Dim cmdSetAdmin As LinkButton = DirectCast(item.FindControl("cmdSetAdmin"), LinkButton)
                If objUser.IsAdmin Then
                    cmdSetAdmin.Text = "<em class='fa fa-check-square'>&nbsp;</em>"
                    If objUser.UserId = UserId Then
                        cmdSetAdmin.Enabled = False
                        cmdSetAdmin.Text = "<em class='text-danger fa fa-check-square'>&nbsp;</em>"
                    End If
                Else
                    cmdSetAdmin.Text = "<em class='fa fa-square'>&nbsp;</em>"
                End If

                If objUser.UserId = UserId Then
                    cmdPasswordReset.Visible = False
                    cmdDelete.Visible = False
                End If
            End If
        End Sub

        Private Sub rptUsers_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptUsers.ItemCreated

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                Dim cmdSetAdmin As LinkButton = DirectCast(item.FindControl("cmdSetAdmin"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
                sm.RegisterAsyncPostBackControl(cmdSetAdmin)

            End If

        End Sub


#End Region


    End Class

End Namespace
