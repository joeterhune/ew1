Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic

Namespace AWS.Modules.Injunctions
    Public Class Notifications
        Inherits SchedulerClient

        Public Sub New(oItem As ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = oItem
        End Sub

        Public Overrides Sub DoWork()
            Try
                Dim notificationEmail As String = ""
                Dim signedThreshold As Integer = 0
                Dim body As String = ""
                Dim email As String = ""
                Dim senderEmail As String = ""
                'Perform required items for logging
                Me.Progressing()
                Dim ctl As New Controller
                Dim objSettings As List(Of InjunctionConfigSettings) = ctl.GetSettings()
                For Each ws As InjunctionConfigSettings In objSettings
                    If ws.SignedThreshold > 0 Then
                        notificationEmail = ws.NotificationEmail
                        senderEmail = ws.SenderEmail
                        signedThreshold = ws.SignedThreshold
                        Dim colInjunctions As List(Of InjunctionsInfo) = ctl.GetUnsignedInjunctions(ws.ModuleId, Date.Now.AddDays(-signedThreshold))
                        If Not colInjunctions Is Nothing AndAlso colInjunctions.Count > 0 Then
                            EmailInjunctionAlerts(colInjunctions, senderEmail, notificationEmail, "The following Injunctions have been waiting for review for over " & signedThreshold.ToString & " days.")
                        End If
                    End If
                Next

                'To log note
                Me.ScheduleHistoryItem.AddLogNote("Notifications Processed Successfully")
                'Show success
                Me.ScheduleHistoryItem.Succeeded = True
            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote("Exception= " & ex.ToString())
                Me.Errored(ex)
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try
        End Sub

        Public Sub EmailInjunctionAlerts(colInjunctions As List(Of InjunctionsInfo), sender As String, email As String, body As String)

            Dim sbInjunctionList As New StringBuilder
            sbInjunctionList.Append(vbCrLf)
            For Each w As InjunctionsInfo In colInjunctions
                Dim createdBy As String = ""
                Dim title As String = ""
                Dim createdDate As String = ""
                Dim JudgeName As String = ""
                If Not w.JudgeName Is Nothing Then
                    JudgeName = w.JudgeName
                End If
                If Not w.CreatedByName Is Nothing Then
                    createdBy = w.CreatedByName
                End If
                If Not w.Title Is Nothing Then
                    title = w.Title
                End If
                If Not w.CreatedDate = Null.NullDate Then
                    createdDate = w.CreatedDate.ToShortDateString
                End If
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append("Injunction ID: ")
                sbInjunctionList.Append(w.InjunctionId)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Title:")
                sbInjunctionList.Append(title)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Submitted on: ")
                sbInjunctionList.Append(createdDate)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Submitted by: ")
                sbInjunctionList.Append(createdBy)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Assigned Judge: ")
                sbInjunctionList.Append(JudgeName)
                sbInjunctionList.Append(vbCrLf)
            Next
            Dim subject As String = "eInjunction Alert"

            body += sbInjunctionList.ToString

            DotNetNuke.Services.Mail.Mail.SendEmail(sender, email, subject, body)


        End Sub
    End Class
End Namespace
