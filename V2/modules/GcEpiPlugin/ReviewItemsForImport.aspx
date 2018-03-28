<%@ Page Language="c#" Codebehind="ReviewItemsForImport.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.ReviewItemsForImport" Title="ReviewItemsForImport" Async="true" %>
<html>
<head id="Head1" runat="server">
    <link href="/EPiServer/CMS/App_Themes/Default/Styles/system.css" type="text/css" rel="stylesheet">
    <link href="/EPiServer/CMS/App_Themes/Default/Styles/ToolButton.css" type="text/css" rel="stylesheet">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
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
        td {
            padding-top: 1%;
            text-align: center;
        }
        
        .auto-style5 {
            width: 1471px;
        }
        
    </style>
   
</head>
<body>
<form id="form1" runat="server">
    <div  style="margin-left: 1%;">
        <h1>GatherContent</h1>
        <h2>Import Items For: <asp:Label runat="server" ID="templateName"></asp:Label></h2>
        <h4><asp:Label runat="server" ID="templateDescription"></asp:Label></h4>
        <p style="font-style: italic;">Project: <asp:Label runat="server" ID="projectName"></asp:Label></p>
        <h4>Default Parent Page/Block: &nbsp;
            <asp:DropDownList runat="server" ID="ddlDefaultParent" class="chosen-select" Height="30px" Width="300px" /> &nbsp; &nbsp;
            <asp:Button runat="server" Text="Set" ID="btnDefaultParentSave" OnClick="BtnDefaultParentSave_OnClick" OnClientClick="loadingAnimation()"/>
        </h4>
    </div>
    <fieldset style="border: none;" class="auto-style5">
        <table class="auto-style4">
            <asp:Repeater runat="server" ID="rptGcItems" OnItemCreated="RptGcItems_OnItemCreated">
                <HeaderTemplate>
                    <thead>
                    <tr>
                        <td></td>
                        <td><h4>Item</h4></td>
                        <td><h4>Status</h4></td>
                        <td><h4>Updated in GC</h4></td>
                        <td><h4>Parent Page/Block</h4></td>
                        <td><h4>Import Status</h4></td>
                        <td><h4>Imported on</h4></td>
                        <td><h4>Update Content</h4></td>
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
                            <h4>
                                <asp:DropDownList runat="server" ID="ddlParentId" class="chosen-select" Height="30px" Width="300px" Enabled="False">
                                </asp:DropDownList>
                            </h4>
                        </td>
                        <td>
                            <asp:HyperLink runat="server" Target="_blank" ID="lnkIsImported" CssClass="LinkNoUnderline"></asp:HyperLink>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="importedOn"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkUpdateContent" Text="Update Item" Visible="False" Enabled="False"/>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                
                </FooterTemplate>
            </asp:Repeater>
        </table>
        <p>
            <asp:Button runat="server" ID="btnPrevious" Text="Back" PostBackUrl="GcEpiTemplateMappings.aspx" OnClientClick ="loadingAnimation()"/>
            <asp:Button runat="server" ID="btnImportItem" Text="Import Items" Enabled="False" OnClick="BtnImportItem_OnClick" 
                        OnClientClick="return confirmDialog('Are you sure you want to import these item(s)?');"/>
            <span style="float: right; margin-right: 2%;"><asp:Button runat="server" ID="btnUpdateItem" Text="Update Items"
                                                                      Visible="False" Enabled="False" OnClick="BtnUpdateItem_OnClick"
                                                                      OnClientClick="return confirmDialog('Are you sure you want to update these item(s)?');"/></span>
        </p>
    </fieldset>
</form>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
<script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</body>
</html>