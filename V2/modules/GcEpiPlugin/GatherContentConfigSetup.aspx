<%@ Page Language="c#" Codebehind="GatherContentConfigSetup.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.GatherContentConfigSetup" Title="GatherContentConfigSetup" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>GatherContent</title>
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
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
        <%--<p>
            <asp:Label runat="server" AssociatedControlID="txtPlatformUrl">Platform URL</asp:Label> <asp:TextBox runat="server" ID="txtPlatformUrl" autocomplete="off" Width="300px" />
        </p>--%>
        <p>
            <asp:Label runat="server" AssociatedControlID="ddlGcAccounts">Accounts</asp:Label> 
            <asp:DropDownList runat="server" class="chosen-select" Height="30px" Width="250px" ID="ddlGcAccounts"/>
        </p>
        <p> 
            <asp:Button  runat="server" ID="btnSave" Text="Save Changes" OnClick="BtnSave_OnClick" OnClientClick ="loadingAnimation()"/>
        </p>
    </fieldset>
    <p>&nbsp;</p>

</form>
</body>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</html>