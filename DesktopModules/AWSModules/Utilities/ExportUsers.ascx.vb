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
                End With
                ctl.AddExportUser(objexportUser)
            Next
        End Sub


#End Region
    End Class

End Namespace
