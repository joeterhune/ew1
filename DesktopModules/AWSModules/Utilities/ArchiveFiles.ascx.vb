Imports System.IO
Imports Atalasoft.Imaging
Imports System.Drawing
Imports Atalasoft.Imaging.Codec.Pdf
Imports Atalasoft.Imaging.Codec
Imports DotNetNuke.Entities.Users

Namespace AWS.Modules.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ArchiveFiles
        Inherits Entities.Modules.PortalModuleBase

#Region " Private Methods "
        Private fileCountArchived As Integer = 0
#End Region

#Region " Methods "

        Private Sub ArchiveInjunctions(filePath As String, cutoffdate As DateTime, deleteRecords As Boolean, archiveRejected As Boolean)
            Dim missedFileErrors As String = ""
            Try
                Dim ctl As New AWS.Modules.Injunctions.Controller
                Dim datasource = ctl.ListReviewedInjunctions(cutoffdate)
                For Each inj As Injunctions.InjunctionsInfo In datasource
                    If inj.StatusId = Injunctions.InjunctionStatus.Rejected Then
                        If archiveRejected = False Then
                            ctl.DeleteFile(inj.FileId)
                            ctl.DeleteInjunctions(inj.InjunctionId)
                            Continue For 'Skip file save
                        End If
                    End If
                    Dim col As New PdfImageCollection()
                    Dim filename As String = filePath
                    If Not filename.EndsWith("\") Then
                        filename = filename + "\"
                    End If
                    filename += inj.CreatedDate.Year.ToString + "\"
					Dim arcDir as new DirectoryInfo(filename)
					If Not arcDir.Exists Then
						arcDir.Create()
					End If
                    'get number of pages
                    Dim imageData As Byte() = GetInjunctionImage(inj.FileId, ctl)
					If imageData Is Nothing Then
						Continue For
					End If
                    Dim pages As Integer = RegisteredDecoders.GetImageInfo(New MemoryStream(imageData)).FrameCount
                    ' Add all pages from a multipage TIFF.
                    For i As Integer = 0 To pages - 1
                        col.Add(New PdfImage(New MemoryStream(imageData), i, PdfCompressionType.Auto))
                    Next i
                    filename += inj.InjunctionId.ToString & ".pdf"
                    Using pdfStream As Stream = New MemoryStream()
                        Using fs As New FileStream(filename, FileMode.Create, FileAccess.Write)
                            Dim pdf As New PdfEncoder()
                            pdf.JpegQuality = 85
                            pdf.Metadata = New PdfMetadata("ID" & inj.InjunctionId.ToString & "-" & inj.Title.Replace(" ", "_"), inj.AgencyName, "Injunction File", "", "", "", inj.CreatedDate, inj.ReviewedDate)
                            ' Make each image fit into an 8.5 x 11 inch page (612 x 792 @ 72 DPI).
                            pdf.SizeMode = PdfPageSizeMode.FitToPage
                            pdf.PageSize = New Size(612, 792)
                            pdf.Save(fs, col, Nothing)
                        End Using
                    End Using
                    Dim file As New FileInfo(filename)
                    If file.Exists Then
                        fileCountArchived += 1
                        ctl.DeleteFile(inj.FileId)
                        If deleteRecords Then
                            ctl.DeleteInjunctions(inj.InjunctionId)
                        End If
                    Else
                        missedFileErrors += inj.InjunctionId.ToString & " was not archived." + Environment.NewLine
                        Continue For
                    End If

                Next
            Catch ex As Exception
			ProcessModuleLoadException(Me, ex)
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            Finally
                If missedFileErrors <> "" Then
                    Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("Archive Failure", missedFileErrors, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Some files could not be archived please view Admin Log for details", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End Try

        End Sub

        Private Sub ArchiveWarrants(filePath As String, cutoffdate As DateTime, deleteRecords As Boolean, archiveRejected As Boolean)
            Dim missedFileErrors As String = ""
            Try
                Dim ctl As New AWS.Modules.Warrants.Controller
                Dim datasource = ctl.ListReviewedWarrants(cutoffdate)
                For Each war As Warrants.WarrantsInfo In datasource
                    If war.StatusId = Warrants.WarrantStatus.Rejected Then
                        If archiveRejected = False Then
                            ctl.DeleteFile(war.FileId)
                            ctl.DeleteWarrants(war.WarrantId)
                            Continue For 'Skip file save
                        End If
                    End If
                    Dim col As New PdfImageCollection()
                    Dim filename As String = filePath
                    If Not filename.EndsWith("\") Then
                        filename = filename + "\"
                    End If
                    filename += war.CreatedDate.Year.ToString + "\"
					Dim arcDir as new DirectoryInfo(filename)
					If Not arcDir.Exists Then
						arcDir.Create()
					End If
                    'get number of pages
                    Dim imageData As Byte() = GetWarrantImage(war.FileId, ctl)
					If imageData Is Nothing Then
						Continue For
					End If
                    Dim pages As Integer = RegisteredDecoders.GetImageInfo(New MemoryStream(imageData)).FrameCount
                    ' Add all pages from a multipage TIFF.
                    For i As Integer = 0 To pages - 1
                        col.Add(New PdfImage(New MemoryStream(imageData), i, PdfCompressionType.Auto))
                    Next i
                    filename += war.WarrantId.ToString & ".pdf"
                    Using pdfStream As Stream = New MemoryStream()
                        Using fs As New FileStream(filename, FileMode.Create, FileAccess.Write)
                            Dim pdf As New PdfEncoder()
                            pdf.JpegQuality = 85
                            pdf.Metadata = New PdfMetadata("ID" & war.WarrantId.ToString & "-" & war.Title.Replace(" ", "_"), war.AgencyName, "Injunction File", "", "", "", war.CreatedDate, war.ReviewedDate)
                            ' Make each image fit into an 8.5 x 11 inch page (612 x 792 @ 72 DPI).
                            pdf.SizeMode = PdfPageSizeMode.FitToPage
                            pdf.PageSize = New Size(612, 792)
                            pdf.Save(fs, col, Nothing)
                        End Using
                    End Using
                    Dim file As New FileInfo(filename)
                    If file.Exists Then
                        fileCountArchived += 1
                        ctl.DeleteFile(war.FileId)
                        If deleteRecords Then
                            ctl.DeleteWarrants(war.WarrantId)
                        End If
                    Else
                        missedFileErrors += war.WarrantId.ToString & " was not archived." + Environment.NewLine
                        Continue For
                    End If

                Next
            Catch ex As Exception
			ProcessModuleLoadException(Me, ex)
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            Finally
                If missedFileErrors <> "" Then
                    Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("Archive Failure", missedFileErrors, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Some files could not be archived please view Admin Log for details", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End Try
        End Sub

        Private Function GetWarrantImage(ByVal id As Integer, ctl As Warrants.Controller) As Byte()
            Dim objWarrantImage As Warrants.WarrantImage = ctl.GetFile(id)
			If Not objWarrantImage Is Nothing Then
				Dim encrypt As New Warrants.Encryptor
				Dim imageData As Byte() = DirectCast(encrypt.DecryptStream(objWarrantImage.Bytes), Byte())
				Return imageData
			Else
				Return Nothing
			End If

        End Function

        Private Function GetInjunctionImage(ByVal id As Integer, ctl As Injunctions.Controller) As Byte()
            Dim objInjunctionImage As Injunctions.InjunctionImage = ctl.GetFile(id)
			If Not objInjunctionImage is Nothing Then
				Dim encrypt As New Injunctions.Encryptor
				Dim imageData As Byte() = DirectCast(encrypt.DecryptStream(objInjunctionImage.Bytes), Byte())
				Return imageData

			Else
				Return Nothing
			End If

        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not IsPostBack Then

                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
            Dim archiveRejected As Boolean = False
            Dim deleteRecords As Boolean = False
            Dim cutoffDate As DateTime = Null.NullDate
            Dim filePath As String = ""
            Dim validationErrors As String = ""
            Dim dir As DirectoryInfo
            Dim datasource = Nothing
            deleteRecords = chkDeleteRecords.Checked
            archiveRejected = chkDeleteRejected.Checked
            DateTime.TryParse(txtDate.Text, cutoffDate)
            filePath = txtFileLocation.Text.Trim
            dir = New DirectoryInfo(filePath)
            If Not dir.Exists Then
                Try
                    dir.Create()
                Catch ex As Exception
                    validationErrors += "<li>Could not create directory. EXCEPTION:" & ex.Message & "</li>"
                End Try
            End If
            If cutoffDate = Null.NullDate Then
                validationErrors += "<li>The cutoff date value is not a valid date</li>"
            End If
            If validationErrors = "" Then
                Try
                    Select Case drpSource.SelectedValue
                        Case "1"
                            ArchiveInjunctions(filePath, cutoffDate, deleteRecords, archiveRejected)
                        Case "2"
                            ArchiveWarrants(filePath, cutoffDate, deleteRecords, archiveRejected)
                    End Select

                Catch ex As Exception
                    validationErrors += "<li>Archive Failed. EXCEPTION:" & ex.Message & "</li>"
                End Try
            End If
            If validationErrors <> "" Then
                validationErrors = "<p>Pleae fix the following errors and try again.</p><ul>" + validationErrors + "</ul>"
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, validationErrors, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            Else
                Dim successMessage As String = ""
                If fileCountArchived > 0 Then
                    successMessage = "Archive Request Complete. " & fileCountArchived.ToString & " Files Archived."
                Else
                    successMessage = "No files selected to archive"
                End If
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, successMessage, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            End If

        End Sub

        Private Sub cmdShowRecords_Click(sender As Object, e As EventArgs) Handles cmdShowRecords.Click
            Dim cutoffDate As DateTime = Null.NullDate
            DateTime.TryParse(txtDate.Text, cutoffDate)

            If Not cutoffDate = Null.NullDate Then
                Dim datasource = Nothing
                Select Case drpSource.SelectedValue
                    Case "1"
                        Dim ctlInj As New AWS.Modules.Injunctions.Controller
                        datasource = ctlInj.ListReviewedInjunctions(cutoffDate)
                    Case "2"
                        Dim ctlWar As New AWS.Modules.Warrants.Controller
                        datasource = ctlWar.ListReviewedWarrants(cutoffDate)
                End Select
                GridView1.DataSource = datasource
                GridView1.DataBind()
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Please enter a valid cutoff date", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Sub

#End Region
    End Class

End Namespace
