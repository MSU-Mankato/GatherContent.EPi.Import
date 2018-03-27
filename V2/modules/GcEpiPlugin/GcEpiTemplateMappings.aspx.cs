using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.PlugIn;
using EPiServer.Security;
using GatherContentConnect;
using GatherContentImport.GcDynamicClasses;
using Newtonsoft.Json;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "Gc Epi Template Mappings", Description = "This is where the user sees all the template mappings", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/GcEpiTemplateMappings.aspx")]
    public partial class GcEpiTemplateMappings : SimplePage
    {

        protected GcConnectClient Client;
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        private readonly List<GcDynamicTemplateMappings> _mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>();
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

            if (_credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup the GatherContent config first!');" +
                               "window.location='/modules/GcEpiPlugin/GatherContentConfigSetup.aspx'</script>");
                Visible = false;
                return;
            }

            Client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var mappings = _mappingsStore.FindAll
                (i => i.AccountId == _credentialsStore.ToList().First().AccountId);
            rptTableMappings.DataSource = mappings;
            rptTableMappings.DataBind();
        }

        protected void BtnAddNew_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/modules/GcEpiPlugin/NewGcMappingStep1.aspx");
        }

        protected void BtnDeleteTemplate_OnClick(object sender, EventArgs e)
        {
            var mappingCount = 0;
            foreach (var key in Request.Form)
            {
                if (!key.ToString().StartsWith("rptTableMappings")) continue;
                var splitStrings = key.ToString().Split('$');
                var templateId = splitStrings[2];

                var index = _mappingsStore.FindIndex(i => i.TemplateId == templateId);
                GcDynamicUtilities.DeleteItem<GcDynamicTemplateMappings>(_mappingsStore[index].Id);
                mappingCount++;
            }
            const string windowLocation = "window.location = '/modules/GcEpiPlugin/GcEpiTemplateMappings.aspx'";
            Response.Write(mappingCount > 0 ? $"<script>alert('{mappingCount} template mapping(s) successfully deleted!');" +
                                              $"{windowLocation}</script>" :
                                               "<script>alert('No mappings selected! Please select the checkbox next to the " +
                                              $"mapping you would like to delete!');{windowLocation}</script>");
        }

        protected void RptTableMappings_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!(e.Item.DataItem is GcDynamicTemplateMappings map)) return;
            Client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var slug = Client.GetAccountById(Convert.ToInt32(map.AccountId)).Slug;
            if (e.Item.FindControl("btnEditTemplateMap") is Button buttonEditTemplateMap)
            {
                var serializedStatusMaps = JsonConvert.SerializeObject(map.StatusMaps);
                var serializedEpiFieldMaps = JsonConvert.SerializeObject(map.EpiFieldMaps);
                buttonEditTemplateMap.PostBackUrl =
                    $"~/modules/GcEpiPlugin/NewGcMappingStep4.aspx?AccountId={map.AccountId}" +
                    $"&ProjectId={map.ProjectId}&TemplateId={map.TemplateId}&PostType={map.PostType}&Author={map.Author}" +
                    $"&DefaultStatus={map.DefaultStatus}&EpiContentType={map.EpiContentType}&StatusMaps={serializedStatusMaps}" +
                    $"&EpiFieldMaps={serializedEpiFieldMaps}&PublishedDateTime={map.PublishedDateTime}";
            }
            if (e.Item.FindControl("lnkAccountSlug") is HyperLink linkAccountSlug)
                linkAccountSlug.NavigateUrl = $"https://{slug}.gathercontent.com/";
            if (e.Item.FindControl("lnkProject") is HyperLink linkProject)
            {
                if (Client.GetProjectById(Convert.ToInt32(map.ProjectId)).Name != null)
                {
                    linkProject.Text = Client.GetProjectById(Convert.ToInt32(map.ProjectId)).Name;
                    linkProject.NavigateUrl = $"https://{slug}.gathercontent.com/templates/{map.ProjectId}";
                }
                else
                {
                    linkProject.Text = "Project is deleted";
                }
            }
            if (e.Item.FindControl("lnkTemplate") is HyperLink linkTemplate)
            {
                if (Client.GetTemplateById(Convert.ToInt32(map.TemplateId)).Name != null)
                {
                    linkTemplate.Text = Client.GetTemplateById(Convert.ToInt32(map.TemplateId)).Name;
                    linkTemplate.NavigateUrl = $"https://{slug}.gathercontent.com/templates/{map.TemplateId}";
                }
                else
                {
                    linkTemplate.Text = "Template is deleted";
                }
            }
            if (e.Item.FindControl("chkTemplate") is CheckBox checkBoxTemplate)
                checkBoxTemplate.ID = $"{map.TemplateId}";
            if (e.Item.FindControl("btnItemsReview") is Button buttonItemsReview)
                buttonItemsReview.PostBackUrl = "~/modules/GcEpiPlugin/ReviewItemsForImport.aspx?" +
                                                $"TemplateId={map.TemplateId}&ProjectId={map.ProjectId}";
            if (e.Item.FindControl("publishedOn") is Label publishedOnLabel)
                publishedOnLabel.Text = map.PublishedDateTime;
        }

    }
}