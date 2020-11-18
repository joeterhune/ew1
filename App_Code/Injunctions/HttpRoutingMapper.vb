Imports DotNetNuke.Web.Api


Namespace AWS.Modules.Injunctions
    Public Class RouteMapper
        Implements IServiceRouteMapper


        Public Sub RegisterRoutes(ByVal mapRouteManager As IMapRoute) Implements IServiceRouteMapper.RegisterRoutes
            Try
                mapRouteManager.MapHttpRoute("AWSModules/Injunctions", "HttpData", "{controller}/{action}/{moduleid}/{county}/{judgetypeid}", {"AWS.Modules.Injunctions"})
                mapRouteManager.MapHttpRoute("AWSModules/Injunctions", "NotificationData", "{controller}/{action}/{judgeId}", {"AWS.Modules.Injunctions"})
                mapRouteManager.MapHttpRoute("AWSModules/Injunctions", "ManageAnnotationData", "{controller}/{action}/{parameter}", {"AWS.Modules.Injunctions"})
                mapRouteManager.MapHttpRoute("AWSModules/Injunctions", "AnnotationData", "{controller}/{action}", {"AWS.Modules.Injunctions"})
            Catch ex As Exception

            End Try


        End Sub

    End Class
End Namespace