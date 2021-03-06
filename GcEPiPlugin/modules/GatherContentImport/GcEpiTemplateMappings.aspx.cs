using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using GatherContentConnect;
using GcEPiPlugin.modules.GatherContentImport.GcDynamicClasses;
using Newtonsoft.Json;

namespace GcEPiPlugin.modules.GatherContentImport
{
    [GuiPlugIn(DisplayName = "GC-EPi Template Mappings", Description = "Shows all the template mappings for a particular account.", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentImport/GcEpiTemplateMappings.aspx")]
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
                Response.Write("<script>alert('Please setup the GatherContent config first!');window.location='/modules/GatherContentImport/GatherContent.aspx'</script>");
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
            Response.Redirect("~/modules/GatherContentImport/NewGcMappingStep1.aspx");
        }

        protected void BtnDeleteTemplate_OnClick(object sender, EventArgs e)
        {
            var mappingCount = 0;
            foreach (var key in Request.Form)
            {
                if (!key.ToString().StartsWith("rptTableMappings")) continue;
                var splitStrings = key.ToString().Split('$');
                var templateId = splitStrings[2];
                var mappingsStore = GcDynamicTemplateMappings.RetrieveStore();
                var index = mappingsStore.FindIndex(i => i.TemplateId == templateId);
                GcDynamicTemplateMappings.DeleteItem(mappingsStore[index].Id);
                mappingCount++;
            }
                Response.Write(mappingCount > 0 ? $"<script>alert('{mappingCount} template mappings successfully deleted!');</script>":
                    "<script>alert('No mappings selected! Please select the checkbox next to the " +
                               "mapping you would like to delete!');</script>");
            PopulateForm();
        }

        protected void RptTableMappings_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
	        if (!(e.Item.DataItem is GcDynamicTemplateMappings map)) return;
            var slug = Client.GetAccountById(Convert.ToInt32(map.AccountId)).Slug;
            if (e.Item.FindControl("btnEditTemplateMap") is Button buttonEditTemplateMap)
            {
                var serializedStatusMaps = JsonConvert.SerializeObject(map.StatusMaps);
                var serializedEpiFieldMaps = JsonConvert.SerializeObject(map.EpiFieldMaps);
                buttonEditTemplateMap.PostBackUrl =
                    $"~/modules/GatherContentImport/NewGcMappingStep4.aspx?AccountId={map.AccountId}" +
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
                buttonItemsReview.PostBackUrl = "~/modules/GatherContentImport/ReviewItemsForImport.aspx?" +
                                                    $"TemplateId={map.TemplateId}&ProjectId={map.ProjectId}";
        }
    }
}