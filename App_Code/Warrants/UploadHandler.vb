Imports System
Imports Telerik.Web.UI

Namespace AWS.Modules.Warrants
    Public Class UploadConfiguration
        Inherits AsyncUploadConfiguration
        Private m_userID As Integer
        Private _persist As Boolean
        Public Property UserID() As Integer
            Get
                Return m_userID
            End Get

            Set(ByVal value As Integer)
                m_userID = value
            End Set
        End Property

        Public Property Persist() As Boolean
            Get
                Return _persist
            End Get
            Set(ByVal value As Boolean)
                _persist = value
            End Set
        End Property

    End Class

    Public Class UploadResult
        Inherits AsyncUploadResult
        Private _fileID As Integer

        Public Property FileID() As Integer
            Get
                Return _fileID
            End Get
            Set(ByVal value As Integer)
                _fileID = value
            End Set
        End Property


    End Class

End Namespace

