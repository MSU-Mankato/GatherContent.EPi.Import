using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.WebPages;
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
            var enableItemFlag = true;
            var credentialsStore = GcDynamicCredentials.RetrieveStore().ToList().First();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var parentId = "";
            Client = new GcConnectClient(credentialsStore.ApiKey, credentialsStore.Email);
            var currentMapping = GcDynamicTemplateMappings
                .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
            if (gcItem == null)
            {
                btnImportItem.Enabled = false;
                return;
            }
            if (e.Item.FindControl("statusName") is Label statusNameLabel)
                statusNameLabel.Text = gcItem.CurrentStatus.Data.Name;
            if (e.Item.FindControl("updatedAt") is Label updatedAtLabel)
                updatedAtLabel.Text = gcItem.UpdatedAt.Date.ToString();
            if (e.Item.FindControl("lnkIsImported") is HyperLink linkIsImported)
            {
                linkIsImported.Text = "---------";
                if (currentMapping.PostType == "PageType")
                {
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.RootPage))
                    {
                        try
                        {
                            var pageData = contentRepository.Get<PageData>(cr);
                            if (pageData.PageName != gcItem.Name || pageData.ParentLink.ID == 2) continue;
                            linkIsImported.Text = "Page Imported";
                            linkIsImported.NavigateUrl = pageData.LinkURL;
                            parentId = pageData.ParentLink.ID.ToString();
                            enableItemFlag = false;
                            break;
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                else
                {
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse("3")))
                    {
                        try
                        {
                            var blockData = contentRepository.Get<BlockData>(cr);
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var content = blockData as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            if (content.Name != gcItem.Name || content.ParentLink.ID == 2) continue;
                            linkIsImported.Text = "Block Imported";
                            parentId = content.ParentLink.ID.ToString();
                            enableItemFlag = false;
                            break;
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                } 
            }
            if (e.Item.FindControl("txtParentId") is TextBox textBoxParentId)
            {
                textBoxParentId.ID = $"txt{gcItem.Id}";
                if (enableItemFlag)
                {
                    textBoxParentId.Enabled = true;
                }
                else
                {
                    textBoxParentId.Text = parentId;
                }
            }
            if (e.Item.FindControl("chkItem") is CheckBox checkBoxItem)
            {
                checkBoxItem.ID = $"chk{gcItem.Id}";
                if (enableItemFlag)
                {
                    checkBoxItem.Enabled = true;
                    checkBoxItem.Visible = true;
                }
            }  
            if (!(e.Item.FindControl("lnkItemName") is HyperLink linkItemName)) return;
            linkItemName.Text = gcItem.Name;
            linkItemName.NavigateUrl = $"https://{Client.GetAccountById(Convert.ToInt32(credentialsStore.AccountId)).Slug}" +
                                       $".gathercontent.com/item/{gcItem.Id}";
        }

        protected void BtnImportItem_OnClick(object sender, EventArgs e)
        {
            var importCount = 0;
            foreach (var key in Request.Form)
            {
                if (!key.ToString().Contains("chk")) continue;
                var importItem = true;
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
                        var pageParent = parentId.IsEmpty() ? ContentReference.RootPage:ContentReference.Parse(parentId);
                        var selectedPageType = currentMapping.EpiContentType;
                        var pageTypeList = contentTypeRepository.List().OfType<PageType>();
                        var pageTypes = pageTypeList as List<PageType> ?? pageTypeList.ToList();
                        foreach (var pageType in pageTypes)                                                                                                                                              
                        {
                            if (selectedPageType.Substring(5) != pageType.Name) continue;
                            PageData myPage;
                            try
                            {
                                myPage = contentRepository.GetDefault<PageData>(pageParent, pageType.ID);
                            }
                            catch (EPiServerException exception)
                            {
                                Console.WriteLine(exception);
                                Response.Write("<script> alert('Invalid Parent Page ID! Try again!') </script>");
                                break;
                            }
                            foreach (var cr in contentRepository.GetDescendents(ContentReference.RootPage))
                            {
                                try
                                {
                                    var pageData = contentRepository.Get<PageData>(cr);
                                    if (pageData.PageName != item.Name || pageData.ParentLink.ID == 2) continue;
                                    Response.Write("<script> alert('Page Already Exists!') </script>");
                                    importItem = false;
                                    importCount = 0;
                                    break;
                                }
                                catch (TypeMismatchException ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
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
                            if (!importItem) continue;
                            {
                                var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
                                saveActions.ForEach(x => {
                                    if (x.ToString() == currentMapping.DefaultStatus)
                                    {
                                        contentRepository.Save(myPage, x, AccessLevel.Administer);
                                    }
                                });
                                importCount++;
                            }
                        }       
                        break;
                    case "BlockType":
                        var blockParent = parentId.IsEmpty() ? ContentReference.Parse("3") : ContentReference.Parse(parentId);
                        var selectedBlockType = currentMapping.EpiContentType;
                        var blockTypeList = contentTypeRepository.List().OfType<BlockType>();
                        var blockTypes = blockTypeList as IList<BlockType> ?? blockTypeList.ToList();
                        foreach (var blockType in blockTypes)
                        {
                            if (selectedBlockType.Substring(6) != blockType.Name) continue;
                            BlockData myBlock;
                            try
                            {
                                myBlock = contentRepository.GetDefault<BlockData>(blockParent, blockType.ID);
                            }
                            catch (EPiServerException exception)
                            {
                                Console.WriteLine(exception);
                                Response.Write("<script> alert('Invalid Parent Block ID! Try again!') </script>");
                                break;
                            }
                            foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse("3")))
                            {
                                try
                                {
                                    var blockData = contentRepository.Get<BlockData>(cr);
                                    // ReSharper disable once SuspiciousTypeConversion.Global
                                    var contentBlock = blockData as IContent;
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (contentBlock.Name != item.Name || contentBlock.ParentLink.ID == 2) continue;
                                    Response.Write("<script> alert('Block Already Exists!') </script>");
                                    importItem = false;
                                    break;
                                }
                                catch (TypeMismatchException ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
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
                            if (!importItem) continue;
                            {
                                var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
                                saveActions.ForEach(x => {
                                    if (x.ToString() == currentMapping.DefaultStatus)
                                    {
                                        contentRepository.Save(content, x, AccessLevel.Administer);
                                    }
                                });
                                importCount++;
                            }
                        }
                        break;
                }
            }
            if (importCount == 1)
            {
                Response.Write("<script> alert('Item successfully imported!') </script>");
            }
            else if (importCount > 1)
            {
                Response.Write($"<script> alert('{importCount} Items successfully imported!') </script>");
            }
            PopulateForm();
        }
    }
}