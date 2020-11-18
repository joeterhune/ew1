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

Namespace AWS.Modules.Notifications

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



#Region "Notification Methods"

        Public Overrides Function AddNotification(requestingJudgeId As Integer, coveringJudgeId As Integer, startDateTime As Date, endDateTime As Date, messageText As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_add_notification", requestingJudgeId, GetNull(coveringJudgeId), startDateTime, endDateTime, GetNull(messageText)), Integer)
        End Function
        Public Overrides Sub DeleteNotification(scheduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_delete_notification", scheduleId)
        End Sub

        Public Overrides Function ListCurrentNotifications() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_list_current_notifications"), IDataReader)
        End Function

        Public Overrides Function ListCurrentNotificationsByJudge(requestingJudgeId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_list_current_notifications_by_judge", requestingJudgeId), IDataReader)
        End Function

        Public Overrides Function ListMyNotifications(requestingJudgeId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_list_my_notifications", requestingJudgeId), IDataReader)
        End Function
        Public Overrides Function ListConflictingNotifications(judgeId As Integer, startDate As Date, endDate As Date) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "aws_notification_list_notification_conflicts", judgeId, startDate, endDate), IDataReader)
        End Function
#End Region



    End Class

End Namespace
