Imports DotNetNuke.Web.Api


Namespace AWS.Modules.Warrants
    Public Class RouteMapper
        Implements IServiceRouteMapper


        Public Sub RegisterRoutes(ByVal mapRouteManager As IMapRoute) Implements IServiceRouteMapper.RegisterRoutes
            Try
                mapRouteManager.MapHttpRoute("AWSModules/Warrants", "HttpData", "{controller}/{action}/{moduleid}/{county}/{judgetypeid}", {"AWS.Modules.Warrants"})
                mapRouteManager.MapHttpRoute("AWSModules/Warrants", "NotificationData", "{controller}/{action}/{judgeId}", {"AWS.Modules.Warrants"})
                mapRouteManager.MapHttpRoute("AWSModules/Warrants", "ManageAnnotationData", "{controller}/{action}/{parameter}", {"AWS.Modules.Warrants"})
                mapRouteManager.MapHttpRoute("AWSModules/Warrants", "AnnotationData", "{controller}/{action}", {"AWS.Modules.Warrants"})
            Catch ex As Exception

            End Try


        End Sub

    End Class
End Namespace