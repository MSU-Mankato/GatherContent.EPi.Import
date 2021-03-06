<%@ Page Language="c#" Codebehind="NewGcMappingStep1.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.modules.GatherContentImport.NewGcMappingStep1" Title="New GatherContent Mapping" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
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
            <asp:Button ID="btnApiStep" runat="server" PostBackUrl="GatherContent.aspx" Text="Back to API step" />
            <asp:Button runat="server" ID="btnNextStep" Text="Next Step" OnClick="BtnNextStep_OnClick"/> 
	
        </p>
    </fieldset>
</form>
</body>
</html>