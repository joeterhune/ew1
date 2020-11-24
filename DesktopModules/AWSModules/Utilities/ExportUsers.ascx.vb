Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Security.Roles
Imports System.Data.OleDb
Imports Telerik.Web.UI

Namespace AWS.Modules.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ExportUsers
        Inherits Entities.Modules.PortalModuleBase

#Region " Private Methods "

#End Region

#Region " Methods "
        Public Function ExistingUser(username As String) As Boolean
            Dim user = UserController.GetUserByName(PortalId, username)
            If user Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function


#End Region

#Region "Event Handlers"


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not IsPostBack Then

                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
            Dim ctl As New Controller
            ctl.DeleteUsers()
            Dim listUsers = UserController.GetUsers(PortalId)
            For Each u As UserInfo In listUsers
                Dim objexportUser As New ExportUser
                With objexportUser
                    .DisplayName = u.DisplayName
                    .Email = u.Email
                    .FirstName = u.FirstName
                    .LastName = u.LastName
                    .Password = UserController.GetPassword(u, "")
                    .UserId = u.UserID
                    .Username = u.Username
                    .Telephone=u.Profile.Telephone
                    .Cell=u.Profile.Cell
                End With
                ctl.AddExportUser(objexportUser)
            Next
        End Sub
        Private Sub cmdImport_Click(sender As Object, e As EventArgs) Handles cmdImport.Click
            Dim ctl As New Controller

            Dim users As List(Of ExportUser) = ctl.ListImportUsers.Where(Function(ex) ex.UserId <> 19).ToList
            Dim message As String = ""
            For Each u As ExportUser In users
                Dim objuser As UserInfo = UserController.GetUserByName(u.Username)

                If objuser Is Nothing Then
                    Dim newUser As New UserInfo
                    With newUser
                        .Username = u.Username
                        .FirstName = u.FirstName
                        .LastName = u.LastName
                        .DisplayName = u.DisplayName
                        .Email = u.Email
                        .Profile.Cell = u.Cell
                        .Profile.Telephone = u.Telephone
                        .PortalID = PortalId
                        .Membership.Password = u.Password
                        .Membership.Approved = True
                    End With
                    Dim status = UserController.CreateUser(newUser)
                    If status <> DotNetNuke.Security.Membership.UserCreateStatus.Success Then
                        message += " " + u.Username + "<br>"
                    Else
                        newUser = UserController.GetUserByName(u.Username)
                        newUser.Profile.Cell = u.Cell
                        newUser.Profile.Telephone = u.Telephone
                        newUser.Profile.FirstName = u.FirstName
                        newUser.Profile.LastName = u.LastName
                        UserController.UpdateUser(PortalId, newUser)
                    End If
                Else
                    UserController.ResetAndChangePassword(objuser, u.Password)
                    objuser.Profile.Cell = u.Cell
                    objuser.Profile.Telephone = u.Telephone
                    objuser.Profile.FirstName = u.FirstName
                    objuser.Profile.LastName = u.LastName
                    objuser.Email = u.Email
                    UserController.UpdateUser(PortalId, objuser)
                End If
            Next
            If message <> "" Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub


#End Region
    End Class

End Namespace
