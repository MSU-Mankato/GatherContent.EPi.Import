<%@ Page Language="c#" Codebehind="NewGcMappingStep1.aspx.cs" AutoEventWireup="False" Inherits="GcEpiPluginV2._0.modules.GatherContentImport.NewGcMappingStep1" Title="NewGcMappingStep1" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <link rel="stylesheet" href="/modules/GatherContentImport/Client_Resources/css/loading.css">
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <fieldset>
        <h3>First, Choose a project.</h3>
        <h4>
            <asp:Label runat="server" ID="accountName" AssociatedControlID="rblGcProjects" ></asp:Label> 
        </h4>
        <asp:RadioButtonList runat="server" ID="rblGcProjects" CellPadding="5" CellSpacing="5" />
        <p>
            <asp:Button ID="btnApiStep" runat="server" PostBackUrl="GatherContent.aspx" Text="Back to API step" OnClientClick="loadingAnimation()" />
            <asp:Button runat="server" ID="btnNextStep" Text="Next Step" OnClick="BtnNextStep_OnClick" OnClientClick="loadingAnimation()"/> 
	    </p>
    </fieldset>
</form>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GatherContentImport/Client_Resources/js/prism.js" type="text/javascript" charset="utf-8"></script>
<script src="/modules/GatherContentImport/Client_Resources/js/loading.js" type="text/javascript"></script>
</body>
</html>