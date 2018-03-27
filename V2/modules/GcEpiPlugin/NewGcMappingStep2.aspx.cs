using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Security;
using GatherContentConnect;
using GatherContentImport.GcDynamicClasses;
using EPiServer;
using Castle.Core.Internal;
using System.Linq;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "New Gc Mapping Step 2", Description = "This is where the user sets the Template Id", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/NewGcMappingStep2.aspx")]
    public partial class NewGcMappingStep2 : SimplePage
    {

        private GcConnectClient _client;
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                AccessDenied();
            }

            if (IsPostBack) return;


            PopulateForm();

        }

        private void PopulateForm()
        {

            if (_credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup your GatherContent config first!');" +
                               "window.location='/modules/GcEpiPlugin/GatherContentConfigSetup.aspx'</script>");
                Visible = false;
                return;
            }

            if (Session["ProjectId"] == null)
            {
                Response.Write("<script>alert('Please select the GatherContent Project!');" +
                               "window.location='/modules/GcEpiPlugin/NewGcMappingStep1.aspx'</script>");
                Visible = false;
                return;
            }
            _client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = _client.GetProjectById(projectId).Name;
            var templates = _client.GetTemplatesByProjectId(Session["ProjectId"].ToString());
            var mappings = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>();
            var rblTemp = new RadioButtonList();
            rblGcTemplates.Items.Clear();
            foreach (var template in templates)
            {
                if (mappings.Any(mapping => mapping.TemplateId == template.Id.ToString()))
                {
                    rblTemp.Items.Add(new ListItem($"{template.Name} &nbsp; <a href='/modules/GcEpiPlugin/ReviewItemsForImport.aspx?" +
                                                   $"TemplateId={template.Id}&ProjectId={projectId}'> " +
                                                   $"Review Items for Import </a> <br>{template.Description}", template.Id.ToString())
                        { Enabled = false });
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
            Response.Redirect("~/modules/GcEpiPlugin/NewGcMappingStep3.aspx");
        }

    }
}