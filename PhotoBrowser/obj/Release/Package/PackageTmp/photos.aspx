<%@ Register TagPrefix="uc1" TagName="PhotoBrowser" Src="PhotoBrowser.ascx" %>
<%@ Page language="c#" validateRequest=false Codebehind="photos.aspx.cs" AutoEventWireup="false" Inherits="Codefresh.PhotosServer.photos" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Tom & Jerry PlaySchool And Daycare : Photos</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">

	<script type="text/javascript" src="PhotoBrowserRes/overlib/overlib.js"><!-- overLIB (c) Erik Bosrup --></script>
  
  </HEAD>
  <body MS_POSITIONING="GridLayout">
      
	<div id="overDiv" style="position:absolute; z-index:1000;"><a href="index.html" style="text-align:center">Back to site</a></div>
	
    <form id="Form1" method="post" runat="server">&nbsp; <uc1:PhotoBrowser id=PhotoBrowser1 runat="server"></uc1:PhotoBrowser>
    </form>
		
  </body>
</HTML>
