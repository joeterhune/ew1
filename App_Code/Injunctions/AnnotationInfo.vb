Imports System
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users

Namespace AWS.Modules.Injunctions

    Public Class AnnotationInfo

#Region "Private Members"
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub


#End Region

#Region "Public Properties"
        Private _userId As Integer
        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal value As Integer)
                _userId = value
            End Set
        End Property

        Private _annotationText As String
        Public Property AnnotationText() As String
            Get
                Return _annotationText
            End Get
            Set(ByVal value As String)
                _annotationText = value
            End Set
        End Property
        Private _annotationId As Integer
        Public Property AnnotationId() As Integer
            Get
                Return _annotationId
            End Get
            Set(ByVal value As Integer)
                _annotationId = value
            End Set
        End Property
#End Region

    End Class

End Namespace
