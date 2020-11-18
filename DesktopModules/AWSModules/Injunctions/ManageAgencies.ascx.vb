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
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageAgencies
        Inherits Entities.Modules.PortalModuleBase


#Region "Methods"
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
            toolbar.HiddenMenu = "ManageAgencies"
            toolbar.NavAdminAgency = ""
            toolbar.NavAdminCounty = EditUrl("admincounty")
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.NavManageUsers = EditUrl("users")
            toolbar.NavOutofOffice = EditUrl("cover")
            toolbar.NavStatus = EditUrl("iadmin")
            toolbar.objAgencyUser = Nothing
            toolbar.SergeantRoleName = SergeantRole
        End Sub
        Private Sub BindData()
            Dim ctl As New Controller
            rptAgencies.DataSource = ctl.ListAgencies(ModuleId)
            rptAgencies.DataBind()
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
                If Not Page.IsPostBack Then
                    InitializeAdminToolbar()
                    lnkCancel.NavigateUrl = NavigateURL()
                    lnkAddAgency.NavigateUrl = EditUrl("editAgency")
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

        Private Sub rptAgencies_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptAgencies.ItemCommand
            Dim _agencyId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "delete" Then
                Dim ctl As New Controller
                Dim listUsers As List(Of AgencyUserInfo) = ctl.ListAgencyUsers(_agencyId)

                For Each lu In listUsers
                    Dim objuser As UserInfo = UserController.GetUserById(PortalId, lu.UserId)
                    objuser.Membership.Approved = False
                    UserController.UpdateUser(PortalId, objuser)
                Next
                ctl.DeleteAgency(_agencyId)
                BindData()
            End If

        End Sub

        Private Sub rptAgencies_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptAgencies.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim objAgency As AgencyInfo = e.Item.DataItem
                Dim lnkEdit As HyperLink = DirectCast(item.FindControl("lnkEdit"), HyperLink)
                Dim lnkUsers As HyperLink = DirectCast(item.FindControl("lnkUsers"), HyperLink)

                lnkEdit.NavigateUrl = EditUrl("AgencyId", objAgency.AgencyId.ToString, "editAgency")
                lnkUsers.NavigateUrl = EditUrl("AgencyId", objAgency.AgencyId.ToString, "adminusers")
            End If
        End Sub

        Private Sub rptAgencies_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptAgencies.ItemCreated
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
            End If

        End Sub

#End Region

    End Class

End Namespace
