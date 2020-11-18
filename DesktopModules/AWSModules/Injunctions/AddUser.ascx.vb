' 
' DotNetNuke� - http:'www.dotnetnuke.com
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
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Mail

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
    Partial Class AddUser
        Inherits Entities.Modules.UserModuleBase

#Region "Members"
        Private JudgeRole As String = "judge"
        Private agencyId As Integer = Null.NullInteger
#End Region

#Region "Methods"
        Private Sub ClearForm()
            txtEmail.Text = ""
            txtFirstName.Text = ""
            txtLastName.Text = ""
            txtUserName.Text = ""
            chkAdmin.Checked = False
            btnAddExisting.Visible = False
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
                If Not Request.QueryString("AgencyId") Is Nothing Then
                    agencyId = Int32.Parse(Request.QueryString("AgencyId"))
                End If

                If Page.IsPostBack = False Then
                    Dim IsAgencyAdmin As Boolean = False
                    Dim ctl As New Controller
                    Dim agencyUsers As List(Of AgencyUserInfo) = ctl.ListAgencyUsers(agencyId)
                    Dim agencyAdmins As List(Of AgencyUserInfo) = agencyUsers.Where(Function(au) au.IsAdmin = True).ToList
                    For Each admin As AgencyUserInfo In agencyAdmins
                        If admin.UserId = UserInfo.UserID Then
                            IsAgencyAdmin = True
                        End If
                    Next
                    If IsAgencyAdmin Then
                        lnkCancel.NavigateUrl = EditUrl("users")
                    ElseIf IsAdmin Then
                        lnkCancel.NavigateUrl = EditUrl("agencyId", agencyId.ToString, "adminusers")
                    End If

                    If IsAgencyAdmin Or IsAdmin Then
                        Dim admins As Integer = agencyAdmins.Count
                        If admins >= 3 Then
                            chkAdmin.Visible = False
                        End If
                    Else
                        'Not an admin
                        Response.Redirect(NavigateURL)
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


        Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            Try

                If Page.IsValid Then

                    'Update DisplayName to conform to Format
                    Dim objUser As UserInfo = UserController.GetUserByName(Me.txtUserName.Text)
                    If objUser Is Nothing Then
                        objUser = New UserInfo
                    Else
                        Dim message As String = "The username " & objUser.Username & " is already in use by " & objUser.DisplayName
                        message += " If you wish to add the existing user, click the Add Existing User button, otherwise chose a different username and press Save to create a new user"
                        Skins.Skin.AddModuleMessage(Me, message, Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        btnAddExisting.Visible = True
                        Exit Sub
                    End If
                    objUser.LastIPAddress = ""
                    objUser.UserID = Null.NullInteger
                    objUser.Username = Me.txtUserName.Text
                    objUser.FirstName = Me.txtFirstName.Text
                    objUser.LastName = Me.txtLastName.Text
                    objUser.Email = Me.txtEmail.Text
                    objUser.DisplayName = Me.txtFirstName.Text & " " & Me.txtLastName.Text
                    objUser.Membership.Password = UserController.GeneratePassword(8)
                    objUser.Membership.UpdatePassword = True
                    objUser.PortalID = PortalId
                    objUser.Membership.Approved = True

                    Dim createStatus As UserCreateStatus = UserController.CreateUser(objUser)

                    If createStatus = UserCreateStatus.Success Then
                        If chkNotify.Checked Then
                            Mail.SendMail(objUser, MessageType.UserRegistrationPublic, PortalSettings)
                        End If
                        Dim ctl As New Controller

                        objUser = UserController.GetUserByName(PortalSettings.PortalId, objUser.Username)
                        Dim body As String = Localization.GetString("", Localization.SharedResourceFile)
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The User was added Successfully.", Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                        Dim objAgencyUser As New AgencyUserInfo
                        objAgencyUser.AgencyId = agencyId
                        objAgencyUser.UserId = objUser.UserID
                        objAgencyUser.IsAdmin = chkAdmin.Checked

                        If IsAdmin Then
                            Dim agencyAdmins As List(Of AgencyUserInfo) = ctl.ListAgencyUsers(agencyId).Where(Function(au) au.IsAdmin = True).ToList
                            For Each admin As AgencyUserInfo In agencyAdmins
                                Dim obju As UserInfo = UserController.GetUserById(PortalId, admin.UserId)
                                Mail.SendEmail(PortalSettings.Email, obju.Email, "User added by site administrator", "The administrator at " & PortalSettings.PortalAlias.HTTPAlias & " has added " & objUser.DisplayName & " as a new user to your agency.")
                            Next
                        End If
                        ctl.AddAgencyUser(objAgencyUser)
                        ClearForm()
                    Else       ' registration error

                        Dim message As String = ""
                        Select Case createStatus
                            Case UserCreateStatus.DuplicateUserName
                                message = "The Username is already in Use.  Please choose a different Username."
                            Case UserCreateStatus.InvalidPassword
                                message = "Invalid Password.  The Password does not meet the minium password strength requirements. The password must be a minimum of 7 characters, must include at least one lower case letter and one upper case letter, and at least one number."
                            Case UserCreateStatus.InvalidEmail
                                message = "The Email address is not in the correct format.  User was not created."
                            Case UserCreateStatus.InvalidUserName
                                message = "The username must be a minimum of four characters. User was not created."
                            Case UserCreateStatus.PasswordMismatch
                                message = "The Password and the Confirmation Password fields do not match.  Please re-enter the password."
                            Case UserCreateStatus.UserAlreadyRegistered
                                message = "The Username is already in Use.  Please choose a different Username."
                            Case UserCreateStatus.UsernameAlreadyExists
                                message = "The Username is already in Use.  Please choose a different Username."
                        End Select
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, message, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If

                Else
                    Dim message As String = "Please enter a first and last name and a valid e-mail address."
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, message, Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub btnAddExisting_Click(sender As Object, e As EventArgs) Handles btnAddExisting.Click
            Try
                Dim ctl As New Controller
                Dim agencyInfo As AgencyInfo = ctl.GetAgency(agencyId)

                Dim objUser As UserInfo = UserController.GetUserByName(PortalId, Me.txtUserName.Text)
                If objUser Is Nothing Then
                    objUser = UserController.GetUserByName(Me.txtUserName.Text)
                    If Not objUser Is Nothing Then
                        UserController.AddUserPortal(PortalId, objUser.UserID)
                        UserController.ApproveUser(objUser)
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Unable to Add Existing User.  Please use a different username", Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                        Exit Sub
                    End If
                End If

                Dim objAgencyUser As New AgencyUserInfo
                objAgencyUser.AgencyId = agencyId
                objAgencyUser.UserId = objUser.UserID
                objAgencyUser.IsAdmin = chkAdmin.Checked

                If IsAdmin Then
                    Dim agencyAdmins As List(Of AgencyUserInfo) = ctl.ListAgencyUsers(agencyId).Where(Function(au) au.IsAdmin = True).ToList
                    For Each admin As AgencyUserInfo In agencyAdmins
                        Dim obju As UserInfo = UserController.GetUserById(PortalId, admin.UserId)
                        Mail.SendEmail(PortalSettings.Email, obju.Email, "User added by site administrator", "The administrator at " & PortalSettings.PortalAlias.HTTPAlias  & " has added " & objUser.DisplayName & " as a new user to your agency.")
                    Next
                End If

                ctl.AddAgencyUser(objAgencyUser)
                If agencyInfo.IsClerk Then
                    If Not Settings("SignedInjunctionRole") Is Nothing Then
                        Dim SignedWarrantRole = Settings("SignedInjunctionRole").ToString.Trim
                        Dim ctlRole As Security.Roles.RoleController = New Security.Roles.RoleController()
                        Dim role = ctlRole.GetRoleByName(PortalId, SignedWarrantRole)
                        ctlRole.AddUserRole(PortalId, objUser.UserID, role.RoleID, Now, Null.NullDate)
                    End If
                End If
                ClearForm()
                If chkNotify.Checked Then
                    Dim body As String = "You can now login to " & PortalSettings.PortalAlias.HTTPAlias  & " and submit injunctions"
                    Mail.SendEmail(UserInfo.Email, objUser.Email, "Welcome to the " & agencyInfo.AgencyName & " edutyjudge group", body)
                End If
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The User was added Successfully.", Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub


#End Region

    End Class

End Namespace

