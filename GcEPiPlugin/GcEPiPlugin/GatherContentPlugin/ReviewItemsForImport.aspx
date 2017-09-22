<%@ Page Language="c#" Codebehind="ReviewItemsForImport.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.ReviewItemsForImport" Title="ReviewItemsForImport" %>
<html>
<head id="Head1" runat="server">
    <title>Review Items For Import</title>
    <style type="text/css">
        .auto-style4 {
            width: 1476px;
        }
        .LinkNoUnderline
        {
            text-decoration:none;
        }
        .btn 
        {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #000;
            background-color: #ababab;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h2>Import Items For: <asp:Label runat="server" ID="templateName"></asp:Label></h2>
    <h4><asp:Label runat="server" ID="templateDescription"></asp:Label></h4>
    <p style="font-style: italic;">Project: <asp:Label runat="server" ID="projectName"></asp:Label></p>
    <fieldset>
        <table class="auto-style4">
            <asp:Repeater runat="server" ID="rptGcItems" OnItemCreated="rptGcItems_OnItemCreated">
                <HeaderTemplate>
                    <thead>
                    <tr>
                        <td></td>
                        <td>Status</td>
                        <td>Item</td>
                        <td>Updated</td>
                        <td>Episerver Title</td>
                    </tr>
                    </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkItem" runat="server" />
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblStatus"></asp:Label>
                        </td>
                        <td>Updated <br> <%#Eval("PublishedDateTime") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                
                </FooterTemplate>
            </asp:Repeater>     
        </table>
        <asp:Table runat="server" ID="tableTemplateMappings" Width="100%">
        </asp:Table>
    </fieldset>
    <p>
        <asp:Button runat="server" ID="btnDeleteTemplate" Text="Delete" />
    </p>
</form>
</body>
</html>