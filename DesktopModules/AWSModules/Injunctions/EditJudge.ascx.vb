' 
' DotNetNuke® - http:'www.dotnetnuke.com
' Copyright (c) 2002-2011
' by DotNetNuke Corporation
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT InjunctionY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE InjunctionIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.


Imports DotNetNuke
Imports System.IO


Namespace AWS.Modules.Injunctions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDynamicModule class is used to manage content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditJudge
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private JudgeId As Integer = Null.NullInteger
        Public signatureHandler As String = ""
        Public Property JudgeSignImages() As List(Of JudgeSignImage)
            Get
                If Not ViewState("JudgeSignImages") Is Nothing Then
                    Return CType(ViewState("JudgeSignImages"), List(Of JudgeSignImage))
                Else
                    Return New List(Of JudgeSignImage)
                End If

            End Get
            Set(ByVal value As List(Of JudgeSignImage))
                ViewState("JudgeSignImages") = value
            End Set
        End Property


        Public ReadOnly Property JudgeRole() As String
            Get
                If Not Settings("JudgeRole") Is Nothing Then
                    Return Settings("JudgeRole")
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property ChiefJudge() As String
            Get
                If Settings("AdminJudge") Is Nothing Then
                    Return ""
                Else
                    Return Settings("AdminJudge")
                End If
            End Get
        End Property
#End Region

#Region "Methods"

        Private Sub AddCounties(ctl As Controller, judgeId As Integer)
            ctl.DeleteJudgeCounties(judgeId, ModuleId)
            For Each i As ListItem In cklCounties.Items
                If i.Selected Then
                    ctl.AddJudgeCounty(judgeId, CInt(i.Value))
                End If
            Next
        End Sub

        Private Sub BindJudges()
            Dim rolectl As New DotNetNuke.Security.Roles.RoleController
            Dim judges = rolectl.GetUsersByRole(PortalId, JudgeRole)
            For Each u As UserInfo In judges
                drpJudge.Items.Add(New ListItem(u.DisplayName.Replace("&nbsp;", " "), u.UserID))
            Next
            drpJudge.Items.Insert(0, New ListItem("< Select Judge >", ""))
        End Sub

        Private Sub BindCounties(counties As List(Of CountyInfo), coljudgeType As List(Of JudgeTypeInfo))
            Dim ctl As New Controller
            Dim lstCounties = ctl.ListCounties(ModuleId)
            cklCounties.DataTextField = "CountyName"
            cklCounties.DataValueField = "CountyId"
            cklCounties.DataSource = lstCounties
            cklCounties.DataBind()
            For Each c As ListItem In cklCounties.Items
                If counties.Select(Function(cn) cn.CountyID).Contains(c.Value) Then
                    c.Selected = True
                End If
            Next
            cklOffenseType.DataSource = ctl.ListJudgeTypes(ModuleId)
            cklOffenseType.DataTextField = "JudgeType"
            cklOffenseType.DataValueField = "JudgeTypeID"
            cklOffenseType.DataBind()

            If coljudgeType.Count > 0 Then
                For Each i As ListItem In cklOffenseType.Items
                    For Each jtype As JudgeTypeInfo In coljudgeType
                        If i.Value = jtype.JudgeTypeID Then
                            i.Selected = True
                        End If
                    Next
                Next
            End If
        End Sub
#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Request.QueryString("judgeId") Is Nothing Then
                    JudgeId = Int32.Parse(Request.QueryString("judgeId"))

                End If
                If ChiefJudge = "" Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "You must define the Chief Judge through Module Settings.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    cmdUpdate.Visible = False
                    Exit Sub
                End If
                If Page.IsPostBack = False Then
                    signatureHandler  =  TemplateSourceDirectory & "/Handlers/imageUpload.ashx"
                    Dim imageHandler = "~" & TemplateSourceDirectory & "/Handlers/image.ashx?enc="
                    BindJudges()
                    If JudgeId <> Null.NullInteger Then
                        pnlSignatures.Visible = True
                        drpJudge.SelectedValue = JudgeId
                        drpJudge.Enabled = False
                        Dim ctl As New Controller
                        Dim objJudge As JudgeInfo = ctl.GetJudge(JudgeId)
                        Dim colJudgeTypes As New List(Of JudgeTypeInfo)
                        colJudgeTypes = ctl.ListJudgeTypesByJudge(JudgeId, ModuleId)
                        BindCounties(objJudge.ListCounties, colJudgeTypes)
                        txtDayStart.Text = objJudge.DayStart.TimeOfDay.ToString
                        txtDayEnd.Text = objJudge.DayEnd.TimeOfDay.ToString
                        Dim encypt As New Encryptor
                        imgSignature.ImageUrl = imageHandler & encypt.QueryStringEncode(objJudge.Signature, UserInfo.Username)
                        imgInitial.ImageUrl = imageHandler & encypt.QueryStringEncode(objJudge.Initial, UserInfo.Username)
                    Else
                        BindCounties(New List(Of CountyInfo), New List(Of JudgeTypeInfo))
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Dim ctl As New Controller
                For Each objSignImage In JudgeSignImages
                    ctl.DeleteFile(objSignImage.FileId)
                Next

                Response.Redirect(EditUrl("JudgeSign"), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                Dim ctl As New Controller
                Dim objJudge As New JudgeInfo
                If JudgeId <> Null.NullInteger Then
                    objJudge = ctl.GetJudge(JudgeId)
                End If
                If hdSignatureIds.Value <> "" Then
                    Dim fileList As String() = hdSignatureIds.Value.Split(",")
                    If fileList.Length > 0 Then
                        For Each valueSet In fileList
                            Dim imageType As String = valueSet.Split(":")(0)
                            Dim fileId As Integer = CType(valueSet.Split(":")(1), Integer)
                            Select Case imageType.ToLower
                                Case "i"
                                    objJudge.Initial = fileId
                                Case "s"
                                    objJudge.Signature = fileId
                            End Select
                        Next
                    End If

                End If

                If JudgeId = Null.NullInteger Then

                    If txtDayStart.Text <> "" Then
                        Dim day As Date = Null.NullDate
                        DateTime.TryParse(DateTime.Today.ToShortDateString + " " + txtDayStart.Text, day)
                        objJudge.DayStart = day
                    End If
                    If txtDayEnd.Text <> "" Then
                        Dim day As Date = Null.NullDate
                        DateTime.TryParse(DateTime.Today.ToShortDateString + " " + txtDayEnd.Text, day)
                        objJudge.DayEnd = day
                    End If
                    objJudge.JudgeId = drpJudge.SelectedValue
                    objJudge.ModuleId = ModuleId
                    Dim judgeid As Integer = ctl.AddJudge(objJudge)
                    AddCounties(ctl, objJudge.JudgeId)
                    ctl.DeleteJudgeJudgeTypeXref(objJudge.JudgeId, ModuleId)

                    For Each item As ListItem In cklOffenseType.Items
                        If item.Selected Then
                            ctl.AddJudgeJudgeTypeXref(objJudge.JudgeId, item.Value)
                        End If
                    Next
                    Try 'Send email to Chief Judge
                        Dim objChiefJudge As UserInfo = UserController.GetUserByName(PortalId, ChiefJudge)
                        Dim fromaddress As String = UserInfo.Email
                        Dim judgeAddress As String = objChiefJudge.Email
                        Dim subject As String = "New Judge Added to eInjunctions Application"
                        Dim body As String = "Please log into the eInjunctions site and access the Judge List to configure the new judge."
                        Services.Mail.Mail.SendEmail(fromaddress, judgeAddress, subject, body)
                        Dim toaddress As String = UserController.GetUserById(PortalId, drpJudge.SelectedValue).Email
                        body = "The site administrator has added your account to the Judge list on the eInjunctions application.  Once the request has been approved you will be able to view Injunctions on the eInjunctions site."
                        Services.Mail.Mail.SendEmail(fromaddress, toaddress, subject, body)
                    Catch ex As Exception
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The Injunction was successfully upload, however, there was an error sending the  email.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End Try
                Else
                    If txtDayStart.Text <> "" Then
                        Dim day As Date = Null.NullDate
                        DateTime.TryParse(DateTime.Today.ToShortDateString + " " + txtDayStart.Text, day)
                        objJudge.DayStart = day
                    End If
                    If txtDayEnd.Text <> "" Then
                        Dim day As Date = Null.NullDate
                        DateTime.TryParse(DateTime.Today.ToShortDateString + " " + txtDayEnd.Text, day)
                        objJudge.DayEnd = day
                    End If
                    AddCounties(ctl, JudgeId)
                    ctl.UpdateJudge(objJudge)
                    ctl.DeleteJudgeJudgeTypeXref(objJudge.JudgeId, ModuleId)
                    For Each item As ListItem In cklOffenseType.Items
                        If item.Selected Then
                            ctl.AddJudgeJudgeTypeXref(objJudge.JudgeId, item.Value)
                        End If
                    Next
                End If



                ' refresh cache
                Entities.Modules.ModuleController.SynchronizeModule(ModuleId)
                ' Redirect back to the portal home page
                Response.Redirect(EditUrl("JudgeSign"), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

    End Class

End Namespace
