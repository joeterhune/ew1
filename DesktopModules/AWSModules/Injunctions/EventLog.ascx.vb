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
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection
Imports Telerik.Web.UI

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
    Partial Class EventLog
        Inherits Entities.Modules.PortalModuleBase


#Region "Private Members"
        Private startDate As DateTime = Now.AddDays(-7)
        Private endDate As DateTime = Now
        Private IsAgencyAdmin As Boolean = False
        Private objAgencyUser As AgencyUserInfo = Nothing
        Private ctl As New Controller
        Private encrypt As New Encryptor


        Private ReadOnly Property IsAdminJudge() As Boolean
            Get
                If AdminJudge <> "" Then
                    Return AdminJudge = UserInfo.Username
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property AdminJudge As String
            Get
                If Not Settings("AdminJudge") Is Nothing Then
                    Return Settings("AdminJudge").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                Dim objJudge As JudgeInfo = ctl.GetJudge(UserId)
                If Not objJudge Is Nothing Then
                    If Not Settings("AdminJudge") Is Nothing Then
                        Dim judgeId As Integer = Null.NullInteger
                        Int32.TryParse(encrypt.QueryStringDecode(objJudge.Approved, AdminJudge), judgeId)
                        Return UserId = judgeId
                    End If
                End If
                Return False
            End Get
        End Property

        Private ReadOnly Property JudgeRoleName() As String
            Get
                If Not Settings("JudgeRole") Is Nothing Then
                    Return Settings("JudgeRole").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property SergeantRoleName() As String
            Get
                If Not Settings("SergeantRole") Is Nothing Then
                    Return Settings("SergeantRole").ToString.Trim
                End If
                Return ""
            End Get
        End Property

        Private ReadOnly Property IsSeargeant As Boolean
            Get
                If SergeantRoleName <> "" Then
                    Return UserInfo.IsInRole(SergeantRoleName)
                End If
                Return False
            End Get

        End Property

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

#Region "private methods"

        Private Sub BindData()
            If IsJudge Then

            ElseIf IsSeargeant Then
                Dim agencyId As Integer = objAgencyUser.AgencyId

            Else

            End If
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

                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


#End Region


    End Class

End Namespace
