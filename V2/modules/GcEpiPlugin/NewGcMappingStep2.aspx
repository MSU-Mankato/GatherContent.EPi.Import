<%@ Page Language="c#" Codebehind="NewGcMappingStep2.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.NewGcMappingStep2" Title="NewGcMappingStep2" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
     <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <fieldset>
        <h3>Next, Select a template to map.</h3>
        <h4>
            <asp:Label runat="server" ID="projectName" AssociatedControlID="rblGcTemplates"></asp:Label> 
        </h4>
        <asp:RadioButtonList ID="rblGcTemplates" runat="server" cellpadding="3" CellSpacing="3"/>
        <p>
            <asp:Button ID="btnPreviousStep" runat="server" PostBackUrl="NewGcMappingStep1.aspx" Text="Previous Step" OnClientClick="loadingAnimation()"/>
            <asp:Button runat="server" ID="btnNextStep" Text="Next Step" OnClick="BtnNextStep_OnClick" OnClientClick="loadingAnimation()"/> 
        </p>
    </fieldset>
</form>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</body>
</html>