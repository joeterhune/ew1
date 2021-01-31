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
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke

Namespace AWS.Modules.Warrants

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' SQL Server implementation of the abstract DataProvider class
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SqlDataProvider
        Inherits DataProvider

#Region "Private Members"

        Private Const ProviderType As String = "data"
        Private Const ModuleQualifier As String = "aws_"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            'Get Connection string from web.config
            _connectionString = Config.GetConnectionString()

            If _connectionString = "" Then
                ' Use connection string specified in provider
                _connectionString = objProvider.Attributes("connectionString")
            End If

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetFullyQualifiedName(ByVal name As String) As String
            Return DatabaseOwner & ObjectQualifier & ModuleQualifier & name
        End Function

        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

#End Region

#Region "Warrants Methods"
        Public Overrides Function GetWarrant(ByVal warrantId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_warrant", warrantId), IDataReader)
        End Function

        Public Overrides Function GetWarrantByFileId(ByVal fileId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_warrant_by_fileid", fileId), IDataReader)
        End Function

        Public Overrides Function ListWarrants(ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants", ModuleId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListWarrantsSigned(ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants_Signed", ModuleId, startDate, endDate), IDataReader)
        End Function


        Public Overrides Function ListReviewedWarrants(cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_reviewed_warrants", cutoffDate), IDataReader)
        End Function

        Public Overrides Function ListWarrantsBySergeant(ModuleId As Integer, userId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants_by_sergeant", ModuleId, userId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListWarrantsByUser(ModuleId As Integer, userid As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants_by_user", ModuleId, userid, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListWarrantsByAgency(ModuleId As Integer, AgencyId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants_by_agency", ModuleId, AgencyId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListWarrantsNotificationNotSent() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_warrants_notification_not_sent"), IDataReader)
        End Function

        Public Overrides Function AddWarrant(ModuleId As Integer, ByVal agencyId As Integer, ByVal createdByUserId As Integer, ByVal createdDate As DateTime, title As String, ByVal statusId As WarrantStatus, ByVal reviewedDate As DateTime, ByVal judgeUserId As Integer, fileId As Integer, warrantType As WarrantType, ByVal saApproved As Boolean, saName As String, defendant As String, notificationSent As Boolean, notificationSentDate As DateTime) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_warrant", ModuleId, GetNull(agencyId), GetNull(createdByUserId), GetNull(createdDate), GetNull(title), GetNull(statusId), GetNull(reviewedDate), GetNull(judgeUserId), GetNull(fileId), GetNull(warrantType), GetNull(saApproved), GetNull(saName), GetNull(defendant), notificationSent, GetNull(notificationSentDate)), Integer)
        End Function

        Public Overrides Sub UpdateWarrant(ByVal warrantId As Integer, ByVal statusId As WarrantStatus, judgeId As Integer, ByVal reviewedDate As DateTime, ByVal isMarked As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_warrant", warrantId, GetNull(statusId), GetNull(judgeId), GetNull(reviewedDate), GetNull(isMarked))
        End Sub
        Public Overrides Sub UpdateWarrantReviewed(ByVal warrantId As Integer, ByVal statusId As WarrantStatus, agencyUserId As Integer, ByVal reviewedDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_warrant_reviewed", warrantId, GetNull(statusId), GetNull(agencyUserId), GetNull(reviewedDate))
        End Sub

        Public Overrides Sub DeleteWarrant(ByVal warrantId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_warrant", warrantId)
        End Sub

        Public Overrides Sub DeleteOldWarrants(ModuleId As Integer, ByVal cutoffDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_old_warrants", ModuleId, cutoffDate)
        End Sub

        Public Overrides Function GetUnsignedWarrants(ModuleId As Integer, cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_unsigned_warrants", ModuleId, cutoffDate), IDataReader)
        End Function

        Public Overrides Function GetUnclaimedWarrants(ModuleId As Integer, cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_unclaimed_warrants", ModuleId, cutoffDate), IDataReader)
        End Function

        Public Overrides Function GetModuleSettings(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_warrant_settings", moduleId), IDataReader)
        End Function

        Public Overrides Function GetSettings() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_settings"), IDataReader)
        End Function

        Public Overrides Sub UpdateSettings(ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_settings", ModuleId, notificationEmail, senderEmail, deleteThreshold, signedThreshold, claimedThreshold, GetNull(demoMode), hours)
        End Sub

        Public Overrides Sub AddSettings(ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_settings", ModuleId, notificationEmail, senderEmail, deleteThreshold, signedThreshold, claimedThreshold, GetNull(demoMode), hours)

        End Sub

        Public Overrides Sub UpdateNotificationSent(warrantId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_notification_sent", warrantId)
        End Sub

        Public Overrides Sub DeleteCompletedWarrants(moduleId As Integer, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_completed_warrants", moduleId, hours)
        End Sub

        Public Overrides Sub UpdateWarrantFileId(warrantId As Integer, fileId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_warrant_fileID", warrantId, fileId)
        End Sub

#End Region

#Region "File Methods"
        Public Overrides Function InsertFile(bytes() As Byte, userid As Integer, uploadDate As DateTime, persist As Boolean, fileType As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_file", bytes, userid, uploadDate, persist, fileType), Integer)
        End Function

        Public Overrides Sub DeleteFile(fileId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_file", fileId)
        End Sub

        Public Overrides Sub UpdateFile(fileId As Integer, bytes() As Byte)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_file", fileId, bytes)
        End Sub

        Public Overrides Function FileExists(fileId As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_fileExists", fileId), Integer)
        End Function

        Public Overrides Function GetFile(fileId As Integer, excludeBytes As Boolean) As IDataReader
            If excludeBytes Then
                Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_fileInfo", fileId), IDataReader)
            End If
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_file", fileId), IDataReader)
        End Function


#End Region

#Region "Agency Methods"
        Public Overrides Function GetAgency(ByVal agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_agency", agencyId), IDataReader)
        End Function

        Public Overrides Function ListAgencies(ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_agencies", ModuleId), IDataReader)
        End Function

        Public Overrides Function AddAgency(ModuleId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, isClerk As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_agency", ModuleId, GetNull(agencyName), GetNull(emailAddress), GetNull(parentAgency), GetNull(isClerk)), Integer)
        End Function

        Public Overrides Sub UpdateAgency(ByVal agencyId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, isClerk As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_agency", agencyId, GetNull(agencyName), GetNull(emailAddress), GetNull(parentAgency), GetNull(isClerk))
        End Sub

        Public Overrides Sub DeleteAgency(ByVal agencyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_agency", agencyId)
        End Sub

        Public Overrides Function GetCountiesByAgency(agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_counties_by_agency", agencyId), IDataReader)
        End Function

        Public Overrides Sub AddAgencyCounty(agencyId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_agency_county", agencyId, countyName)
        End Sub

        Public Overrides Sub DeleteAgencyCounties(agencyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_agency_counties", agencyId)
        End Sub

#End Region

#Region "UserByAgency Methods"
        Public Overrides Function ListAgencyUsers(agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_agency_users", agencyId), IDataReader)
        End Function

        Public Overrides Function GetUser(ModuleId As Integer, userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_user", ModuleId, userId), IDataReader)
        End Function
        Public Overrides Function ListUserAgencies(ModuleId As Integer, userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_user", ModuleId, userId), IDataReader)
        End Function

        Public Overrides Function GetAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_agency_user", agencyId, userId), IDataReader)
        End Function

        Public Overrides Sub AddAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_agency_user", agencyId, userId, isAdmin)
        End Sub

        Public Overrides Sub UpdateAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_agency_user", agencyId, userId, isAdmin)
        End Sub

        Public Overrides Sub DeleteAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_agency_user", agencyId, userId)
        End Sub
#End Region

#Region "Judge Methods"
        Public Overrides Function ListJudges(ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_judges", ModuleId), IDataReader)
        End Function

        Public Overrides Function AddJudge(ModuleId As Integer, judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime) As Integer
            Return CType(SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_judge", ModuleId, judgeId, signature, initial, judgeTypeId, dayStart, dayEnd), Integer)
        End Function

        Public Overrides Sub DeleteJudge(judgeId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_judge", judgeId)
        End Sub

        Public Overrides Function GetJudge(judgeId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_judge", judgeId), IDataReader)
        End Function

        Public Overrides Function GetJudgeImage(judgeId As Integer, ImageType As Integer) As Byte()
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_judge_image", judgeId, ImageType), Byte())
        End Function

        Public Overrides Sub UpdateJudge(judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_judge", judgeId, signature, initial, judgeTypeId, dayStart, dayEnd)
        End Sub

        Public Overrides Sub UpdateJudgeApproval(judgeId As Integer, approved As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_approve_judge", judgeId, approved)
        End Sub

#End Region

#Region " Log Events "
        Public Overrides Sub AddEvent(warrantId As Integer, eventTime As Date, eventDescription As String, userid As Integer, ip As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_event", warrantId, GetNull(eventTime), eventDescription, GetNull(userid), ip)
        End Sub
        Public Overrides Function GetAgencyEvents(AgencyId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_agency_events", AgencyId, startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetAllEvents(startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_all_events", startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetUserEvents(UserId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_user_events", UserId, startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetWarrantEvents(WarrantId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_warrant_events", WarrantId), IDataReader)
        End Function

#End Region

#Region "County Methods"
        Public Overrides Sub AddCounty(moduleId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_county", moduleId, GetNull(countyName))
        End Sub

        Public Overrides Sub DeleteCounty(countyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_county", countyId)
        End Sub

        Public Overrides Sub UpdateCounty(countyId As Integer, moduleId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_county", countyId, moduleId, GetNull(countyName))
        End Sub

        Public Overrides Function GetCounty(countyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_county", countyId), IDataReader)
        End Function

        Public Overrides Function ListCounties(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_counties", moduleId), IDataReader)
        End Function

        'following multiple counties per judge
        Public Overrides Sub AddJudgeCounty(judgeId As Integer, countyid As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_judge_by_county", judgeId, countyid)
        End Sub

        Public Overrides Sub DeleteJudgeCounty(judgeId As Integer, moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_judge_by_county", judgeId, moduleId)
        End Sub

        Public Overrides Function ListCountiesByJudge(judgeId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_counties_by_judge", judgeId, moduleId), IDataReader)
        End Function

        Public Overrides Function ListJudgesByCounty(countyId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_judges_by_county", countyId, moduleId), IDataReader)
        End Function

#End Region

#Region "Judge Type Methods"
        Public Overrides Sub AddJudgeType(moduleId As Integer, judgeType As String, judgeTypeCode As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_judge_type", moduleId, GetNull(judgeType), GetNull(judgeTypeCode))
        End Sub

        Public Overrides Sub DeleteJudgeType(judgeTypeID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_judge_type", judgeTypeID)
        End Sub

        Public Overrides Sub UpdateJudgeType(judgeTypeID As Integer, judgeType As String, judgeTypeCode As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_update_judge_type", judgeTypeID, GetNull(judgeType), GetNull(judgeTypeCode))
        End Sub

        Public Overrides Function GetJudgeType(judgeTypeID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_judge_type", judgeTypeID), IDataReader)
        End Function

        Public Overrides Function ListJudgeTypes(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_judge_types", moduleId), IDataReader)

        End Function

        ''' Following added for multiple Divisions per Judge
        Public Overrides Function ListJudgeTypesByJudge(judgeid As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_judge_types_by_judge", judgeid, moduleId), IDataReader)
        End Function

        Public Overrides Sub AddJudgeJudgeTypeXref(judgeId As Integer, judgeTypeId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_judge_by_judge_type", judgeId, judgeTypeId)
        End Sub

        Public Overrides Sub DeleteJudgeJudgeTypeXref(judgeId As Integer, moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_judge_by_judge_type", judgeId, moduleId)
        End Sub

        Public Overrides Function ListJudgesByJudgeType(judgeTypeId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_list_judges_by_judge_type", judgeTypeId, moduleId), IDataReader)
        End Function

#End Region

#Region "Annotation Methods"
        Public Overrides Sub AddAnnotationText(userId As Integer, text As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_add_annotation", userId, text)
        End Sub
        Public Overrides Sub DeleteAnnotaion(annotationId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_delete_annotation", annotationId)
        End Sub
        Public Overrides Function GetAnnotations(userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_warrant_get_annotations", userId), IDataReader)
        End Function
#End Region

    End Class

End Namespace
