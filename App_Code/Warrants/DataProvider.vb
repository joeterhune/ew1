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
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.


Imports System
Imports System.Data
Imports DotNetNuke

Namespace AWS.Modules.Warrants

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An abstract class for the data access layer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "AWS.Modules.Warrants", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Warrants Methods"
        Public MustOverride Function GetWarrant(ByVal warrantId As Integer) As IDataReader
        Public MustOverride Function ListWarrants(ByVal ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function ListWarrantsSigned(ByVal ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader

        Public MustOverride Function ListReviewedWarrants(cutoffDate As DateTime) As IDataReader

        Public MustOverride Function ListWarrantsByUser(ByVal ModuleId As Integer, userid As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function ListWarrantsByAgency(ByVal ModuleId As Integer, agencyId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function ListWarrantsBySergeant(ByVal ModuleId As Integer, userId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function ListWarrantsNotificationNotSent() As IDataReader
        Public MustOverride Function AddWarrant(ByVal ModuleID As Integer, ByVal agencyId As Integer, ByVal createdByUserId As Integer, ByVal createdDate As DateTime, title As String, ByVal statusId As WarrantStatus, ByVal reviewedDate As DateTime, ByVal judgeUserId As Integer, fileId As Integer, warrantType As WarrantType, saApproved As Boolean, saName As String, defendant As String, notificationSent As Boolean, notificationSentDate As DateTime) As Integer
        Public MustOverride Sub UpdateWarrant(ByVal warrantId As Integer, ByVal statusId As WarrantStatus, judgeId As Integer, ByVal reviewedDate As DateTime, ByVal IsMarked As Boolean)
        Public MustOverride Sub UpdateWarrantReviewed(ByVal warrantId As Integer, ByVal statusId As WarrantStatus, agencyUserId As Integer, ByVal reviewedDate As DateTime)
        Public MustOverride Sub DeleteWarrant(ByVal warrantId As Integer)
        Public MustOverride Function GetWarrantByFileId(ByVal fileId As Integer) As IDataReader
        Public MustOverride Sub DeleteOldWarrants(ByVal ModuleId As Integer, ByVal cutoffDate As DateTime)
        Public MustOverride Function GetUnsignedWarrants(ModuleId As Integer, ByVal cutoffDate As DateTime) As IDataReader
        Public MustOverride Function GetUnclaimedWarrants(ModuleId As Integer, ByVal cutoffDate As DateTime) As IDataReader
        Public MustOverride Function GetSettings() As IDataReader
        Public MustOverride Function GetModuleSettings(moduleId As Integer) As IDataReader
        Public MustOverride Sub UpdateSettings(ByVal ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
        Public MustOverride Sub AddSettings(ByVal ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
        Public MustOverride Sub UpdateNotificationSent(warrantId As Integer)
        Public MustOverride Sub DeleteCompletedWarrants(ModuleId As Integer, hours As Integer)
        Public MustOverride Sub UpdateWarrantFileId(warrantId As Integer, fileId As Integer)

#End Region

#Region "Annotation Methods"
        Public MustOverride Sub AddAnnotationText(userId As Integer, text As String)
        Public MustOverride Sub DeleteAnnotaion(annotationId As Integer)
        Public MustOverride Function GetAnnotations(userId As Integer) As IDataReader

#End Region

#Region "File Methods"
        Public MustOverride Function InsertFile(bytes As Byte(), userId As Integer, uploadDate As DateTime, persist As Boolean, fileType As Integer) As Integer
        Public MustOverride Sub DeleteFile(fileId As Integer)
        Public MustOverride Sub UpdateFile(fileId As Integer, bytes As Byte())
        Public MustOverride Function FileExists(fileId As Integer) As Integer
        Public MustOverride Function GetFile(fileId As Integer, excludeBytes As Boolean) As IDataReader
#End Region

#Region "UserByAgency Methods"
        Public MustOverride Function GetAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer) As IDataReader
        Public MustOverride Function ListAgencyUsers(ByVal agencyId As Integer) As IDataReader
        Public MustOverride Sub AddAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
        Public MustOverride Sub UpdateAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
        Public MustOverride Sub DeleteAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer)
        Public MustOverride Function GetUser(moduleId As Integer, userId As Integer) As IDataReader
#End Region

#Region "Agencies Methods"
        Public MustOverride Function GetAgency(ByVal agencyId As Integer) As IDataReader
        Public MustOverride Function ListAgencies(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function AddAgency(ByVal ModuleId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, isClerk As Boolean) As Integer
        Public MustOverride Sub UpdateAgency(ByVal agencyId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, isClerk As Boolean)
        Public MustOverride Sub DeleteAgency(ByVal agencyId As Integer)
        Public MustOverride Function GetCountiesByAgency(agencyId As Integer) As IDataReader
        Public MustOverride Sub AddAgencyCounty(agencyId As Integer, countyName As String)
        Public MustOverride Sub DeleteAgencyCounties(agencyId As Integer)

#End Region

#Region "Judge Methods"
        Public MustOverride Function GetJudge(judgeId As Integer) As IDataReader
        Public MustOverride Function ListJudges(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function AddJudge(ByVal ModuleId As Integer, judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime) As integer
        Public MustOverride Sub UpdateJudge(judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime)
        Public MustOverride Sub DeleteJudge(judgeId As Integer)
        Public MustOverride Sub UpdateJudgeApproval(judgeId As Integer, approved As String)
        Public MustOverride Function GetJudgeImage(judgeId As Integer, ImageType As Integer) As Byte()

#End Region

#Region "Event Log"
        Public MustOverride Sub AddEvent(warrantId As Integer, eventTime As DateTime, eventDescription As String, userid As Integer, ip As String)
        Public MustOverride Function GetAgencyEvents(AgencyId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function GetWarrantEvents(WarrantId As Integer) As IDataReader
        Public MustOverride Function GetUserEvents(UserId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
        Public MustOverride Function GetAllEvents(startDate As DateTime, endDate As DateTime) As IDataReader
#End Region

#Region "County Methods"
        Public MustOverride Sub AddCounty(moduleId As Integer, countyName As String)
        Public MustOverride Sub UpdateCounty(countyId As Integer, moduleId As Integer, countyName As String)
        Public MustOverride Sub DeleteCounty(countyId As Integer)
        Public MustOverride Function GetCounty(countyId As Integer) As IDataReader
        Public MustOverride Function ListCounties(moduleId As Integer) As IDataReader
        ' multiple counties per judge
        Public MustOverride Sub AddJudgeCounty(judgeId As Integer, countyid As Integer)
        Public MustOverride Sub DeleteJudgeCounty(judgeId As Integer, moduleId As Integer)
        Public MustOverride Function ListCountiesByJudge(judgeId As Integer, moduleId As Integer) As IDataReader
        Public MustOverride Function ListJudgesByCounty(countyId As Integer, moduleId As Integer) As IDataReader

#End Region

#Region " Judge Type Methods "
        Public MustOverride Sub AddJudgeType(moduleId As Integer, judgeType As String, judgeTypeCode As String)
        Public MustOverride Sub UpdateJudgeType(judgeTypeID As Integer, judgeType As String, judgeTypeCode As String)
        Public MustOverride Sub DeleteJudgeType(judgeTypeID As Integer)
        Public MustOverride Function GetJudgeType(judgeTypeID As Integer) As IDataReader
        Public MustOverride Function ListJudgeTypes(moduleId As Integer) As IDataReader

        ''' Following Added for multiple divisions per Judge
        Public MustOverride Sub AddJudgeJudgeTypeXref(judgeId As Integer, judgeTypeId As Integer)
        Public MustOverride Sub DeleteJudgeJudgeTypeXref(judgeId As Integer, moduleId As Integer)
        Public MustOverride Function ListJudgesByJudgeType(judgeTypeId As Integer, moduleId As Integer) As IDataReader
        Public MustOverride Function ListJudgeTypesByJudge(judgeId As Integer, moduleId As Integer) As IDataReader

        Public MustOverride Function ListUserAgencies(moduleId As Integer, userid As Integer) As IDataReader
           
#End Region

    End Class

End Namespace
