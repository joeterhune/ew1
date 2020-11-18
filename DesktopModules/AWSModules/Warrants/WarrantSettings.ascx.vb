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
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection
Imports Telerik.Web.UI
Imports System.IO

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
    Partial Class Settings
        Inherits Entities.Modules.ModuleSettingsBase

#Region "Private Members"


#End Region

#Region "methods"
        Private Sub BindList()
            Dim ctl As New DotNetNuke.Security.Roles.RoleController
            Dim listroles = ctl.GetRoles(PortalId)
            For Each r As DotNetNuke.Security.Roles.RoleInfo In listroles
                drpJudgeRole.Items.Add(New ListItem(r.RoleName))
                drpAgencySupervisor.Items.Add(New ListItem(r.RoleName))
                drpJARole.Items.Add(New ListItem(r.RoleName))
                drpSignedWarrantRole.Items.Add(New ListItem(r.RoleName))
                drpAdminJudge.Items.Add(new listitem(r.RoleName))
            Next
            drpJudgeRole.Items.Insert(0, New ListItem("< Select Role >", ""))
            drpAgencySupervisor.Items.Insert(0, New ListItem("< Select Role >", ""))
            drpJARole.Items.Insert(0, New ListItem("< Select Role >", ""))
            drpSignedWarrantRole.Items.Insert(0, New ListItem("< Select Role >", ""))
            drpAdminJudge.Items.Insert(0, New ListItem("< Select Role >", ""))

        End Sub

        Private Sub BindTabs()
            cboJAPage.DataSource = TabController.GetPortalTabs(PortalSettings.PortalId, PortalSettings.ActiveTab.TabID, True, "< Select Page >", True, False, False, False, True)
            cboJAPage.DataBind()
            cboContactList.DataSource = TabController.GetPortalTabs(PortalSettings.PortalId, PortalSettings.ActiveTab.TabID, True, "< Select Page >", True, False, False, False, True)
            cboContactList.DataBind()

        End Sub

#End Region

#Region "Base Method Implementations"
        Public Overrides Sub LoadSettings()
            Try
                If Not Page.IsPostBack Then
                    BindList()
                    BindTabs()
                    Dim JudgeRole As String = CType(TabModuleSettings("JudgeRole"), String)
                    drpJudgeRole.SelectedValue = JudgeRole
                    Dim JaRole As String = CType(TabModuleSettings("JaRole"), String)
                    drpJARole.SelectedValue = JaRole
                    Dim SergeantRole As String = CType(TabModuleSettings("SergeantRole"), String)
                    drpAgencySupervisor.SelectedValue = SergeantRole
                    Dim SignedWarrantRole As String = CType(TabModuleSettings("SignedWarrantRole"), String)
                    drpSignedWarrantRole.SelectedValue = SignedWarrantRole
                    Dim jaPage As String = CType(TabModuleSettings("JaPage"), String)
                    cboJAPage.SelectedValue = jaPage
                    Dim contactPage As String = CType(TabModuleSettings("ContactPage"), String)
                    cboContactList.SelectedValue=contactPage
                    Dim AdminJudge As String = CType(TabModuleSettings("AdminJudge"), String)
                    If AdminJudge <> "" Then
                        drpAdminJudge.SelectedValue = AdminJudge
                    End If
                    Dim demoMode As Boolean = False
                    Boolean.TryParse(TabModuleSettings("DemoMode"), demoMode)
                    If demoMode Then
                        chkDemoMode.Checked = True
                    End If
                    Dim ctl As New Controller
                    Dim objSettings As WarrantConfigSettings = ctl.GetModuleSettings(ModuleId)
                    If Not objSettings Is Nothing Then
                        txtClaimedThreshold.Text = objSettings.ClaimedThreshold
                        txtDeleteThreshold.Text = objSettings.DeleteThreshold
                        txtNotificationEmail.Text = objSettings.NotificationEmail
                        txtSignedThreshold.Text = objSettings.SignedThreshold
                        txtSenderEmail.Text = objSettings.SenderEmail
                        chkDemoMode.Checked = objSettings.DemoMode
                        txtCompletedThreshold.Text = objSettings.Hours
                    End If
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Overrides Sub UpdateSettings()
            Try

                Dim objModules As New Entities.Modules.ModuleController
                Dim JudgeRole As String = drpJudgeRole.SelectedValue
                Dim JaRole As String = drpJARole.SelectedValue
                Dim AdminJudge As String = drpAdminJudge.SelectedValue
                Dim SergeantRole As String = drpAgencySupervisor.SelectedValue
                Dim DemoMode As Boolean = chkDemoMode.Checked
                Dim JaPage As String = cboJAPage.SelectedValue
                Dim SignedWarrantRole As String = drpSignedWarrantRole.SelectedValue
                Dim ContactPage As String = cboContactList.SelectedValue
                If SignedWarrantRole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "SignedWarrantRole", SignedWarrantRole.Trim)
                End If

                If SergeantRole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "SergeantRole", SergeantRole.Trim)
                End If

                If JudgeRole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "JudgeRole", JudgeRole.Trim)
                End If
                If JaRole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "JaRole", JaRole.Trim)
                End If
                If AdminJudge.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "AdminJudge", AdminJudge.Trim)
                End If
                If JaPage.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "JaPage", JaPage.Trim)
                End If
                If ContactPage.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "ContactPage", ContactPage.Trim)
                End If

                Dim objSettings As New WarrantConfigSettings
                objSettings.ModuleId = ModuleId
                objSettings.ClaimedThreshold = txtClaimedThreshold.Text
                objSettings.DeleteThreshold = txtDeleteThreshold.Text
                objSettings.NotificationEmail = txtNotificationEmail.Text
                objSettings.SenderEmail = txtSenderEmail.Text
                objSettings.SignedThreshold = txtSignedThreshold.Text
                objSettings.DemoMode = chkDemoMode.Checked
                Dim hours As String = txtCompletedThreshold.Text
                objSettings.Hours = IIf(IsNumeric(txtCompletedThreshold.Text), Int32.Parse(txtCompletedThreshold.Text), 0)
                Dim ctl As New Controller
                ctl.UpdateWarrantSettings(objSettings)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


#End Region

       
    End Class

End Namespace
