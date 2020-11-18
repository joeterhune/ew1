Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.IO
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Tiff
Imports Atalasoft.PdfDoc

Namespace AWS.Modules.Warrants

    Public Class ReorderThumbnails
        Public Sub New()
        End Sub

        Public Shared Sub Reorder(ByVal path As String, ByVal DragIndex As Integer, ByVal DropIndex As Integer)
            Using fs As Stream = New FileStream(path, FileMode.Open, FileAccess.Read)
                Dim info As ImageInfo = RegisteredDecoders.GetImageInfo(fs)
                fs.Position = 0

                SwapTiff(fs, path, DragIndex, DropIndex)
               
            End Using

            File.Delete(path)
            File.Move(path & "_tmp", path)
        End Sub

        Private Shared Sub SwapTiff(ByVal fs As Stream, ByVal path As String, ByVal DragIndex As Integer, ByVal DropIndex As Integer)
            Dim tDoc As New TiffDocument(fs)
            Dim tPage As TiffPage = tDoc.Pages(DragIndex)
            tDoc.Pages.RemoveAt(DragIndex)
            tDoc.Pages.Insert(DropIndex, tPage)
            tDoc.Save(path & "_tmp")
        End Sub

    End Class
End Namespace