using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using EPiServer;
using EPiServer.Security;
using GatherContentConnect;
using GcEPiPlugin.modules.GatherContentPlugin.GcDynamicClasses;


namespace GcEPiPlugin.modules.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New GC Mapping Step 2", Description = "part 2 of gc mapping", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentPlugin/NewGcMappingStep2.aspx")]
    public partial class NewGcMappingStep2 : SimplePage
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
            var rblTemp = new RadioButtonList();
            foreach (var template in templates)
            {
                if (mappings.Any(mapping => mapping.TemplateId == template.Id.ToString()))
                {
                    rblTemp.Items.Add(new ListItem( $"{template.Name} &nbsp; <a href='/modules/GatherContentPlugin/ReviewItemsForImport.aspx?" +
                                                   $"TemplateId={template.Id}&ProjectId={projectId}'> " +
                                                   $"Review Items for Import </a> <br>{template.Description}", template.Id.ToString()){ Enabled = false });
                }
                else
                {
                    rblGcTemplates.Items.Add(new ListItem(template.Name + "<br>" + template.Description, template.Id.ToString()));
                }
            }
            foreach (ListItem item in rblTemp.Items)
            {
                rblGcTemplates.Items.Add(item);
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
            Response.Redirect("~/modules/GatherContentPlugin/NewGcMappingStep3.aspx");
        }
    }
}