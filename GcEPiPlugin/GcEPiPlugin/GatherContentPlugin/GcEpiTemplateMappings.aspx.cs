using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;
using Newtonsoft.Json;

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
            if (credentialsStore.IsNullOrEmpty())
            {
                Visible = false;
                return;
            }
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,
                credentialsStore.ToList().First().Email);
            var mappings = GcDynamicTemplateMappings.RetrieveStore().FindAll
                (i => i.AccountId == credentialsStore.ToList().First().AccountId);
            rptTableMappings.DataSource = mappings;
            rptTableMappings.DataBind();
        }

        protected void BtnAddNew_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/GatherContentPlugin/NewGcMapping.aspx");
        }

        protected void BtnDeleteTemplate_OnClick(object sender, EventArgs e)
        {
            foreach (var key in Request.Form)
            {
                if (!key.ToString().StartsWith("rptTableMappings")) continue;
                var splitStrings = key.ToString().Split('$');
                var templateId = splitStrings[2];
                var mappingsStore = GcDynamicTemplateMappings.RetrieveStore();
                var index = mappingsStore.FindIndex(i => i.TemplateId == templateId);
                GcDynamicTemplateMappings.DeleteItem(mappingsStore[index].Id);
            }
            PopulateForm();
        }

        protected void RptTableMappings_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
	        if (!(e.Item.DataItem is GcDynamicTemplateMappings map)) return;
            var slug = Client.GetAccountById(Convert.ToInt32(map.AccountId)).Slug;
            if (e.Item.FindControl("btnTemplate") is Button buttonTemplate)
            {
                var serializedStatusMaps = JsonConvert.SerializeObject(map.StatusMaps);
                var serializedEpiFieldMaps = JsonConvert.SerializeObject(map.EpiFieldMaps);
                buttonTemplate.PostBackUrl =
                    $"~/GatherContentPlugin/NewGcMappingV4.aspx?AccountId={map.AccountId}" +
                    $"&ProjectId={map.ProjectId}&TemplateId={map.TemplateId}&PostType={map.PostType}&Author={map.Author}" +
                    $"&DefaultStatus={map.DefaultStatus}&EpiContentType={map.EpiContentType}&StatusMaps={serializedStatusMaps}" +
                    $"&EpiFieldMaps={serializedEpiFieldMaps}&PublishedDateTime={map.PublishedDateTime}";
            }
            if (e.Item.FindControl("lnkAccountSlug") is HyperLink linkAccountSlug)
                linkAccountSlug.NavigateUrl = $"https://{slug}.gathercontent.com/";
            if (e.Item.FindControl("lnkProject") is HyperLink linkProject)
                linkProject.NavigateUrl = $"https://{slug}.gathercontent.com/projects/view/{map.ProjectId}";
            if (e.Item.FindControl("lnkTemplate") is HyperLink linkTemplate)
                linkTemplate.NavigateUrl = $"https://{slug}.gathercontent.com/templates/{map.TemplateId}";
            if (e.Item.FindControl("chkTemplate") is CheckBox checkBoxTemplate)
                checkBoxTemplate.ID = $"{map.TemplateId}";
            if (e.Item.FindControl("btnItemsReview") is Button buttonItemsReview)
                buttonItemsReview.PostBackUrl = "~/GatherContentPlugin/ReviewItemsForImport.aspx?" +
                                                    $"TemplateId={map.TemplateId}&ProjectId={map.ProjectId}";
        }
    }
}