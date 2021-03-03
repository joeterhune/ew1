Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic

Namespace AWS.Modules.Warrants
    Public Class Notifications
        Inherits SchedulerClient

        Public Sub New(oItem As ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = oItem
        End Sub


        Public Overrides Sub DoWork()
            Try
                Dim notificationEmail As String = ""
                Dim senderEmail As String = ""
                Dim signedThreshold As Integer = 0
                Dim body As String = ""
                Dim email As String = ""

                'Perform required items for logging
                Me.Progressing()
                Dim ctl As New Controller
                Dim objSettings As List(Of WarrantConfigSettings) = ctl.GetSettings()
                For Each ws As WarrantConfigSettings In objSettings
                    If ws.SignedThreshold > 0 Then
                        notificationEmail = ws.NotificationEmail
                        senderEmail = ws.SenderEmail
                        signedThreshold = ws.SignedThreshold
                        Dim colWarrants As List(Of WarrantsInfo) = ctl.GetUnsignedWarrants(ws.ModuleId, Date.Now.AddDays(-signedThreshold))
                        If Not colWarrants Is Nothing AndAlso colWarrants.Count > 0 Then
                            EmailWarrantAlerts(colWarrants, senderEmail, notificationEmail, "The following documents have been waiting for review for over " & signedThreshold.ToString & " days.")
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

        Public Sub EmailWarrantAlerts(colWarrants As List(Of WarrantsInfo), senderEmail As String, email As String, body As String)

            Dim sbWarrantList As New StringBuilder
            sbWarrantList.Append(vbCrLf)
            For Each w As WarrantsInfo In colWarrants
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
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append("Document ID: ")
                sbWarrantList.Append(w.WarrantId)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Title:")
                sbWarrantList.Append(title)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Submitted on: ")
                sbWarrantList.Append(createdDate)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Submitted by: ")
                sbWarrantList.Append(createdBy)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Assigned Judge: ")
                sbWarrantList.Append(JudgeName)
                sbWarrantList.Append(vbCrLf)
            Next
            Dim subject As String = "eSubmit Alert"

            body += sbWarrantList.ToString

            DotNetNuke.Services.Mail.Mail.SendEmail(senderEmail, email, subject, body)


        End Sub
    End Class
End Namespace
