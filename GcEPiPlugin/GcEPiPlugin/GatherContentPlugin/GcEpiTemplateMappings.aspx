<%@ Page Language="c#" Codebehind="GcEpiTemplateMappings.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.GcEpiTemplateMappings" Title="Template Mappings" %>
<html>
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h2>Template Mappings</h2>
    <p>
        <asp:Button runat="server" Text="Add new" ID="btnAddnew" OnClick="BtnAddNew_OnClick"/>
    </p>
    <fieldset>      
        <asp:Table runat="server" ID="tableTemplateMappings" Width="100%">
        </asp:Table>
    </fieldset>
    <p>
        <asp:Button runat="server" ID="btnApply" Text="Apply" />
    </p>
</form>
</body>
</html>