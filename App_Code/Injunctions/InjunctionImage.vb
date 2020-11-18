
Imports System

Namespace AWS.Modules.Injunctions
    Public Class InjunctionImage
        Private _fileID As Integer
        Private _bytes As Byte()
        Private _uploadedBy As Integer
        Private _uploadedDate As DateTime
        Private _injunctionId As Integer
        Private _fileType As InjunctionFileType
        Private _isMarked As Boolean

        Public Property InjunctionId() As Integer
            Get
                Return _injunctionId
            End Get
            Set(ByVal value As Integer)
                _injunctionId = value
            End Set
        End Property

        Public Property FileID() As Integer
            Get
                Return _fileID
            End Get
            Set(ByVal value As Integer)
                _fileID = value
            End Set
        End Property

        Public Property Bytes() As Byte()
            Get
                Return _bytes
            End Get
            Set(ByVal value As Byte())
                _bytes = value
            End Set
        End Property

        Public Property UploadedBy() As Integer
            Get
                Return _uploadedBy
            End Get
            Set(ByVal value As Integer)
                _uploadedBy = value
            End Set
        End Property

        Public Property UploadedDate() As DateTime
            Get
                Return _uploadedDate
            End Get
            Set(ByVal value As DateTime)
                _uploadedDate = value
            End Set
        End Property
        Public Property FileType() As InjunctionFileType
            Get
                Return _fileType
            End Get
            Set(ByVal value As InjunctionFileType)
                _fileType = value
            End Set
        End Property
        Public Property IsMarked() As Boolean
            Get
                Return _isMarked
            End Get
            Set(ByVal value As Boolean)
                _isMarked = value
            End Set
        End Property
    End Class

    <Serializable>
    Public Class JudgeSignImage
        Private _fileId As Integer
        Public Property FileId() As Integer
            Get
                Return _fileId
            End Get
            Set(ByVal value As Integer)
                _fileId = value
            End Set
        End Property
        Private _signatureType As String
        Public Property SignatureType() As String
            Get
                Return _signatureType
            End Get
            Set(ByVal value As String)
                _signatureType = value
            End Set
        End Property
        Private _fileName As String
        Public Property FileName() As String
            Get
                Return _fileName
            End Get
            Set(ByVal value As String)
                _fileName = value
            End Set
        End Property

    End Class

    Public Enum InjunctionFileType
        tiff = 1
        pdf = 2
        nonwarrant = 0
    End Enum

End Namespace
