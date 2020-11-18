Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic

Namespace AWS.Modules.Injunctions
    Public Class InjunctionNotifications
        Inherits SchedulerClient

        Private PortalId As Integer = 0
        Public Sub New(oItem As ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = oItem
        End Sub


        Public Overrides Sub DoWork()
            Try

                'Perform required items for logging
                Me.Progressing()
		 If DateTime.Now.DayOfWeek = DayOfWeek.Saturday Or DateTime.Now.DayOfWeek = DayOfWeek.Sunday Then
                    'To log note
                    Me.ScheduleHistoryItem.AddLogNote("Weekend Notifications skipped")
                    'Show success
                    Me.ScheduleHistoryItem.Succeeded = True
                    Exit Sub
                End If 
                Dim ctl As New Controller
                Dim modCtl As New DotNetNuke.Entities.Modules.ModuleController
                Dim objSettings As List(Of InjunctionConfigSettings) = ctl.GetSettings()
                For Each ws As InjunctionConfigSettings In objSettings

                    Dim myMod As DotNetNuke.Entities.Modules.ModuleInfo = modCtl.GetModule(ws.ModuleId)
                    PortalId = myMod.PortalID
                    Dim colInjunctions As List(Of InjunctionsInfo) = ctl.ListInjunctionsNotificationNotSent()
                    If Not colInjunctions Is Nothing AndAlso colInjunctions.Count > 0 Then
                        SendEmailResponse(colInjunctions)
                    End If
                Next


                'To log note
                Me.ScheduleHistoryItem.AddLogNote(" Queued Injunction Notifications Sent Successfully")
                'Show success
                Me.ScheduleHistoryItem.Succeeded = True


            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote("Exception= " & ex.ToString())
                Me.Errored(ex)
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try
        End Sub

        Private Function GetCoverJudge(coverJudgeId As Integer) As UserInfo
            Return UserController.GetUserById(0, coverJudgeId)
        End Function
        Private Sub SendEmailResponse(colInjunctions As List(Of InjunctionsInfo))
            Dim ctl As New Controller
            Dim nCtl As New AWS.Modules.Notifications.Controller
            Dim subject As String = ""
            Dim userDisplay As String = ""
            Dim body As String = ""
            Dim notificationSent As Boolean
            For Each w As InjunctionsInfo In colInjunctions
                notificationSent = False
                subject = ""
                userDisplay = ""
                body = ""
                Dim sender As UserInfo = UserController.GetUserById(PortalId, w.CreatedByUserId)
                Dim fromaddress As String = sender.Email
                Dim judge As UserInfo = UserController.GetUserById(PortalId, w.JudgeUserId)
                Dim isOutofOffice As Boolean = False
                Dim CoverJudgeInfo As UserInfo = Nothing
                Dim coverJudgeName As String = ""

                If Not judge Is Nothing Then
                    Dim toAddress As String = judge.Email
                    Dim address2 As String = ""
                    Dim address3 As String = ""
                    Dim colNotifications As New List(Of AWS.Modules.Notifications.NotificationInfo)
                    colNotifications = nCtl.ListCurrentNotificationsByJudge(judge.UserID)
                    If Not colNotifications Is Nothing AndAlso colNotifications.Count > 0 Then
                        Dim objNotification As AWS.Modules.Notifications.NotificationInfo = colNotifications.FirstOrDefault
                        isOutofOffice = True
                        If objNotification.CoveringJudgeId > 0 Then
                            Dim jInfo As JudgeInfo = ctl.GetJudge(objNotification.CoveringJudgeId)
                            If DateTime.Now.TimeOfDay >= jInfo.DayStart.TimeOfDay And DateTime.Now.TimeOfDay <= jInfo.DayEnd.TimeOfDay Then
                                CoverJudgeInfo = GetCoverJudge(objNotification.CoveringJudgeId)
                                coverJudgeName = CoverJudgeInfo.DisplayName
                                If Not CoverJudgeInfo.Profile.ProfileProperties("Email2") Is Nothing Then
                                    address2 = CoverJudgeInfo.Profile.ProfileProperties("Email2").PropertyValue
                                End If
                                If Not CoverJudgeInfo.Profile.ProfileProperties("Email3") Is Nothing Then
                                    address3 = CoverJudgeInfo.Profile.ProfileProperties("Email3").PropertyValue
                                End If
                                subject = "New Injunction Forwarded from  " & judge.DisplayName.Replace("&nbsp;", " ") & " for Your Review"
                                userDisplay = sender.DisplayName.Replace("&nbsp;", " ")
                                body = "A new Injunction (ID: " & w.InjunctionId & ")  has been submitted by " & userDisplay & " from " & w.AgencyName & vbCrLf & vbCrLf
                                body += judge.DisplayName.Replace("&nbsp;", " ") & " is out of the office and has asked that Injunctions be forwarded to you during their absence."
                                Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                                If address2 <> "" Then
                                    Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                                End If
                                If address3 <> "" Then
                                    Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                                End If
                                notificationSent = True
                            End If
                        Else
                            Dim jInfo As JudgeInfo = ctl.GetJudge(w.JudgeUserId)
                            If DateTime.Now.TimeOfDay >= jInfo.DayStart.TimeOfDay And DateTime.Now.TimeOfDay <= jInfo.DayEnd.TimeOfDay Then
                                subject = "New Injunction Submitted for Review"
                                userDisplay = sender.DisplayName.Replace("&nbsp;", " ")
                                body = "A new Injunction has been submitted by " & userDisplay & " from " & w.AgencyName
                                If Not judge.Profile.ProfileProperties("Email2") Is Nothing Then
                                    address2 = judge.Profile.ProfileProperties("Email2").PropertyValue
                                End If
                                If Not judge.Profile.ProfileProperties("Email3") Is Nothing Then
                                    address3 = judge.Profile.ProfileProperties("Email3").PropertyValue
                                End If

                                Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)

                                If address2 <> "" Then
                                    Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                                End If

                                If address3 <> "" Then
                                    Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                                End If
                                notificationSent = True
                            End If
                        End If
                    Else
                        Dim jInfo As JudgeInfo = ctl.GetJudge(w.JudgeUserId)
                        If DateTime.Now.TimeOfDay >= jInfo.DayStart.TimeOfDay And DateTime.Now.TimeOfDay <= jInfo.DayEnd.TimeOfDay Then

                            subject = "New Injunction Submitted for Review"
                            userDisplay = sender.DisplayName.Replace("&nbsp;", " ")
                            body = "A new Injunction (ID: " & w.InjunctionId & ") has been submitted by " & userDisplay & " from " & w.AgencyName
                            If Not judge.Profile.ProfileProperties("Email2") Is Nothing Then
                                address2 = judge.Profile.ProfileProperties("Email2").PropertyValue
                            End If
                            If Not judge.Profile.ProfileProperties("Email3") Is Nothing Then
                                address3 = judge.Profile.ProfileProperties("Email3").PropertyValue
                            End If
                            Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                            If address2 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                            End If
                            If address3 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                            End If
                            notificationSent = True
                        End If

                    End If
                End If
                If notificationSent Then
                    ctl.UpdateNotificationSent(w.InjunctionId)
                End If
            Next
        End Sub

    End Class
End Namespace
