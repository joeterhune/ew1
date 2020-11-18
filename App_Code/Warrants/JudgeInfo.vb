Imports System
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users

Namespace AWS.Modules.Warrants

    Public Class JudgeInfo

#Region "Private Members"
        Private _judgeId As Integer
        Private _signature As Integer
        Private _initial As Integer
        Private _approved As String
        Private _portalSettings As PortalSettings
        Private _moduleId As Integer
        Private _county As String
        Private _judgeTypeId As Integer
        Private _dayStart As DateTime
        Private _dayEnd As DateTime
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub


#End Region

#Region "Public Properties"

        Public Property DayEnd() As DateTime
            Get
                Return _dayEnd
            End Get
            Set(ByVal value As DateTime)
                _dayEnd = value
            End Set
        End Property
        Public Property DayStart() As DateTime
            Get
                Return _dayStart
            End Get
            Set(ByVal value As DateTime)
                _dayStart = value
            End Set
        End Property
        Public ReadOnly Property User As UserInfo
            Get
                If _portalSettings Is Nothing Then
                    _portalSettings = GetPortalSettings()
                End If
                Dim _user As UserInfo = UserController.GetUserById(_portalSettings.PortalId, _judgeId)
                If _user Is Nothing Then
                    Return New UserInfo
                Else
                    Return _user
                End If

            End Get
        End Property

        Public Property County() As String
            Get
                Return _county
            End Get
            Set(ByVal value As String)
                _county = value
            End Set
        End Property

        Public ReadOnly Property CountyCode As String
            Get
                Return _county.Substring(0, 3).ToUpper
            End Get
        End Property

        Public Property JudgeTypeId() As Integer
            Get
                Return _judgeTypeId
            End Get
            Set(ByVal value As Integer)
                _judgeTypeId = value
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

        Public Property JudgeId() As Integer
            Get
                Return _judgeId
            End Get
            Set(ByVal value As Integer)
                _judgeId = value
            End Set
        End Property

        Public Property Signature() As FileType
            Get
                Return _signature
            End Get
            Set(ByVal Value As FileType)
                _signature = Value
            End Set
        End Property

        Public Property Initial() As FileType
            Get
                Return _initial
            End Get
            Set(ByVal Value As FileType)
                _initial = Value
            End Set
        End Property

        Public Property Approved() As String
            Get
                Return _approved
            End Get
            Set(ByVal value As String)
                _approved = value
            End Set
        End Property

        Public ReadOnly Property HasSignature As Boolean
            Get
                Return _signature > 0
            End Get
        End Property

        Public ReadOnly Property HasInitial As Boolean
            Get
                Return _initial > 0
            End Get
        End Property


        Public ReadOnly Property InitialImage As Atalasoft.Annotate.AnnotationImage
            Get
                Dim ctl As New Controller
                Dim encrypt As New Encryptor
                Dim image As WarrantImage = ctl.GetFile(_initial)
                If Not image Is Nothing Then
                    Dim buffer As Byte() = encrypt.DecryptStream(image.Bytes)
                    Dim tempImage As New Atalasoft.Annotate.AnnotationImage(New IO.MemoryStream(buffer))
                    Return tempImage
                Else
                    Return New Atalasoft.Annotate.AnnotationImage
                End If
            End Get
        End Property

        Public ReadOnly Property SignatureImage As Atalasoft.Annotate.AnnotationImage
            Get
                Dim ctl As New Controller
                Dim encrypt As New Encryptor
                Dim image As WarrantImage = ctl.GetFile(_signature)
                If Not image Is Nothing Then
                    Dim buffer As Byte() = encrypt.DecryptStream(image.Bytes)
                    Dim tempImage As New Atalasoft.Annotate.AnnotationImage(New IO.MemoryStream(buffer))
                    Return tempImage
                Else
                    Return New Atalasoft.Annotate.AnnotationImage
                End If
            End Get
        End Property

        Public ReadOnly Property ListCounties() As List(Of CountyInfo)
            Get
                Dim ctl As New Controller
                Return ctl.ListJudgeCounties(JudgeId, ModuleId)
            End Get
        End Property

#End Region

    End Class
    Public Class JudgeTypeInfo

#Region " Private Members "
        Private _judgeTypeId As Integer
        Private _moduleId As Integer
        Private _judgeType As String
        Private _judgeTypeCode As String
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub
#End Region

#Region " Public Properties "
        Public Property JudgeTypeID() As Integer
            Get
                Return _judgeTypeId
            End Get
            Set(ByVal value As Integer)
                _judgeTypeId = value
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

        Public Property JudgeType() As String
            Get
                Return _judgeType
            End Get
            Set(ByVal value As String)
                _judgeType = value
            End Set
        End Property

        Public Property JudgeTypeCode() As String
            Get
                Return _judgeTypeCode
            End Get
            Set(ByVal value As String)
                _judgeTypeCode = value
            End Set
        End Property

#End Region

    End Class
End Namespace
