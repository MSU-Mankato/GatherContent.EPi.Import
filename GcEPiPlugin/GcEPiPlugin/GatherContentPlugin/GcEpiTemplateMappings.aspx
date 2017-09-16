<%@ Page Language="c#" Codebehind="GcEpiTemplateMappings.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.GatherContentPlugin.GcEpiTemplateMappings" Title="Template Mappings" %>
<html>
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <style type="text/css">
        .auto-style1 {
            width: 408px;
            height: 42px;
        }
        .auto-style3 {
            width: 319px;
            height: 42px;
        }
        .auto-style4 {
            width: 1481px;
        }
        .auto-style5 {
            height: 42px;
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
        <asp:Repeater runat="server" ID="rptTableMappings" OnItemCreated="rptTableMappings_OnItemCreated">
            <HeaderTemplate>
                <thead>
                    <tr>
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
                    <td><asp:LinkButton runat="server" ID="lnkButtonTemplate" OnClick="lnkButtonTemplate_OnClick">
                        <%#Client.GetTemplateById(Convert.ToInt32(Eval("TemplateId"))).Name %>
                    </asp:LinkButton></td>
                    <td>Published <br> <%#Eval("PublishedDateTime") %></td>
                    <td><asp:LinkButton runat="server" ID="lnkButtonAccountSlug">
                        <%#Client.GetAccountById(Convert.ToInt32(Eval("AccountId"))).Slug %>
                    </asp:LinkButton></td>
                    <td><asp:LinkButton runat="server" ID="lnkButtonProjectId">
                        <%#Eval("ProjectId") %>
                    </asp:LinkButton></td>
                    <td><asp:LinkButton runat="server" ID="lnkButtonTemplateId">
                        <%#Eval("TemplateId") %>
                    </asp:LinkButton></td>
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
        <asp:Button runat="server" ID="btnApply" OnClick="BtnApply_OnClick" Text="Apply" />
    </p>
</form>
</body>
</html>