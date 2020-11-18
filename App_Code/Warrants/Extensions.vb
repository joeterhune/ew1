Imports System.Runtime.CompilerServices
Imports Atalasoft.Imaging.Codec

Namespace AWS.Modules.Warrants
    Public Module Extensions

        <Extension()>
        Function EnumDisplayNameFor(ByVal item As [Enum]) As String
            Dim type = item.[GetType]()
            Dim member = type.GetMember(item.ToString())
            Dim displayName As DisplayAttribute = CType(member(0).GetCustomAttributes(GetType(DisplayAttribute), False).FirstOrDefault(), DisplayAttribute)
            If displayName IsNot Nothing Then
                Return displayName.Name.ToString()
            End If

            Return item.ToString()
        End Function

        Sub AddPdfDecoder()
            Dim pdfRegistered As Boolean = False

            For Each decoder As ImageDecoder In RegisteredDecoders.Decoders
                Dim type = decoder.GetType()
                If type.Name.ToLower = "pdfdecoder" Then
                    pdfRegistered = True
                End If
            Next
            If Not pdfRegistered Then
                RegisteredDecoders.Decoders.Add(New Atalasoft.Imaging.Codec.Pdf.PdfDecoder())
            End If

        End Sub
    End Module

End Namespace
