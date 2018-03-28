<%@ Page Language="c#" Codebehind="NewGcMappingStep4.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.NewGcMappingStep4" Title="NewGcMappingStep4" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h3>Create Mapping for: <asp:Label runat="server" ID="templateName"></asp:Label></h3>
    <h4><asp:Label runat="server" ID="templateDescription"></asp:Label></h4>
    <p style="font-style: italic;">Project: <asp:Label runat="server" ID="projectName"></asp:Label></p>
    <fieldset>
        <legend>Content</legend>        
        <asp:Table runat="server" ID="tableMappings" cellpadding="7" CellSpacing="7" Width="100%">
        </asp:Table>
    </fieldset>
    <p>
        <asp:Button ID="btnPreviousStep" runat="server" PostBackUrl="NewGcMappingStep3.aspx?ddlEpiContentType=enable" Text="Previous Step" OnClientClick="loadingAnimation()"/>
        <asp:Button runat="server" ID="btnSaveMapping" Text="Save Mapping" OnClick="BtnSaveMapping_OnClick"
                    OnClientClick="return confirmDialog('Are you sure you want to save this mapping?');" /> 
    </p>
</form>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</body>
</html>