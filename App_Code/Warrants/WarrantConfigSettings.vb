Imports System
Imports System.Data

Namespace AWS.Modules.Warrants

    Public Class WarrantConfigSettings

#Region "Private Members"
        Private _notificationEmail As String
        Private _deleteThreshold As Integer
        Private _signedThreshold As Integer
        Private _claimedThreshold As Integer
        Private _moduleID As Integer
        Private _demoMode As Boolean
        Private _notificationRecoveryTime As TimeSpan
        Private _hours As Integer
        Private _portalId As Integer


#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"
        Private _senderEmail As String
        Public Property SenderEmail() As String
            Get
                Return _senderEmail
            End Get
            Set(ByVal value As String)
                _senderEmail = value
            End Set
        End Property
        Public Property PortalId() As Integer
            Get
                Return _portalId
            End Get
            Set(ByVal value As Integer)
                _portalId = value
            End Set
        End Property
        Public Property DemoMode() As Boolean
            Get
                Return _demoMode
            End Get
            Set(ByVal value As Boolean)
                _demoMode = value
            End Set
        End Property


        Public Property ModuleId() As Integer
            Get
                Return _moduleID
            End Get
            Set(ByVal value As Integer)
                _moduleID = value
            End Set
        End Property


        Public Property ClaimedThreshold() As Integer
            Get
                Return _claimedThreshold
            End Get
            Set(ByVal value As Integer)
                _claimedThreshold = value
            End Set
        End Property

        Public Property SignedThreshold() As Integer
            Get
                Return _signedThreshold
            End Get
            Set(ByVal value As Integer)
                _signedThreshold = value
            End Set
        End Property

        Public Property DeleteThreshold() As Integer
            Get
                Return _deleteThreshold
            End Get
            Set(ByVal value As Integer)
                _deleteThreshold = value
            End Set
        End Property

        Public Property NotificationEmail() As String
            Get
                Return _notificationEmail
            End Get
            Set(ByVal value As String)
                _notificationEmail = value
            End Set
        End Property

        Public Property NotificationRecoveryTime As TimeSpan
            Get
                Return _notificationRecoveryTime
            End Get
            Set(value As TimeSpan)
                _notificationRecoveryTime = value
            End Set
        End Property

        Public Property Hours() As Integer
            Get
                Return _hours
            End Get
            Set(ByVal value As Integer)
                _hours = value
            End Set
        End Property
#End Region

    End Class

End Namespace
