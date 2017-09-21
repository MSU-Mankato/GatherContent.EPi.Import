<%@ Page Language="c#" Codebehind="GcEpiTemplateMappings.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.GcEpiTemplateMappings" Title="Template Mappings" %>
<html>
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <style type="text/css">
        .auto-style4 {
            width: 1453px;
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
        .auto-style10 {
            width: 28px;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h1>GatherContent</h1>
    <h2>Template Mappings</h2>
    <p>
        <asp:Button runat="server" Text="Add new" ID="btnAddnew" OnClick="BtnAddNew_OnClick"/>
    </p>
    <fieldset>
       <table class="auto-style4">
        <asp:Repeater runat="server" ID="rptTableMappings" OnItemCreated="RptTableMappings_OnItemCreated">
            <HeaderTemplate>
                <thead>
                    <tr>
                        <td></td>
                        <td>Title</td>
                        <td>Date</td>
                        <td>Account Slug</td>
                        <td>Project id</td>
                        <td>Template id</td>
                    </tr>
                </thead>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkTemplate" runat="server" />
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="lnkButtonTemplate" CssClass="LinkNoUnderline"> 
                        <%#Client.GetTemplateById(Convert.ToInt32(Eval("TemplateId"))).Name %> &nbsp;
                    </asp:LinkButton ></td>
                    <td>Published <br> <%#Eval("PublishedDateTime") %></td>
                    <td><asp:HyperLink runat="server" ID="lnkAccountSlug" Target="_blank" CssClass="LinkNoUnderline ">
                        <%#Client.GetAccountById(Convert.ToInt32(Eval("AccountId"))).Slug %>
                    </asp:HyperLink></td>
                    <td><asp:HyperLink runat="server" ID="lnkProjectId" Target="_blank" CssClass="LinkNoUnderline">
                        <%#Eval("ProjectId") %>
                    </asp:HyperLink></td>
                    <td><asp:HyperLink runat="server" ID="lnkTemplateId" Target="_blank" CssClass="LinkNoUnderline">
                        <%#Eval("TemplateId") %>
                    </asp:HyperLink></td>
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
        <asp:Button runat="server" ID="btnDeleteTemplate" OnClick="BtnDeleteTemplate_OnClick" Text="Delete" />
    </p>
</form>
</body>
</html>