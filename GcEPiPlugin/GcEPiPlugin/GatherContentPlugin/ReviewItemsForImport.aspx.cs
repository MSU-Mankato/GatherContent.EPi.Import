using System;
using System.Collections.Generic;
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
                checkBoxItem.ID = $"chk{gcItem.Id}";
            if (e.Item.FindControl("txtParentId") is TextBox textBoxParentId)
                textBoxParentId.ID = $"txt{gcItem.Id}";
            if (!(e.Item.FindControl("lnkItemName") is HyperLink linkItemName)) return;
            linkItemName.Text = gcItem.Name;
            linkItemName.NavigateUrl = $"https://{Client.GetAccountById(Convert.ToInt32(credentialsStore.AccountId)).Slug}" +
                                       $".gathercontent.com/item/{gcItem.Id}";
        }

        protected void BtnImportItem_OnClick(object sender, EventArgs e)
        {
            foreach (var key in Request.Form)
            {
                if (!key.ToString().Contains("chk")) continue;
                var splitString = key.ToString().Split('$');
                var credentialsStore = GcDynamicCredentials.RetrieveStore().ToList().First();
                Client = new GcConnectClient(credentialsStore.ApiKey, credentialsStore.Email);
                var itemId = splitString[2].Substring(3);
                var item = Client.GetItemById(itemId);
                var currentMapping = GcDynamicTemplateMappings
                    .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
                var parentId = Request.Form[key.ToString().Replace("chk", "txt")];
                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                switch (currentMapping.PostType)
                {
                    case "PageType":
                        var pageParent = ContentReference.Parse(parentId);
                        var selectedPageType = currentMapping.EpiContentType;
                        var pageTypeList = contentTypeRepository.List().OfType<PageType>();
                        var pageTypes = pageTypeList as IList<PageType> ?? pageTypeList.ToList();
                        if (parentId.IsNullOrEmpty() || pageTypes.All(a => a.ID != Convert.ToInt32(parentId)))
                            pageParent = ContentReference.RootPage;
                        foreach (var pageType in pageTypes)                                                                                                                                              
                        {
                            if (selectedPageType.Substring(5) != pageType.Name) continue;
                            var myPage = contentRepository.GetDefault<PageData>(pageParent, pageType.ID);
                            myPage.PageName = item.Name;
                            foreach (var map in currentMapping.EpiFieldMaps)
                            {
                                var splitStrings = map.Split('~');
                                var fieldName = splitStrings[0];
                                var propDef = pageType.PropertyDefinitions.ToList().Find(p => p.Name == fieldName);
                                if (propDef == null) continue;
                                var configs = item.Config.ToList();
                                configs.ForEach(j => j.Elements.ForEach(x =>
                                {
                                    if (x.Name == splitStrings[1])
                                        myPage.Property[propDef.Name].Value = x.Value;
                                }));
                            }
                            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
                            saveActions.ForEach(x => { if (x.ToString() == currentMapping.DefaultStatus)
                                {
                                    contentRepository.Save(myPage, x, AccessLevel.Administer);
                                }
                            });
                        }       
                        break;
                    case "BlockType":
                        var blockParent = ContentReference.Parse(parentId);
                        var selectedBlockType = currentMapping.EpiContentType;
                        var blockTypeList = contentTypeRepository.List().OfType<BlockType>();
                        var blockTypes = blockTypeList as IList<BlockType> ?? blockTypeList.ToList();
                        if (parentId.IsNullOrEmpty() || blockTypes.All(a => a.ID != Convert.ToInt32(parentId)))
                            blockParent = ContentReference.Parse("3");
                        foreach (var blockType in blockTypes)
                        {
                            if (selectedBlockType.Substring(6) != blockType.Name) continue;
                            var myBlock = contentRepository.GetDefault<BlockData>(blockParent, blockType.ID);
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var content = myBlock as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            content.Name = item.Name;
                            foreach (var map in currentMapping.EpiFieldMaps)
                            {
                                var splitStrings = map.Split('~');
                                var fieldName = splitStrings[0];
                                var propDef = blockType.PropertyDefinitions.ToList().Find(p => p.Name == fieldName);
                                if (propDef == null) continue;
                                var configs = item.Config.ToList();
                                configs.ForEach(j => j.Elements.ForEach(x =>
                                {
                                    if (x.Name == splitStrings[1])
                                        myBlock.Property[propDef.Name].Value = x.Value;
                                }));
                            }
                            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
                            saveActions.ForEach(x => {
                                if (x.ToString() == currentMapping.DefaultStatus)
                                {
                                    contentRepository.Save(content, x, AccessLevel.Administer);
                                }
                            });
                        }
                        break;
                }
            }
            PopulateForm();
        }
    }
}