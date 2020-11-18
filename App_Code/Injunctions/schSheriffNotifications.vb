Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic


'This class is used to create a schedule in Dotnetnuke to notify sheriff agencyies that they have signed Injunctions that have not been picked up.

Namespace AWS.Modules.Injunctions
    Public Class NotificationsSheriff
        Inherits SchedulerClient
        Private deleteThreshold As Integer = 0
        Private claimedThreshold As Integer = 0

        Public Sub New(oItem As ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = oItem
        End Sub


        Public Overrides Sub DoWork()
            Try

                Dim body As String = ""
                Dim email As String = ""
                Dim notificationEmail As String = ""
                Dim senderemail As String = ""
                'Perform required items for logging
                Me.Progressing()
                Dim ctl As New Controller
                Dim objSettings As List(Of InjunctionConfigSettings) = ctl.GetSettings()
                For Each ws As InjunctionConfigSettings In objSettings
                    senderemail = ws.SenderEmail
                    notificationEmail = ws.NotificationEmail
                    claimedThreshold = ws.ClaimedThreshold
                    deleteThreshold = ws.DeleteThreshold
                    If deleteThreshold > 0 Then

                        ctl.DeleteOldInjunctions(ws.ModuleId, Now.AddDays(-deleteThreshold))
                    End If
                    If claimedThreshold > 0 Then
                        Dim colInjunctions As List(Of InjunctionsInfo) = ctl.GetUnclaimedInjunctions(ws.ModuleId, Now.AddDays(-claimedThreshold))
                        Dim colAgencies = From w In colInjunctions Select w.AgencyId Distinct
                        body = "The following Injunctions have been signed but have not been reveiwed by your agency for over " & claimedThreshold.ToString & " days."
                        If Not colInjunctions Is Nothing Then
                            For Each a As Integer In colAgencies
                                Try

                                    Dim objagency As AgencyInfo = ctl.GetAgency(a)
                                    Dim colAgencyInjunctions = colInjunctions.Where(Function(aw) aw.AgencyId = a).ToList
                                    EmailInjunctionAlerts(colAgencyInjunctions, senderemail, objagency.EmailAddress, body + objagency.AgencyName)
                                Catch

                                End Try
                            Next
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

        Public Sub EmailInjunctionAlerts(colInjunctions As List(Of InjunctionsInfo), senderemail As String, email As String, body As String)

            Dim sbInjunctionList As New StringBuilder
            sbInjunctionList.Append(vbCrLf)

            For Each w As InjunctionsInfo In colInjunctions
                Dim createdBy As String = ""
                Dim title As String = ""
                Dim createdDate As String = ""
                Dim judgeName As String = ""
                If Not w.CreatedByName Is Nothing Then
                    createdBy = w.CreatedByName
                End If
                If Not w.Title Is Nothing Then
                    title = w.Title
                End If
                If Not w.JudgeName Is Nothing Then
                    judgeName = w.JudgeName
                End If
                If Not w.CreatedDate = Null.NullDate Then
                    createdDate = w.CreatedDate.ToShortDateString
                End If
                EmailOfficer(w, senderemail)
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
                sbInjunctionList.Append(judgeName)
                sbInjunctionList.Append(vbCrLf)
            Next
            Dim subject As String = "eInjunction Alert"

            body += sbInjunctionList.ToString

            DotNetNuke.Services.Mail.Mail.SendEmail(senderemail, email, subject, body)


        End Sub

        Public Sub EmailOfficer(w As InjunctionsInfo, senderemail As String)
            Dim objUser As UserInfo = UserController.GetUserById(0, w.CreatedByUserId)
            If Not objUser Is Nothing Then
                Dim sbInjunctionList As New StringBuilder
                sbInjunctionList.Append("Injunction ID: ")
                sbInjunctionList.Append(w.InjunctionId)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Title:")
                sbInjunctionList.Append(w.Title)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Submitted on: ")
                sbInjunctionList.Append(w.CreatedDate.ToString)
                sbInjunctionList.Append(vbCrLf)
                sbInjunctionList.Append(" Submitted by: ")
                sbInjunctionList.Append(w.CreatedByName.ToString)
                sbInjunctionList.Append(vbCrLf)

                Dim subject As String = "eInjunction Alert"
                Dim body As String = "The Injunction" & w.Title & "has been signed but has not been reveiwed by your agency for over " & claimedThreshold.ToString & " days."
                body += sbInjunctionList.ToString

                DotNetNuke.Services.Mail.Mail.SendEmail(senderemail, objUser.Email, subject, body)

            End If
        End Sub
    End Class
End Namespace
