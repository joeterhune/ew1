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
    Partial Class ExistingUser
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private agencyId As Integer = Null.NullInteger

#End Region

#Region "Methods"


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

                If Not Request.QueryString("AgencyId") Is Nothing Then
                    agencyId = Int32.Parse(Request.QueryString("AgencyId"))
                End If
                If Page.IsPostBack = False Then
                    lnkCancel.NavigateUrl = EditUrl("agencyId", agencyId.ToString, "adminusers")
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            Dim objExisting As UserInfo = UserController.GetUserByName(PortalId, txtExistingUser.Text)
            If Not objExisting Is Nothing Then
                Dim objagencyUser As New AgencyUserInfo
                objagencyUser.AgencyId = agencyId
                objagencyUser.UserId = objExisting.UserID
                objagencyUser.IsAdmin = False
                Dim ctl As New Controller
                ctl.AddAgencyUser(objagencyUser)
                txtExistingUser.Text = ""
                Response.Redirect(EditUrl("agencyId", agencyId.ToString, "adminusers"))
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "Username entered does not match an existing user in this portal.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

#End Region


    End Class

End Namespace
