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
    Partial Class ManageCounties
        Inherits Entities.Modules.PortalModuleBase

        Private ctl As New Controller

#Region "Methods"
        Private Sub BindCounties()
            rptCounties.DataSource = ctl.ListCounties(ModuleId)
            rptCounties.DataBind()
        End Sub

        Private Sub BindJudgeTypes()
            rptTypes.DataSource = ctl.ListJudgeTypes(ModuleId)
            rptTypes.DataBind()
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
            toolbar.HiddenMenu = "ManageLists"
            toolbar.NavAdminAgency = EditUrl("adminagency")
            toolbar.NavAdminCounty = ""
            toolbar.NavAnnotations = EditUrl("annotations")
            toolbar.NavJudgeSign = EditUrl("judgesign")
            toolbar.NavManageUsers = EditUrl("users")
            toolbar.NavStatus = EditUrl("wadmin")
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
                If Not IsPostBack Then
                    InitializeAdminToolbar()
                    lnkCancel.NavigateUrl = NavigateURL()
                    lnkCancelCounty.NavigateUrl = EditUrl("admincounty")
                    lnkCancelDivisionType.NavigateUrl = EditUrl("admincounty")
                    BindCounties()
                    BindJudgeTypes()
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub UpdatePanel_Unload(ByVal sender As Object, ByVal e As EventArgs)
            Dim methodInfo As MethodInfo = GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance).Where(Function(i) i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First()
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {TryCast(sender, UpdatePanel)})
        End Sub

        Private Sub rptCounties_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptCounties.ItemCommand
            Dim _countyId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "edit" Then
                Dim objCounty As CountyInfo = ctl.GetCounty(_countyId)
                hdCountyId.Value = _countyId
                lnkCancelCounty.Visible = True
                cmdSaveCounty.Text = "Update County"
                txtCounty.Text = objCounty.CountyName
                TabName.Value = "nav-county"

            End If
            If e.CommandName.ToLower = "delete" Then
                ctl.DeleteCounty(_countyId)
                BindCounties()
                TabName.Value = "nav-county"

            End If
        End Sub

        Private Sub rptCounties_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptCounties.ItemCreated
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                Dim cmdEdit As LinkButton = DirectCast(item.FindControl("cmdEdit"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
                sm.RegisterAsyncPostBackControl(cmdEdit)
            End If

        End Sub

        Private Sub rptTypes_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptTypes.ItemCommand
            Dim _judgeTypeId As Integer = CType(e.CommandArgument, Integer)
            If e.CommandName.ToLower = "edit" Then
                Dim objJudgeType As JudgeTypeInfo = ctl.GetJudgeType(_judgeTypeId)
                hdDivisionTypeId.Value = _judgeTypeId
                lnkCancelDivisionType.Visible = True
                cmdAddDivisionType.Text = "Update Division Type"
                txtDivisionType.Text = objJudgeType.JudgeType
                TabName.Value = "nav-division"
            End If
            If e.CommandName.ToLower = "delete" Then
                ctl.DeleteJudgeType(_judgeTypeId)
                BindJudgeTypes()
                TabName.Value = "nav-division"

            End If

        End Sub

        Private Sub rptTypes_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptTypes.ItemCreated
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim item As RepeaterItem = e.Item
                Dim sm = ScriptManager.GetCurrent(Page)
                Dim cmdDelete As LinkButton = DirectCast(item.FindControl("cmdDelete"), LinkButton)
                Dim cmdEdit As LinkButton = DirectCast(item.FindControl("cmdEdit"), LinkButton)
                sm.RegisterAsyncPostBackControl(cmdDelete)
                sm.RegisterAsyncPostBackControl(cmdEdit)
            End If

        End Sub

        Private Sub cmdAddDivisionType_Click(sender As Object, e As EventArgs) Handles cmdAddDivisionType.Click
            Dim objJudgeType As JudgeTypeInfo = New JudgeTypeInfo With {
                .JudgeType = txtDivisionType.Text.Trim,
                .ModuleID = ModuleId
            }

            If hdDivisionTypeId.Value <> "" Then
                objJudgeType.JudgeTypeID = CType(hdDivisionTypeId.Value, Integer)
                ctl.UpdateJudgeType(objJudgeType)
                hdDivisionTypeId.Value = ""
                cmdAddDivisionType.Text = "Add Division Type"
                lnkCancelDivisionType.Visible = False
            Else
                ctl.AddJudgeType(objJudgeType)
            End If
            txtDivisionType.Text = ""
            BindJudgeTypes()
            TabName.Value = "nav-division"

        End Sub

        Private Sub cmdSaveCounty_Click(sender As Object, e As EventArgs) Handles cmdSaveCounty.Click
            Dim objCounty As CountyInfo = New CountyInfo With {
            .CountyName = txtCounty.Text.Trim,
            .ModuleID = ModuleId
            }

            If hdCountyId.Value <> "" Then
                objCounty.CountyID = CType(hdCountyId.Value, Integer)
                ctl.UpdateCounty(objCounty)
                hdCountyId.Value = ""
                cmdSaveCounty.Text = "Add County"
                lnkCancelCounty.Visible = False
            Else
                ctl.AddCounty(objCounty)
            End If
            txtCounty.Text = ""
            BindCounties()
            TabName.Value = "nav-county"

        End Sub

#End Region

    End Class

End Namespace
