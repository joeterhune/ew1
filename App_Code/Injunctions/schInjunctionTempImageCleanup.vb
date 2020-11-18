Imports DotNetNuke.Services.Scheduling
Imports System.IO


Namespace AWS.Modules.Injunctions
    Public Class InjunctionTempImageCleanup
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

                Dim tempImageDirectory As String = DotNetNuke.Common.Globals.ApplicationMapPath & "\App_Data\RadUploadTemp"
                Dim count As Integer = 0
                Dim dir As New DirectoryInfo(tempImageDirectory)
                For Each f As FileInfo In dir.GetFiles("*.tif")
                    If f.CreationTime > Now().AddDays(-1) Then
                        f.Delete()
                        count += 1
                    End If
                Next
                'To log note
                Me.ScheduleHistoryItem.AddLogNote(count.ToString & " Temp Files Deleted")
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
