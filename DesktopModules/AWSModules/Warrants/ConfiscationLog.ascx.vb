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
Imports System.IO
Imports Atalasoft.Imaging.Codec
Imports Telerik.Web.UI
Imports Atalasoft.Imaging.Codec.Pdf
Imports PdfSharp.Pdf

Namespace AWS.Modules.Warrants

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDynamicModule class is used to manage content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ConfiscationLog
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Public warrantId As String = ""
        Public uploadHandler As String = ""
     '   Private archiveDirectory As String = ConfigurationManager.AppSettings("CompletedWarrants")
        Private message As String = ""

        Private ReadOnly Property IsSiteAdmin As Boolean
            Get
                Dim isAdmin As Boolean = UserInfo.IsSuperUser
                If Not isAdmin Then
                    Return UserInfo.IsInRole("Administrators")
                End If
                Return isAdmin
            End Get
        End Property

#End Region

#Region "Methods"


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

                If Not Request.QueryString("warrantId") Is Nothing Then
                    warrantId = Request.QueryString("warrantId")
                End If
                If Page.IsPostBack = False Then
                    uploadHandler = TemplateSourceDirectory & "/Handlers/returnService.ashx"

                    lnkCancel.NavigateUrl = NavigateURL()
                    Dim ctl As New Controller
                    Dim objagencyUser As AgencyUserInfo = ctl.GetUser(ModuleId, UserId)
                    If Not objagencyUser Is Nothing And Not IsSiteAdmin Then
                        If objagencyUser.IsAdmin Then
                            Response.Redirect(NavigateURL)
                        End If
                        Dim AgencyId As Integer = objagencyUser.AgencyId
                        Dim objAgency As AgencyInfo = ctl.GetAgency(AgencyId)
                        If objAgency.IsClerk Then
                            Response.Redirect(NavigateURL)
                        End If
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Invalid User Access.  Please contact the site administrator with the error.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        dvForm.Visible = False
                        Exit Sub
                    End If

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
