Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging
Imports System.IO
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.PdfDoc
Imports Atalasoft.Imaging.Codec.Tiff
Imports Atalasoft.Imaging.Codec.Pdf

Namespace AWS.Modules.Injunctions
    ''' <summary>
    ''' This class shows the application of an ImageCommand,
    ''' and then replacing the altered image in a PDF or Tiff.
    ''' </summary>
    Public Class ImageProcessing
        Public Sub New()
        End Sub

        Public Shared Sub ApplyCommand(ByVal command As ImageCommand, ByVal temp As AtalaImage, ByVal path As String, ByVal frame As Integer)
            If command IsNot Nothing Then
                If command.InPlaceProcessing Then
                    command.Apply(temp)
                Else
                    temp = command.Apply(temp).Image
                End If

                SaveChanges(temp, path, frame)
            End If
        End Sub

        Public Shared Function CreatePDF(imageData As Byte(), warrantTitle As String, objInjuction As InjunctionsInfo, ctl As Controller) As Boolean
            Dim pdfFileName As String = ConfigurationManager.AppSettings("CompletedInjunctions")
            Dim imageInfo As InjunctionImage = ctl.GetFile(objInjuction.FileId, True)
            Dim yearDirectoryName As String = pdfFileName + objInjuction.CreatedDate.Year.ToString
            Dim yearDirectoryInfo As New DirectoryInfo(yearDirectoryName)
            If Not yearDirectoryInfo.Exists Then
                yearDirectoryInfo.Create()
            End If

            pdfFileName += objInjuction.CreatedDate.Year.ToString + "\" + objInjuction.InjunctionId.ToString + ".pdf"
            If imageInfo.FileType = InjunctionFileType.pdf Then
                Using pdfStream As New MemoryStream(imageData)
                    pdfStream.Seek(0, SeekOrigin.Begin)
                    Using fs As New FileStream(pdfFileName, FileMode.OpenOrCreate)
                        pdfStream.CopyTo(fs)
                        fs.Flush()
                    End Using
                End Using
            Else
                Dim col As New PdfImageCollection()
                Dim pages As Integer = RegisteredDecoders.GetImageInfo(New MemoryStream(imageData)).FrameCount
                ' Add all pages from a multipage TIFF.
                For i As Integer = 0 To pages - 1
                    col.Add(New PdfImage(New MemoryStream(imageData), i, PdfCompressionType.Auto))
                Next i
                ' Create the PDF.
                ' Set any properties.
                ' Make each image fit into an 8.5 x 11 inch page (612 x 792 @ 72 DPI).
                Dim pdf As New PdfEncoder With {
                    .JpegQuality = 85,
                    .Metadata = New PdfMetadata(warrantTitle, "Circuit Court", "Injuction File", "", "", "", objInjuction.CreatedDate, objInjuction.ReviewedDate),
                    .SizeMode = PdfPageSizeMode.FitToPage,
                    .PageSize = New Size(612, 792)
                }
                Using pdfStream As New MemoryStream()
                    pdf.Save(pdfStream, col, Nothing)
                    'You have to rewind the MemoryStream before copying
                    pdfStream.Seek(0, SeekOrigin.Begin)

                    Using fs As New FileStream(pdfFileName, FileMode.OpenOrCreate)
                        pdfStream.CopyTo(fs)
                        fs.Flush()
                    End Using
                End Using

            End If
            Dim pdfFileInfo As New FileInfo(pdfFileName)
            Return pdfFileInfo.Exists
        End Function

        Public Shared Function InsertFile(objInjunction As InjunctionsInfo, pdfFileName As String) As Boolean
            Dim ctl As New Controller
            Dim fileType As InjunctionFileType = InjunctionFileType.pdf
            Dim yearDirectoryName As String = pdfFileName + objInjunction.CreatedDate.Year.ToString
            Dim yearDirectoryInfo As New DirectoryInfo(yearDirectoryName)
            If Not yearDirectoryInfo.Exists Then
                yearDirectoryInfo.Create()
            End If

            pdfFileName += objInjunction.CreatedDate.Year.ToString + "\" + objInjunction.InjunctionId.ToString + ".pdf"
            Dim pdfInfo As New FileInfo(pdfFileName)
            If pdfInfo.Exists Then
                Try
                    pdfInfo.OpenRead()
                    Dim imagedata As Byte() = GetImageBytes(pdfInfo.OpenRead())
                    Dim fileId = ctl.InsertFile(imagedata, objInjunction.CreatedByUserId, False, fileType)
                    ctl.UpdateInjunctionFileId(objInjunction.InjunctionId, fileId)
                    Return True
                Catch ex As Exception
                    LogException(ex)
                    Return False
                End Try
            Else
                Return False
            End If

        End Function

        Private Shared Function GetImageBytes(ByVal stream As Stream) As Byte()
            Dim buffer As Byte()
            Dim encrypt As New Encryptor
            Dim filesize = stream.Length
            buffer = New Byte(stream.Length - 1) {}
            stream.Read(buffer, 0, filesize)
            Return encrypt.EncryptStream(buffer)
        End Function

        Public Shared Sub SaveChanges(ByVal image As AtalaImage, ByVal path As String, ByVal frame As Integer)
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim info As ImageInfo = RegisteredDecoders.GetImageInfo(fs)
                fs.Position = 0
                If info.ImageType = ImageType.Tiff Then
                    EditTiff(image, fs, frame, path)
                Else
                    EditPDF(image, fs, frame, path)
                End If
            End Using

            File.Delete(path)
            File.Move(path & "_tmp", path)
        End Sub

        Public Shared Sub EditTiff(ByVal image As AtalaImage, ByVal fs As Stream, ByVal frame As Integer, ByVal path As String)
            Dim tDoc As New TiffDocument(fs, True)
            Dim tPage As New TiffPage(image)
            tDoc.Pages.RemoveAt(frame)
            tDoc.Pages.Insert(frame, tPage)
            tDoc.Save(path & "_tmp")
        End Sub

        Public Shared Sub EditPDF(ByVal image As AtalaImage, ByVal fs As FileStream, ByVal frame As Integer, ByVal path As String)
            Dim ms As MemoryStream = New MemoryStream()
            image.Save(ms, New PdfEncoder(), Nothing)
            Dim newDoc As PdfDocument = New PdfDocument(ms)
            Dim pDoc As PdfDocument = Nothing
            Try
                pDoc = New PdfDocument(Nothing, Nothing, fs, Nothing, New Repair.RepairOptions())
            Catch ex As PdfException
                Try
                    Dim opts As Repair.RepairOptions = New Repair.RepairOptions()
                    opts.StructureOptions.ForceRebuildCrossReferenceTable = True
                    opts.StructureOptions.RestoreOrphanedPages = False
                    pDoc = New PdfDocument(Nothing, Nothing, fs, Nothing, opts)
                    pDoc.AllowSavingOfPreviouslySignedDocuments=true
                Catch ex2 As PdfException
                    ProcessPageLoadException(ex2)
                End Try
            End Try
            pDoc.Pages.RemoveAt(frame)
            pDoc.Pages.Insert(frame, newDoc.Pages(0))
            pDoc.Save(path & "_tmp")
        End Sub

    End Class
End Namespace