Imports Microsoft.VisualBasic
Imports System.IO
Imports DotNetNuke
Imports System.Web.UI
Imports System.Text

Namespace AWS.Modules.Warrants


    Public Class ViewStateBase
        Inherits Entities.Modules.PortalModuleBase
        Private Shared aspNetFormElements() As String = {"__EVENTTARGET", "__EVENTARGUMENT", "__VIEWSTATE", "__EVENTVALIDATION", "__VIEWSTATEENCRYPTED"}

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            Dim stringWriter As New StringWriter()
            Dim htmlWriter As New HtmlTextWriter(stringWriter)
            MyBase.Render(htmlWriter)
            Dim html As String = stringWriter.ToString()
            Dim formStart As Integer = html.IndexOf("<form")
            Dim endForm As Integer = -1
            If formStart >= 0 Then
                endForm = html.IndexOf(">", formStart)
            End If

            If endForm >= 0 Then
                Dim viewStateBuilder As New StringBuilder()
                For Each element As String In aspNetFormElements
                    Dim startPoint As Integer = html.IndexOf("<input type=""hidden"" name=""" & element & """")
                    If startPoint >= 0 AndAlso startPoint > endForm Then
                        Dim endPoint As Integer = html.IndexOf("/>", startPoint)
                        If endPoint >= 0 Then
                            endPoint += 2
                            Dim viewStateInput As String = html.Substring(startPoint, endPoint - startPoint)
                            html = html.Remove(startPoint, endPoint - startPoint)
                            viewStateBuilder.Append(viewStateInput).Append(vbCrLf)
                        End If
                    End If
                Next element

                If viewStateBuilder.Length > 0 Then
                    viewStateBuilder.Insert(0, vbCrLf)
                    html = html.Insert(endForm + 1, viewStateBuilder.ToString())
                End If
            End If

            writer.Write(html)
        End Sub

    End Class
End Namespace