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
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke

Namespace AWS.Modules.Injunctions

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

#Region "Injunction Methods"
        Public Overrides Function GetInjunction(ByVal injunctionId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_injunction", injunctionId), IDataReader)
        End Function

        Public Overrides Function GetInjunctionByFileId(ByVal fileId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_injunction_by_fileId", fileId), IDataReader)
        End Function

        Public Overrides Function ListInjunctions(ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_injunctions", ModuleId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListInjunctionsSigned(ModuleId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_injunctions_Signed", ModuleId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListReviewedInjunctions(cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_reviewed_injunctions", cutoffDate), IDataReader)
        End Function

        Public Overrides Function ListInjunctionsBySergeant(ModuleId As Integer, userId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_injunctions_by_sergeant", ModuleId, userId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListInjunctionsByUser(ModuleId As Integer, userid As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_injunctions_by_user", ModuleId, userid, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListInjunctionsByAgency(ModuleId As Integer, AgencyId As Integer, startDate As DateTime, endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_injunctions_by_agency", ModuleId, AgencyId, startDate, endDate), IDataReader)
        End Function

        Public Overrides Function ListInjunctionsNotificationNotSent() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_notification_not_sent"), IDataReader)
        End Function

        Public Overrides Function AddInjunction(ModuleId As Integer, ByVal agencyId As Integer, ByVal createdByUserId As Integer, ByVal createdDate As DateTime, title As String, ByVal statusId As InjunctionStatus, ByVal reviewedDate As DateTime, ByVal judgeUserId As Integer, fileId As Integer, InjunctionType As Integer, defendant As String, notificationSent As Boolean, notificationSentDate As DateTime) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_injunction", ModuleId, GetNull(agencyId), GetNull(createdByUserId), GetNull(createdDate), GetNull(title), GetNull(statusId), GetNull(reviewedDate), GetNull(judgeUserId), GetNull(fileId), GetNull(InjunctionType), GetNull(defendant), notificationSent, GetNull(notificationSentDate)), Integer)
        End Function

        Public Overrides Sub UpdateInjunction(ByVal injunctionId As Integer, ByVal statusId As InjunctionStatus, ByVal judgeId As Integer, ByVal reviewedDate As DateTime, ByVal isMarked As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_injunction", injunctionId, GetNull(statusId), GetNull(judgeId), GetNull(reviewedDate), GetNull(isMarked))
        End Sub

        Public Overrides Sub UpdateInjunctionReviewed(ByVal injunctionId As Integer, ByVal statusId As InjunctionStatus, agencyUserId As Integer, ByVal reviewedDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_injunction_reviewed", injunctionId, GetNull(statusId), GetNull(agencyUserId), GetNull(reviewedDate))
        End Sub

        Public Overrides Sub DeleteInjunction(ByVal injunctionId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_injunction", injunctionId)
        End Sub

        Public Overrides Sub DeleteOldInjunctions(ModuleId As Integer, ByVal cutoffDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_old_injunctions", ModuleId, cutoffDate)
        End Sub

        Public Overrides Function GetUnsignedInjunctions(ModuleId As Integer, cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_unsigned_injunctions", ModuleId, cutoffDate), IDataReader)
        End Function

        Public Overrides Function GetUnclaimedInjunctions(ModuleId As Integer, cutoffDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_unclaimed_injunctions", ModuleId, cutoffDate), IDataReader)
        End Function

        Public Overrides Function GetModuleSettings(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_injunction_settings", moduleId), IDataReader)
        End Function

        Public Overrides Function GetSettings() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_settings"), IDataReader)
        End Function

        Public Overrides Sub UpdateSettings(ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_settings", ModuleId, notificationEmail, senderEmail, deleteThreshold, signedThreshold, claimedThreshold, GetNull(demoMode), hours)
        End Sub

        Public Overrides Sub AddSettings(ModuleId As Integer, notificationEmail As String, senderEmail As String, deleteThreshold As Integer, signedThreshold As Integer, claimedThreshold As Integer, demoMode As Boolean, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_settings", ModuleId, notificationEmail, senderEmail, deleteThreshold, signedThreshold, claimedThreshold, GetNull(demoMode), hours)

        End Sub

        Public Overrides Sub UpdateNotificationSent(injunctionId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_notification_sent", injunctionId)
        End Sub

        Public Overrides Sub DeleteCompletedInjunctions(moduleId As Integer, hours As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_completed_injunctions", moduleId, hours)
        End Sub

#End Region

#Region "File Methods"
        Public Overrides Function InsertFile(bytes() As Byte, userid As Integer, uploadDate As DateTime, persist As Boolean, fileType As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_file", bytes, userid, uploadDate, persist, fileType), Integer)
        End Function

        Public Overrides Function GetFile(fileId As Integer, excludeBytes As Boolean) As IDataReader
            If excludeBytes Then
                Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_fileInfo", fileId), IDataReader)
            End If
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_file", fileId), IDataReader)
        End Function

        Public Overrides Sub DeleteFile(FileId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_file", FileId)
        End Sub

        Public Overrides Sub UpdateFile(FileId As Integer, bytes() As Byte)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_file", FileId, bytes)
        End Sub

        Public Overrides Function FileExists(fileId As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_fileExists", fileId), Integer)
        End Function

        Public Overrides Sub UpdateInjunctionFileId(injunctionId As Integer, fileId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_injunction_fileID", injunctionId, fileId)
        End Sub
#End Region

#Region "Agency Methods"
        Public Overrides Function GetAgency(ByVal agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_agency", agencyId), IDataReader)
        End Function

        Public Overrides Function ListAgencies(ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_agencies", ModuleId), IDataReader)
        End Function

        Public Overrides Function AddAgency(ModuleId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, IsClerk As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_agency", ModuleId, GetNull(agencyName), GetNull(emailAddress), GetNull(parentAgency), GetNull(IsClerk)), Integer)
        End Function

        Public Overrides Sub UpdateAgency(ByVal agencyId As Integer, ByVal agencyName As String, emailAddress As String, parentAgency As String, IsClerk As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_agency", agencyId, GetNull(agencyName), GetNull(emailAddress), GetNull(parentAgency), GetNull(IsClerk))
        End Sub

        Public Overrides Sub DeleteAgency(ByVal agencyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_agency", agencyId)
        End Sub

        Public Overrides Function GetCountiesByAgency(agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_counties_by_agency", agencyId), IDataReader)
        End Function

        Public Overrides Sub AddAgencyCounty(agencyId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_agency_county", agencyId, countyName)
        End Sub

        Public Overrides Sub DeleteAgencyCounties(agencyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_agency_counties", agencyId)
        End Sub

#End Region

#Region "UserByAgency Methods"
        Public Overrides Function ListAgencyUsers(agencyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_agency_users", agencyId), IDataReader)
        End Function

        Public Overrides Function GetUser(ModuleId As Integer, userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_user", ModuleId, userId), IDataReader)
        End Function

        Public Overrides Function GetAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_agency_user", agencyId, userId), IDataReader)
        End Function

        Public Overrides Sub AddAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_agency_user", agencyId, userId, isAdmin)
        End Sub

        Public Overrides Sub UpdateAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_agency_user", agencyId, userId, isAdmin)
        End Sub

        Public Overrides Sub DeleteAgencyUser(ByVal agencyId As Integer, ByVal userId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_agency_user", agencyId, userId)
        End Sub

          Public Overrides Function ListUserAgencies(moduleId As Integer, userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_user", ModuleId, userId), IDataReader)
        End Function
#End Region

#Region "Judge Methods"
        Public Overrides Function ListJudges(ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_judges", ModuleId), IDataReader)
        End Function

        Public Overrides Function AddJudge(ModuleId As Integer, judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime) As Integer
            Return CType(SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_judge", ModuleId, judgeId, signature, initial, judgeTypeId, dayStart, dayEnd), Integer)
        End Function

        Public Overrides Sub DeleteJudge(judgeId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_judge", judgeId)
        End Sub

        Public Overrides Function GetJudge(judgeId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_judge", judgeId), IDataReader)
        End Function

        Public Overrides Function GetJudgeImage(judgeId As Integer, ImageType As Integer) As Byte()
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_judge_image", judgeId, ImageType), Byte())
        End Function

        Public Overrides Sub UpdateJudge(judgeId As Integer, signature As Integer, initial As Integer, judgeTypeId As Integer, dayStart As DateTime, dayEnd As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_judge", judgeId, signature, initial, judgeTypeId, dayStart, dayEnd)
        End Sub

        Public Overrides Sub UpdateJudgeApproval(judgeId As Integer, approved As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_approve_judge", judgeId, approved)
        End Sub

#End Region

#Region " Log Events "
        Public Overrides Sub AddEvent(injunctionId As Integer, eventTime As Date, eventDescription As String, userid As Integer, ip As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_event", injunctionId, GetNull(eventTime), eventDescription, GetNull(userid), ip)
        End Sub
        Public Overrides Function GetAgencyEvents(AgencyId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_agency_events", AgencyId, startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetAllEvents(startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_all_events", startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetUserEvents(UserId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_user_events", UserId, startDate, endDate), IDataReader)
        End Function
        Public Overrides Function GetInjunctionEvents(injunctionId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_injunction_events", injunctionId), IDataReader)
        End Function

#End Region

#Region "County Methods"
        Public Overrides Sub AddCounty(moduleId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_county", moduleId, GetNull(countyName))
        End Sub

        Public Overrides Sub DeleteCounty(countyId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_county", countyId)
        End Sub

        Public Overrides Sub UpdateCounty(countyId As Integer, moduleId As Integer, countyName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_county", countyId, moduleId, GetNull(countyName))
        End Sub

        Public Overrides Function GetCounty(countyId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_county", countyId), IDataReader)
        End Function

        Public Overrides Function ListCounties(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_counties", moduleId), IDataReader)
        End Function

        ' following multiple counties per judge
        Public Overrides Sub AddJudgeCounty(judgeId As Integer, countyid As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_judge_by_county", judgeId, countyid)
        End Sub

        Public Overrides Sub DeleteJudgeCounty(judgeId As Integer, moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_judge_by_county", judgeId, moduleId)
        End Sub

        Public Overrides Function ListCountiesByJudge(judgeId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_counties_by_judge", judgeId, moduleId), IDataReader)
        End Function

        Public Overrides Function ListJudgesByCounty(countyId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_judges_by_county", countyId, moduleId), IDataReader)
        End Function

#End Region

#Region "Judge Type Methods"
        Public Overrides Sub AddJudgeType(moduleId As Integer, judgeType As String, judgeTypeCode As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_judge_type", moduleId, GetNull(judgeType), GetNull(judgeTypeCode))
        End Sub

        Public Overrides Sub DeleteJudgeType(judgeTypeID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_judge_type", judgeTypeID)
        End Sub

        Public Overrides Sub UpdateJudgeType(judgeTypeID As Integer, judgeType As String, judgeTypeCode As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_update_judge_type", judgeTypeID, GetNull(judgeType), GetNull(judgeTypeCode))
        End Sub

        Public Overrides Function GetJudgeType(judgeTypeID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_judge_type", judgeTypeID), IDataReader)
        End Function

        Public Overrides Function ListJudgeTypes(moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_judge_types", moduleId), IDataReader)

        End Function

        ''' Following added for multiple Divisions per Judge
        Public Overrides Function ListJudgeTypesByJudge(judgeid As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_judge_types_by_judge", judgeid, moduleId), IDataReader)
        End Function

        Public Overrides Sub AddJudgeJudgeTypeXref(judgeId As Integer, judgeTypeId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_judge_by_judge_type", judgeId, judgeTypeId)
        End Sub

        Public Overrides Sub DeleteJudgeJudgeTypeXref(judgeId As Integer, moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_judge_by_judge_type", judgeId, moduleId)
        End Sub

        Public Overrides Function ListJudgesByJudgeType(judgeTypeId As Integer, moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_list_judges_by_judge_type", judgeTypeId, moduleId), IDataReader)
        End Function

#End Region

#Region "Annotation Methods"
        Public Overrides Sub AddAnnotationText(userId As Integer, text As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_add_annotation", userId, text)
        End Sub
        Public Overrides Sub DeleteAnnotaion(annotationId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_delete_annotation", annotationId)
        End Sub
        Public Overrides Function GetAnnotations(userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_injunction_get_annotations", userId), IDataReader)
        End Function

      
#End Region

    End Class

End Namespace
