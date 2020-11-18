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
Imports System.Reflection
Imports DotNetNuke.Services.Mail

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
    Partial Class ManageUsers
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"
        Private AgencyId As Integer = Null.NullInteger
        Private objAgency As AgencyInfo = Nothing
        Private ctl As New Controller
        Public ModuleTitle As String
#End Region

#Region "Methods"

        Public Function GetImage(isadmin As Boolean) As String
            If isadmin Then
                Return "~/images/checked.gif"
            Else
                Return "~/images/unchecked.gif"
            End If
        End Function

        Private Sub BindData()
            rptUsers.DataSource = ctl.ListAgencyUsers(AgencyId)
            rptUsers.DataBind()
        End Sub

        Private Sub InitializeAdminToolbar()
            Dim AdminJudge As String = ""
            Dim JudgeRoleName As String = ""
            Dim SergeantRole As String = ""
            If Not Settings("AdminJudge") Is Nothing Then
                AdminJudge = Settings("AdminJudge").ToString.Trim
            End If
            If AdminJudge <> "" Then
                toolbar.IsAdminJudge = (AdminJudge = UserInfo.Username)
            End If
            If Not Settings("JudgeRole") Is Nothing Then
                JudgeRoleName = Settings("JudgeRole").ToString.Trim
            End If
            If Not Settings("SergeantRole") Is Nothing Then
                SergeantRole = Settings("SergeantRole").ToString.Trim
            End If
            toolbar.AdminJudge = AdminJudge
            toolbar.IsClerk = False
            toolbar.IsJudge = False
            toolbar.IsSiteAdmin = (UserInfo.IsSuperUser Or UserInfo.IsInRole("Administrators"))
            toolbar.JudgeRoleName = JudgeRoleName
            toolbar.NavAddInjunction = EditUrl("addInjunction")
            toolbar.NavAdminAgency = EditUrl("adminagency")
            toolbar.NavAdminCounty = EditUrl("admincounty")
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.navStatus = EditUrl("wadmin")
            toolbar.NavManageUsers = EditUrl("users")
            toolbar.NavOutofOffice = EditUrl("cover")
            toolbar.objAgencyUser = Nothing
            toolbar.SergeantRoleName = SergeantRole
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
                    AgencyId = Int32.Parse(Request.QueryString("AgencyId"))
                End If

                If Not Page.IsPostBack Then
                    InitializeAdminToolbar()
                    lnkCancel.NavigateUrl = EditUrl("agencyId", AgencyId.ToString, "adminagency")
                    lnkAddUser.NavigateUrl = EditUrl("agencyId", AgencyId.ToString, "adduser")
                    objAgency = ctl.GetAgency(AgencyId)
                    If Not objAgency Is Nothing Then
                        ModuleTitle = "Manage users for <strong class=""text-primary"">" & objAgency.AgencyName & "</strong>"
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

        Protected Sub btnExistingUser_Click(sender As Object, e As EventArgs) Handles btnExistingUser.Click
            Response.Redirect(EditUrl("agencyId", AgencyId.ToString, "existuser"))
        End Sub

        Private Sub rptUsers_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptUsers.ItemCommand

            Dim _userId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "setadmin" Then
                Dim userid As Integer = e.CommandArgument
                Dim objAgencyUser As AgencyUserInfo = ctl.GetAgencyUser(AgencyId, userid)
                If objAgencyUser.IsAdmin Then
                    objAgencyUser.IsAdmin = False
                Else
                    objAgencyUser.IsAdmin = True
                End If
                ctl.UpdateAgencyUser(objAgencyUser)
                BindData()
            End If

            If e.CommandName.ToLower = "delete" Then
                Dim objUser As UserInfo = UserController.GetUserById(PortalId, _userId)
                ctl.DeleteAgencyUser(AgencyId, _userId)
                UserController.DeleteUser(objUser, False, True)
                objUser.Membership.Approved = False
                UserController.UpdateUser(PortalId, objUser, False)
                BindData()
            End If

        End Sub

        Private Sub rptUsers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUsers.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim objUser As AgencyUserInfo = e.Item.DataItem
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
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

