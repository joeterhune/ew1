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
    Partial Class EditInjunctions
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private coverJudgeName As String = ""
        Private isOutofOffice As Boolean = False
        Private Judgeinfo As UserInfo = Nothing
        Public uploadHandler As String = ""
        Public Property AgencyId() As Integer
            Get
                If Not ViewState("AgencyId") Is Nothing Then
                    Return Int32.Parse(ViewState("AgencyId"))
                Else
                    Return Null.NullInteger
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("AgencyId") = value
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

        Private ReadOnly Property IsSiteAdmin As Boolean
            Get
                Dim isAdmin As Boolean = UserInfo.IsSuperUser
                If Not isAdmin Then
                    Return UserInfo.IsInRole("Administrators")
                End If
                Return isAdmin
            End Get
        End Property
#End Region

#Region "Methods"
        Private Sub BindOffenseTypes(agencyId As Integer)
            Dim ctl As New Controller
            rblOffenseType.DataSource = ctl.ListJudgeTypes(ModuleId)
            rblOffenseType.DataTextField = "JudgeType"
            rblOffenseType.DataValueField = "JudgeTypeID"
            rblOffenseType.DataBind()

            Dim objAgency As AgencyInfo = ctl.GetAgency(agencyId)
            If objAgency.CountyList.Trim <> "" Then

                Dim countyList = objAgency.CountyList.Split(",")
                For Each c As String In countyList
                    Dim countyText As String = c.Trim
                    drpCounty.Items.Add(New ListItem(countyText, countyText))
                Next
                drpCounty.Items.Insert(0, New ListItem("< Select County >", ""))
                If drpCounty.Items.Count > 2 Then
                    divCounty.Visible = True
                Else
                    drpCounty.SelectedIndex = 1
                End If
            Else
                UI.Skins.Skin.AddModuleMessage(Me, "Your agencies county list has not been properly configured.  Please contact your site administrator with this message.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

        Private Function GetCoverJudge(coverJudgeId As Integer) As UserInfo
            Return UserController.GetUserById(PortalId, coverJudgeId)
        End Function

        ''' <summary>
        ''' Deprecated use SendEmailResponse(agencyName As String, DocumentId As String, SendNow As Boolean)
        ''' </summary>
        ''' <param name="agencyName">Name of the Sending Agency</param>
        ''' <param name="DocumentId">Unique Id of the Document</param>
        Private Sub SendEmailResponse(agencyName As String, DocumentId As String)
            Dim fromaddress As String = UserInfo.Email
            Dim ctl As New Controller
            Dim nCtl As New AWS.Modules.Notifications.Controller
            Judgeinfo = UserController.GetUserById(PortalId, hdJudgeId.Value)
            Dim CoverJudgeInfo As UserInfo = Nothing
            Dim subject As String = ""
            Dim userDisplay As String = ""
            Dim body As String = ""
            If Not Judgeinfo Is Nothing Then
                Dim toAddress As String = Judgeinfo.Email
                Dim address2 As String = ""
                Dim address3 As String = ""
                Dim colNotifications As New List(Of AWS.Modules.Notifications.NotificationInfo)
                colNotifications = nCtl.ListCurrentNotificationsByJudge(Judgeinfo.UserID)
                If Not colNotifications Is Nothing AndAlso colNotifications.Count > 0 Then
                    Dim objNotification As AWS.Modules.Notifications.NotificationInfo = colNotifications.FirstOrDefault
                    isOutofOffice = True
                    If objNotification.CoveringJudgeId > 0 Then
                        CoverJudgeInfo = GetCoverJudge(objNotification.CoveringJudgeId)
                        toAddress = CoverJudgeInfo.Email
                        coverJudgeName = CoverJudgeInfo.DisplayName
                        If Not CoverJudgeInfo.Profile.ProfileProperties("Email2") Is Nothing Then
                            address2 = CoverJudgeInfo.Profile.ProfileProperties("Email2").PropertyValue
                        End If
                        If Not CoverJudgeInfo.Profile.ProfileProperties("Email3") Is Nothing Then
                            address3 = CoverJudgeInfo.Profile.ProfileProperties("Email3").PropertyValue
                        End If
                        subject = "New Document Forwarded from  " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " for Your Review"
                        userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                        body = "A new Document (ID: " & DocumentId & ")  has been submitted by " & userDisplay & " from " & agencyName & vbCrLf & vbCrLf
                        body += Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is out of the office and has asked that Submissions be forwarded to you during their absence."

                        Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                        If address2 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                        End If
                        If address3 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                        End If

                        subject = "Out of Office Notice"
                        body = " The Honorable " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is Out of the Office. " & vbCrLf & vbCrLf
                        body += "Your notification has been forwarded to the Honorable " & coverJudgeName
                        Services.Mail.Mail.SendEmail(PortalSettings.Email, UserInfo.Email, subject, body)

                    Else
                        subject = "New Document Submitted for Review"
                        userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                        body = "A new Document has been submitted by " & userDisplay & " from " & agencyName
                        If Not Judgeinfo.Profile.ProfileProperties("Email2") Is Nothing Then
                            address2 = Judgeinfo.Profile.ProfileProperties("Email2").PropertyValue
                        End If
                        If Not Judgeinfo.Profile.ProfileProperties("Email3") Is Nothing Then
                            address3 = Judgeinfo.Profile.ProfileProperties("Email3").PropertyValue
                        End If

                        Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                        If address2 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                        End If
                        If address3 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                        End If

                        subject = "Out of Office Notice"
                        body = " The Honorable " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is Out of the Office. Please review the notice below" & vbCrLf & vbCrLf
                        body += objNotification.MessageText
                        Services.Mail.Mail.SendEmail(PortalSettings.Email, UserInfo.Email, subject, body)
                    End If

                Else
                    subject = "New Document Submitted for Review"
                    userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                    body = "A new Document (ID: " & DocumentId & ") has been submitted by " & userDisplay & " from " & agencyName
                    If Not Judgeinfo.Profile.ProfileProperties("Email2") Is Nothing Then
                        address2 = Judgeinfo.Profile.ProfileProperties("Email2").PropertyValue
                    End If
                    If Not Judgeinfo.Profile.ProfileProperties("Email3") Is Nothing Then
                        address3 = Judgeinfo.Profile.ProfileProperties("Email3").PropertyValue
                    End If

                    Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                    If address2 <> "" Then
                        Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                    End If
                    If address3 <> "" Then
                        Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                    End If

                End If
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The selected Judge's record could not be found. No email notification can be sent", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Sub

        Private Sub SendEmailResponse(agencyName As String, DocumentId As String, SendNow As Boolean)
            Dim fromaddress As String = UserInfo.Email
            Dim ctl As New Controller
            Dim nCtl As New AWS.Modules.Notifications.Controller
            Judgeinfo = UserController.GetUserById(PortalId, hdJudgeId.Value)
            Dim CoverJudgeInfo As UserInfo = Nothing
            Dim subject As String = ""
            Dim userDisplay As String = ""
            Dim body As String = ""
            If Not Judgeinfo Is Nothing Then
                Dim toAddress As String = Judgeinfo.Email
                Dim address2 As String = ""
                Dim address3 As String = ""
                Dim colNotifications As New List(Of AWS.Modules.Notifications.NotificationInfo)
                colNotifications = nCtl.ListCurrentNotificationsByJudge(Judgeinfo.UserID)
                If Not colNotifications Is Nothing AndAlso colNotifications.Count > 0 Then
                    Dim objNotification As AWS.Modules.Notifications.NotificationInfo = colNotifications.FirstOrDefault
                    isOutofOffice = True
                    If objNotification.CoveringJudgeId > 0 Then
                        CoverJudgeInfo = GetCoverJudge(objNotification.CoveringJudgeId)
                        coverJudgeName = CoverJudgeInfo.DisplayName
                        If Not CoverJudgeInfo.Profile.ProfileProperties("Email2") Is Nothing Then
                            address2 = CoverJudgeInfo.Profile.ProfileProperties("Email2").PropertyValue
                        End If
                        If Not CoverJudgeInfo.Profile.ProfileProperties("Email3") Is Nothing Then
                            address3 = CoverJudgeInfo.Profile.ProfileProperties("Email3").PropertyValue
                        End If
                        subject = "New Document Forwarded from  " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " for Your Review"
                        userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                        body = "A new Document (ID: " & DocumentId & ")  has been submitted by " & userDisplay & " from " & agencyName & vbCrLf & vbCrLf
                        body += Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is out of the office and has asked that Submissions be forwarded to you during their absence."
                        If SendNow Then
                            Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                            If address2 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                            End If
                            If address3 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                            End If
                        End If
                        subject = "Out of Office Notice"
                        body = " The Honorable " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is Out of the Office. " & vbCrLf & vbCrLf
                        body += "Your notification has been forwarded to the Honorable " & coverJudgeName
                        Services.Mail.Mail.SendEmail(PortalSettings.Email, UserInfo.Email, subject, body)

                    Else
                        subject = "New Document Submitted for Review"
                        userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                        body = "A new Document has been submitted by " & userDisplay & " from " & agencyName
                        If Not Judgeinfo.Profile.ProfileProperties("Email2") Is Nothing Then
                            address2 = Judgeinfo.Profile.ProfileProperties("Email2").PropertyValue
                        End If
                        If Not Judgeinfo.Profile.ProfileProperties("Email3") Is Nothing Then
                            address3 = Judgeinfo.Profile.ProfileProperties("Email3").PropertyValue
                        End If
                        If SendNow Then
                            Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                            If address2 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                            End If
                            If address3 <> "" Then
                                Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                            End If
                        End If
                        subject = "Out of Office Notice"
                        body = " The Honorable " & Judgeinfo.DisplayName.Replace("&nbsp;", " ") & " is Out of the Office. Please review the notice below" & vbCrLf & vbCrLf
                        body += objNotification.MessageText
                        Services.Mail.Mail.SendEmail(PortalSettings.Email, UserInfo.Email, subject, body)
                    End If

                Else
                    subject = "New Document Submitted for Review"
                    userDisplay = UserInfo.DisplayName.Replace("&nbsp;", " ")
                    body = "A new Document (ID: " & DocumentId & ") has been submitted by " & userDisplay & " from " & agencyName
                    If Not Judgeinfo.Profile.ProfileProperties("Email2") Is Nothing Then
                        address2 = Judgeinfo.Profile.ProfileProperties("Email2").PropertyValue
                    End If
                    If Not Judgeinfo.Profile.ProfileProperties("Email3") Is Nothing Then
                        address3 = Judgeinfo.Profile.ProfileProperties("Email3").PropertyValue
                    End If
                    If SendNow Then
                        Services.Mail.Mail.SendEmail(fromaddress, toAddress, subject, body)
                        If address2 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address2, subject, body)
                        End If
                        If address3 <> "" Then
                            Services.Mail.Mail.SendEmail(fromaddress, address3, subject, body)
                        End If
                    End If
                End If
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The selected Judge's record could not be found. No email notification can be sent", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
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
                uploadHandler = TemplateSourceDirectory & "/Handlers/FileUpload.ashx"
                If Page.IsPostBack = False Then
                    Dim county As String = ""
                    If JudgeRole = "" Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The Documents Application has not been configured. Please contact the site administrator", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        pnlInjunction.Visible = False
                        cmdUpdate.Visible = False
                        Exit Sub
                    End If
                    Dim ctl As New Controller
                    Dim objagencyUser As AgencyUserInfo = ctl.GetUser(ModuleId, UserId)
                    If Not objagencyUser Is Nothing And Not IsSiteAdmin Then
                        If objagencyUser.IsAdmin Then
                            Response.Redirect(NavigateURL)
                        End If
                        AgencyId = objagencyUser.AgencyId
                        Dim objAgency As AgencyInfo = ctl.GetAgency(AgencyId)
                        If objAgency.IsClerk Then
                            Response.Redirect(NavigateURL)
                        End If

                        BindOffenseTypes(AgencyId)
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Invalid User Access.  Please contact the site administrator with the error.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        pnlInjunction.Visible = False
                        cmdUpdate.Visible = False
                        Exit Sub
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
                If hdFileId.Value <> "" Then
                    Dim ctl As New Controller
                    ctl.DeleteFile(CType(hdFileId.Value, Integer))
                End If
                Response.Redirect(NavigateURL(), True)
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
                Dim objInjunction As New InjunctionsInfo

                If hdFileId.Value <> "" Then
                    objInjunction.FileId = hdFileId.Value
                Else
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The file ID could not be found.  Please cancel this process and try again.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    Exit Sub
                End If
                Dim ctl As New Controller
                Dim SendNow As Boolean = True
                If rblEmergency.SelectedValue = "0" Then
                    SendNow = False
                Else
                    objInjunction.NotificationSentDate = DateTime.Now
                End If
                Dim injunctionType As Integer = 0
                Int32.TryParse(rblOffenseType.SelectedValue, injunctionType)
                objInjunction.ModuleId = ModuleId
                objInjunction.AgencyId = AgencyId
                objInjunction.CreatedByUserId = UserId
                objInjunction.CreatedDate = Now
                objInjunction.Title = txtTitle.Text
                objInjunction.JudgeUserId = hdJudgeId.Value
                objInjunction.InjunctionType = injunctionType
                objInjunction.StatusId = InjunctionStatus.NewInjunction
                objInjunction.Defendant = txtDefendant.Text
                objInjunction.NotificationSent = SendNow
                Dim DocumentId As Integer = ctl.AddInjunction(objInjunction)
                Try
                    Dim objAgency As AgencyInfo = ctl.GetAgency(AgencyId)
                    SendEmailResponse(objAgency.AgencyName, DocumentId, SendNow)
                Catch ex As Exception
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "The Document was successfully upload, however, there was an error sending the  email.", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    ProcessModuleLoadException(Me, ex, False)
                End Try
                ' refresh cache
                Entities.Modules.ModuleController.SynchronizeModule(ModuleId)
                ' Redirect back to the portal home page
                If isOutofOffice Then
                    Dim judgeName As String = Judgeinfo.DisplayName
                    Dim notificationMessage As String = judgeName & " is out of the office. "
                    If coverJudgeName <> "" Then
                        notificationMessage += " Your Document has been forwarded to " & coverJudgeName
                    Else
                        notificationMessage += " You will receive an email with further information from the Judge"
                    End If
                    Response.Redirect(NavigateURL("", "msg=" & notificationMessage), True)
                Else
                    Response.Redirect(NavigateURL(), True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim ctl As New Controller
            Dim judges As List(Of JudgeInfo) = ctl.ListJudges(ModuleId)
            For Each j In judges
                Page.ClientScript.RegisterForEventValidation(drpJudge.UniqueID, j.JudgeId.ToString())
            Next

            Page.ClientScript.RegisterForEventValidation(drpJudge.UniqueID, "")
            MyBase.Render(writer)
        End Sub

        Protected Sub rblOffenseType_DataBound(sender As Object, e As EventArgs) Handles rblOffenseType.DataBound
            Dim rbl As RadioButtonList = sender
            For Each li As ListItem In rbl.Items
                li.Attributes.Add("onclick", "FillJudges()()")
            Next

        End Sub

        Protected Sub valEmergency_ServerValidate(source As Object, args As ServerValidateEventArgs)
            args.IsValid = False
            Dim judgeId = hdJudgeId.Value
            Dim ctl As New Controller
            Dim judge = ctl.GetJudge(judgeId)
            Dim NotifyJudge As Boolean = True
            If DateTime.Now.TimeOfDay > judge.DayEnd.TimeOfDay Or DateTime.Now.TimeOfDay < judge.DayStart.TimeOfDay Then
                NotifyJudge = False
            End If
            If NotifyJudge Then
                args.IsValid = True
            Else
                If rblEmergency.SelectedIndex >= 0 Then
                    args.IsValid = True
                End If
            End If
        End Sub
#End Region

    End Class

End Namespace
