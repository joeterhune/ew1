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


Imports System
Imports System.Data
Imports DotNetNuke

Namespace AWS.Modules.Notifications

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An abstract class for the data access layer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "AWS.Modules.Notifications", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region " Notification Methods "
        Public MustOverride Function AddNotification(requestingJudgeId As Integer, coveringJudgeId As Integer, startDateTime As DateTime, endDateTime As DateTime, messageText As String) As Integer
        Public MustOverride Sub DeleteNotification(scheduleId As Integer)
        Public MustOverride Function ListMyNotifications(requestingJudgeId As Integer) As IDataReader
        Public MustOverride Function ListCurrentNotifications() As IDataReader
        Public MustOverride Function ListCurrentNotificationsByJudge(requestingJudgeId As Integer) As IDataReader
        Public MustOverride Function ListConflictingNotifications(JudgeId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader

#End Region

    End Class

End Namespace
