Imports System.IO
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Tiff
Imports Atalasoft.PdfDoc
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.Codec.Pdf

Namespace AWS.Modules.Warrants
    ''' <summary>
    ''' This class shows how to remove or insert pages in a
    ''' PDF or Tiff.
    ''' </summary>
    Public Class PageManipulation
        Public Sub New()
        End Sub

        Public Shared Sub InsertPageAndSave(ByVal path As String, ByVal frame As Integer)
            Dim newImage As AtalaImage = New AtalaImage(2550, 3263, PixelFormat.Pixel1bppIndexed)
            newImage.Resolution = New Dpi(200, 200, ResolutionUnit.DotsPerInch)

            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                '                 We're assuming only two file types are being used in 
                '                 * this application, TIF and PDF. 
                Dim info As ImageInfo = RegisteredDecoders.GetImageInfo(fs)
                fs.Position = 0

                If info.ImageType = ImageType.Tiff Then

                    Dim tDoc As TiffDocument = New TiffDocument(fs)
                    If tDoc.Pages.Count = frame Then
                        tDoc.Pages.Add(New TiffPage(newImage))
                    Else
                        tDoc.Pages.Insert(frame, New TiffPage(newImage))
                    End If
                    tDoc.Save(path & "_tmp")
                Else
                    Dim pDoc As PdfDocument = New PdfDocument(fs)
                    Dim ms As MemoryStream = New MemoryStream()
                    newImage.Save(ms, New PdfEncoder(), Nothing)
                    Dim newDoc As PdfDocument = New PdfDocument(ms)

                    If pDoc.Pages.Count = frame Then
                        pDoc.Pages.Add(newDoc.Pages(0))
                    Else
                        pDoc.Pages.Insert(frame, newDoc.Pages(0))
                    End If
                    pDoc.Save(path & "_tmp")
                End If
            End Using
            File.Delete(path)
            File.Move(path & "_tmp", path)

        End Sub

        Public Shared Sub RemovePageAndSave(ByVal path As String, ByVal frame As Integer)
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                '                 We're assuming only two file types are being used in 
                '                 * this application, TIF and PDF. 

                Dim info As ImageInfo = RegisteredDecoders.GetImageInfo(fs)
                fs.Position = 0
                If info.ImageType = ImageType.Tiff Then
                    Dim tDoc As New TiffDocument(fs)
                    tDoc.Pages.RemoveAt(frame)
                    tDoc.Save(path & "_tmp")
                Else
                    Dim pDoc As PdfDocument = New PdfDocument(fs)
                    pDoc.Pages.RemoveAt(frame)
                    pDoc.Save(path + "_tmp")
                End If
            End Using

            File.Delete(path)
            File.Move(path & "_tmp", path)

        End Sub

    End Class
End Namespace