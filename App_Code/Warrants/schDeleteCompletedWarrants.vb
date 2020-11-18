Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Scheduling
Imports Microsoft.VisualBasic


'This class is used to create a schedule in Dotnetnuke to notify sheriff agencyies that they have signed warrants that have not been picked up.

Namespace AWS.Modules.Warrants
    Public Class DeleteCompletedWarrants
        Inherits SchedulerClient
        Public Sub New(oItem As ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = oItem
        End Sub


        Public Overrides Sub DoWork()
            Try

                'Perform required items for logging
                Me.Progressing()

                Dim ctl As New Controller
                Dim colSettings As List(Of WarrantConfigSettings) = ctl.GetSettings()
                If Not colSettings Is Nothing AndAlso colSettings.Count > 0 Then


                    For Each s As WarrantConfigSettings In colSettings
                        If s.Hours > 0 Then
                            ctl.DeleteCompletedWarrants(s.ModuleId, s.Hours)
                        End If

                    Next
                End If

                'To log note
                Me.ScheduleHistoryItem.AddLogNote("Completed Warrants Deleted")
                'Show success
                Me.ScheduleHistoryItem.Succeeded = True
            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote("Exception= " & ex.ToString())
                Me.Errored(ex)
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
            End Try
        End Sub
    End Class
End Namespace
