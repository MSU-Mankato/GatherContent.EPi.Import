<%@ Page Language="c#" Codebehind="ReviewItemsForImport.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.modules.GatherContentPlugin.ReviewItemsForImport" Title="ReviewItemsForImport" %>
<html>
<head id="Head1" runat="server">
    <title>Review Items For Import</title>
    <style type="text/css">
        .auto-style4 {
            width: 1477px;
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
        .auto-style7 {
            height: 25px;
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
            <asp:Repeater runat="server" ID="rptGcItems" OnItemCreated="RptGcItems_OnItemCreated">
                <HeaderTemplate>
                    <thead>
                    <tr>
                        <td></td>
                        <td>Item</td>
                        <td>Status</td>
                        <td>Updated On</td>
                        <td>Parent Page/Block ID</td>
                        <td>Import Status</td>
                    </tr>
                    </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkItem" runat="server" Enabled="False" Visible="False"/>
                        </td>
                        <td>
                            <asp:HyperLink runat="server" ID="lnkItemName" Target="_blank" CssClass="LinkNoUnderline">
                            </asp:HyperLink>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="statusName">
                            </asp:Label>
                        </td>
                        <td><asp:Label runat="server" ID="updatedAt">
                        </asp:Label></td>
                        <td>
                            <asp:TextBox runat="server" ID="txtParentId" Enabled="False"></asp:TextBox>
                        </td>
                        <td>
                            <asp:HyperLink runat="server" Target="_blank" ID="lnkIsImported" CssClass="LinkNoUnderline"></asp:HyperLink>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </fieldset>
    <p>
        <asp:Button runat="server" ID="btnImportItem" Text="Import Items" Enabled="False" OnClick="BtnImportItem_OnClick" />
    </p>
</form>
</body>
</html>