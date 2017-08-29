<%@ Page Language="c#" Codebehind="NewGcMapping.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.NewGcMapping" Title="New GatherContent Mapping" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <fieldset>
        <p>
            <asp:Label runat="server" ID="txtAccountName" AssociatedControlID="rblGcProjects"></asp:Label> 
            <asp:RadioButtonList EnableViewState="True" ViewStateMode="Enabled" runat="server" ID="rblGcProjects"/>
        </p>
        <p>
            <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="BtnSave_OnClick"/> 
        </p>
    </fieldset>
</form>
</body>
</html>