<%@ WebHandler Language="VB" Class="AWS.Modules.Injunctions.PDFViewer" %>

Imports System.IO
Imports Atalasoft.Imaging.Codec.Pdf
Imports Atalasoft.Imaging.Codec

Namespace AWS.Modules.Injunctions
    Public Class PDFViewer
        Implements IHttpHandler

        Private ReadOnly archiveDirectory As String = ConfigurationManager.AppSettings("CompletedInjunctions")
        Private ctl As New Controller
        Private objuser As UserInfo

        Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
            Try
                Extensions.AddPdfDecoder
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
                    Dim objInjunction As InjunctionsInfo = ctl.GetInjunctionByFileId(fileId)
                    Dim fileExists As Integer = ctl.FileExists(fileId)
                    Dim docTitle As String = objInjunction.Title
                    If Not objInjunction Is Nothing Then
                        Dim objLog As New LogInfo With {
                            .UserId = objuser.UserID,
                            .EventTime = Now(),
                            .EventDescription = "Viewed PDF Read Only Mode",
                            .injunctionId = objInjunction.InjunctionId,
                            .IP = GetIp(context)
                        }
                        ctl.AddEvent(objLog)
                    Else
                        Throw New Exception("Unassociated Injunction file")
                    End If

                    context.Response.ClearContent()
                    context.Response.Clear()
                    If fileExists > 0 Then
                        context.Response.ContentType = "application/pdf"
                        context.Response.AddHeader("content-disposition", "inline; filename=Warrant.pdf")
                        Dim col As New PdfImageCollection()
                        Dim injuctionFile As InjunctionImage = GetInjuctionFile(objInjunction.fileid)
                        Dim imageData As Byte() = GetImage(injuctionFile)
                        If injuctionFile.FileType = InjunctionFileType.pdf Then
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
                                .Metadata = New PdfMetadata(docTitle, "Circuit Court", "Injuction File", "", "", "", objInjunction.CreatedDate, objInjunction.ReviewedDate),
                                .SizeMode = PdfPageSizeMode.FitToPage,
                                .PageSize = New Size(612, 792)
                            }
                            ' Set any properties.
                            Using pdfStream As New MemoryStream()
                                pdf.Save(pdfStream, col, Nothing)
                                pdfStream.Seek(0, SeekOrigin.Begin)
                                Dim pdfBytes(pdfStream.Length - 1) As Byte
                                pdfStream.Read(pdfBytes, 0, CInt(pdfStream.Length))
                                context.Response.BinaryWrite(pdfBytes)
                            End Using

                        End If
                    Else
                        Dim pdfFileName As String = objInjunction.InjunctionId.ToString & ".pdf"
                        Dim pdfFilePath As String = archiveDirectory & objInjunction.CreatedDate.Year.ToString & "\" & pdfFileName
                        Dim pdfFileInfo As New FileInfo(pdfFilePath)
                        If pdfFileInfo.Exists Then
                            context.Response.ContentType = "application/pdf"
                            Dim headerValue = If((context.Request.UserAgent.ToLower().Contains("msie")), String.Format("inline; filename =""{0}""", Uri.EscapeDataString(pdfFileName)), String.Format("inline; filename =""{0}""", pdfFileName))
                            context.Response.AddHeader("Content-Disposition", headerValue + ";")
                            context.Response.TransmitFile(pdfFilePath)
                        Else
                            Throw New Exception("File not Found in Archive")
                        End if
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

        Private function GetInjuctionFile(ByVal id As integer) As InjunctionImage
            Return ctl.GetFile(id)
        End function

        Private Function GetImage(image As InjunctionImage) As Byte()
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
