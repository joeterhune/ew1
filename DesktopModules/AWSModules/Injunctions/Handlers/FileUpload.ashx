<%@ WebHandler Language="VB" Class="AWS.Modules.Injunctions.FileHandler" %>

Imports System.IO

Namespace AWS.Modules.Injunctions

    Public Class FileHandler
        Implements IHttpHandler

        Private objuser As UserInfo
        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            If context.Request.Files.Count > 0 Then
                objuser = UserController.Instance.GetCurrentUserInfo
                Dim files As HttpFileCollection = context.Request.Files
                Dim file As HttpPostedFile = files(0)
                Dim filename As String = file.FileName
                Dim fileId As Integer = 0
                Try
                    fileId = InsertFile(file, filename, objuser.UserID, False)
                Catch ex As Exception
                    LogException(ex)
                End Try
                context.Response.ContentType = "text/plain"
                context.Response.Write(fileId.ToString)
            End If
        End Sub
        Public Function InsertFile(ByVal file As HttpPostedFile, filename As String, ByVal userID As Integer, persist As Boolean) As Integer
            Dim ctl As New Controller
            Dim fileType As InjunctionFileType
            Dim imageData As Byte() = GetImageBytes(file.InputStream)
            Dim fileId As Integer = Null.NullInteger
            Dim extension As String = Path.GetExtension(filename)

            Select Case extension.ToLower
                Case ".pdf"
                    fileType = InjunctionFileType.pdf
                Case ".tiff"
                    fileType = InjunctionFileType.tiff
                Case ".tif"
                    fileType = InjunctionFileType.tiff
                Case Else
                    fileType = InjunctionFileType.nonwarrant
            End Select
            If fileType = InjunctionFileType.nonwarrant Then
                Return 0
            End If

            fileId = ctl.InsertFile(imageData, userID, persist, fileType)
            Return fileId
        End Function

        Public Function GetImageBytes(ByVal stream As Stream) As Byte()
            Dim buffer As Byte()
            Dim encrypt As New Encryptor
            Dim filesize = stream.Length
            buffer = New Byte(stream.Length - 1) {}
            stream.Read(buffer, 0, filesize)
            Return encrypt.EncryptStream(buffer)
        End Function

    End Class
End Namespace