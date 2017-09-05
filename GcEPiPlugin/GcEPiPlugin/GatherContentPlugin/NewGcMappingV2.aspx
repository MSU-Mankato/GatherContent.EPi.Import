<%@ Page Language="c#" Codebehind="NewGcMappingV2.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.NewGcMappingV2" Title="New Mapping Part-2" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <fieldset>
        <h3>Next, Select a template to map.</h3>
        <h4>
            <asp:Label runat="server" ID="projectName" AssociatedControlID="rblGcTemplates"></asp:Label> 
        </h4>
        <asp:RadioButtonList ID="rblGcTemplates" runat="server"/>
        <p>
            <asp:Button ID="btnPreviousStep" runat="server" PostBackUrl="NewGcMapping.aspx" Text="Previous Step" />
            <asp:Button runat="server" ID="btnNextStep" Text="Next Step" OnClick="BtnNextStep_OnClick"/> 
        </p>
    </fieldset>
</form>
</body>
</html>