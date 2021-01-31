Imports System
Imports System.Data

Namespace AWS.Modules.Warrants

    Public Class WarrantsInfo

#Region "Private Members"
        Private _warrantId As Integer
        Private _agencyId As Integer
        Private _createdByUserId As Integer
        Private _createdDate As DateTime
        Private _statusId As WarrantStatus
        Private _reviewedDate As DateTime
        Private _judgeUserId As Integer
        Private _fileId As Integer
        Private _judgeName As String
        Private _agencyName As String
        Private _createdByName As String
        Private _title As String
        Private _warrantType As WarrantType
        Private _moduleId As Integer
        Private _saApproved As Boolean
        Private _saName As String
        Private _reviewedByJudgeUserId As Integer
        Private _ReviewedByAgencyUserId As Integer
        Private _reviewedByAgencyDate As DateTime
        Private _defendant As String
        Private _notificationSent As Boolean
        Private _notificationSentDate As DateTime
        Private _county As String
        Private _isMarked As Boolean


#End Region

#Region "Constructors"
        Public Sub New()
        End Sub


#End Region

#Region "Public Properties"

        Public Property County() As String
            Get
                Return _county
            End Get
            Set(ByVal value As String)
                _county = value
            End Set
        End Property

        Public Property NotificationSentDate() As DateTime
            Get
                Return _notificationSentDate
            End Get
            Set(ByVal value As DateTime)
                _notificationSentDate = value
            End Set
        End Property
        Public Property NotificationSent() As Boolean
            Get
                Return _notificationSent
            End Get
            Set(ByVal value As Boolean)
                _notificationSent = value
            End Set
        End Property
        Public Property ReviewedByJudgeUserId() As Integer
            Get
                Return _reviewedByJudgeUserId
            End Get
            Set(ByVal value As Integer)
                _reviewedByJudgeUserId = value
            End Set
        End Property

        Public Property ReviewedByAgencyDate() As DateTime
            Get
                Return _reviewedByAgencyDate
            End Get
            Set(ByVal value As DateTime)
                _reviewedByAgencyDate = value
            End Set
        End Property

        Public Property ReviewedByAgencyUserId() As Integer
            Get
                Return _ReviewedByAgencyUserId
            End Get
            Set(ByVal value As Integer)
                _ReviewedByAgencyUserId = value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal value As Integer)
                _moduleId = value
            End Set
        End Property

        Public Property WarrantType() As WarrantType
            Get
                Return _warrantType
            End Get
            Set(ByVal value As WarrantType)
                _warrantType = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Property WarrantId() As Integer
            Get
                Return _warrantId
            End Get
            Set(ByVal Value As Integer)
                _warrantId = Value
            End Set
        End Property

       Public Property AgencyName() As String
            Get
                If _agencyName <> "" Then
                    Return _agencyName
                Else
                    Dim ctl As New Controller
                    Return ctl.GetAgency(_agencyId).AgencyName
                End If
            End Get
            Set(ByVal value As String)
                _agencyName = value
            End Set
        End Property

         Public Property CreatedByName() As String
            Get
                If _createdByName <> "" Then
                    Return _createdByName
                Else
                    Dim ctl As New Controller
                    Dim portal As PortalSettings = PortalSettings.Current
                    Return UserController.GetUserById(portal.PortalId, CreatedByUserId).DisplayName
                End If
                Return _createdByName
            End Get
            Set(ByVal value As String)
                _createdByName = value
            End Set
        End Property

        Public Property AgencyId() As Integer
            Get
                Return _agencyId
            End Get
            Set(ByVal Value As Integer)
                _agencyId = Value
            End Set
        End Property

        Public Property CreatedByUserId() As Integer
            Get
                Return _createdByUserId
            End Get
            Set(ByVal Value As Integer)
                _createdByUserId = Value
            End Set
        End Property

        Public Property CreatedDate() As DateTime
            Get
                Return _createdDate
            End Get
            Set(ByVal Value As DateTime)
                _createdDate = Value
            End Set
        End Property

        Public Property StatusId() As WarrantStatus
            Get
                Return _statusId
            End Get
            Set(ByVal Value As WarrantStatus)
                _statusId = Value
            End Set
        End Property

        Public Property ReviewedDate() As DateTime
            Get
                Return _reviewedDate
            End Get
            Set(ByVal Value As DateTime)
                _reviewedDate = Value
            End Set
        End Property

        Public Property JudgeName() As String
            Get
                Return _judgeName
            End Get
            Set(ByVal value As String)
                _judgeName = value
            End Set
        End Property

        Public Property JudgeUserId() As Integer
            Get
                Return _judgeUserId
            End Get
            Set(ByVal Value As Integer)
                _judgeUserId = Value
            End Set
        End Property

        Public Property FileId() As Integer
            Get
                Return _fileId
            End Get
            Set(ByVal Value As Integer)
                _fileId = Value
            End Set
        End Property

        Public Property SaApproved() As Boolean
            Get
                Return _saApproved
            End Get
            Set(ByVal value As Boolean)
                _saApproved = value
            End Set
        End Property

        Public Property SaName() As String
            Get
                Return _saName
            End Get
            Set(ByVal value As String)
                _saName = value
            End Set
        End Property

        Public Property Defendant() As String
            Get
                Return _defendant
            End Get
            Set(ByVal value As String)
                _defendant = value
            End Set
        End Property

        Public ReadOnly Property StatusToolTip() As String
            Get
                Select Case StatusId
                    Case WarrantStatus.NewWarrant
                        Return "New"
                    Case WarrantStatus.Rejected
                        Return "Rejected by " & GetJudgeName(_reviewedByJudgeUserId)
                    Case WarrantStatus.Signed
                        Return "Signed by " & GetJudgeName(_reviewedByJudgeUserId)
                    Case WarrantStatus.UnderReview
                        Return "Under Review By " & GetJudgeName(_reviewedByJudgeUserId)
                    Case WarrantStatus.Reviewed
                      '  Return "Completed"
                       Dim hoursPast As Integer = DateDiff(DateInterval.Hour, _reviewedByAgencyDate, DateTime.Now)
                        If hoursPast > 24 Then
                            Return "Completed - this warrant is scheduled to be deleted"
                        Else
                            Return "Completed - Will be automatically deleted in " & 24 - DateDiff(DateInterval.Hour, _reviewedByAgencyDate, DateTime.Now).ToString & " hours!"
                        End If
                    Case WarrantStatus.ReturnService
                        Return "Return of Service"

                End Select
                Return ""
            End Get

        End Property

        Public ReadOnly Property Status() As String
            Get
                Select Case StatusId
                    Case WarrantStatus.NewWarrant
                        Return "N"
                    Case WarrantStatus.Rejected
                        Return "R"
                    Case WarrantStatus.Signed
                        Return "S"
                    Case WarrantStatus.UnderReview
                        Return "U"
                    Case WarrantStatus.Reviewed
                        Return "C"
                    Case WarrantStatus.ReturnService
                        Return "RS"
                End Select
                Return ""
            End Get

        End Property

        Private Function GetJudgeName(userid As Integer) As String
            Dim ctl As New Controller
            Dim objJudge As JudgeInfo = ctl.GetJudge(userid)
            If Not objJudge Is Nothing Then
                Return objJudge.User.DisplayName
            Else
                Return ""
            End If
        End Function

        Public Property IsMarked() As Boolean
            Get
                Return _isMarked
            End Get
            Set(ByVal value As Boolean)
                _isMarked = value
            End Set
        End Property

#End Region

    End Class

    Public Class AgencyUserInfo

#Region "Private Members"
        Private _agencyId As Integer
        Private _userId As Integer
        Private _isAdmin As Boolean
        Private _agencyName As String
        Private _displayName As String


#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal agencyId As Integer, ByVal userId As Integer, ByVal isAdmin As Boolean)
            Me.AgencyId = agencyId
            Me.UserId = userId
            Me.IsAdmin = isAdmin
        End Sub
#End Region

#Region "Public Properties"
        Public Property AgencyId() As Integer
            Get
                Return _agencyId
            End Get
            Set(ByVal Value As Integer)
                _agencyId = Value
            End Set
        End Property


        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal Value As Integer)
                _userId = Value
            End Set
        End Property

        Public Property IsAdmin() As Boolean
            Get
                Return _isAdmin
            End Get
            Set(ByVal Value As Boolean)
                _isAdmin = Value
            End Set
        End Property

        Public Property AgencyName() As String
            Get
                Return _agencyName
            End Get
            Set(ByVal value As String)
                _agencyName = value
            End Set
        End Property

        Public Property DisplayName() As String
            Get
                Return _displayName
            End Get
            Set(ByVal value As String)
                _displayName = value
            End Set
        End Property

        Private _username As String
        Public Property Username() As String
            Get
                Return _username
            End Get
            Set(ByVal value As String)
                _username = value
            End Set
        End Property


#End Region

    End Class

    Public Class AgencyInfo

#Region "Private Members"
        Private _agencyId As Integer
        Private _agencyName As String
        Private _emailAddress As String
        Private _parentAgency As String
        Private _moduleId As Integer
        Private _county As String
        Private _isClerk As Boolean

#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal agencyId As Integer, ByVal agencyName As String)
            Me.AgencyId = agencyId
            Me.AgencyName = agencyName
        End Sub
#End Region

#Region "Public Properties"
        Public Property IsClerk() As Boolean
            Get
                Return _isClerk
            End Get
            Set(ByVal value As Boolean)
                _isClerk = value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal value As Integer)
                _moduleId = value
            End Set
        End Property

        Public Property ParentAgency() As String
            Get
                Return _parentAgency
            End Get
            Set(ByVal value As String)
                _parentAgency = value
            End Set
        End Property


        Public Property EmailAddress() As String
            Get
                Return _emailAddress
            End Get
            Set(ByVal value As String)
                _emailAddress = value
            End Set
        End Property

        Public Property AgencyId() As Integer
            Get
                Return _agencyId
            End Get
            Set(ByVal Value As Integer)
                _agencyId = Value
            End Set
        End Property

        Public Property AgencyName() As String
            Get
                Return _agencyName
            End Get
            Set(ByVal Value As String)
                _agencyName = Value
            End Set
        End Property

        Public Property County() As String
            Get
                Return _county
            End Get
            Set(ByVal value As String)
                _county = value
            End Set
        End Property

        Public ReadOnly Property CountyList As String
            Get
                Return GetCountyList(_agencyId)
            End Get
        End Property
#End Region

#Region "Methods"
        Private Function GetCountyList(agencyId As Integer) As String
            Dim ctl As New Controller
            Dim countyList = ctl.GetCountiesByAgency(agencyId)
            Dim returnList As String = ""
            For Each c As AgencyCounty In countyList
                returnList += c.CountyName
                returnList += ", "
            Next
            Dim charsToTrim() As Char = {","c, " "c}
            Return returnList.Trim(charsToTrim)
        End Function
#End Region
    End Class

    Public Class AgencyCounty
        Public Sub New()
        End Sub

        Public Sub New(ByVal agencyId As Integer, ByVal countyName As String)
            Me.AgencyId = agencyId
            Me.CountyName = countyName
        End Sub

        Private _agencyId As Integer
        Public Property AgencyId() As Integer
            Get
                Return _agencyId
            End Get
            Set(ByVal value As Integer)
                _agencyId = value
            End Set
        End Property

        Private _countyName As String
        Public Property CountyName() As String
            Get
                Return _countyName
            End Get
            Set(ByVal value As String)
                _countyName = value
            End Set
        End Property

    End Class


    Public Class LogInfo

#Region "Private Members"
        Private _logId As Integer
        Private _warrantId As Integer
        Private _eventTime As DateTime
        Private _eventDescription As String
        Private _userId As Integer
        Private _ip As String

#End Region

#Region "Constructors"
        Public Sub New()
        End Sub
#End Region

#Region "Public PRoperties"
        Public Property LogId() As Integer
            Get
                Return _logId
            End Get
            Set(ByVal value As Integer)
                _logId = value
            End Set
        End Property

        Public Property WarrantId() As Integer
            Get
                Return _warrantId
            End Get
            Set(ByVal value As Integer)
                _warrantId = value
            End Set
        End Property

        Public Property EventTime() As DateTime
            Get
                Return _eventTime
            End Get
            Set(ByVal value As DateTime)
                _eventTime = value
            End Set
        End Property

        Public Property EventDescription() As String
            Get
                Return _eventDescription
            End Get
            Set(ByVal value As String)
                _eventDescription = value
            End Set
        End Property

        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal value As Integer)
                _userId = value
            End Set
        End Property

        Public Property IP() As String
            Get
                Return _ip
            End Get
            Set(ByVal value As String)
                _ip = value
            End Set
        End Property

#End Region

    End Class

    Public Enum FileType
        warrant = 1
        signature = 2
        initial = 3
    End Enum

    Public Enum WarrantStatus
        NewWarrant = 1
        UnderReview = 2
        Signed = 3
        Rejected = 4
        Reviewed = 5
        ReturnService = 6
    End Enum

    Public Enum WarrantType
        Search = 2
        Arrest = 1
        Generic = 0
    End Enum

    Public Class CountyInfo

#Region "Private Members"
        Private _countyId As Integer
        Private _moduleId As Integer
        Private _countyName As String

#End Region

#Region "Constructors"
        Public Sub New()
        End Sub


#End Region

#Region "Public Properties"
        Public Property CountyID() As Integer
            Get
                Return _countyId
            End Get
            Set(ByVal value As Integer)
                _countyId = value
            End Set
        End Property

        Public Property ModuleID() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal value As Integer)
                _moduleId = value
            End Set
        End Property

        Public Property CountyName() As String
            Get
                Return _countyName
            End Get
            Set(ByVal value As String)
                _countyName = value
            End Set
        End Property

        Public ReadOnly Property CountyCode() As String
            Get
                Return _countyName.Substring(0, 3)
            End Get

        End Property


#End Region

    End Class

End Namespace
