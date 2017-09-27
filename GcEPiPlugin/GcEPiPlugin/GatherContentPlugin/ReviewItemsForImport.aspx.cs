using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using GatherContentConnect;
using GatherContentConnect.Objects;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "Review Items For Import", Description = "", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/ReviewItemsForImport.aspx")]
    public partial class ReviewItemsForImport : SimplePage
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
            Session["TemplateId"] = Server.UrlDecode(Request.QueryString["TemplateId"]);
            Session["ProjectId"] = Server.UrlDecode(Request.QueryString["ProjectId"]);
            if (credentialsStore.IsNullOrEmpty() || Session["TemplateId"] == null || Session["ProjectId"] == null)
            {
                Visible = false;
                return;
            }
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            var gcTemplate = Client.GetTemplateById(templateId);
            templateName.Text = gcTemplate.Name;
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = Client.GetProjectById(projectId).Name;
            templateDescription.Text = gcTemplate.Description;
            rptGcItems.DataSource = Client.GetItemsByTemplateId(templateId, projectId);
            rptGcItems.DataBind();
        }

        protected void RptGcItems_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var gcItem = e.Item.DataItem as GcItem;
            var credentialsStore = GcDynamicCredentials.RetrieveStore().ToList().First();
            Client = new GcConnectClient(credentialsStore.ApiKey, credentialsStore.Email);
            if (gcItem == null) return;
            if (e.Item.FindControl("statusName") is Label statusNameLabel)
                statusNameLabel.Text = gcItem.CurrentStatus.Data.Name;
            if (e.Item.FindControl("updatedAt") is Label updatedAtLabel)
                updatedAtLabel.Text = gcItem.UpdatedAt.Date.ToString();
            if (e.Item.FindControl("chkItem") is CheckBox checkBoxItem)
                checkBoxItem.ID = $"{gcItem.Id}";
            if (!(e.Item.FindControl("lnkItemName") is HyperLink linkItemName)) return;
            linkItemName.Text = gcItem.Name;
            linkItemName.NavigateUrl = $"https://{Client.GetAccountById(Convert.ToInt32(credentialsStore.AccountId)).Slug}" +
                                       $".gathercontent.com/item/{gcItem.Id}";
        }

        protected void BtnImportItem_OnClick(object sender, EventArgs e)
        {
            foreach (var key in Request.Form)
            {
                if (!key.ToString().StartsWith("rptGcItems")) continue;
                var splitString = key.ToString().Split('$');
                var credentialsStore = GcDynamicCredentials.RetrieveStore().ToList().First();
                Client = new GcConnectClient(credentialsStore.ApiKey, credentialsStore.Email);
                var itemId = splitString[2];
                var item = Client.GetItemById(itemId);
                var currentMapping = GcDynamicTemplateMappings
                    .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
                var destinationUrl = ContentReference.RootPage;
                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                switch (currentMapping.PostType)
                {
                    case "PageType":
                        var selectedPageType = currentMapping.EpiContentType;
                        var pageTypeList = contentTypeRepository.List().OfType<PageType>();
                        foreach (var pageType in pageTypeList)
                        {
                            if (selectedPageType.Substring(5) != pageType.Name) continue;
                            var page = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(t => t.GetTypes())
                                .Where(t => t.IsClass && t.Namespace == "GcEPiPlugin.Models.Pages")
                                .ToList().Find(j => j.Name == pageType.Name);
                            var pageData = typeof(IContentRepository).GetMethod("GetDefault", new[] {typeof(ContentReference)})
                                .MakeGenericMethod(page).Invoke(contentRepository, new object[] { destinationUrl });
                            var myPage = (PageData)pageData;
                            myPage.PageName = item.Name;
                            foreach (var propDef in pageType.PropertyDefinitions)
                            {
                                foreach (var map in currentMapping.EpiFieldMaps)
                                {
                                    var splitStrings = map.Split('~');
                                    var label = splitStrings[0];
                                    if (label != propDef.Name) continue;
                                    var configs = item.Config.ToList();
                                    configs.ForEach(j => j.Elements.ForEach(x =>
                                    {
                                        if (x.Label == splitStrings[1])
                                            myPage.Property[label].Value = x.Value;
                                    }));
                                }
                            }
                            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
                            saveActions.ForEach(x => {if (x.ToString() == currentMapping.DefaultStatus)
                                {
                                    contentRepository.Save(myPage, x, AccessLevel.Administer);
                                }
                            } );
                        }
                                 
                        break;
                    case "BlockType":
                        break;
                }
            }
            PopulateForm();
        }
    }
}