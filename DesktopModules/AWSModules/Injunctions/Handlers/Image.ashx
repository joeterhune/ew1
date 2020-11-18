<%@ WebHandler Language="VB" Class="AWS.Modules.Injunctions.SignatureImage" %>

Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports System.Drawing.Imaging


Namespace AWS.Modules.Injunctions
    Public Class SignatureImage
        Implements IHttpHandler
        Private encryptor As New Encryptor
        Private ctl As New Controller
        Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
            Try
                Dim encryptedValue As String = ""
                If Not context.Request.QueryString("enc") Is Nothing Then
                    encryptedValue = context.Request.QueryString("enc")
                End If
                If encryptedValue = "" Then
                    Throw New ArgumentException("Invalid encryption string")
                End If
                Dim objuser As UserInfo = UserController.Instance.GetCurrentUserInfo
                Dim fileId As String = encryptor.QueryStringDecode(encryptedValue, objuser.Username)
                Dim imageData As Byte() = GetImage(fileId)
                context.Response.ContentType = "image/png"
                context.Response.BinaryWrite(imageData)
                context.Response.Flush()

            Catch ex As Exception

                context.Response.[End]()
            End Try
        End Sub

        Private Function GetImage(ByVal id As Integer) As Byte()
            Dim objInjunctionImage As InjunctionImage = ctl.GetFile(id)

            Dim imageData As Byte() = DirectCast(encryptor.DecryptStream(objInjunctionImage.Bytes), Byte())
            Return imageData

        End Function

        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

    End Class
End Namespace
