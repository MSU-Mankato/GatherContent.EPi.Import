<%@ Page Language="c#" Codebehind="ReviewItemsForImport.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.modules.GatherContentImport.ReviewItemsForImport" Title="ReviewItemsForImport" %>
<html>
<head id="Head1" runat="server">
    <link href=�/EPiServer/CMS/App_Themes/Default/Styles/system.css� type=�text/css� rel=�stylesheet�>
    <link href=�/EPiServer/CMS/App_Themes/Default/Styles/ToolButton.css� type=�text/css� rel=�stylesheet�>
    <title>Review Items For Import</title>
    <style type="text/css">
        .auto-style4 {
            width: 1477px;
        }
        .auto-style4 tr:nth-child(even){
            background-color: #f2f2f2;
        }
        .auto-style4 thead {
            background-color: #d2d2d2;
        }
        .auto-style4 thead tr td{
            font-weight: bold;
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
        td {
            padding-top: 1%;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h2>Import Items For: <asp:Label runat="server" ID="templateName"></asp:Label></h2>
    <h4><asp:Label runat="server" ID="templateDescription"></asp:Label></h4>
    <p style="font-style: italic;">Project: <asp:Label runat="server" ID="projectName"></asp:Label></p>
    <h4>Default Parent Page/Block:
        <asp:DropDownList runat="server" ID="ddlDefaultParent"/> &nbsp; &nbsp;
        <asp:Button runat="server" Text="Set" ID="btnDefaultParentSave" OnClick="btnDefaultParentSave_OnClick"/>
    </h4>
        <table class="auto-style4">
            <asp:Repeater runat="server" ID="rptGcItems" OnItemCreated="RptGcItems_OnItemCreated">
                <HeaderTemplate>
                    <thead>
                    <tr>
                        <td></td>
                        <td>Item</td>
                        <td>Status</td>
                        <td>Updated</td>
                        <td>Parent Page/Block</td>
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
                            <asp:DropDownList runat="server" ID="ddlParentId" Enabled="False">
                            </asp:DropDownList>
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
    <p>
        <asp:Button runat="server" ID="btnPrevious" Text="Back" PostBackUrl="GcEpiTemplateMappings.aspx" />
        <asp:Button runat="server" ID="btnImportItem" Text="Import Items" Enabled="False" OnClick="BtnImportItem_OnClick" />
    </p>
</form>
</body>
</html>