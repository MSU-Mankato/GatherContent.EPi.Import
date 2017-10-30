<%@ Page Language="c#" Codebehind="GcEpiTemplateMappings.aspx.cs" AutoEventWireup="False" Inherits="GcEPiPlugin.modules.GatherContentPlugin.GcEpiTemplateMappings" Title="Template Mappings" %>
<html>
<head id="Head1" runat="server">
    <title>New GatherContent Mapping</title>
    <style type="text/css">
        .auto-style4 {
            width: 1394px;
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
        .auto-style5 {
			width: 194px;
		}
        table {
	        border: 1px solid black;
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
       <table class="auto-style4">
        <asp:Repeater runat="server" ID="rptTableMappings" OnItemCreated="RptTableMappings_OnItemCreated">
            <HeaderTemplate>
	           <thead>
                    <tr>
                        <td></td>
                        <td></td>
	                    <td>GatherContent Template</td>
						<td>GatherContent Project</td>
	                    <td>Date</td>
                    </tr>
                </thead>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkTemplate" runat="server" />
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnEditTemplateMap"  Text="Edit Mapping">
                    </asp:Button>
                        <asp:Button runat="server" ID="btnItemsReview" Text="Review Items" >   
                        </asp:Button>
                    </td>
	                <td><asp:HyperLink runat="server" ID="lnkTemplate" Target="_blank" CssClass="LinkNoUnderline">
		                <%#Client.GetTemplateById(Convert.ToInt32(Eval("TemplateId"))).Name %>
	                </asp:HyperLink></td>
                    <td><asp:HyperLink runat="server" ID="lnkProject" Target="_blank" CssClass="LinkNoUnderline">
                        <%#Client.GetProjectById(Convert.ToInt32(Eval("ProjectId"))).Name %>
                    </asp:HyperLink></td>
					<td>Published<br> <%#Eval("PublishedDateTime") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
	            
                
            </FooterTemplate>
        </asp:Repeater>     
       </table>
    <p>
        <asp:Button runat="server" ID="btnDeleteTemplate" OnClick="BtnDeleteTemplate_OnClick" Text="Delete" />
    </p>
</form>
</body>
</html>