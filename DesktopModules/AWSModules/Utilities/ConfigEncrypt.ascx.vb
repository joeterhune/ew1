Imports System.Web.Configuration
Imports DotNetNuke
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Entities.Profile

Namespace AWS.Modules.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewDynamicModule class displays the content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ConfigEncryption
        Inherits Entities.Modules.PortalModuleBase

#Region " Private Methods "
        Private Sub ProtectSection(ByVal sectionName As String, ByVal provider As String)
            Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)

            Dim section As ConfigurationSection = config.GetSection(sectionName)

            If Not section Is Nothing AndAlso (Not section.SectionInformation.IsProtected) Then
                section.SectionInformation.ProtectSection(provider)
                config.Save()
            End If
        End Sub

        Private Sub UnProtectSection(ByVal sectionName As String)
            Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)

            Dim section As ConfigurationSection = config.GetSection(sectionName)

            If Not section Is Nothing AndAlso section.SectionInformation.IsProtected Then
                section.SectionInformation.UnprotectSection()
                config.Save()
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
                lnkImportUsers.NavigateUrl = EditUrl("Import")
                lnkExportUsers.NavigateUrl = EditUrl("Export")
                lnkSynUsers.NavigateUrl = EditUrl("Sync")
		lnkArchive.NavigateUrl = EditUrl("archive")
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdProtect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdProtect.Click
            ProtectSection("appSettings", "DataProtectionConfigurationProvider")
            ProtectSection("connectionStrings", "DataProtectionConfigurationProvider")
        End Sub

        Protected Sub cmdUnProtect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUnProtect.Click
            UnProtectSection("appSettings")
            UnProtectSection("connectionStrings")

        End Sub

#End Region

    End Class

End Namespace
