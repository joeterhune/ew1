<%@ WebHandler Language="VB" Class="AWS.Modules.Warrants.WarrantHandler" %>

Imports System
Imports System.Web
Imports Telerik.Web.UI
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Data.SqlClient
Imports System.IO

Namespace AWS.Modules.Warrants


    Public Class WarrantHandler
        Inherits AsyncUploadHandler
        Implements System.Web.SessionState.IRequiresSessionState

        Protected Overrides Function Process(ByVal file As UploadedFile, ByVal context As HttpContext, ByVal configuration As IAsyncUploadConfiguration, ByVal tempFileName As String) As IAsyncUploadResult

            Dim result As UploadResult = CreateDefaultUploadResult(Of UploadResult)(file)
            Dim userID As Integer = -1
            Dim persist As Boolean = False
            Dim upConfiguration As UploadConfiguration = TryCast(configuration, UploadConfiguration)
            If upConfiguration IsNot Nothing Then
                userID = upConfiguration.UserID
                persist = upConfiguration.Persist
            End If

            result.FileID = InsertFile(file, userID, persist)

            Return result
        End Function

        Public Function InsertFile(ByVal file As UploadedFile, ByVal userID As Integer, persist As Boolean) As Integer
            Dim ctl As New Controller
            Dim fileType As WarrantFileType
            Dim imageData As Byte() = GetImageBytes(file.InputStream)
            Dim fileId As Integer = Null.NullInteger
            Select Case file.GetExtension.ToLower
                Case ".pdf"
                    fileType = WarrantFileType.pdf
                Case ".tiff"
                    fileType = WarrantFileType.tiff
                Case ".tif"
                    fileType = WarrantFileType.tiff
                Case Else
                    fileType = WarrantFileType.nonwarrant
            End Select
            fileId = ctl.InsertFile(imageData, userID, persist, filetype)
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