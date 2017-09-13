using System;
using System.Linq;
using System.Web.UI.WebControls;
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
            var settingsStore = GcDynamicSettings.RetrieveStore();
            if (credentialsStore.Count <= 0 || settingsStore.Count <= 0 || Session["ProjectId"] == null )
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = _client.GetProjectById(projectId).Name;
            var templates = _client.GetTemplatesByProjectId(Session["ProjectId"].ToString());
            templates.ToList().ForEach(template => rblGcTemplates.Items.Add(
                new ListItem(template.Name + "<br>" + template.Description, template.Id.ToString())));
            rblGcTemplates.SelectedIndex = 0;
			Session["TemplateId"] = rblGcTemplates.SelectedValue;
            Session["PostType"] = null;
            Session["Author"] = null;
            Session["DefaultStatus"] = null;
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
			var selectedValue = Request.Form["rblGcTemplates"];
			Session["TemplateId"] = selectedValue;
			Response.Redirect("~/GatherContentPlugin/NewGcMappingV3.aspx");
        }
    }
}