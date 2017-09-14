using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using EPiServer;
using EPiServer.Security;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping Part 2", Description = "part 2 of gc mapping", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/NewGcMappingV2.aspx")]
    public partial class NewGcMappingV2 : SimplePage
    {

        private GcConnectClient _client;
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
            if (credentialsStore.IsNullOrEmpty() || Session["ProjectId"] == null )
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = _client.GetProjectById(projectId).Name;
            var templates = _client.GetTemplatesByProjectId(Session["ProjectId"].ToString());
            var mappings = GcDynamicTemplateMappings.RetrieveStore();
            foreach (var template in templates)
            {
                if (mappings.Any(mapping => mapping.ProjectId == Session["ProjectId"].ToString() && mapping.TemplateId == template.Id.ToString()))
                {
                    rblGcTemplates.Items.Add(new ListItem(template.Name + " <a href='https://mnsu.gathercontent.com'> " +
                                                          "Edit Template Mapping </a> <br>" +
                                                          template.Description, template.Id.ToString()){ Enabled = false });
                }
                else
                {
                    rblGcTemplates.Items.Add(new ListItem(template.Name + "<br>" + template.Description, template.Id.ToString()));
                }
            }
            var buffer = new ListItem[rblGcTemplates.Items.Count];
            rblGcTemplates.Items.CopyTo(buffer, 0);
            if (buffer.First().Enabled)
            {
                rblGcTemplates.SelectedIndex = 0;
                Session["TemplateId"] = rblGcTemplates.SelectedValue;
            }
            Session["PostType"] = null;
            Session["Author"] = null;
            Session["DefaultStatus"] = null;
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
			var selectedValue = Request.Form["rblGcTemplates"];
            if (selectedValue == null)
            {
                PopulateForm();
                return;
            }
            Session["TemplateId"] = selectedValue;
            Response.Redirect("~/GatherContentPlugin/NewGcMappingV3.aspx");
        }
    }
}