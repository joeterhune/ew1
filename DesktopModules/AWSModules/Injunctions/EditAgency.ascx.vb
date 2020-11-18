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
    Partial Class EditAgency
        Inherits Entities.Modules.PortalModuleBase

#Region "Members"
        Private agencyId As Integer = Null.NullInteger
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

                    lnkCancel.NavigateUrl = EditUrl("adminAgency")

                    Dim ctl As New Controller
                    If agencyId <> Null.NullInteger Then
                        Dim objAgency As AgencyInfo = ctl.GetAgency(agencyId)
                        If Not objAgency Is Nothing Then
                            txtAgencyName.Text = objAgency.AgencyName
                            txtEmail.Text = objAgency.EmailAddress
                            txtParentAgency.Text = objAgency.ParentAgency
                            If objAgency.IsClerk Then
                                chkIsClerk.Checked = True
                            Else
                                chkIsClerk.Checked = False
                            End If

                        End If
                    End If

                    Dim colCounties = ctl.ListCounties(ModuleId).Select(Function(cty) cty.CountyName)
                    chkCounties.DataSource = colCounties
                    chkCounties.DataBind()
                    If agencyId <> Null.NullInteger Then
                        Dim colSelectedCounties = ctl.GetCountiesByAgency(agencyId).Select(Function(cty) cty.CountyName)
                        For Each item In colSelectedCounties
                            Dim listItem As ListItem = Me.chkCounties.Items.FindByText(item)
                            If listItem IsNot Nothing Then listItem.Selected = True
                        Next
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


        Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            Try

                If Page.IsValid Then
                    Dim agency As AgencyInfo = Nothing
                    Dim ctl As New Controller
                    If agencyId <> Null.NullInteger Then
                        agency = ctl.GetAgency(agencyId)
                    Else
                        agency = New AgencyInfo
                    End If

                    agency.AgencyName = txtAgencyName.Text
                    agency.EmailAddress = txtEmail.Text
                    agency.ModuleId = ModuleId
                    agency.ParentAgency = txtParentAgency.Text
                    agency.IsClerk = chkIsClerk.Checked
                    If agencyId <> Null.NullInteger Then
                        ctl.UpdateAgency(agency)
                    Else
                        agency.AgencyId = Null.NullInteger
                        agencyId = ctl.AddAgency(agency)
                    End If
                    ctl.DeleteAgencyCounties(agencyId)

                    For Each li As ListItem In chkCounties.Items
                        If li.Selected Then
                            Dim agencyCounty As New AgencyCounty(agencyId, li.Value)
                            ctl.AddAgencyCounty(agencyCounty)
                        End If
                    Next 
                    Response.Redirect(EditUrl("adminAgency"))
                Else
                    Dim message As String = "Please review your entries making sure to complete all required fields."
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, message, Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub


#End Region

    End Class

End Namespace
