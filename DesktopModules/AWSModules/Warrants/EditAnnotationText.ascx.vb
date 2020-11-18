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
    Partial Class EditAnnotationText
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private ctl As New Controller

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim objJudge As JudgeInfo = ctl.GetJudge(UserId)
                If Not objJudge Is Nothing Then
                    Return True
                End If
                Return False
            End Get
        End Property
#End Region

#Region "Methods"
        Private Sub BindList()
            Dim annotations As List(Of AnnotationInfo) = ctl.GetAnnotations(UserId)
            rptAnnotations.DataSource = annotations
            rptAnnotations.DataBind()
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
                If Not IsJudge Then
                    Response.Redirect(NavigateURL, True)
                End If
                If Page.IsPostBack = False Then
                    BindList()
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                Dim matches As Integer = ctl.GetAnnotations(UserId).Where(Function(at) at.AnnotationText.ToLower.Trim = txtAnnotation.Text.ToLower.Trim).Count
                If matches = 0 Then
                    ctl.AddAnnotation(UserId, txtAnnotation.Text.Trim)
                    txtAnnotation.Text = ""
                    BindList()
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdDelete_Click(sender As Object, e As EventArgs)
            Dim lnk As LinkButton = CType(sender, LinkButton)
            Select Case lnk.CommandName
                Case "Delete"
                    ctl.DeleteAnnotation(CType(lnk.CommandArgument, Int32))
                    BindList()
            End Select
        End Sub

#End Region

    End Class

End Namespace
