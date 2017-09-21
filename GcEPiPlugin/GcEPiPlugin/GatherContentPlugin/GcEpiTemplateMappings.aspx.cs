using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,
                credentialsStore.ToList().First().Email);
            var mappings = GcDynamicTemplateMappings.RetrieveStore();
            rptTableMappings.DataSource = mappings;
            rptTableMappings.DataBind();
        }

        protected void BtnAddNew_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/GatherContentPlugin/NewGcMapping.aspx");
        }

        protected void BtnDeleteTemplate_OnClick(object sender, EventArgs e)
        {
            foreach (Control item in rptTableMappings.Items)
            {
                if (item.FindControl("") is CheckBox checkBox)
                {
                    var templateId = checkBox.ID.Substring(17,6);
                }
            }
            PopulateForm();
        }

        protected void RptTableMappings_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var map = e.Item.DataItem as GcDynamicTemplateMappings;
            if (map == null) return;
            var slug = Client.GetAccountById(Convert.ToInt32(map.AccountId)).Slug;
            if (e.Item.FindControl("lnkButtonTemplate") is LinkButton linkButtonTemplate)
            {
                var serializedStatusMaps = JsonConvert.SerializeObject(map.StatusMaps);
                var serializedEpiFieldMaps = JsonConvert.SerializeObject(map.EpiFieldMaps);
                linkButtonTemplate.PostBackUrl =
                    $"~/GatherContentPlugin/NewGcMappingV4.aspx?AccountId={map.AccountId}" +
                    $"&ProjectId={map.ProjectId}&TemplateId={map.TemplateId}&PostType={map.PostType}&Author={map.Author}" +
                    $"&DefaultStatus={map.DefaultStatus}&EpiContentType={map.EpiContentType}&StatusMaps={serializedStatusMaps}" +
                    $"&EpiFieldMaps={serializedEpiFieldMaps}&PublishedDateTime={map.PublishedDateTime}";
            }
            if (e.Item.FindControl("lnkAccountSlug") is HyperLink linkAccountSlug)
                linkAccountSlug.NavigateUrl = $"https://{slug}.gathercontent.com/";
            if (e.Item.FindControl("lnkProjectId") is HyperLink linkProjectId)
                linkProjectId.NavigateUrl = $"https://{slug}.gathercontent.com/projects/view/{map.ProjectId}";
            if (e.Item.FindControl("lnkTemplateId") is HyperLink linkTemplateId)
                linkTemplateId.NavigateUrl = $"https://{slug}.gathercontent.com/templates/{map.TemplateId}";
            if (e.Item.FindControl("chkTemplate") is CheckBox checkBoxTemplate)
                checkBoxTemplate.ID = $"chk{map.TemplateId}";
        }
    }
}