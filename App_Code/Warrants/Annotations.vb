Imports Atalasoft.Annotate
Imports Atalasoft.Imaging
Imports System.IO
Imports Atalasoft.Annotate.UI
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

Namespace AWS.Modules.Warrants
    Public Class Annotations

        Public Sub New()
        End Sub

#Region "Set up default annotations"

        Public Function MyHighlighter() As AnnotationData
            Dim highlighter As New RectangleData With {
                .Name = "DefaultHighlighter",
                .Outline = New AnnotationPen(Color.Transparent, 1),
                .Fill = New AnnotationBrush(Color.FromArgb(128, Color.Yellow))
            }
            Return highlighter
        End Function

        Public Function MyFreehand() As AnnotationData
            Dim freehand As New FreehandData With {
                .Name = "DefaultFreehand",
                .Outline = New AnnotationPen(Color.DarkBlue, 6)
            }
            Return freehand
        End Function

        Public Function MyRedaction() As AnnotationData
            Dim redaction As New RectangleData With {
                .Name = "DefaultRedaction",
                .Fill = New AnnotationBrush(Color.Black)
            }
            Return redaction
        End Function

        Public Function MyText(imageWidth As Integer) As AnnotationData
            Dim fontsize As Integer = imageWidth * 0.017
            Dim text As New TextData With {
                .AutoSize = False,
                .Name = "DefaultTextAnnotation",
                .Text = "Double-click to edit",
                .Font = New AnnotationFont("Arial", fontsize),
                .FontBrush = New AnnotationBrush(Color.Black),
                .Fill = New AnnotationBrush(Color.Transparent),
                .Outline = New AnnotationPen(New AnnotationBrush(Color.Transparent), 1)
            }
            Return text
        End Function

        Public Function MyDate(imageWidth As Integer) As AnnotationData
            Dim fontsize As Integer = imageWidth * 0.017 'ratio for best sizing
            Dim dateStamp As New EmbeddedImageData(RenderDateGraphic(fontsize)) With {
                .Name = "DefaultDateStamp",
                .CanResize = False
            }

            Return dateStamp
        End Function

        Public Function MyPerschedule(imagePath As String, imageWidth As Integer) As AnnotationData
            Dim widthRatio As Decimal = 239 / 1600
            Dim heightRatio As Decimal = 33 / 1600
            Dim newHeight As Integer = CType(heightRatio * imageWidth, Integer)
            Dim newWidth As Integer = CType(widthRatio * imageWidth, Integer)
            Dim text As New EmbeddedImageData(New AnnotationImage(imagePath)) With {
                .Name = "DefaultPerscheduleStamp",
                .CanResize = False,
                .Size = New Size(newWidth, newHeight)
            }
            Return text
        End Function

        Public Function MySignatureStamp(ByVal fileId As Integer, imageWidth As Integer) As AnnotationData
            Dim width As Integer = CType(imageWidth / 4, Integer)
            Dim ctl As New Controller
            Dim encryptor As New Encryptor
            Dim objWarratnImage As WarrantImage = ctl.GetFile(fileId)
            Dim ms As New MemoryStream(encryptor.DecryptStream(objWarratnImage.Bytes))
            Dim image As New AnnotationImage(ms)
            Dim ratio As Decimal = image.Height / image.Width
            Dim height As Integer = width * ratio
            Dim signature As New EmbeddedImageData(image) With {
                .Size = New Size(width, height),
                .Name = "DefaultSignature"
            }
            Return signature
        End Function

        Public Function MyInitialStamp(ByVal FileId As Integer, imageWidth As Integer) As AnnotationData
            Dim width As Integer = CType(imageWidth / 10, Integer)
            Dim ctl As New Controller
            Dim encryptor As New Encryptor
            Dim objWarratnImage As WarrantImage = ctl.GetFile(FileId)
            Dim ms As New MemoryStream(encryptor.DecryptStream(objWarratnImage.Bytes))
            Dim image As New AnnotationImage(ms)
            Dim ratio As Decimal = image.Height / image.Width
            Dim height As Integer = width * ratio

            Dim initial As New EmbeddedImageData(image) With {
                .Size = New Size(width, height),
                .CanResize = False,
                .Name = "DefaultInitial"
            }

            Return initial

        End Function

        Public Function MyCheckStamp(ByVal imagePath As String, imageWidth As Integer) As AnnotationData
            Dim imageRatio As Decimal = 16 / 800
            Dim newWidth As Integer = CType(imageRatio * imageWidth, Integer)
            Dim image As New AnnotationImage(imagePath)
            Dim checkMark As New EmbeddedImageData(image) With {
                .Name = "DefaultCheckMark",
                .CanResize = False,
                .Size = New Size(newWidth, newWidth)
            }

            Return checkMark

        End Function


#End Region

#Region "Process Annotations"
        Private Function RenderDateGraphic(fontsize As Integer) As AnnotationImage
            Dim ptext As String = Today.ToShortDateString
            Dim pSize As Integer = fontsize
            Dim b As Bitmap = New Bitmap(1, 1)
            Dim g As Graphics = Graphics.FromImage(b)
            Dim f As Font = New Font("Arial", pSize)
            Dim w As Integer = g.MeasureString(ptext, f).Width
            Dim h As Integer = g.MeasureString(ptext, f).Height
            Dim c1 As Color = Color.Black
            Dim c2 As Color = Color.Transparent
            b = New Bitmap(w, h)
            g = Graphics.FromImage(b)
            g.Clear(c2)
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
            g.DrawString(ptext, f, New SolidBrush(c1), 0, 0)
            g.Flush()
            Dim m As New MemoryStream
            b.Save(m, ImageFormat.Png)

            Dim returnImage As New AnnotationImage(m)
            Return returnImage

        End Function

        Public Shared Function AddWatermark(Bitmap As Bitmap, WatermarkText As String, TextColor As Color, OpacityPercent As Integer, FontFamily As String, FontStyle As FontStyle, MaxFontSize As Integer, insert As Point) As Bitmap

            Dim phWidth As Integer = Bitmap.Width
            Dim phHeight As Integer = Bitmap.Height
            Dim bmPhoto As New Bitmap(phWidth, phHeight, Imaging.PixelFormat.Format32bppPArgb)

            bmPhoto.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution)

            'load the Bitmap into a Graphics object 
            Dim grPhoto As Graphics = Graphics.FromImage(bmPhoto)
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias
            grPhoto.DrawImage(Bitmap, New Rectangle(0, 0, phWidth, phHeight), 0, 0, phWidth, phHeight, GraphicsUnit.Pixel)

            '-------------------------------------------------------
            'to maximize the size of the Date message 
            'test multiple Font sizes to determine the largest posible 
            'font we can use for the width of the signature
            '-------------------------------------------------------
            Dim sizes As Integer() = New Integer() {35, 30, 25, 20, 16, 12, 10}
            Dim crFont As Font = Nothing
            Dim crSize As New SizeF()

            'Loop through the defined sizes checking the length of the Date string
            'If its length in pixles is less then the image width choose this Font size.
            For i As Integer = 0 To 6
                crFont = New Font("Arial", sizes(i), FontStyle.Bold)
                'Measure the Date string in this Font
                crSize = grPhoto.MeasureString(WatermarkText, crFont)

                If CUShort(crSize.Width) < CUShort(phWidth) Then
                    Exit For
                End If
            Next

            'Since all photographs will have varying heights, determine a 
            'position 50% from the bottom of the image
            Dim yPixlesFromBottom As Integer = CInt(Math.Truncate(phHeight * 0.5))

            'Now that we have a point size use the Dates string height 
            'to determine a y-coordinate to draw the string of the photograph
            Dim yPosFromBottom As Single = ((phHeight - yPixlesFromBottom) - (crSize.Height / 2))

            'Determine its x-coordinate by calculating the center of the width of the image
            Dim xCenterOfImg As Single = (phWidth / 2)

            'Define the text layout by setting the text alignment to centered
            Dim StrFormat As New StringFormat With {
                .Alignment = StringAlignment.Center
            }

            Dim semiTransBrush As New SolidBrush(Color.FromArgb(128, 0, 0, 0))

            'Draw the Date string a second time to create a shadow effect
            'Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(WatermarkText, crFont, semiTransBrush, New PointF(xCenterOfImg, yPosFromBottom), StrFormat)
            Bitmap = bmPhoto

            Return Bitmap
        End Function

        Public Shared Sub BurnAnnotations(ByVal image As AtalaImage, ByVal annoCollection As AnnotationUICollection, ByVal path As String, ByVal frame As Integer)

            'References to Atalasoft.DotImage.WinControls.dll and System.Windows.Forms
            'are necessary to use this method for burning annotations.
            Dim HasSignature As Boolean = False
            Using av As New AnnotateViewer()
                Dim colTmp As New AnnotationUICollection

                For Each itype As AnnotationUI In annoCollection
                    If TypeOf itype Is EmbeddedImageAnnotation Then
                        Dim embeddedImage As EmbeddedImageAnnotation = itype
                        If embeddedImage.Name = "DefaultSignature" Or embeddedImage.Name = "DefaultInitial" Then
                            HasSignature = True
                            Dim annImage As AnnotationImage = embeddedImage.Image
                            Dim bmp As Bitmap = CType(embeddedImage.Image.ImageObject(), Bitmap)
                            Dim insertpoint As Integer = (bmp.Height / 2) + 12
                            Dim fontsize As Integer = bmp.Width / 20
                            Dim color As Color = Color.FromArgb(10, 0, 0, 0)
                            embeddedImage.Image = New AnnotationImage(AddWatermark(bmp, Date.Now.ToString, color, 80, "Arial", FontStyle.Bold, fontsize, New Point(10, insertpoint)))
                            colTmp.Add(embeddedImage)
                        Else
                            colTmp.Add(itype)
                        End If
                    Else
                        colTmp.Add(itype)
                    End If
                Next
                If HasSignature Then
                    If image.PixelFormat <> Atalasoft.Imaging.PixelFormat.Pixel8bppGrayscale Then
                        image = image.GetChangedPixelFormat(Atalasoft.Imaging.PixelFormat.Pixel8bppGrayscale)
                    End If
                End If
                av.Burn(colTmp, image)
            End Using

            ImageProcessing.SaveChanges(image, path, frame)
        End Sub
#End Region
    End Class

End Namespace
