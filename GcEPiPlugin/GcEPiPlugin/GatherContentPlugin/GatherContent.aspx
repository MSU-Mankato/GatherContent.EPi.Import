<%@ Page Language="c#" Codebehind="GatherContent.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.GatherContent" Title="GatherContentAccounts" %>
<%@ Import Namespace="EPiServer.Shell" %>
<%@ Import Namespace="EPiServer" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <EPiServer:RequiredClientResources RenderingArea="Header" ID="RequiredResourcesHeader" runat="server" />
    <link rel="stylesheet" type="text/css" href="<%= Paths.ToShellResource("ClientResources/Shell.css")  %>"/>
    <link rel="stylesheet" type="text/css" href="<%= Paths.ToShellResource("ClientResources/ShellCoreLightTheme.css")  %>"/>
    <link rel="stylesheet" type="text/css" href="<%= UriSupport.ResolveUrlFromUIBySettings("ClientResources/Epi/Base/CMS.css")  %>"/>
    <title>GatherContent</title>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <fieldset>
        <p>
            <asp:Label runat="server" AssociatedControlID="txtEmailAddress">Email Address</asp:Label> <asp:TextBox runat="server" ID="txtEmailAddress" autocomplete="off" Width="400px" />
        </p>
        <p>
            <asp:Label runat="server" AssociatedControlID="txtApiKey">API Key</asp:Label> <asp:TextBox runat="server" ID="txtApiKey" autocomplete="off" Width="400px" />
        </p>
        <p>
            <asp:Label runat="server" AssociatedControlID="txtPlatformUrl">Platform URL</asp:Label> <asp:TextBox runat="server" ID="txtPlatformUrl" autocomplete="off" Width="300px" />
        </p>
        <p>
            <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="BtnSave_OnClick"/> 
        </p>
    </fieldset>
     <p>
        <asp:Label runat="server" AssociatedControlID="ddlGcAccounts">Accounts</asp:Label> <asp:DropDownList runat="server" ID="ddlGcAccounts"/>
    </p>

</form>
</body>
</html>