<%@ Page Language="c#" Codebehind="NewGcMappingV3.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.NewGcMappingV3" Title="NewGcMappingV3" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <style type="text/css">
        .auto-style1 {
            height: 42px;
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
                    <td class="auto-style1">Mapped EPiServer Field</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Post Type</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlPostTypes" Height="28px" Width="194px">
                            <asp:ListItem Text="Page Type" Value="PageType"></asp:ListItem>
                            <asp:ListItem Text="Block Type" Value="BlockType"></asp:ListItem>
                            <asp:ListItem Text="Media Type" Value="MediaType"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Author</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlAuthors" Height="28px" Width="194px"/>
                    </td>
                </tr>
                <tr>
                    <td>Default Status</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlStatuses" Height="28px" Width="194px"/>
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
        <asp:Button ID="btnPreviousStep" runat="server" PostBackUrl="NewGcMappingV2.aspx" Text="Previous Step" />
        <asp:Button runat="server" ID="btnNextStep" Text="Next Step" OnClick="BtnNextStep_OnClick"/> 
    </p>
</form>
</body>
</html>