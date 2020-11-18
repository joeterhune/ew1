Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports Atalasoft.PdfDoc
Imports Atalasoft.PdfDoc.Repair

Namespace AWS.Modules.Warrants
  public  Module PdfDocCombineWithRepair
        Sub Combine(ByVal outFile As String, ByVal inFiles As String(), ByVal repairOpts As RepairOptions)
            Combine(Nothing, Nothing, outFile, inFiles, repairOpts)
        End Sub

        Sub Combine(ByVal userPass As String, ByVal ownerPass As String, ByVal outFile As String, ByVal inFiles As String(), ByVal repairOpts As RepairOptions)
            If inFiles IsNot Nothing AndAlso inFiles.Length > 0 Then
                Dim streams As List(Of Stream) = New List(Of Stream)()

                For Each fil As String In inFiles
                    streams.Add(New FileStream(fil, FileMode.Open, FileAccess.Read, FileShare.Read))
                Next

                Dim outStream As FileStream = New FileStream(outFile, FileMode.Create)
                Combine(userPass, ownerPass, outStream, streams.ToArray(), repairOpts)
                outStream.Close()
                outStream.Dispose()

                For Each stm As Stream In streams
                    stm.Close()
                    stm.Dispose()
                Next
            End If
        End Sub

        Sub Combine(ByVal outStream As Stream, ByVal streams As Stream(), ByVal repairOpts As RepairOptions)
            Combine(Nothing, Nothing, outStream, streams, repairOpts)
        End Sub

        Sub Combine(ByVal userPass As String, ByVal ownerPass As String, ByVal outStream As Stream, ByVal streams As Stream(), ByVal repairOpts As RepairOptions)
            Dim docs As List(Of PdfDocument) = New List(Of PdfDocument)()

            For Each stm As Stream In streams
                stm.Seek(0, SeekOrigin.Begin)
                docs.Add(New PdfDocument(userPass, ownerPass, stm, Nothing, repairOpts))
            Next

            Dim outDoc As PdfDocument = New PdfDocument()

            For Each inDoc As PdfDocument In docs

                For Each page As PdfPage In inDoc.Pages
                    outDoc.Pages.Add(page)
                Next
            Next

            outStream.Seek(0, SeekOrigin.Begin)
            outDoc.Save(outStream)
            outDoc.Close()

            For Each inDoc As PdfDocument In docs
                inDoc.Close()
            Next
        End Sub
    End Module
End Namespace
