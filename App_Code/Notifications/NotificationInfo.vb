

Namespace AWS.Modules.Notifications

    Public Class NotificationInfo

#Region "Private Members"
        Private _scheduleId As Integer
        Private _requestingJudgeId As Integer
        Private _coveringJudgeId As Integer
        Private _startDateTime As DateTime
        Private _endDateTime As DateTime
        Private _messageText As String
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        Public Property ScheduleId() As Integer
            Get
                Return _scheduleId
            End Get
            Set(ByVal value As Integer)
                _scheduleId = value
            End Set
        End Property

        Public Property RequestingJudgeId() As Integer
            Get
                Return _requestingJudgeId
            End Get
            Set(ByVal value As Integer)
                _requestingJudgeId = value
            End Set
        End Property

        Public Property CoveringJudgeId() As Integer
            Get
                Return _coveringJudgeId
            End Get
            Set(ByVal value As Integer)
                _coveringJudgeId = value
            End Set
        End Property

        Public Property StartDateTime() As DateTime
            Get
                Return _startDateTime
            End Get
            Set(ByVal value As DateTime)
                _startDateTime = value
            End Set
        End Property

        Public Property EndDateTime() As DateTime
            Get
                Return _endDateTime
            End Get
            Set(ByVal value As DateTime)
                _endDateTime = value
            End Set
        End Property

        Public Property MessageText() As String
            Get
                Return _messageText
            End Get
            Set(ByVal value As String)
                _messageText = value
            End Set
        End Property

        Public ReadOnly Property CoveringJudgeName As String
            Get
                Dim FullName As String = ""
                Dim objUser As UserInfo = UserController.GetUserById(PortalSettings.Current.PortalId, _coveringJudgeId)
                If Not objUser Is Nothing Then
                    Return objUser.DisplayName
                End If
                Return FullName
            End Get
        End Property
#End Region

    End Class
End Namespace
