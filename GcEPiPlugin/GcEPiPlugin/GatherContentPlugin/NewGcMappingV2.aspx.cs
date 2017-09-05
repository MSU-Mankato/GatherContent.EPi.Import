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
        private GcDynamicSettings _settings;
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
            if (credentialsStore.Count <= 0 || settingsStore.Count <= 0 || string.IsNullOrEmpty(settingsStore.ToList().First().ProjectId))
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(settingsStore.ToList().First().ProjectId);
            projectName.Text = _client.GetProjectById(projectId).Name;
            var templates = _client.GetTemplatesByProjectId(settingsStore.ToList().First().ProjectId);
            templates.ToList().ForEach(template => rblGcTemplates.Items.Add(
                new ListItem(template.Name + "<br>" + template.Description, template.Id.ToString())));
            if (settingsStore.Count <= 0 || string.IsNullOrEmpty(settingsStore.ToList().First().TemplateId))
            {
                rblGcTemplates.SelectedIndex = 0;
                _settings = new GcDynamicSettings(templateId: rblGcTemplates.SelectedValue);
                GcDynamicSettings.SaveStore(_settings);
                settingsStore = GcDynamicSettings.RetrieveStore();
            }
            var templateId = settingsStore.ToList().First().TemplateId;
            rblGcTemplates.SelectedValue = templateId;
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
            var selectedValue = Request.Form["rblGcTemplates"];
            _settings = new GcDynamicSettings(templateId: selectedValue);
            GcDynamicSettings.SaveStore(_settings);
            Response.Redirect("~/GatherContentPlugin/NewGcMappingV3.aspx");
        }
    }
}