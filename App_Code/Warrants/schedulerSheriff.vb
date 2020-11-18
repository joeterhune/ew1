Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic


'This class is used to create a schedule in Dotnetnuke to notify sheriff agencyies that they have signed warrants that have not been picked up.

Namespace AWS.Modules.Warrants
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
                Dim senderEmail As String = ""
                'Perform required items for logging
                Me.Progressing()
                Dim ctl As New Controller
                Dim objSettings As List(Of WarrantConfigSettings) = ctl.GetSettings()
                For Each ws As WarrantConfigSettings In objSettings
                    senderEmail = ws.SenderEmail
                    notificationEmail = ws.NotificationEmail
                    claimedThreshold = ws.ClaimedThreshold
                    deleteThreshold = ws.DeleteThreshold
                    If deleteThreshold > 0 Then

                        ctl.DeleteOldWarrants(ws.ModuleId, Now.AddDays(-deleteThreshold))
                    End If
                    If claimedThreshold > 0 Then
                        Dim colWarrants As List(Of WarrantsInfo) = ctl.GetUnclaimedWarrants(ws.ModuleId, Now.AddDays(-claimedThreshold))
                        Dim colAgencies = From w In colWarrants Select w.AgencyId Distinct
                        body = "The following warrants have been signed but have not been reveiwed by your agency for over " & claimedThreshold.ToString & " days."
                        If Not colWarrants Is Nothing Then
                            For Each a As Integer In colAgencies
                                Try

                                    Dim objagency As AgencyInfo = ctl.GetAgency(a)
                                    Dim colAgencyWarrants = colWarrants.Where(Function(aw) aw.AgencyId = a).ToList
                                    EmailWarrantAlerts(colAgencyWarrants, senderEmail, objagency.EmailAddress, body + objagency.AgencyName)
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

        Public Sub EmailWarrantAlerts(colWarrants As List(Of WarrantsInfo), senderEmail As String, email As String, body As String)

            Dim sbWarrantList As New StringBuilder
            sbWarrantList.Append(vbCrLf)

            For Each w As WarrantsInfo In colWarrants
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
                EmailOfficer(w, senderEmail)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append("Warrant ID: ")
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
                sbWarrantList.Append(judgeName)
                sbWarrantList.Append(vbCrLf)
            Next
            Dim subject As String = "eWarrant Alert"

            body += sbWarrantList.ToString

            DotNetNuke.Services.Mail.Mail.SendEmail(senderEmail, email, subject, body)


        End Sub

        Public Sub EmailOfficer(w As WarrantsInfo, senderEmail As String)
            Dim objUser As UserInfo = UserController.GetUserById(0, w.CreatedByUserId)
            If Not objUser Is Nothing Then
                Dim sbWarrantList As New StringBuilder
                sbWarrantList.Append("Warrant ID: ")
                sbWarrantList.Append(w.WarrantId)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Title:")
                sbWarrantList.Append(w.Title)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Submitted on: ")
                sbWarrantList.Append(w.CreatedDate.ToString)
                sbWarrantList.Append(vbCrLf)
                sbWarrantList.Append(" Submitted by: ")
                sbWarrantList.Append(w.CreatedByName.ToString)
                sbWarrantList.Append(vbCrLf)

                Dim subject As String = "eWarrant Alert"
                Dim body As String = "The Warrant" & w.Title & "has been signed but has not been reveiwed by your agency for over " & claimedThreshold.ToString & " days."
                body += sbWarrantList.ToString

                DotNetNuke.Services.Mail.Mail.SendEmail(senderEmail, objUser.Email, subject, body)

            End If
        End Sub
    End Class
End Namespace
