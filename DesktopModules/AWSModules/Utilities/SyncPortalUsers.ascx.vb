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
    Partial Class SyncPortalUsers
        Inherits Entities.Modules.PortalModuleBase

#Region " Methods "

        Private ctl As New Controller


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
                    Dim selectedPortal As Integer = Null.NullInteger
                    Dim pctl As New PortalController
                    Dim listPortals As List(Of PortalInfo) = pctl.GetPortalList("")
                    For Each p As PortalInfo In listPortals
                          If p.PortalID <> PortalId Then
                        drpPortals.Items.Add(New ListItem(p.PortalName, p.PortalID))
                          End If
                    Next
                    Int32.TryParse(drpPortals.SelectedValue, selectedPortal)
                    If drpPortals.SelectedIndex >= 0 Then
                        Dim listUserInfo As New List(Of UserInfo)
                        Dim listuser As List(Of portalUser) = ctl.ListPortalUsers(selectedPortal,PortalId)
                        For Each pu As portalUser In listuser
                            Dim objuser As UserInfo = UserController.GetUserById(selectedPortal, pu.UserId)
							If Not objuser is Nothing then
								listUserInfo.Add(objuser)
							End If
                            
                        Next
                        If listUserInfo.Count <= 0 Then
                            cmdUpdate.Enabled = False

                        End If
                        GridView1.DataSource = listUserInfo
                        GridView1.DataBind()
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "No additional portals. Nothing to Sync", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        cmdUpdate.Enabled = False
                    End If
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
            Dim selectedPortal As Integer = Null.NullInteger

            Int32.TryParse(drpPortals.SelectedValue, selectedPortal)

            Dim listuser As List(Of portalUser) = ctl.ListPortalUsers(selectedPortal,Portalid)
            Dim ctlRole As RoleController = New RoleController()
            Dim role As RoleInfo = ctlRole.GetRoleByName(PortalId, "Registered Users")
            For Each u As portalUser In listuser
                Try
                    ctl.AddPortalUser(PortalId, u.UserId)
                    Dim user As UserInfo = UserController.GetUserById(PortalId, u.UserId)
                    If Not user.IsInRole(role.RoleName) Then
                        ctlRole.AddUserRole(PortalId, u.UserId, role.RoleID, Now(), Nothing)
                    End If
                Catch

                End Try
            Next
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "All Users from " & drpPortals.SelectedItem.Text & " have been added to this site", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            cmdUpdate.Enabled = False

        End Sub

        Private Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
            If e.CommandName = "add" Then
               ' Dim rowIndex As Integer = TryCast(TryCast(sender, Button).NamingContainer, GridViewRow).RowIndex
				Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim id As Integer = Convert.ToInt32(GridView1.DataKeys(rowIndex).Values(0))
				
                ctl.AddPortalUser(PortalId, id)
                Dim user As UserInfo = UserController.GetUserById(PortalId, id)
				If not user is nothing then
					Dim ctlRole As RoleController = New RoleController()
					Dim role As RoleInfo = ctlRole.GetRoleByName(PortalId, "Registered Users")
					If Not role Is Nothing Then
						If Not user.IsInRole(role.RoleName) Then
							ctlRole.AddUserRole(PortalId, id, role.RoleID, Now(), Nothing)
						End If

					End If
				End If
            End If
			Response.Redirect(EditUrl("sync"))
        End Sub

#End Region
    End Class

End Namespace
