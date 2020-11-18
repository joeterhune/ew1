<%@ Control Language="VB" AutoEventWireup="true" %>
<menu class="sub" label="Navigation">
    <asp:ImageButton class="button" ID="Button_PreviousPage" runat="server" ImageUrl="~/images/Page-Previous.png"
        ToolTip="Moves to the Previous Page" Height="24px" Width="24px" OnClientClick="PreviousPage(); return false;" />
    
    <asp:ImageButton class="button" ID="Button_NextPage" runat="server" ImageUrl="~/images/Page-Next.png"
        ToolTip="Moves to the Next Page" Height="24px" Width="24px" OnClientClick="NextPage(); return false;" />
     <asp:ImageButton class="button" ID="Button_InsertPage" runat="server" ImageUrl="~/images/Insert.png"
        ToolTip="Inserts New Page" Height="24px" Width="24px" OnClientClick="InsertPage(); return false;" />
</menu>
<a>Navigation</a>
<script type="text/javascript">

    function PreviousPage() {
        var index = WebThumbnailViewer1.getSelectedIndex();
        index--;
        if (index > -1) {
            WebThumbnailViewer1.SelectThumb(index);
        }
    }

    function NextPage() {
        var index = WebThumbnailViewer1.getSelectedIndex();
        index++;
        if (index < WebThumbnailViewer1.getCount()) {
            WebThumbnailViewer1.SelectThumb(index);
        }
    }

    function InsertPage() {
        var index = WebThumbnailViewer1.getSelectedIndex();
            WebAnnotationViewer1.RemoteInvoked = function () {
            WebAnnotationViewer1.RemoteInvoked = null;
            WebThumbnailViewer1.OpenUrl(WebAnnotationViewer1.getReturnValue());
        }
        WebAnnotationViewer1.RemoteInvoke('RemoteInsertPage');
        WebThumbnailViewer1.SelectThumb(index);
    }
    function RefreshThumbnailViewer() {
        WebAnnotationViewer1.RemoteInvoked = function () { };
        var newUrl = WebAnnotationViewer1.getReturnValue();
        WebThumbnailViewer1.OpenUrl(newUrl);
        var frame = WebThumbnailViewer1.getCount();
        WebThumbnailViewer1.SelectThumb(frame-1);
    }

</script>
