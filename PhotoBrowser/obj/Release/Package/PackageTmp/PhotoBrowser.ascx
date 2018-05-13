<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PhotoBrowser.ascx.cs" Inherits="Codefresh.PhotosBrowser.PhotoBrowser" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="squishyWARE.WebComponents.squishyTREE" Assembly="squishyTREE" %>

<LINK REL=STYLESHEET TYPE="text/css" HREF="PhotoBrowserRes\PhotoBrowser.css">

<style type="text/css">

DIV#tipDiv { BORDER-RIGHT: #336 1px solid; PADDING-RIGHT: 4px; BORDER-TOP: #336 1px solid; PADDING-LEFT: 4px; FONT-SIZE: 11px; Z-INDEX: 10000; LEFT: 0px; VISIBILITY: hidden; PADDING-BOTTOM: 4px; BORDER-LEFT: #336 1px solid; COLOR: #000; LINE-HEIGHT: 1.2; PADDING-TOP: 4px; BORDER-BOTTOM: #336 1px solid; POSITION: absolute; TOP: 0px; BACKGROUND-COLOR: #dee7f7 }

</style>

<script type="text/javascript">

	function doTooltip(e, msg)
	{
		if ( typeof Tooltip == "undefined" || !Tooltip.ready ) return;
		Tooltip.show(e, msg);
	}

	function hideTip()
	{
		if ( typeof Tooltip == "undefined" || !Tooltip.ready ) return;
		Tooltip.hide();
	}

	</script>

<P>
<TABLE id=Table2 style="WIDTH: 655px; HEIGHT: 521px" cellSpacing=0 cellPadding=0 
border=0>
  <TBODY>
  <TR>
    <TD style="WIDTH: 195px" vAlign=top width=195>
      <TABLE id=Table4 cellSpacing=0 cellPadding=0 border=0>
        <TR>
          <TD style="WIDTH: 300px; HEIGHT: 290px" vAlign=top 
          ><cc1:treeview id=tvwMain 
            Width="238px" runat="server"></cc1:treeview></TD>
          <TD style="WIDTH: 300px; HEIGHT: 290px" vAlign=top 
          >&nbsp;&nbsp;&nbsp; </TD></TR>
        <TR>
          <TD style="WIDTH: 300px"><asp:panel 
            id=pnlComments runat="server" 
            Visible="False"><SPAN class=smallText>
            <TABLE id=Table3 cellSpacing=0 cellPadding=0 border=0>
              <TR>
                <TD height=10>
                  <HR align=left width="80%" SIZE=1>
                </TD></TR>
              <TR>
                <TD>
                  <TABLE id=Table5 cellSpacing=0 cellPadding=0 border=0>
                    <TR>
                      <TD style="HEIGHT: 20px">Comments:</TD></TR>
                    <TR>
                      <TD><asp:Literal id=litPhotoComments runat="server"></asp:Literal></TD></TR>
                    <TR>
                      <TD>
                        <TABLE id=Table6 cellSpacing=1 cellPadding=1 border=0>
                          <TR>
                            <TD width=40>Name</TD>
                            <TD><asp:TextBox id=txtCommentName runat="server" Width="131px" MaxLength="20" CssClass="text"></asp:TextBox></TD></TR>
                          <TR>
                            <TD style="HEIGHT: 26px" width=50>Comment</TD>
                            <TD style="HEIGHT: 26px"><asp:TextBox id=txtCommentText runat="server" Width="100%" MaxLength="150" CssClass="text"></asp:TextBox></TD></TR>
                          <TR>
                            <TD><asp:Button id=btnAddComment runat="server" Text="Add"></asp:Button></TD>
                            <TD><asp:RequiredFieldValidator id=RequiredFieldValidator1 runat="server" ControlToValidate="txtCommentName" ErrorMessage="Name is required"></asp:RequiredFieldValidator><BR><asp:RequiredFieldValidator id=RequiredFieldValidator2 runat="server" ControlToValidate="txtCommentText" ErrorMessage="Comment is required"></asp:RequiredFieldValidator></TD></TR></TABLE></TD></TR></TABLE></TD></TR></TABLE></SPAN></asp:panel></TD>
          <TD style="WIDTH: 300px"></TD></TR></TABLE></TD>
    <TD vAlign=top width="85%"><asp:panel 
      id=pnlPhotoGridContents Width="393px" Height="196px" 
      runat="server" Visible="False" HorizontalAlign="Left">
      <TABLE id=tblPhotoGridContents cellSpacing=0 cellPadding=0 border=0>
        <TR>
          <TD class=pageNavCell style="HEIGHT: 25px" align=center>
            <TABLE align=center border=0>
              <TR>
                <TD class=pageNavCell style="WIDTH: 23px"><asp:HyperLink id=hylPreviousPage1 runat="server" ToolTip="Previous Page" ImageUrl="PhotoBrowserResprevious.gif"></asp:HyperLink></TD>
                <TD class=pageNavCell>Pages:&nbsp; <asp:PlaceHolder id=plhPageLinks1 runat="server"></asp:PlaceHolder><SPAN 
                  class=currentPageNum>&nbsp; </SPAN></TD>
                <TD class=pageNavCell align=center><asp:HyperLink id=hylNextPage1 runat="server" CssClass="pageNavCell" ToolTip="Next Page" ImageUrl="PhotoBrowserResnext.gif"></asp:HyperLink></TD></TR></TABLE></TD></TR>
        <TR>
          <TD><asp:Table id=tblPhotos runat="server" CellSpacing="10" BorderStyle="None" CellPadding="4" EnableViewState="False"></asp:Table></TD></TR>
        <TR>
          <TD class=pageNavCell align=center>
            <TABLE align=center border=0>
              <TR>
                <TD class=pageNavCell><asp:HyperLink id=hylPreviousPage2 runat="server" ToolTip="Previous Page" ImageUrl="PhotoBrowserResprevious.gif"></asp:HyperLink></TD>
                <TD class=pageNavCell>Pages:&nbsp; <asp:PlaceHolder id=plhPageLinks2 runat="server"></asp:PlaceHolder><SPAN 
                  class=currentPageNum>&nbsp; </SPAN></TD>
                <TD class=pageNavCell align=center><asp:HyperLink id=hylNextPage2 runat="server" CssClass="pageNavCell" ToolTip="Next Page" ImageUrl="PhotoBrowserResnext.gif"></asp:HyperLink></TD></TR></TABLE></TD></TR></TABLE>
      <DIV>
      <DIV>&nbsp;</DIV></DIV></asp:panel><asp:panel id=pnlPhotoContents 
       Width="389px" runat="server" Visible="False">
      <TABLE id=Table1 cellSpacing=0 cellPadding=0 border=0>
        <TR>
          <TD>
            <TABLE id=Table7 cellPadding=5 align=center border=0>
              <TR>
                <TD align=center width="33%"><asp:HyperLink id=hlkPreviousImage runat="server" ToolTip="Previous Page" ImageUrl="PhotoBrowserResprevious.gif"></asp:HyperLink><BR><SPAN 
                  class=smallText>Previous Image</SPAN> </TD>
                <TD vAlign=bottom align=center width="34%">
                  <DIV align=center><SPAN class=smallText><asp:HyperLink id=hlkReturnToThumbnails1 runat="server" ToolTip="Return to Thumbnails" ImageUrl="PhotoBrowserRes/index.gif"></asp:HyperLink><BR>Return 
                  to Thumbnails</SPAN></DIV></TD>
                <TD align=center width="33%"><asp:HyperLink id=hlkNextImage runat="server" CssClass="pageNavCell" ToolTip="Next Page" ImageUrl="PhotoBrowserResnext.gif"></asp:HyperLink><BR><SPAN 
                  class=smallText>Next Image</SPAN></TD></TR></TABLE></TD></TR>
        <TR>
          <TD align=left>
            <TABLE>
              <TR>
                <TD><asp:Image id=imgPhoto runat="server"></asp:Image></TD></TR></TABLE></TD></TR>
        <TR>
          <TD align=center height=40><asp:Label id=lblViewedCount runat="server">Viewed Count</asp:Label></TD></TR>
        <TR>
          <TD><asp:Panel id=pnlImageNavigationBottom runat="server">
            <TABLE id=Table9 width="100%">
              <TR><!-- Previous thumbnail -->
                <TD style="WIDTH: 150px" align=left width=150 height="100%" 
                rowSpan=2>
                  <TABLE id=Table8>
                    <TR>
                      <TD class=tdNav align=center>
                        <DIV><asp:HyperLink id=hlkPreviousImagePhoto runat="server" CssClass="navImg" ToolTip="Previous Image">Previous Image</asp:HyperLink></DIV>
                        <DIV><asp:HyperLink id=hlkPreviousImageName runat="server">Name</asp:HyperLink></DIV></TD></TR></TABLE></TD><!-- Current image comments -->
                <TD style="HEIGHT: 20px" vAlign=top align=center 
                width="34%"></TD><!-- Next thumbnail -->
                <TD align=right width="33%" height="100%" rowSpan=2>
                  <TABLE id=Table10>
                    <TR>
                      <TD class=tdNav align=center>
                        <DIV><asp:HyperLink id=hlkNextImagePhoto runat="server" ToolTip="Next Image">Next Image</asp:HyperLink><BR><asp:HyperLink id=hlkNextImageName runat="server">Name</asp:HyperLink></DIV></TD></TR></TABLE></TD></TR>
              <TR>
                <TD vAlign=bottom align=center width="34%">
                  <DIV align=center><SPAN class=smallText><asp:HyperLink id=hlkReturnToThumbnails2 runat="server" ToolTip="Return to Thumbnails" ImageUrl="PhotoBrowserRes/index.gif"></asp:HyperLink><BR>Return 
                  to 
        Thumbnails</SPAN></DIV></TD></TR></TABLE></asp:Panel></TD></TR></TABLE></asp:panel></TD></TR></TBODY></TABLE></P>
