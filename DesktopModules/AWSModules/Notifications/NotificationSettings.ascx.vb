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

Namespace AWS.Modules.Notifications

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class NotificationSettings
        Inherits Entities.Modules.ModuleSettingsBase

#Region "Private Members"


#End Region

#Region "methods"
        Private Sub BindList()
            Dim ctl As New DotNetNuke.Security.Roles.RoleController

            Dim listroles = ctl.GetRoles(PortalId)
            For Each r As DotNetNuke.Security.Roles.RoleInfo In listroles
                drpJudgeRole.Items.Add(New ListItem(r.RoleName))
                drpJARole.Items.Add(New ListItem(r.RoleName))
            Next
            drpJudgeRole.Items.Insert(0, New ListItem("< Select Role >", ""))
            drpJARole.Items.Insert(0, New ListItem("< Select Role >", ""))

        End Sub


#End Region

#Region "Base Method Implementations"
        Public Overrides Sub LoadSettings()
            Try
                If Not Page.IsPostBack Then
                    BindList()
                    Dim JudgeRole As String = CType(TabModuleSettings("JudgeRole"), String)
                    drpJudgeRole.SelectedValue = JudgeRole
                    Dim JARole As String = CType(TabModuleSettings("JARole"), String)
                    drpJARole.SelectedValue = JARole
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Overrides Sub UpdateSettings()
            Try

                Dim objModules As New Entities.Modules.ModuleController
                Dim JudgeRole As String = drpJudgeRole.SelectedValue
                Dim JARole As String = drpJARole.SelectedValue

                If JARole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "JARole", JARole.Trim)
                End If
                If JudgeRole.Trim <> "" Then
                    objModules.UpdateTabModuleSetting(TabModuleId, "JudgeRole", JudgeRole.Trim)
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


#End Region

#Region "Event Handlers"


#End Region


    End Class

End Namespace
