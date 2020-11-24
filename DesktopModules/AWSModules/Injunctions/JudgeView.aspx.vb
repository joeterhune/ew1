Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging.WebControls
Imports Atalasoft.Annotate.UI
Imports System.IO
Imports Atalasoft.Annotate
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Pdf
Imports DotNetNuke

Namespace AWS.Modules.Injunctions


    Partial Class JudgeView
        Inherits System.Web.UI.Page

#Region "Properties"
        Private _injunctionId As String = ""
        Private _judgeInfo As JudgeInfo
        Private _tabModuleId As Integer = Null.NullInteger
        Private _moduleId As Integer = Null.NullInteger
        Private _judgeRole As String
        Private _adminJudge As String
        Private _ctl As New Controller
        Private _userInfo As UserInfo
        Private _encryptor As New Encryptor

        Public ReadOnly Property NavigateUrl() As String
            Get

                Dim portal As PortalSettings = GetPortalSettings()
                Return DotNetNuke.Common.Globals.NavigateURL(portal.HomeTabId, "annotations", "mid=" & _moduleId)
            End Get

        End Property

        Public Property UrlReferrer() As String
            Get
                If Not ViewState("UrlReferrer") Is Nothing Then
                    Return ViewState("UrlReferrer")
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                ViewState("UrlReferrer") = value
            End Set
        End Property

        Public ReadOnly Property ImageCache() As String
            Get
                Return System.Web.Configuration.WebConfigurationManager.AppSettings("AtalasoftWebControls_Cache")
            End Get

        End Property

        Private Property JudgeRole() As String
            Get
                Return _judgeRole
            End Get
            Set(value As String)
                _judgeRole = value
            End Set
        End Property

        Private Property AdminJudge As String
            Get
                Return _adminJudge
            End Get
            Set(value As String)
                _adminJudge = value
            End Set
        End Property

        Private ReadOnly Property JudgeInfo As JudgeInfo
            Get
                If _judgeInfo Is Nothing Then
                    _judgeInfo = _ctl.GetJudge(UserInfo.UserID)
                End If
                Return _judgeInfo
            End Get

        End Property

        Private ReadOnly Property UserInfo As UserInfo
            Get
                If _userInfo Is Nothing Then
                    _userInfo = UserController.Instance.GetCurrentUserInfo()
                End If
                Return _userInfo
            End Get

        End Property

        Private ReadOnly Property IsJudge() As Boolean
            Get
                If AdminJudge <> "" Then
                    Dim judgeId As Integer = Null.NullInteger
                    Int32.TryParse(_encryptor.QueryStringDecode(JudgeInfo.Approved, AdminJudge), judgeId)
                    Return UserInfo.UserID = judgeId
                End If
                Return False
            End Get
        End Property
#End Region

#Region "Methods"

        Private Sub InitializeProperties()
            Dim objModuleController As New DotNetNuke.Entities.Modules.ModuleController
            Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = objModuleController.GetTabModule(_tabModuleId)
            Dim tempValue As Hashtable = objModule.TabModuleSettings
            JudgeRole = tempValue("JudgeRole").ToString
            AdminJudge = tempValue("AdminJudge").ToString

            _moduleId = objModule.ModuleID

        End Sub

        Public Function GetBytsForFile(filePath As String) As Byte()

            Dim fileByte As Byte()
            Using fileStream As FileStream = File.OpenRead(filePath)
                fileByte = New Byte(fileStream.Length - 1) {}
                fileStream.Read(fileByte, 0, fileStream.Length)
            End Using

            Return fileByte
        End Function

        Private Sub Convert(ByVal inFile As String, ByVal outFile As String, ByVal encoder As MultiFramedImageEncoder)

            Using src As ImageSource = New FileSystemImageSource(inFile, True)
                Using s As Stream = File.OpenWrite(outFile)
                    encoder.Save(s, src, Nothing)
                End Using
            End Using
        End Sub

        Private Sub PrepareAnnotationLayers()
            ' We need to make sure that there are the same number of layers as pages
            Dim frameCount As Integer = WebThumbnailViewer1.Count

            If WebAnnotationViewer1.Annotations.Layers.Count <> frameCount Then
                Do While WebAnnotationViewer1.Annotations.Layers.Count < frameCount
                    WebAnnotationViewer1.Annotations.Layers.Add(New LayerAnnotation())
                Loop

                Do While WebAnnotationViewer1.Annotations.Layers.Count > frameCount
                    WebAnnotationViewer1.Annotations.Layers.RemoveAt(WebAnnotationViewer1.Annotations.Layers.Count - 1)
                Loop

                WebAnnotationViewer1.UpdateAnnotations()
            End If
        End Sub

        Public Sub Reorder(ByVal DragIndex As Integer, ByVal DropIndex As Integer)
            'not really a swap. A remove and insert.
            Dim url As String = WebThumbnailViewer1.Url
            Dim path As String = Page.MapPath(url)

            ReorderThumbnails.Reorder(path, DragIndex, DropIndex)
        End Sub

        Private Sub CancelCheckout(redirect As Boolean)
            Try

                Dim objInjunction As InjunctionsInfo = _ctl.GetInjunctions(_injunctionId)
                If objInjunction.StatusId = InjunctionStatus.UnderReview Then
                    objInjunction.StatusId = InjunctionStatus.NewInjunction
                End If
                _ctl.UpdateInjunctions(objInjunction)

            Catch ex As Exception
                Response.Redirect("/")
            End Try
        End Sub

        Private Function GetIp() As String
            Dim clientIp As String = Page.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If Not String.IsNullOrEmpty(clientIp) Then
                Dim forwardedIps As String() = clientIp.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                clientIp = forwardedIps(forwardedIps.Length - 1)
            Else
                clientIp = Page.Request.ServerVariables("REMOTE_ADDR")
            End If
            Return clientIp
        End Function


#End Region

#Region "Event Handlers"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            Try
                HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(False)
                HttpContext.Current.Response.Cache.SetNoStore()
                Response.Cache.SetExpires(DateTime.Now)
                Response.Cache.SetValidUntilExpires(True)
                Response.Cache.AppendCacheExtension("no-cache")
                Response.Expires = 0
                Dim encryptedValue As String = ""
                If Not Request.QueryString("enc") Is Nothing Then
                    encryptedValue = Request.QueryString("enc")
                    _injunctionId = _encryptor.QueryStringDecode(encryptedValue, UserInfo.Username)
                Else
                    Throw New ArgumentException("The Injunction File Could not be Retrieved from the Database")
                End If
                If Not Request.QueryString("ModuleId") Is Nothing Then
                    _tabModuleId = Int32.Parse(Request.QueryString("ModuleId"))
                End If
                InitializeProperties()
                ' Put user code to initialize the page here
                If Not Page.IsPostBack Then
                    UrlReferrer = Request.UrlReferrer.AbsoluteUri
                    If Not IsJudge Then
                        Response.Redirect(UrlReferrer)
                    End If
                    If IsNumeric(_injunctionId) Then
                        Dim objInjunction As InjunctionsInfo = _ctl.GetInjunctions(_injunctionId)
                        If Not objInjunction Is Nothing Then
                            If objInjunction.StatusId = InjunctionStatus.Rejected Then
                                container.Visible = False
                                phMessage.Controls.Add(DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("Notice", "The requested Injunction has already been reviewed and rejected.  <a href='/'>Return to the Injunctions list</a> and click the refresh link to update the Injunctions available for signing", Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo))
                            End If
                            If objInjunction.StatusId = InjunctionStatus.Signed Then
                                container.Visible = False
                                phMessage.Controls.Add(DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("Notice", "The requested Injunction has already been reviewed and signed.  <a href='/'>Return to the Injunctions list</a> and click the refresh link to update the Injunctions available for signing", Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo))
                            End If

                            Dim objLog As New LogInfo With {
                                .UserId = UserInfo.UserID,
                                .EventTime = Now(),
                                .EventDescription = "Viewed in Edit Mode",
                                .InjunctionId = objInjunction.InjunctionId,
                                .IP = GetIp()
                            }
                            _ctl.AddEvent(objLog)
                            Extensions.AddPdfDecoder()
                            Dim objInjunctionFile As InjunctionImage = _ctl.GetFile(objInjunction.FileId)
                            If Not objInjunctionFile Is Nothing Then
                                Dim buffer As Byte() = _encryptor.DecryptStream(objInjunctionFile.Bytes)
                                Using fileData As New MemoryStream(buffer)
                                    WebThumbnailViewer1.Open(fileData)
                                    PrepareAnnotationLayers()
                                    WebAnnotationViewer1.AutoZoom = AutoZoomMode.FitToWidth
                                    WebThumbnailViewer1.SelectedIndex = 0
                                End Using
                                Dim imageWidth As Integer = WebAnnotationViewer1.Image.Width
                                InitializeDefaultAnnotations(JudgeInfo, UserInfo.Username, imageWidth)
                            Else
                                Throw New ArgumentException("The Injunction File Could not be Retrieved from the Database")
                            End If
                        Else
                            Throw New ArgumentException("Injunction Information Invalid")
                        End If
                    Else
                        Throw New ArgumentException("Injunction Information Invalid")
                    End If
                    ' Prepare and open the default image file
                End If
                If hdtoCancel.Value = "false" Then
                    WebAnnotationViewer1.ClearAnnotations()
                    CancelCheckout(False)
                End If
            Catch ex As Exception
                phMessage.Controls.Add(DotNetNuke.UI.Skins.Skin.GetModuleMessageControl("Error", "An error has occurred processing the file.<br /> Refresh the Page and try again", Skins.Controls.ModuleMessage.ModuleMessageType.RedError))
                DotNetNuke.Services.Exceptions.LogException(ex)
                CancelCheckout(False)
            End Try
        End Sub

        Protected Sub WebThumbnailViewer1_ThumbnailDrop(ByVal sender As Object, ByVal e As Atalasoft.Imaging.WebControls.ThumbnailDropEventArgs)
            ' Call me from the OnThumbnailDragDrop event of the WTV
            Dim dgi As Integer = e.DragIndex
            Dim dpi As Integer = e.DropIndex
            Reorder(dgi, dpi)
        End Sub

#End Region

#Region "Remote Calls"

        <RemoteInvokable>
        Public Function ApproveInjunction(enc As String) As String
            Dim returnString As String = "2|default"
            Try
                Dim frame As Integer = WebAnnotationViewer1.CurrentPage - 1
                Dim url As String = WebAnnotationViewer1.ImageUrl
                Dim path As String = Page.MapPath(url)
                ' This assumes that you used OpenUrl on a relative file path, with a frame index
                Dim imgPath As String = Page.MapPath(WebAnnotationViewer1.ImageUrl)
                Dim info As ImageInfo = Atalasoft.Imaging.Codec.RegisteredDecoders.GetImageInfo(imgPath)
                If UserInfo.Username = "" Then
                    returnString = "0|Your session has expired. Please log in and try again."
                    Return returnString
                End If
                _injunctionId = _encryptor.QueryStringDecode(enc, UserInfo.Username)
                Dim objInjunction As InjunctionsInfo = _ctl.GetInjunctions(_injunctionId)
                If Not objInjunction Is Nothing Then
                    If Not objInjunction.IsMarked Then
                        InsertDocumentId(objInjunction.InjunctionId)
                    End If
                    objInjunction.StatusId = InjunctionStatus.Signed
                    objInjunction.IsMarked = True
                    objInjunction.ReviewedByJudgeUserId = UserInfo.UserID
                    Dim InjunctionTitle As String = objInjunction.Title
                    objInjunction.ReviewedDate = Now()
                    _ctl.UpdateInjunctions(objInjunction)
                    If info.FrameCount = WebAnnotationViewer1.Annotations.Layers.Count Then
                        For fi As Integer = 0 To info.FrameCount - 1
                            Using page As New AtalaImage(imgPath, fi, Nothing)
                                Annotations.BurnAnnotations(page, WebAnnotationViewer1.Annotations.Layers(fi).Items, path, fi)
                                WebAnnotationViewer1.Annotations.Layers(fi).Items.Clear()
                            End Using
                        Next fi
                    End If
                    WebAnnotationViewer1.ClearAnnotations()
                    If ImageProcessing.CreatePDF(GetBytsForFile(path), InjunctionTitle, objInjunction, _ctl) Then
                        _ctl.DeleteFile(objInjunction.FileId)
                    Else
                        Dim encryptedBytes As Byte() = _encryptor.EncryptStream(GetBytsForFile(path))
                        _ctl.UpdateFile(objInjunction.FileId, encryptedBytes)
                    End If

                    Try
                        Dim oldImage As New FileInfo(imgPath)
                        If oldImage.Exists Then
                            Try
                                oldImage.Delete()
                            Catch

                            End Try
                        End If
                        Dim portalId As Integer = 0
                        Dim portalSettings As PortalSettings = GetPortalSettings()
                        If Not portalSettings Is Nothing Then
                            portalId = portalSettings.PortalId
                        End If
                        Dim fromaddress As String = PortalSettings.Email
                        Dim toaddress As String = UserController.GetUserById(portalId, objInjunction.CreatedByUserId).Email
                        Dim subject As String = "Injunction Submission Accepted"

                        Dim body As String = "The Injunction submission [" & InjunctionTitle & "] has been signed."
                        Services.Mail.Mail.SendEmail(fromaddress, toaddress, subject, body)
                    Catch ex As Exception
                        LogException(ex)
                        returnString += "0|The Injunction was approved, but an error occurred sending the email messages."
                    End Try
                Else
                    WebAnnotationViewer1.ClearAnnotations()
                    returnString = "0|Injunction could not be found. Please try again."
                End If

            Catch ex As Exception
                WebAnnotationViewer1.ClearAnnotations()
                returnString = "-1|" + ex.Message + "|" + ex.StackTrace.ToString()
                LogException(ex)
            End Try
            Return returnString
        End Function

        <RemoteInvokable>
        Public Function CancelInjunction(enc As String) As String
            Dim returnString As String = "2|default"
            Try
                WebAnnotationViewer1.ClearAnnotations()
                If UserInfo.Username = "" Then
                    returnString = "0|Your session has expired. Please log in and try again."
                    Return returnString
                End If
                _injunctionId = _encryptor.QueryStringDecode(enc, UserInfo.Username)
                CancelCheckout(True)
            Catch ex As Exception
                WebAnnotationViewer1.ClearAnnotations()
                returnString = "-1|" + ex.Message + "|" + ex.StackTrace.ToString()
                LogException(ex)

            End Try
            Return returnString
        End Function

        <RemoteInvokable>
        Public Function RejectInjunction(enc As String, rejecttest As String) As String
            Dim returnString As String = "2|default"
            Try
                Dim frame As Integer = WebAnnotationViewer1.CurrentPage - 1
                Dim url As String = WebAnnotationViewer1.ImageUrl
                Dim path As String = Page.MapPath(url)
                If UserInfo.Username = "" Then
                    returnString = "0|Your session has expired. Please log in and try again."
                    Return returnString
                End If
                Dim imgPath As String = Page.MapPath(WebAnnotationViewer1.ImageUrl)
                Dim info As ImageInfo = Atalasoft.Imaging.Codec.RegisteredDecoders.GetImageInfo(imgPath)

                _injunctionId = _encryptor.QueryStringDecode(enc, UserInfo.Username)

                Dim objInjunction As InjunctionsInfo = _ctl.GetInjunctions(_injunctionId)
                If Not objInjunction Is Nothing Then
                    If Not objInjunction.IsMarked Then
                        InsertDocumentId(objInjunction.InjunctionId)
                    End If
                    Dim portalId As Integer = 0
                    Dim portalSettings As PortalSettings = GetPortalSettings()
                    If Not portalSettings Is Nothing Then
                        portalId = portalSettings.PortalId
                    End If
                    Dim InjunctionTitle As String = objInjunction.Title
                    objInjunction.ReviewedByJudgeUserId = UserInfo.UserID
                    objInjunction.IsMarked = True
                    objInjunction.StatusId = InjunctionStatus.Rejected
                    objInjunction.ReviewedDate = Now
                    _ctl.UpdateInjunctions(objInjunction)
                    If info.FrameCount = WebAnnotationViewer1.Annotations.Layers.Count Then
                        For fi As Integer = 0 To info.FrameCount - 1
                            Using page As New AtalaImage(imgPath, fi, Nothing)
                                Annotations.BurnAnnotations(page, WebAnnotationViewer1.Annotations.Layers(fi).Items, path, fi)
                                WebAnnotationViewer1.Annotations.Layers(fi).Items.Clear()
                            End Using
                        Next fi
                    End If
                    WebAnnotationViewer1.ClearAnnotations()
                    If imageprocessing.CreatePDF(GetBytsForFile(path), InjunctionTitle, objInjunction,_ctl) Then
                        _ctl.DeleteFile(objInjunction.FileId)
                    Else
                        Dim encryptedBytes As Byte() = _encryptor.EncryptStream(GetBytsForFile(path))
                        _ctl.UpdateFile(objInjunction.FileId, encryptedBytes)
                    End If

                    Try
                        Dim fromaddress As String = PortalSettings.Email
                        Dim toaddress As String = UserController.GetUserById(portalId, objInjunction.CreatedByUserId).Email
                        Dim subject As String = "Injunction Submission Rejected"

                        Dim body As String = "The Injunction submission [" & InjunctionTitle & "] was rejected for the following reason: " & rejecttest
                        Services.Mail.Mail.SendEmail(fromaddress, toaddress, subject, body)
                    Catch ex As Exception
                        returnString += "0|The Injunction was rejected, but an error occurred sending the email messages."
                        LogException(ex)

                    End Try
                Else
                    returnString = "0|Injunction could not be updated. Please try again."
                End If
            Catch ex As Exception
                WebAnnotationViewer1.ClearAnnotations()
                returnString = "-1|" + ex.Message + "|" + ex.StackTrace.ToString()
                LogException(ex)

            End Try
            Return returnString
        End Function

        <RemoteInvokable>
        Public Sub RemoteChangeAnnoText(newText As String)
            If WebAnnotationViewer1.Annotations.SelectedAnnotations.Count > 0 Then
                Dim rawAnno As AnnotationUI = WebAnnotationViewer1.Annotations.SelectedAnnotations(0)
                Dim text As TextAnnotation = TryCast(rawAnno, TextAnnotation)

                If text IsNot Nothing Then
                    text.Text = newText
                    text.AllowEditing = False
                End If
                Dim rect As RectangleAnnotation = TryCast(rawAnno, RectangleAnnotation)

                If rect IsNot Nothing Then
                    rect.Fill = New AnnotationBrush(Color.Blue)
                End If
                WebAnnotationViewer1.UpdateAnnotations()
                WebAnnotationViewer1.UpdateClient()
            End If
        End Sub

        <RemoteInvokable>
        Public Sub RemoteCreateText(aText As String, locX As Integer, locY As Integer, aWidth As Integer, aHeight As Integer)
            Dim imageWidth As Integer = WebAnnotationViewer1.Image.Width
            Dim fontsize As Integer = imageWidth * 0.017
            Dim font As AnnotationFont = New AnnotationFont("Arial", fontsize)
            Dim tAnno As New TextAnnotation With {
                .Location = New PointF(CSng(locX), CSng(locY)),
                .Size = GetBoxSize(aText, fontsize, imageWidth),
                .Text = aText,
                .Name = "CustomTextAnnotation",
                .Outline = New Atalasoft.Annotate.AnnotationPen(System.Drawing.Color.Transparent, 1),
                .Fill = New Atalasoft.Annotate.AnnotationBrush(System.Drawing.Color.Transparent),
                .Font = font
            }

            If WebAnnotationViewer1.Annotations.Layers.Count = 0 Then
                WebAnnotationViewer1.Annotations.Layers.Add(New LayerAnnotation())
            End If

            If WebAnnotationViewer1.Annotations.CurrentLayer Is Nothing Then
                WebAnnotationViewer1.Annotations.CurrentLayer = WebAnnotationViewer1.Annotations.Layers(0)
            End If
            WebAnnotationViewer1.Annotations.CurrentLayer.Items.Add(tAnno)
            WebAnnotationViewer1.UpdateAnnotations()

        End Sub

#End Region

#Region "Annotations"
        Public Sub InsertDocumentId(DocumentId As Integer)
            Dim location As New System.Drawing.PointF(10, 10)
            Dim resWidth = WebAnnotationViewer1.Image.Resolution.X
            For i = 0 To WebAnnotationViewer1.Annotations.Layers.Count - 1
                WebAnnotationViewer1.Annotations.Layers(i).Items.Add(CreateStamp(location, "ID#: " + DocumentId.ToString, resWidth))
            Next
        End Sub

        <RemoteInvokable>
        Public Sub InsertTextAnnotation(stampText As String)
            Dim annotationToRemove As Atalasoft.Annotate.UI.AnnotationUI = Nothing
            Dim location As New System.Drawing.PointF
            Dim foundAnnotation As Boolean = False
            For Each i As Atalasoft.Annotate.UI.AnnotationUI In WebAnnotationViewer1.Annotations.SelectedAnnotations
                If i.Name = "DefaultPerscheduleStamp" Then
                    annotationToRemove = i
                    location = i.Location
                    foundAnnotation = True
                    Exit For
                End If
            Next
            If foundAnnotation And stampText <> "" Then
                Dim resWidth = WebAnnotationViewer1.Image.Resolution.X
                WebAnnotationViewer1.Annotations.CurrentLayer.Items.Remove(annotationToRemove)
                WebAnnotationViewer1.Annotations.CurrentLayer.Items.Add(CreateStamp(location, stampText, resWidth))
                WebAnnotationViewer1.UpdateAnnotations()
            End If
        End Sub

        '<RemoteInvokable>
        'Public Sub InsertDateAnnotation(dateText As String)
        '    Dim annotationToRemove As Atalasoft.Annotate.UI.AnnotationUI = Nothing
        '    Dim location As New System.Drawing.PointF
        '    Dim foundAnnotation As Boolean = False
        '    For Each i As Atalasoft.Annotate.UI.AnnotationUI In WebAnnotationViewer1.Annotations.SelectedAnnotations
        '        If i.Name = "DefaultDateStamp" Then
        '            annotationToRemove = i
        '            location = i.Location
        '            foundAnnotation = True
        '            Exit For
        '        End If
        '    Next
        '    If foundAnnotation And dateText <> "" Then
        '        WebAnnotationViewer1.Annotations.CurrentLayer.Items.Remove(annotationToRemove)
        '        WebAnnotationViewer1.Annotations.CurrentLayer.Items.Add(CreateDateStamp(location, dateText))
        '        WebAnnotationViewer1.UpdateAnnotations()

        '    End If
        'End Sub

        Private Sub Processing(ByVal cmd As ImageCommand)
            Dim temp As AtalaImage = WebAnnotationViewer1.Image
            Dim url As String = WebAnnotationViewer1.ImageUrl
            Dim path As String = Page.MapPath(url)
            Dim frame As Integer = WebAnnotationViewer1.CurrentPage - 1

            ImageProcessing.ApplyCommand(cmd, temp, path, frame)

            WebAnnotationViewer1.OpenUrl(url, frame)
            WebAnnotationViewer1.UpdateClient()
        End Sub

        <RemoteInvokable>
        Public Sub RemoteReplaceImage(ByVal singleUrl As String)
            Dim temp As New AtalaImage(Page.MapPath(ImageCache & singleUrl))
            Dim url As String = WebAnnotationViewer1.ImageUrl
            Dim path As String = Page.MapPath(url)
            Dim frame As Integer = WebAnnotationViewer1.CurrentPage - 1

            ImageProcessing.SaveChanges(temp, path, frame)

            WebAnnotationViewer1.OpenUrl(url, frame)
            WebAnnotationViewer1.UpdateClient()
        End Sub

        Private Sub InitializeDefaultAnnotations(judge As JudgeInfo, username As String, imageWidth As Integer)
            Dim defaultAnnos As New Annotations()
            Dim filePath As String = Server.MapPath("~/images/perschedule.png")
            Dim checkMarkPath As String = Server.MapPath("~/images/check.png")
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyHighlighter())
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyFreehand())
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyRedaction())
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyText(imageWidth))
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyCheckStamp(checkMarkPath, imageWidth))
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyInitialStamp(judge.Initial, imageWidth))
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MySignatureStamp(judge.Signature, imageWidth))
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyDate(imageWidth))
            WebAnnotationViewer1.Annotations.DefaultAnnotations.Add(defaultAnnos.MyPerschedule(filePath, imageWidth))
        End Sub

        Private Function CreateStamp(location As System.Drawing.PointF, stampText As String, resolution As Integer) As TextAnnotation
            Dim stampAnn As TextAnnotation = New TextAnnotation(stampText) With {
                .Visible = True,
                .AutoSize = False,
                .CanResize = True,
                .Size = New System.Drawing.SizeF(GetWidthOfString(stampText, resolution), 60),
                .location = location
            }
            stampAnn.Font.Name = "Arial"
            stampAnn.Font.Size = CType(resolution / 6.5, Integer)
            stampAnn.Font.Bold = False
            Return stampAnn
        End Function

        'Private Function CreateDateStamp(location As System.Drawing.PointF, stampText As String) As TextAnnotation
        '    Dim res As Dpi = New Dpi()
        '    res = WebAnnotationViewer1.Image.Resolution
        '    Dim xres As Double = res.X
        '    Dim fSize As Single = 0
        '    If xres > 0 Then
        '        fSize = CType(xres / 7.5, Single)
        '    End If

        '    Dim stampAnn As TextAnnotation = New TextAnnotation(stampText) With {
        '        .Visible = True,
        '        .AutoSize = False,
        '        .Size = New System.Drawing.SizeF(GetWidthOfString(stampText, fSize * 1.2), fSize * 1.5),
        '        .CanResize = True,
        '        .Location = location
        '    }
        '    stampAnn.Font.Name = "Arial"
        '    stampAnn.Font.Size = fSize
        '    stampAnn.Font.Bold = False

        '    Return stampAnn
        'End Function

        Private Function GetWidthOfString(str As String, fsize As Single) As Single
            Dim objBitmap As Bitmap = Nothing
            Dim objGraphics As Graphics = Nothing

            objBitmap = New Bitmap(500, 200)
            objGraphics = Graphics.FromImage(objBitmap)

            Dim stringSize As SizeF = objGraphics.MeasureString(str, New Font("Arial", fsize))

            objBitmap.Dispose()
            objGraphics.Dispose()
            Return stringSize.Width
        End Function

        Private Function GetWidthOfString(str As String, resolution As Integer) As Single
            Dim objBitmap As Bitmap = Nothing
            Dim objGraphics As Graphics = Nothing
            Dim width As Integer = CType(resolution / 2, Integer)
            objBitmap = New Bitmap(width, 200)
            objGraphics = Graphics.FromImage(objBitmap)

            Dim stringSize As SizeF = objGraphics.MeasureString(str, New Font("Arial", 30))

            objBitmap.Dispose()
            objGraphics.Dispose()
            Return stringSize.Width
        End Function

        Private Function GetBoxSize(str As String, fontsize As Integer, ImageWidth As Integer) As System.Drawing.SizeF
            Dim objBitmap As Bitmap = Nothing
            Dim objGraphics As Graphics = Nothing
            Dim maxBoxSize As Integer = ImageWidth * 0.75
            objBitmap = New Bitmap(500, 200)
            objGraphics = Graphics.FromImage(objBitmap)

            Dim stringSize As SizeF = objGraphics.MeasureString(str, New Font("Arial", fontsize))
            If stringSize.Width > maxBoxSize Then
                Dim rows As Single = Math.Ceiling(stringSize.Width / maxBoxSize)
                stringSize.Width = maxBoxSize
                stringSize.Height = stringSize.Height * rows + 30
            End If
            stringSize.Width = stringSize.Width + 30
            stringSize.Height = stringSize.Height + 20
            objBitmap.Dispose()
            objGraphics.Dispose()
            Return stringSize
        End Function

#End Region

#Region "PageManipulation"

        <RemoteInvokable>
        Public Sub RemoteDeleteImage()
            Dim url As String = WebAnnotationViewer1.ImageUrl
            Dim path As String = Page.MapPath(url)
            Dim frame As Integer = WebAnnotationViewer1.CurrentPage - 1

            PageManipulation.RemovePageAndSave(path, frame)
            WebAnnotationViewer1.OpenUrl(url, frame)
        End Sub

        <RemoteInvokable>
        Public Function RemoteInsertPage() As String
            Dim url As String = WebAnnotationViewer1.ImageUrl
            Dim path As String = Page.MapPath(url)
            Dim frame As Integer = WebAnnotationViewer1.Annotations.Layers.Count


            PageManipulation.InsertPageAndSave(path, frame)
            WebAnnotationViewer1.OpenUrl(url, frame)

            WebAnnotationViewer1.UpdateClient()
            InsertLayer()
            Return WebAnnotationViewer1.ImageUrl
        End Function

        <RemoteInvokable>
        Public Sub InsertLayer()
            WebAnnotationViewer1.Annotations.Layers.Add(New LayerAnnotation())
            WebAnnotationViewer1.UpdateAnnotations()
        End Sub
#End Region


    End Class
End Namespace