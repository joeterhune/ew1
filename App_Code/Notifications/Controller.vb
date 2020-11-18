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
Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities

Namespace AWS.Modules.Notifications

    ''' <summary>
    ''' The Controller class for Warrants
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Controller

#Region "Notification Methods"
        Public Function ListCurrentNotifications() As List(Of NotificationInfo)
            Return CBO.FillCollection(Of NotificationInfo)(DataProvider.Instance().ListCurrentNotifications)
        End Function
        Public Function ListMyNotifications(requestingJudgeId As Integer) As List(Of NotificationInfo)
            Return CBO.FillCollection(Of NotificationInfo)(DataProvider.Instance().ListMyNotifications(requestingJudgeId))
        End Function
        Public Function ListCurrentNotificationsByJudge(requestingJudgeId As Integer) As List(Of NotificationInfo)
            Return CBO.FillCollection(Of NotificationInfo)(DataProvider.Instance().ListCurrentNotificationsByJudge(requestingJudgeId))
        End Function

        Public Sub DeleteNotification(ByVal scheduleId As Integer)
            DataProvider.Instance().DeleteNotification(scheduleId)
        End Sub
        Public Function AddNotification(ByVal objNotification As NotificationInfo) As Integer
            Return CType(DataProvider.Instance().AddNotification(objNotification.RequestingJudgeId, objNotification.CoveringJudgeId, objNotification.StartDateTime, objNotification.EndDateTime, objNotification.MessageText), Integer)
        End Function

        Public Function ListConflictingNotifications(judgeId As Integer, startDate As DateTime, endDate As DateTime) As List(Of NotificationInfo)
            Return CBO.FillCollection(Of NotificationInfo)(DataProvider.Instance().ListConflictingNotifications(judgeId, startDate, endDate))
        End Function
#End Region

    End Class
End Namespace
