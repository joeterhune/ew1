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
Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities

Namespace AWS.Modules.Utilities

    Public Class Controller

#Region "Utilities Methods"
        Public Sub AddExportUser(ByVal objUser As ExportUser)
            DataProvider.Instance().AddUser(objUser.UserId, objUser.Username, objUser.Password, objUser.DisplayName, objUser.LastName, objUser.FirstName, objUser.Email)
        End Sub

        Public Sub DeleteUsers()
            DataProvider.Instance().DeleteUsers()
        End Sub

        Public Function ListImportUsers() As List(Of ExportUser)
            Return CBO.FillCollection(Of ExportUser)(DataProvider.Instance().ListImportUsers())
        End Function

        Public Function ListPortalUsers(portalId As Integer,currentPortalId as integer) As List(Of portalUser)
            Return CBO.FillCollection(Of portalUser)(DataProvider.Instance().ListPortalUsers(portalId,currentPortalId))
        End Function

        Public Sub AddPortalUser(portalId As Integer, userId As Integer)
            DataProvider.Instance.AddPortalUser(portalId, userId)
        End Sub

#End Region

    End Class
End Namespace
