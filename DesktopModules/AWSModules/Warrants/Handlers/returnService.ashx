<%@ WebHandler Language="VB" Class="AWS.Modules.Warrants.ReturnServiceHandler" %>
Imports System.IO
Namespace AWS.Modules.Warrants
    Public Class ReturnServiceHandler
        Implements IHttpHandler

        private warrantId As string = ""
        private response as string = ""
        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            If context.Request.Files.Count > 0 Then
                Dim objuser As UserInfo
                objuser = UserController.Instance.GetCurrentUserInfo
                Dim files As HttpFileCollection = context.Request.Files
                Dim file As HttpPostedFile = files(0)
                warrantId = context.Request.Params("wid")
                Try
                    Dim ctl as new Controller
                    Dim objWarrant = ctl.GetWarrants(warrantid)
                    if Not objWarrant Is nothing then
                        if objwarrant.CreatedByUserId = objuser.UserID then
                            Dim archiveDirectory As String = ConfigurationManager.AppSettings("CompletedWarrants")
                            Dim attachmentFileName As String = archiveDirectory + "Temp\" + objWarrant.WarrantId.ToString() + "rs.pdf"
                            file.SaveAs(attachmentFileName)

                            Dim result as Boolean = AttachConfiscationLog(objWarrant, attachmentFileName)
                            if result then
                                response = "Return Service Attached Successfully"
                                objWarrant.StatusId = WarrantStatus.ReturnService
                                ctl.UpdateWarrants(objWarrant)
                            else
                                response = "<strong class='alert-danger'>Error:</strong> Error Attaching Return Service"
                            End If

                        else
                            response = "<strong class='alert-danger'>Error:</strong> Only Document Owner may attach return service"
                        End If
                        context.Response.ContentType = "text/plain"
                        context.Response.Write(response)
                    else
                        response = "<strong class='alert-danger'>Error:</strong> Document record does not exit. Could not locate Document File"
                    End If
                Catch ex As Exception
                    LogException(ex)
                    response = "<strong class='alert-danger'>Error:</strong> Unexpected Error Attempting Return of Service Attachment"
                    context.Response.Write(response)
                End Try
            End If
        End Sub

        Private Function AttachConfiscationLog(objWarrant As WarrantsInfo, rsfile as string) As boolean
            Dim archiveDirectory As String = ConfigurationManager.AppSettings("CompletedWarrants")
            Dim warrantFileName As String = archiveDirectory + objWarrant.CreatedDate.Year.ToString + "\" + objWarrant.WarrantId.ToString() + ".pdf"
            Dim file As New FileInfo(warrantFileName)
            Dim tempDir As New DirectoryInfo(archiveDirectory + "Temp")
            Dim outFile As String = archiveDirectory + "Temp\" + objWarrant.WarrantId.ToString() + ".pdf"
            If Not tempDir.Exists Then
                tempDir.Create()
            End If
            If file.Exists Then
                file.CopyTo(warrantFileName + ".bck", True)
                dim isMerged = False
                Try
                    isMerged = MergePdfs(warrantFileName, rsfile, outFile)
                    If Not isMerged Then
                        Dim backFile As New FileInfo(warrantFileName + ".bck")
                        if backFile.Exists then
                            backFile.CopyTo(warrantFileName, True)
                        End If
                        response = "<strong class='alert-danger'>Error:</strong> Return of Service Failed. Please upload the file and try again."
                    Else
                        Dim tempWarrant As New FileInfo(outFile)
                        tempWarrant.CopyTo(warrantFileName, True)
                        response = "Return Service Attached Successfully"
                    End If
                    Dim oldbackFile As New FileInfo(warrantFileName + ".bck")
                    if oldbackFile.Exists then
                        oldbackFile.Delete()
                    End If
                    Dim tempFile As New FileInfo(outFile)
                    if tempfile.Exists then
                        tempFile.Delete()
                    End If
                    Dim attach As New FileInfo(rsfile)
                    if attach.Exists then
                        attach.Delete()
                    End If
                    return isMerged
                Catch ex As Exception
                    LogException(ex)
                    response = "<strong class='alert-danger'>Error:</strong> Unexpected Error Attaching Return of Service"
                    Return False
                End Try
            Else
                return false
            End If
            Return True
        End Function

        Private Function MergePdfs(ByVal FirstFilePath As String, ByVal SecondFilePath As String, ByVal OutputPath As String) As Boolean
            Try
                Extensions.AddPdfDecoder()
                Dim inFiles As String() = New String() {FirstFilePath, SecondFilePath}
                Dim opts As Atalasoft.PdfDoc.Repair.RepairOptions = New Atalasoft.PdfDoc.Repair.RepairOptions()
                opts.StructureOptions.ForceRebuildCrossReferenceTable = True
                opts.StructureOptions.RestoreOrphanedPages = False

                Warrants.PdfDocCombineWithRepair.Combine(OutputPath, inFiles, opts)
                Return True

            Catch ex As Exception
                LogException(ex)
                Return False
            End Try
        End Function

    End Class
End Namespace