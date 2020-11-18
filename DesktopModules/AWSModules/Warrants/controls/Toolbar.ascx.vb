Imports Telerik.Web.UI

Namespace AWS.Modules.Warrants
    Partial Class Toolbar
        Inherits ViewStateBase

#Region "Properties"
        Private _judgeRoleName As String = ""
        Public Property JudgeRoleName() As String
            Get
                Return _judgeRoleName
            End Get
            Set(ByVal value As String)
                _judgeRoleName = value
            End Set
        End Property

        Private _adminJudge As String = ""
        Public Property AdminJudge() As String
            Get
                If Not Settings("AdminJudge") Is Nothing Then
                    _adminJudge = Settings("AdminJudge").ToString.Trim
                End If
                Return _adminJudge
            End Get
            Set(ByVal value As String)
                _adminJudge = value
            End Set
        End Property

        Private _sergeantRoleName As String = ""
        Public Property SergeantRoleName() As String
            Get
                Return _sergeantRoleName
            End Get
            Set(ByVal value As String)
                _sergeantRoleName = value
            End Set
        End Property

        Private _isJudge As Boolean = False
        Public Property IsJudge() As Boolean
            Get
                Return _isJudge
            End Get
            Set(ByVal value As Boolean)
                _isJudge = value
            End Set
        End Property

        Private _isSiteAdmin As Boolean = False
        Public Property IsSiteAdmin() As Boolean
            Get
                Return _isSiteAdmin
            End Get
            Set(ByVal value As Boolean)
                _isSiteAdmin = value
            End Set
        End Property

        Private _isAdminJudge As Boolean = False
        Public Property IsAdminJudge() As Boolean
            Get
                Return _isAdminJudge
            End Get
            Set(ByVal value As Boolean)
                _isAdminJudge = value
            End Set
        End Property

        Private _objAgencyUser As AgencyUserInfo = Nothing
        Public Property objAgencyUser() As AgencyUserInfo
            Get
                Return _objAgencyUser
            End Get
            Set(ByVal value As AgencyUserInfo)
                _objAgencyUser = value
            End Set
        End Property

        Private _isClerk As Boolean = False
        Public Property IsClerk() As Boolean
            Get
                Return _isClerk
            End Get
            Set(ByVal value As Boolean)
                _isClerk = value
            End Set
        End Property

        Private _navJudgeSign As String = ""
        Public Property NavJudgeSign() As String
            Get
                Return _navJudgeSign
            End Get
            Set(ByVal value As String)
                _navJudgeSign = value
            End Set
        End Property

        Private _navAdminAgency As String = ""
        Public Property NavAdminAgency() As String
            Get
                Return _navAdminAgency
            End Get
            Set(ByVal value As String)
                _navAdminAgency = value
            End Set
        End Property

        Private _navAdminCounty As String = ""
        Public Property NavAdminCounty() As String
            Get
                Return _navAdminCounty
            End Get
            Set(ByVal value As String)
                _navAdminCounty = value
            End Set
        End Property

        Private _navManageUsers As String = ""
        Public Property NavManageUsers() As String
            Get
                Return _navManageUsers
            End Get
            Set(ByVal value As String)
                _navManageUsers = value
            End Set
        End Property

        Private _navAddWarrant As String = ""
        Public Property NavAddWarrant() As String
            Get
                Return _navAddWarrant
            End Get
            Set(ByVal value As String)
                _navAddWarrant = value
            End Set
        End Property

        Private _navAnnotations As String
        Public Property NavAnnotations() As String
            Get
                Return _navAnnotations
            End Get
            Set(ByVal value As String)
                _navAnnotations = value
            End Set
        End Property

        Private _navStatus As String = ""
        Public Property NavStatus() As String
            Get
                Return _navStatus
            End Get
            Set(ByVal value As String)
                _navStatus = value
            End Set
        End Property

        Private _navOutofOffice As String = ""
        Public Property NavOutofOffice() As String
            Get
                Return _navOutofOffice
            End Get
            Set(ByVal value As String)
                _navOutofOffice = value
            End Set
        End Property

        Private _hiddenMenu As String = ""
        Public Property HiddenMenu() As String
            Get
                Return _hiddenMenu
            End Get
            Set(ByVal value As String)
                _hiddenMenu = value
            End Set
        End Property

        Private _contactListUrl As String = ""
        Public Property ContactListUrl() As String
            Get
                Return _contactListUrl
            End Get
            Set(ByVal value As String)
                _contactListUrl = value
            End Set
        End Property

#End Region

#Region "Methods"
        Private Sub DisableCurrentLink()
            Select Case HiddenMenu
                Case "ManageAgencies"
                    lnkAgency.Visible = False
                Case "Annotations"
                    lnkAnnotations.Visible = False
                Case "ManageJudges"
                    lnkApprove.Visible = False
                Case "Contacts"
                    lnkContacts.Visible = False
                Case "ManageLists"
                    lnkCounty.Visible = False
                Case "Cover"
                    lnkCover.Visible = False
                Case "ManageUsers"
                    lnkManage.Visible = False
                Case "Status"
                    lnkStatus.Visible = False
                Case "Upload"
                    lnkUpload.Visible = False

            End Select
        End Sub
#End Region

#Region "Events"
        Private Sub Toolbar_Load(sender As Object, e As EventArgs) Handles Me.Load
            If Not IsPostBack Then

                If JudgeRoleName = "" Or AdminJudge = "" Or SergeantRoleName = "" Then
                    rtoolbar.Visible = False
                    Exit Sub
                End If
                If ContactListUrl <> "" Then
                    Dim contactTabId As Integer = 0
                    Int32.TryParse(ContactListUrl, contactTabId)
                    If contactTabId > 0 Then
                        Dim ctlTab As New TabController
                        Dim tab As DotNetNuke.Entities.Tabs.TabInfo = ctlTab.GetTab(contactTabId, PortalId, True)
                        lnkContacts.NavigateUrl = "/" + tab.TabPath.Trim("/")
                        lnkContacts.Visible = True
                    Else
                        lnkContacts.Visible = False
                    End If
                Else
                    lnkContacts.Visible = False
                End If
                If IsAdminJudge Or IsSiteAdmin Then
                    rtoolbar.Visible = True
                    lnkUpload.Visible = False
                    lnkManage.Visible = False
                    lnkApprove.Visible = True
                    lnkApprove.NavigateUrl = NavJudgeSign
                    If IsSiteAdmin Then
                        lnkStatus.Visible = True
                        lnkAgency.Visible = True
                        lnkAgency.NavigateUrl = NavAdminAgency
                        lnkCounty.Visible = True
                        lnkCounty.NavigateUrl = NavAdminCounty
                        lnkStatus.Visible = True
                        lnkStatus.NavigateUrl = NavStatus
                    End If
                Else
                    If objAgencyUser Is Nothing And Not IsJudge Then
                        rtoolbar.Visible = False
                    Else
                        If Not objAgencyUser Is Nothing Then
                            If IsClerk Then
                                If objAgencyUser.IsAdmin Then
                                    lnkUpload.Visible = False
                                    lnkContacts.Visible = False
                                    lnkManage.Visible = True
                                    lnkManage.NavigateUrl = NavManageUsers
                                Else
                                    rtoolbar.Visible = False
                                End If
                            Else
                                If objAgencyUser.IsAdmin Then
                                    lnkUpload.Visible = False
                                    lnkContacts.Visible = False
                                    lnkManage.Visible = True
                                    lnkManage.NavigateUrl = NavManageUsers
                                End If

                            End If
                        End If

                        lnkUpload.NavigateUrl = NavAddWarrant
                    End If
                End If
                If _isJudge Then
                    lnkUpload.Visible = False
                    lnkManage.Visible = False
                    lnkCover.Visible = True
                    lnkAnnotations.Visible = True
                    lnkCover.NavigateUrl = NavOutofOffice
                    lnkAnnotations.NavigateUrl = NavAnnotations
                End If
                DisableCurrentLink()
            End If
        End Sub


#End Region
    End Class

End Namespace

