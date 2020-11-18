Imports System.IO
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Tiff
Imports Atalasoft.PdfDoc
Imports Atalasoft.Imaging

Namespace AWS.Modules.Injunctions
    ''' <summary>
    ''' This class shows how to remove or insert pages in a
    ''' PDF or Tiff.
    ''' </summary>
    Public Class PageManipulation
        Public Sub New()
        End Sub

        Public Shared Sub InsertPageAndSave(ByVal path As String, ByVal frame As Integer)
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                '                 We're assuming only two file types are being used in 
                '                 * this application, TIF and PDF. 
                Dim info As ImageInfo = RegisteredDecoders.GetImageInfo(fs)
                fs.Position = 0
                Dim newImage As AtalaImage = New AtalaImage(2550, 3263, PixelFormat.Pixel1bppIndexed)
                newImage.Resolution = New Dpi(200, 200, ResolutionUnit.DotsPerInch)
                Dim tdoc As New TiffDocument(fs)
                If tdoc.Pages.Count = frame Then
                    tdoc.Pages.Add(New TiffPage(newImage, TiffCompression.Group4FaxEncoding))
                Else
                    tdoc.Pages.Insert(frame, New TiffPage(newImage, TiffCompression.Group4FaxEncoding))
                End If
                tdoc.Save(path & "_tmp")
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

                Dim tDoc As New TiffDocument(fs)
                tDoc.Pages.RemoveAt(frame)
                tDoc.Save(path & "_tmp")


            End Using

            File.Delete(path)
            File.Move(path & "_tmp", path)

        End Sub

        Public Sub InsertTiffPage(ByVal index As Integer, ByVal image As AtalaImage, ByVal fileName As String)
            Dim fs As FileStream = New FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)

            ' Validate the file.
            Dim decoder As TiffDecoder = New TiffDecoder()
            If Not decoder.IsValidFormat(fs) Then
                Throw New ArgumentException("The filename provided is not a valid TIFF file.")
            End If

            fs.Seek(0, SeekOrigin.Begin)

            ' Validate and correct the index.
            Dim count As Integer = decoder.GetFrameCount(fs)
            If index > count Then index = count
            If index < 0 Then index = 0

            ' Save the new image to a temp file.
            Dim NewFile As String = System.IO.Path.GetTempFileName()
            Dim NewFs As FileStream = New FileStream(NewFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None)

            ' Loop through the pages and insert the new page.
            count = count + 1
            Dim readIndex As Integer = 0
            Dim encoder As TiffEncoder = New TiffEncoder()

            Dim i As Integer
            For i = 0 To count - 1 Step i + 1
                Dim img As AtalaImage = Nothing
                If i = index Then
                    img = image
                Else
                    fs.Seek(0, SeekOrigin.Begin)
                    img = decoder.Read(fs, readIndex, Nothing)
                    readIndex = readIndex + 1
                End If

                ' Set the compression.
                If img.PixelFormat = PixelFormat.Pixel1bppIndexed Then
                    encoder.Compression = TiffCompression.Group4FaxEncoding
                Else
                    encoder.Compression = TiffCompression.Lzw
                End If

                NewFs.Seek(0, SeekOrigin.Begin)
                encoder.Save(NewFs, img, Nothing)

                ' All pages except the first must be appended.
                encoder.Append = True
            Next

            fs.Close()
            NewFs.Close()

            ' Copy the temporary image to replace the old one.
            File.Copy(NewFile, fileName, True)

            ' Delete the temp file.
            File.Delete(NewFile)
        End Sub
    End Class
End Namespace