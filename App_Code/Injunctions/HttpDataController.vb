Imports System.Net
Imports System.Web.Http
Imports System.Net.Http
Imports System.Collections.Generic
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Lists
Imports System.Linq

Namespace AWS.Modules.Injunctions
    Public Class HttpDataController
        Inherits DnnApiController

#Region "Web Methods"

        <HttpGet, ActionName("judges"), DnnAuthorize>
        Public Function GetJudges(moduleid As Integer, county As String, judgetypeid As Integer) As List(Of NameValueItem)
            Try
                Dim ctl As New Controller
                Dim judges = ctl.ListJudgesByJudgeType(judgetypeid, moduleid)
                Return judges.Where(Function(jud) jud.County = county).Select(Function(nl) New NameValueItem With {.Text = nl.User.DisplayName.Replace("&nbsp;", " ") & " (" & nl.CountyCode & ")", .Value = nl.JudgeId}).ToList()

            Catch ex As System.Exception
                'Log to DotNetNuke and reply with Error
                Exceptions.LogException(ex)
                Return Nothing
            End Try
        End Function

        <HttpGet, ActionName("notifyJudge"), DnnAuthorize>
        Public Function NotifiyJudge(judgeId As Integer) As NotificationResponse
            Dim ctl As New Controller
            Dim nCtl As New AWS.Modules.Notifications.Controller
            Dim judge As JudgeInfo = New JudgeInfo
            Dim colNotifications As New List(Of AWS.Modules.Notifications.NotificationInfo)
            Dim notification As New NotificationResponse
            notification.Available = True
            notification.StartOfDay = ""

            colNotifications = nCtl.ListCurrentNotificationsByJudge(judgeId)
            If Not colNotifications Is Nothing AndAlso colNotifications.Count > 0 Then
                Dim objNotification As AWS.Modules.Notifications.NotificationInfo = colNotifications.FirstOrDefault
                If objNotification.CoveringJudgeId > 0 Then
                    judge = ctl.GetJudge(objNotification.CoveringJudgeId)
                Else
                    judge = ctl.GetJudge(judgeId)
                End If
            Else
                judge = ctl.GetJudge(judgeId)
            End If
            If judge.DayEnd = Null.NullDate Or judge.DayStart = Null.NullDate Then
                Return notification
            End If
            If DateTime.Now.TimeOfDay > judge.DayEnd.TimeOfDay Or DateTime.Now.TimeOfDay < judge.DayStart.TimeOfDay Then
                notification.Available = False
                notification.StartOfDay = judge.DayStart.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                Return notification
            End If
            Return notification
        End Function

        <HttpPost, ActionName("AddAnnotation"), DnnAuthorize>
        Public Function AddAnnotationText(text As AnnotationTextItem) As HttpResponseMessage
            Try
                Dim ctl As New Controller
                Dim colAnnotations As List(Of AnnotationInfo) = ctl.GetAnnotations(UserInfo.UserID)
                For Each a In colAnnotations
                    If a.AnnotationText.ToLower.Trim = text.AnnotationText.ToLower.Trim Then
                        Return Request.CreateResponse(HttpStatusCode.OK)
                    End If
                Next
                If (text.AnnotationText <> "") Then
                    ctl.AddAnnotation(UserInfo.UserID, text.AnnotationText.Trim)

                End If
                Return Request.CreateResponse(HttpStatusCode.OK)
            Catch ex As Exception
                Exceptions.LogException(ex)
                Return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message)
            End Try
        End Function

        <HttpPost, ActionName("DeleteAnnotation"), DnnAuthorize>
        Public Function DeleteAnnotationText(annotationId As AnnotationIdItem) As HttpResponseMessage
            Try
                Dim ctl As New Controller
                ctl.DeleteAnnotation(annotationId.AnnotationId)
                Return Request.CreateResponse(HttpStatusCode.OK)
            Catch ex As Exception
                Exceptions.LogException(ex)
                Return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message)
            End Try
        End Function

        <HttpGet, ActionName("GetAnnotations"), DnnAuthorize>
        Public Function GetAnnotations() As List(Of NameValueItem)
            Try
                Dim ctl As New Controller
                Return ctl.GetAnnotations(UserInfo.UserID).Select(Function(nl) New NameValueItem With {.Text = nl.AnnotationText, .Value = nl.AnnotationId}).OrderBy(Function(n) n.Text).ToList
            Catch ex As Exception
                Exceptions.LogException(ex)
                Return Nothing
            End Try
        End Function

        Public Class NameValueItem
            Public Property Value() As String
            Public Property Text() As String
        End Class

        Public Class NotificationResponse
            Public Property Available() As Boolean
            Public Property StartOfDay() As String
        End Class

        Public Class AnnotationTextItem
            Public Property AnnotationText As String
        End Class

        Public Class AnnotationIdItem
            Public Property AnnotationId As Integer
        End Class


#End Region

    End Class
End Namespace