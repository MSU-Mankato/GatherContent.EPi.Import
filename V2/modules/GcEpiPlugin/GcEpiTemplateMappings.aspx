<%@ Page Language="c#" CodeBehind="GcEpiTemplateMappings.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.GcEpiTemplateMappings" Title="GcEpiTemplateMappings" %>

<html>
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
    <style type="text/css">
        .LinkNoUnderline {
            text-decoration: none;
        }

        .btn {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #000;
            background-color: #ababab;
        }

        .auto-style5 {
            width: 194px;
        }

        .auto-style4 {
            width: 1477px;
        }

            .auto-style4 tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .auto-style4 thead {
                background-color: #d2d2d2;
            }

                .auto-style4 thead tr td {
                    font-weight: bold;
                }

        .LinkNoUnderline {
            text-decoration: none;
        }

        .auto-style7 {
            height: 25px;
        }

        td {
            padding-top: 1%;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>GatherContent</h1>
        <h2>Template Mappings</h2>
        <p>
            <asp:Button runat="server" Text="Add new" ID="btnAddnew" OnClick="BtnAddNew_OnClick" />
        </p>
        <table class="auto-style4">
            <asp:Repeater runat="server" ID="rptTableMappings" OnItemCreated="RptTableMappings_OnItemCreated">
                <HeaderTemplate>
                    <thead>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>GatherContent Template</td>
                            <td>GatherContent Project</td>
                            <td>Published Date</td>
                        </tr>
                    </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <h4>
                                <asp:CheckBox ID="chkTemplate" runat="server" /></h4>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnEditTemplateMap" Text="Edit Mapping" OnClientClick="loadingAnimation()"></asp:Button>
                            &nbsp; &nbsp;
                        <asp:Button runat="server" ID="btnItemsReview" Text="Review Items" OnClientClick="loadingAnimation()"></asp:Button>
                        </td>
                        <td>
                            <asp:HyperLink runat="server" ID="lnkTemplate" Target="_blank" CssClass="LinkNoUnderline" /></td>
                        <td>
                            <asp:HyperLink runat="server" ID="lnkProject" Target="_blank" CssClass="LinkNoUnderline" /></td>
                        <td>
                            <asp:Label runat="server" ID="publishedOn"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </table>
        <p>
            <asp:Button runat="server" ID="btnDeleteTemplate" OnClick="BtnDeleteTemplate_OnClick" Text="Delete"
                OnClientClick="return confirmDialog('Are you sure you want to delete these mapping(s)?');" />
        </p>
    </form>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js" type="text/javascript"></script>
    <script src="/modules/GcEpiPlugin/ClientResources/js/chosen.jquery.js" type="text/javascript"></script>
    <script src="/modules/GcEpiPlugin/ClientResources/js/loading.js" type="text/javascript"></script>
</body>
</html>
