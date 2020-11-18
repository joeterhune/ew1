<%@ WebHandler Language="VB" Class="AWS.Modules.Warrants.PDFViewer" %>
Imports System.IO
Imports Atalasoft.Imaging.Codec.Pdf
Imports Atalasoft.Imaging.Codec
Namespace AWS.Modules.Warrants
    Public Class PDFViewer
        Implements IHttpHandler

        Private ctl As New Controller
        Private objuser As UserInfo
        Private insertJavaScript As Boolean = False
        Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
            Try
                Extensions.AddPdfDecoder

                Dim encryptedValue As String = ""
                If Not context.Request.QueryString("enc") Is Nothing Then
                    encryptedValue = context.Request.QueryString("enc")
                End If
                If encryptedValue = "" Then
                    Throw New ArgumentException("Invalid encryption string")
                End If
                objuser = UserController.Instance.GetCurrentUserInfo
                If Not objuser Is Nothing Then
                    Dim encrypt As New Encryptor
                    Dim fileId As String = encrypt.QueryStringDecode(encryptedValue, objuser.Username)
                    Dim objwarrant As WarrantsInfo = ctl.GetWarrantByFileId(fileId)
                    Dim fileExists As Integer = ctl.FileExists(fileId)
                    Dim warrantTitle As String = objwarrant.Title
                    If Not objwarrant Is Nothing Then
                        Dim objLog As New LogInfo With {
                            .UserId = objuser.UserID,
                            .EventTime = Now(),
                            .EventDescription = "Viewed PDF Read Only Mode",
                            .WarrantId = objwarrant.WarrantId,
                            .IP = GetIp(context)
                        }
                        ctl.AddEvent(objLog)
                        If objwarrant.StatusId = WarrantStatus.Reviewed Then
                            insertJavaScript = True
                        End If
                        If objwarrant.StatusId = WarrantStatus.Rejected Or objwarrant.StatusId = WarrantStatus.Singed Then
                            If Not IsJudge(objuser.UserID) Then
                                objwarrant.StatusId = WarrantStatus.Reviewed
                                objwarrant.ReviewedByAgencyUserId = objuser.UserID
                                objwarrant.ReviewedByAgencyDate = Now
                                ctl.UpdateWarrantsReviewed(objwarrant)
                                insertJavaScript = True
                            End If
                        End If
                    Else
                        Throw New Exception("Unassociated warrant file")
                    End If
                    context.Response.ClearContent()
                    context.Response.Clear()
                    If fileExists > 0 Then
                        context.Response.ContentType = "application/pdf"
                        context.Response.AddHeader("content-disposition", "inline; filename=Warrant.pdf")
                        Dim col As New PdfImageCollection()
                        Dim warrantFile As WarrantImage = GetWarrantFile(objwarrant.fileid)
                        Dim imageData As Byte() = GetImage(warrantFile)
                        If warrantFile.FileType = WarrantFileType.pdf Then
                            context.Response.BinaryWrite(imageData)
                        Else
                            Dim pages As Integer = RegisteredDecoders.GetImageInfo(New MemoryStream(imageData)).FrameCount
                            ' Add all pages from a multipage TIFF.
                            For i As Integer = 0 To pages - 1
                                col.Add(New PdfImage(New MemoryStream(imageData), i, PdfCompressionType.Auto))
                            Next i
                            ' Create the PDF.
                            Dim pdf As New PdfEncoder With {
                                .JpegQuality = 85,
                                .Metadata = New PdfMetadata(warrantTitle, "Circuit Court", "Warrant File", "", "", "", objwarrant.CreatedDate, objwarrant.ReviewedDate),
                                .SizeMode = PdfPageSizeMode.FitToPage,
                                .PageSize = New Size(612, 792)
                            }
                            Using pdfStream As New MemoryStream()
                                pdf.Save(pdfStream, col, Nothing)
                                pdfStream.Seek(0, SeekOrigin.Begin)
                                Dim pdfBytes(pdfStream.Length - 1) As Byte
                                pdfStream.Read(pdfBytes, 0, CInt(pdfStream.Length))
                                context.Response.BinaryWrite(pdfBytes)
                            End Using
                        End If
                    Else
                        Throw New Exception("File not Found in Archive")
                    End If

                End If

            Catch ex As Exception
                context.Response.Write("<html><body><h1>Error Processing File</h2><p>An error occurred processing the requested file.</p><div style='color:red'>")
                context.Response.Write(ex.Message)
                LogException(ex)
                context.Response.Write("</div></body></html>")

            Finally
                context.Response.Flush()
                context.Response.[End]()
            End Try
        End Sub
        Private function GetWarrantFile(ByVal id As integer) As WarrantImage
            Return ctl.GetFile(id)
        End function

        Private Function GetImage(image As warrantImage) As Byte()
            Dim encrypt As New Encryptor
            Dim imageData As Byte() = DirectCast(encrypt.DecryptStream(image.Bytes), Byte())
            Return imageData

        End Function
        Private Function GetIp(context As HttpContext) As String
            Dim clientIp As String = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If Not String.IsNullOrEmpty(clientIp) Then
                Dim forwardedIps As String() = clientIp.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                clientIp = forwardedIps(forwardedIps.Length - 1)
            Else
                clientIp = context.Request.ServerVariables("REMOTE_ADDR")
            End If
            Return clientIp
        End Function

        Private Function IsJudge(userId As Integer) As Boolean
            Dim objjudge As JudgeInfo = ctl.GetJudge(userId)
            If objjudge Is Nothing Then
                Return False
            Else
                Return True
            End If
            Return False
        End Function

        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

    End Class
End Namespace
