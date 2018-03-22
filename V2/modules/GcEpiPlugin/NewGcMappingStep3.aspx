<%@ Page Language="c#" Codebehind="NewGcMappingStep3.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.NewGcMappingStep3" Title="NewGcMappingStep3" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
    <style type="text/css">
        .auto-style1 {
            height: 42px;
        }
        .auto-style2 {
            height: 42px;
            width: 329px;
        }
        .auto-style3 {
            width: 329px;
        }
    </style>
   
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h3>Create Mapping for: <asp:Label runat="server" ID="templateName"></asp:Label></h3>
    <h4><asp:Label runat="server" ID="templateDescription"></asp:Label></h4>
    <p style="font-style: italic;">Project: <asp:Label runat="server" ID="projectName"></asp:Label></p>
    <fieldset>
        <legend>Mapping Defaults</legend>
        <table style="width: 100%;">
            <thead>
            <tr>
                <td class="auto-style1">GatherContent Field</td>
                <td class="auto-style2">Mapped EPiServer Type</td>
                <td class="auto-style1">Page/Block Type</td>
            </tr>
            </thead>
            <tbody>
            <tr>
                <td>Post Type</td>
                <td class="auto-style3">
                    <asp:DropDownList runat="server" ID="ddlPostTypes" AutoPostBack="True" class="chosen-select" OnSelectedIndexChanged="DdlPostTypes_OnSelectedIndexChanged" Height="30px" Width="250px">
                        <asp:ListItem Text="Select a Post Type" Value="-1"></asp:ListItem>
                        <asp:ListItem Text="Page Type" Value="PageType"></asp:ListItem>
                        <asp:ListItem Text="Block Type" Value="BlockType"></asp:ListItem>
                        <%--<asp:ListItem Text="Media Type" Value="MediaType"></asp:ListItem>--%>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlEpiContentTypes" class="chosen-select" Enabled="False" Visible="False" Height="30px" Width="250px"/>
                </td>
            </tr>
            <tr>
                <td>Author</td>
                <td class="auto-style3">
                    <asp:DropDownList runat="server" ID="ddlAuthors" class="chosen-select"  Height="30px" Width="250px"/>
                </td>
            </tr>
            <tr>
                <td>Default Status</td>
                <td class="auto-style3">
                    <asp:DropDownList runat="server" ID="ddlStatuses" class="chosen-select" Height="30px" Width="250px"/>
                </td>
            </tr>
            </tbody>
        </table>
    </fieldset>
    <hr>
    <fieldset>
        <asp:Table runat="server" ID="tableGcStatusesMap" Width="100%">
        </asp:Table>
    </fieldset>
    <p>
        <asp:Button ID="btnPreviousStep" runat="server" PostBackUrl="NewGcMappingStep2.aspx" Text="Previous Step" OnClientClick="loadingAnimation()"/>
        <asp:Button runat="server" ID="btnNextStep" Enabled="False" Text="Next Step" OnClick="BtnNextStep_OnClick" OnClientClick="loadingAnimation()"/> 
    </p>
</form>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</body>
</html>