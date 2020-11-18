Imports System
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users

Namespace AWS.Modules.Utilities

    Public Class ExportUser

#Region "Private Members"
        Private _userId As Integer
        Private _firstname As String
        Private _lastname As String
        Private _displayname As String
        Private _email As String
        Private _username As String
        Private _password As String

#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"
        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal value As String)
                _password = value
            End Set
        End Property

        Public Property Username() As String
            Get
                Return _username
            End Get
            Set(ByVal value As String)
                _username = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return _email
            End Get
            Set(ByVal value As String)
                _email = value
            End Set
        End Property

        Public Property DisplayName() As String
            Get
                Return _displayname
            End Get
            Set(ByVal value As String)
                _displayname = value
            End Set
        End Property
        Public Property LastName() As String
            Get
                Return _lastname
            End Get
            Set(ByVal value As String)
                _lastname = value
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

        Public Property FirstName() As String
            Get
                Return _firstname
            End Get
            Set(ByVal value As String)
                _firstname = value
            End Set
        End Property

#End Region

    End Class

    Public Class portalUser
        Private _userId As Integer
        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal value As Integer)
                _userId = value
            End Set
        End Property
    End Class

End Namespace
