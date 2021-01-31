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


Imports System
Imports System.Data
Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities

Namespace AWS.Modules.Injunctions

    ''' <summary>
    ''' The Controller class for Injunctions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Controller

#Region "Injunctions Methods"
        Public Function GetInjunctions(ByVal InjunctionId As Integer) As InjunctionsInfo
            Return CBO.FillObject(Of InjunctionsInfo)(DataProvider.Instance().GetInjunction(InjunctionId))
        End Function

        Public Function GetInjunctionByFileId(ByVal fileId As Integer) As InjunctionsInfo
            '    Return CType(CBO.FillObject(DataProvider.Instance().GetInjunctionByFileId(fileId), GetType(InjunctionsInfo)), InjunctionsInfo)
            Return CBO.FillObject(Of InjunctionsInfo)(DataProvider.Instance().GetInjunctionByFileId(fileId))
        End Function

        Public Function ListInjunctions(moduleId As Integer, startDate As DateTime, endDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctions(moduleId, startDate, endDate))
        End Function

        Public Function ListInjunctionsSigned(moduleId As Integer, startDate As DateTime, endDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctionsSigned(moduleId, startDate, endDate))
        End Function

        Public Function ListReviewedInjunctions(cutoffDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListReviewedInjunctions(cutoffDate))
        End Function

        Public Function ListInjunctionsBySergeant(moduleId As Integer, userId As Integer, startDate As DateTime, endDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctionsBySergeant(moduleId, userId, startDate, endDate))
        End Function

        Public Function ListInjunctionsByUser(moduleId As Integer, userId As Integer, startDate As DateTime, endDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctionsByUser(moduleId, userId, startDate, endDate))
        End Function

        Public Function ListInjunctionsByAgency(moduleId As Integer, agencyId As Integer, startDate As DateTime, endDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctionsByAgency(moduleId, agencyId, startDate, endDate))
        End Function

        Public Function ListInjunctionsNotificationNotSent() As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().ListInjunctionsNotificationNotSent())
        End Function

        Public Function AddInjunction(ByVal objInjunction As InjunctionsInfo) As Integer
            Dim InjunctionId As Integer = Null.NullInteger
            InjunctionId = CType(DataProvider.Instance().AddInjunction(objInjunction.ModuleId, objInjunction.AgencyId, objInjunction.CreatedByUserId, objInjunction.CreatedDate, objInjunction.Title, objInjunction.StatusId, objInjunction.ReviewedDate, objInjunction.JudgeUserId, objInjunction.FileId, objInjunction.InjunctionType, objInjunction.Defendant, objInjunction.NotificationSent, objInjunction.NotificationSentDate), Integer)
            Return InjunctionId
        End Function

        Public Sub UpdateInjunctions(ByVal objInjunction As InjunctionsInfo)
            DataProvider.Instance().UpdateInjunction(objInjunction.InjunctionId, objInjunction.StatusId, objInjunction.ReviewedByJudgeUserId, objInjunction.ReviewedDate, objInjunction.IsMarked)
        End Sub

        Public Sub UpdateNotificationSent(InjunctionId As Integer)
            DataProvider.Instance().UpdateNotificationSent(InjunctionId)
        End Sub
        Public Sub UpdateInjunctionsReviewed(ByVal objInjunction As InjunctionsInfo)
            DataProvider.Instance().UpdateInjunctionReviewed(objInjunction.InjunctionId, objInjunction.StatusId, objInjunction.ReviewedByAgencyUserId, objInjunction.ReviewedByAgencyDate)
        End Sub

        Public Sub DeleteInjunctions(ByVal InjunctionId As Integer)
            DataProvider.Instance().DeleteInjunction(InjunctionId)
        End Sub

        Public Sub DeleteOldInjunctions(moduleId As Integer, cutoffDate As DateTime)
            DataProvider.Instance().DeleteOldInjunctions(moduleId, cutoffDate)
        End Sub

        Public Function GetUnsignedInjunctions(moduleId As Integer, cutoffDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().GetUnsignedInjunctions(moduleId, cutoffDate))
        End Function

        Public Function GetUnclaimedInjunctions(moduleid As Integer, cutoffDate As DateTime) As List(Of InjunctionsInfo)
            Return CBO.FillCollection(Of InjunctionsInfo)(DataProvider.Instance().GetUnclaimedInjunctions(moduleid, cutoffDate))
        End Function

        Public Function GetSettings() As List(Of InjunctionConfigSettings)
            Return CBO.FillCollection(Of InjunctionConfigSettings)(DataProvider.Instance().GetSettings)
        End Function


        Public Function GetModuleSettings(moduleId As Integer) As InjunctionConfigSettings
            '    Return CType(CBO.FillObject(DataProvider.Instance().GetModuleSettings(moduleId), GetType(InjunctionConfigSettings)), InjunctionConfigSettings)
            Return CBO.FillObject(Of InjunctionConfigSettings)(DataProvider.Instance().GetModuleSettings(moduleId))
        End Function

        Public Sub UpdateInjunctionSettings(objSettings As InjunctionConfigSettings)
            DataProvider.Instance().UpdateSettings(objSettings.ModuleId, objSettings.NotificationEmail, objSettings.SenderEmail, objSettings.DeleteThreshold, objSettings.SignedThreshold, objSettings.ClaimedThreshold, objSettings.DemoMode, objSettings.Hours)
        End Sub

        Public Sub AddInjunctionSettings(objSettings As InjunctionConfigSettings)
            DataProvider.Instance().AddSettings(objSettings.ModuleId, objSettings.NotificationEmail, objSettings.SenderEmail, objSettings.DeleteThreshold, objSettings.SignedThreshold, objSettings.ClaimedThreshold, objSettings.DemoMode, objSettings.Hours)
        End Sub

        Public Sub DeleteCompletedInjunctions(moduleId As Integer, hours As Integer)
            DataProvider.Instance().DeleteCompletedInjunctions(moduleId, hours)
        End Sub

        Public Sub UpdateInjunctionFileId(injunctionId As Integer, fileId As Integer)
            DataProvider.Instance().UpdateInjunctionFileId(injunctionId, fileId)
        End Sub

#End Region

#Region "File Methods"
        Public Function InsertFile(bytes As Byte(), userid As Integer, persist As Boolean, fileType As Integer) As Integer
            Return CType(DataProvider.Instance().InsertFile(bytes, userid, Date.Now(), persist, fileType), Integer)
        End Function

        Public Function GetFile(fileId As Integer) As InjunctionImage
            Return CBO.FillObject(Of InjunctionImage)(DataProvider.Instance().GetFile(fileId, False))
        End Function

        Public Function GetFile(fileId As Integer, excludeBytes As Boolean) As InjunctionImage
            Return CBO.FillObject(Of InjunctionImage)(DataProvider.Instance().GetFile(fileId, excludeBytes))
        End Function

        Public Sub DeleteFile(FileID As Integer)
            DataProvider.Instance().DeleteFile(FileID)
        End Sub

        Public Sub UpdateFile(fileId As Integer, bytes As Byte())
            DataProvider.Instance().UpdateFile(fileId, bytes)
        End Sub

        Public Function FileExists(fileId As Integer) As Integer
            Return CType(DataProvider.Instance().FileExists(fileId), Integer)
        End Function

#End Region

#Region "Agecy Methods"
        Public Function GetAgency(ByVal agencyId As Integer) As AgencyInfo
            ' Return CType(CBO.FillObject, GetType(AgencyInfo)), AgencyInfo)
            Return CBO.FillObject(Of AgencyInfo)(DataProvider.Instance().GetAgency(agencyId))
        End Function

        Public Function ListAgencies(moduleId As Integer) As List(Of AgencyInfo)
            Return CBO.FillCollection(Of AgencyInfo)(DataProvider.Instance().ListAgencies(moduleId))
        End Function

        Public Function AddAgency(ByVal objAgencies As AgencyInfo) As Integer
            Return CType(DataProvider.Instance().AddAgency(objAgencies.ModuleId, objAgencies.AgencyName, objAgencies.EmailAddress, objAgencies.ParentAgency, objAgencies.IsClerk), Integer)
        End Function

        Public Sub UpdateAgency(ByVal objAgencies As AgencyInfo)
            DataProvider.Instance().UpdateAgency(objAgencies.AgencyId, objAgencies.AgencyName, objAgencies.EmailAddress, objAgencies.ParentAgency, objAgencies.IsClerk)
        End Sub

        Public Sub DeleteAgency(ByVal agencyId As Integer)
            DataProvider.Instance().DeleteAgency(agencyId)
        End Sub
        Public Function GetCountiesByAgency(agencyId As Integer) As List(Of AgencyCounty)
            Return CBO.FillCollection(Of AgencyCounty)(DataProvider.Instance().GetCountiesByAgency(agencyId))
        End Function

        Public Sub DeleteAgencyCounties(agencyId As Integer)
            DataProvider.Instance().DeleteAgencyCounties(agencyId)
        End Sub

        Public Sub AddAgencyCounty(objAgencyCounty As AgencyCounty)
            DataProvider.Instance().AddAgencyCounty(objAgencyCounty.AgencyId, objAgencyCounty.CountyName)
        End Sub

#End Region

#Region "User Agency"
        Public Function ListAgencyUsers(agencyId As Integer) As List(Of AgencyUserInfo)
            Return CBO.FillCollection(Of AgencyUserInfo)(DataProvider.Instance().ListAgencyUsers(agencyId))
        End Function

        Public Function GetUser(moduleId As Integer, ByVal userId As Integer) As AgencyUserInfo
            'Return CType(CBO.FillObject(DataProvider.Instance().GetUser(moduleId, userId), GetType(AgencyUserInfo)), AgencyUserInfo)
            Return CBO.FillObject(Of AgencyUserInfo)(DataProvider.Instance().GetUser(moduleId, userId))
        End Function

        Public Function GetAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer) As AgencyUserInfo
            '    Return CType(CBO.FillObject(DataProvider.Instance().GetAgencyUser(agencyId, userId), GetType(AgencyUserInfo)), AgencyUserInfo)
            Return CBO.FillObject(Of AgencyUserInfo)(DataProvider.Instance().GetAgencyUser(agencyId, userId))
        End Function

        Public Sub AddAgencyUser(ByVal objAgencyUser As AgencyUserInfo)
            DataProvider.Instance().AddAgencyUser(objAgencyUser.AgencyId, objAgencyUser.UserId, objAgencyUser.IsAdmin)
        End Sub

        Public Sub UpdateAgencyUser(ByVal objAgencyUser As AgencyUserInfo)
            DataProvider.Instance().UpdateAgencyUser(objAgencyUser.AgencyId, objAgencyUser.UserId, objAgencyUser.IsAdmin)
        End Sub

        Public Sub DeleteAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer)
            DataProvider.Instance().DeleteAgencyUser(agencyId, userId)
        End Sub
         Public Function ListUserAgencies(moduleId As Integer, userId As Integer) As List(Of AgencyUserInfo)
             Return CBO.FillCollection(Of AgencyUserInfo)(DataProvider.Instance().ListUserAgencies(moduleId,userId))
        End Function
#End Region

#Region "Judge Methods"
        Public Function GetJudge(judgeId As Integer) As JudgeInfo
            Return CBO.FillObject(Of JudgeInfo)(DataProvider.Instance().GetJudge(judgeId))
        End Function

        Public Function AddJudge(objJudge As JudgeInfo) As Integer
            Return CType(DataProvider.Instance().AddJudge(objJudge.ModuleId, objJudge.JudgeId, objJudge.Signature, objJudge.Initial, objJudge.JudgeTypeId, objJudge.DayStart, objJudge.DayEnd), Integer)
        End Function

        Public Sub UpdateJudge(objJudge As JudgeInfo)
            DataProvider.Instance().UpdateJudge(objJudge.JudgeId, objJudge.Signature, objJudge.Initial, objJudge.JudgeTypeId, objJudge.DayStart, objJudge.DayEnd)
        End Sub

        Public Sub ApproveJudge(judgeId As Integer, ApprovalCode As String)
            DataProvider.Instance().UpdateJudgeApproval(judgeId, ApprovalCode)
        End Sub

        Public Sub DeleteJudge(judgeId As Integer)
            DataProvider.Instance().DeleteJudge(judgeId)
        End Sub

        Public Function GetJudgeImage(judgeId As Integer, ImageType As FileType) As Byte()
            Return CType(DataProvider.Instance.GetJudgeImage(judgeId, ImageType), Byte())
        End Function

        Public Function ListJudges(moduleId As Integer) As List(Of JudgeInfo)
            Return CBO.FillCollection(Of JudgeInfo)(DataProvider.Instance().ListJudges(moduleId))
        End Function


#End Region

#Region " Log Methods"

        Public Sub AddEvent(objLogEvent As LogInfo)
            DataProvider.Instance.AddEvent(objLogEvent.InjunctionId, objLogEvent.EventTime, objLogEvent.EventDescription, objLogEvent.UserId, objLogEvent.IP)
        End Sub

        Public Function GetAgencyEvents(agencyId As Integer, startdate As DateTime, endDate As DateTime) As IDataReader
            Return DataProvider.Instance().GetAgencyEvents(agencyId, startdate, endDate)
        End Function

        Public Function GetUserEvents(userid As Integer, startdate As DateTime, endDate As DateTime) As IDataReader
            Return DataProvider.Instance().GetAgencyEvents(userid, startdate, endDate)
        End Function

        Public Function GetAllEvents(startdate As DateTime, endDate As DateTime) As IDataReader
            Return DataProvider.Instance().GetAllEvents(startdate, endDate)
        End Function

        Public Function GetInjunctionEvents(InjunctionId As Integer) As IDataReader
            Return DataProvider.Instance().GetInjunctionEvents(InjunctionId)
        End Function

#End Region

#Region "County Methods"
        Public Function GetCounty(ByVal countyId As Integer) As CountyInfo
            Return CBO.FillObject(Of CountyInfo)(DataProvider.Instance().GetCounty(countyId))
        End Function

        Public Function ListCounties(moduleId As Integer) As List(Of CountyInfo)
            Return CBO.FillCollection(Of CountyInfo)(DataProvider.Instance().ListCounties(moduleId))
        End Function

        Public Sub AddCounty(ByVal objCounties As CountyInfo)
            DataProvider.Instance().AddCounty(objCounties.ModuleID, objCounties.CountyName)
        End Sub

        Public Sub UpdateCounty(ByVal objCounties As CountyInfo)
            DataProvider.Instance().UpdateCounty(objCounties.CountyID, objCounties.ModuleID, objCounties.CountyName)
        End Sub

        Public Sub DeleteCounty(ByVal countyId As Integer)
            DataProvider.Instance().DeleteCounty(countyId)
        End Sub

        ' following multiple counties per judge
        Public Function ListJudgeCounties(judgeId As Integer, moduleId As Integer) As List(Of CountyInfo)
            Return CBO.FillCollection(Of CountyInfo)(DataProvider.Instance().ListCountiesByJudge(judgeId, moduleId))
        End Function

        Public Function ListJudgesByCounty(countyId As Integer, moduleId As Integer) As List(Of JudgeInfo)
            Return CBO.FillCollection(Of JudgeInfo)(DataProvider.Instance().ListJudgesByCounty(countyId, moduleId))
        End Function

        Public Sub DeleteJudgeCounties(judgeid As Integer, moduleId As Integer)
            DataProvider.Instance().DeleteJudgeCounty(judgeid, moduleId)
        End Sub

        Public Sub AddJudgeCounty(judgeId As Integer, countyId As Integer)
            DataProvider.Instance().AddJudgeCounty(judgeId, countyId)
        End Sub

#End Region

#Region "Judge Type Methods"
        Public Function GetJudgeType(ByVal judgeTypeID As Integer) As JudgeTypeInfo
            Return CBO.FillObject(Of JudgeTypeInfo)(DataProvider.Instance().GetJudgeType(judgeTypeID))
        End Function

        Public Function ListJudgeTypes(moduleId As Integer) As List(Of JudgeTypeInfo)
            Return CBO.FillCollection(Of JudgeTypeInfo)(DataProvider.Instance().ListJudgeTypes(moduleId))
        End Function

        Public Sub AddJudgeType(ByVal objJudgeTypeInfo As JudgeTypeInfo)
            DataProvider.Instance().AddJudgeType(objJudgeTypeInfo.ModuleID, objJudgeTypeInfo.JudgeType, objJudgeTypeInfo.JudgeTypeCode)
        End Sub

        Public Sub UpdateJudgeType(ByVal objJudgeType As JudgeTypeInfo)
            DataProvider.Instance().UpdateJudgeType(objJudgeType.JudgeTypeID, objJudgeType.JudgeType, objJudgeType.JudgeTypeCode)
        End Sub

        Public Sub DeleteJudgeType(ByVal judgeTypeId As Integer)
            DataProvider.Instance().DeleteJudgeType(judgeTypeId)
        End Sub

        ''' Add for multiple divsions by judge
        Public Sub AddJudgeJudgeTypeXref(judgeId As Integer, judgeTypeId As Integer)
            DataProvider.Instance().AddJudgeJudgeTypeXref(judgeId, judgeTypeId)
        End Sub

        Public Sub DeleteJudgeJudgeTypeXref(judgeId As Integer, moduleId As Integer)
            DataProvider.Instance().DeleteJudgeJudgeTypeXref(judgeId, moduleId)
        End Sub

        Public Function ListJudgesByJudgeType(judgeTypeId As Integer, moduleId As Integer) As List(Of JudgeInfo)
            Dim mCtl As New Entities.Modules.ModuleController
            Dim warrantModule As Entities.Modules.ModuleInfo = mCtl.GetModule(moduleId)
            Dim InjunctionJudgerole As String = CType(warrantModule.TabModuleSettings("AdminJudge"), String)
            Dim judges As List(Of JudgeInfo) = CBO.FillCollection(Of JudgeInfo)(DataProvider.Instance().ListJudgesByJudgeType(judgeTypeId, moduleId))
            Dim outJudgea As New List(Of JudgeInfo)
            For Each j As JudgeInfo In judges
                If IsApproved(j.Approved, j.JudgeId, InjunctionJudgerole) Then
                    outJudgea.Add(j)
                End If
            Next
            Return outJudgea
        End Function

        Public Function ListJudgeTypesByJudge(judgeId As Integer, moduleId As Integer) As List(Of JudgeTypeInfo)
            Return CBO.FillCollection(Of JudgeTypeInfo)(DataProvider.Instance().ListJudgeTypesByJudge(judgeId, moduleId))
        End Function

        Private Function IsApproved(encryptedText As String, judgeId As String, AdminJudgeRole As String) As Boolean
            Dim approved As Boolean = False
            Dim portalId As Integer = PortalSettings.Current.PortalId
            Dim judgeInfo As UserInfo = UserController.GetUserById(portalId, judgeId)
            If Not judgeInfo Is Nothing Then
                Dim tempJudgeId As String = ""
                Dim encrypt As New Encryptor
                Try
                    tempJudgeId = encrypt.QueryStringDecode(encryptedText, AdminJudgeRole)
                Catch
                End Try
                If tempJudgeId = judgeId Then
                    approved = True
                End If
            End If
            Return approved
        End Function

       

#End Region

#Region "Annotation Methods"
        Public Function GetAnnotations(userId As Integer) As List(Of AnnotationInfo)
            Return CBO.FillCollection(Of AnnotationInfo)(DataProvider.Instance().GetAnnotations(userId))
        End Function
        Public Sub AddAnnotation(userid As Integer, text As String)
            DataProvider.Instance().AddAnnotationText(userid, text)
        End Sub
        Public Sub DeleteAnnotation(annotationId As Integer)
            DataProvider.Instance().DeleteAnnotaion(annotationId)
        End Sub
#End Region


    End Class
End Namespace
