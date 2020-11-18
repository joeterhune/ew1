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
    Partial Class AutoAddUser
        Inherits Entities.Modules.PortalModuleBase

#Region " Private Methods "

#End Region

#Region " Methods "


        Public Sub createDnnUser(username As String, displayname As String, email As String, firstname As String, lastname As String, password As String)
            Dim newUser As New UserInfo()
            newUser.Username = username
            newUser.PortalID = PortalId
            newUser.DisplayName = displayname
            newUser.Email = email
            newUser.FirstName = firstname
            newUser.LastName = lastname
            newUser.Membership.Password = password
            Dim rc As UserCreateStatus = UserController.CreateUser(newUser)
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
                If Not IsPostBack Then
                    Dim ctl As New Controller
                    Dim listuser As List(Of ExportUser) = ctl.ListImportUsers
                    GridView1.DataSource = listuser
                    GridView1.DataBind()
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
            Dim ctl As New Controller
            Dim listuser As List(Of ExportUser) = ctl.ListImportUsers
            For Each u As ExportUser In listuser
                createDnnUser(u.Username, u.DisplayName, u.Email, u.FirstName, u.LastName, u.Password)
            Next
        End Sub

#End Region
    End Class

End Namespace
