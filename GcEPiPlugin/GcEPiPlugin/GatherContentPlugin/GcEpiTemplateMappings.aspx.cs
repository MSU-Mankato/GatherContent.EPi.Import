using System;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "Template Mappings", Description = "Shows all the template mappings for a particular account.", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/GcEpiTemplateMappings.aspx")]
    public partial class GcEpiTemplateMappings : SimplePage
    {
        protected GcConnectClient Client;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                AccessDenied();
            }

            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        private void PopulateForm()
        {
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,
                credentialsStore.ToList().First().Email);
            //var tHeadRow = new TableRow {Height = 42};
            //tHeadRow.Cells.Add(new TableCell {Text = "Title"});
            //tHeadRow.Cells.Add(new TableCell {Text = "Date"});
            //tHeadRow.Cells.Add(new TableCell {Text = "Account Slug"});
            //tHeadRow.Cells.Add(new TableCell {Text = "Project id"});
            //tHeadRow.Cells.Add(new TableCell {Text = "Template id"});
            //tableTemplateMappings.Rows.Add(tHeadRow);
            var mappings = GcDynamicTemplateMappings.RetrieveStore();
            rptTableMappings.DataSource = mappings;
            rptTableMappings.DataBind();
            //foreach (var i in mappings)
            //{
            //    var tRow = new TableRow();
            //    tableTemplateMappings.Rows.Add(tRow);
            //    for (var cellIndex = 1; cellIndex <= 5; cellIndex++)
            //    {
            //        var tCell = new TableCell {Width = 500};
            //        switch (cellIndex)
            //        {
            //            case 1:
            //                var linkBtnTemplate = new LinkButton
            //                {
            //                    Text = Client.GetTemplateById(Convert.ToInt32(i.TemplateId)).Name
            //                };
            //                linkBtnTemplate.Click += delegate
            //                {
            //                    Session["AccountId"] = i.AccountId;
            //                    Session["ProjectId"] = i.ProjectId;
            //                };
            //                linkBtnTemplate.PostBackUrl = "~/GatherContentPlugin/NewGcMappingV4.aspx";
            //                tCell.Controls.Add(linkBtnTemplate);
            //                break;
            //            case 2:
            //                tCell.Text = "Published <br>" + i.PublishedDateTime;
            //                break;
            //            case 3:
            //                var linkBtnSlug = new LinkButton
            //                {
            //                    Text = Client.GetAccountById(Convert.ToInt32(i.AccountId)).Slug
            //                };
            //                tCell.Controls.Add(linkBtnSlug);
            //                break;
            //            case 4:
            //                var linkBtnProjectId = new LinkButton
            //                {
            //                    Text = i.ProjectId
            //                };
            //                tCell.Controls.Add(linkBtnProjectId);
            //                break;
            //            default:
            //                var linkBtnTemplateId = new LinkButton
            //                {
            //                    Text = i.TemplateId
            //                };
            //                tCell.Controls.Add(linkBtnTemplateId);
            //                break;
            //        }
            //        tRow.Cells.Add(tCell);
            //    }
            //}
        }

        protected void BtnAddNew_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/GatherContentPlugin/NewGcMapping.aspx");
        }

        protected void BtnApply_OnClick(object sender, EventArgs e)
        {
            PopulateForm();
        }

        protected void rptTableMappings_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item) return;
            var linkButtonTemplate = e.Item.FindControl("lnkButtonTemplate") as LinkButton;
            //linkButtonTemplate.PostBackUrl = "~/GatherContentPlugin/NewGcMappingV4.aspx";
        }

        protected void lnkButtonTemplate_OnClick(object sender, EventArgs e)
        {
            Response.Write("<script>alert('hey there')</script>");
        }
    }
}