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
using GcEPiPlugin.modules.GatherContentImport.GcDynamicClasses;

namespace GcEPiPlugin.modules.GatherContentImport
{
    [GuiPlugIn(DisplayName = "Review GC Items For Import", Description = "", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentImport/ReviewItemsForImport.aspx")]
    public partial class ReviewItemsForImport : SimplePage
    {
        protected GcConnectClient Client;
        private string _defaultParentId;
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
            if (credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup your GatherContent config first!');window.location='/modules/GatherContentImport/GatherContent.aspx'</script>");
                Visible = false;
                return;
            }

            if (Session["TemplateId"] == null || Session["ProjectId"] == null)
            {
                Response.Write("<script>alert('This page is not directly accessible! Review your GatherContent items from Template Mappings page!');window.location='/modules/GatherContentImport/GcEpiTemplateMappings.aspx'</script>");
                Visible = false;
                return;
            }
            var currentMapping = GcDynamicTemplateMappings
                .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            var gcTemplate = Client.GetTemplateById(templateId);
            templateName.Text = gcTemplate.Name;
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = Client.GetProjectById(projectId).Name;
            templateDescription.Text = gcTemplate.Description;
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var recycleBin = contentRepository.GetDescendents(ContentReference.Parse("2")).ToList();
            switch (currentMapping.PostType)
            {
                case "PageType":
                    ddlDefaultParent.Items.Add(new ListItem("Root Page", "1"));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.RootPage))
                    {
                        try
                        {
                            var pageData = contentRepository.Get<PageData>(cr);
                            if (recycleBin.Contains(pageData.ContentLink) || pageData.ContentLink.ID == 2) continue;
                            ddlDefaultParent.Items.Add(new ListItem(pageData.PageName, pageData.ContentLink.ID.ToString()));
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    break;
                case "BlockType":

                    // Add the root parent before everything else.
                    ddlDefaultParent.Items.Add(new ListItem("SysGlobalAssets", "3"));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse("3")))
                    {
                        try
                        {
                            var blockData = contentRepository.Get<ContentFolder>(cr);
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var content = blockData as IContent;

                            // If the block is in recycle bin,
                            // Then do not add it to the drop down.
                            if (recycleBin.Contains(content.ContentLink)) continue;


                            // ReSharper disable once PossibleNullReferenceException
                            if (recycleBin.Contains(content.ContentLink) || content.ContentLink.ID == 2) continue;
                            ddlDefaultParent.Items.Add(new ListItem(content.Name, content.ContentLink.ID.ToString()));
                           
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    break;
            }
            rptGcItems.DataSource = Client.GetItemsByTemplateId(templateId, projectId);
            rptGcItems.DataBind();
        }

        protected void btnDefaultParentSave_OnClick(object sender, EventArgs e)
        {
            _defaultParentId = Request.Form["ddlDefaultParent"];
            Response.Redirect($"~/modules/GatherContentImport/ReviewItemsForImport.aspx?DefaultParentId={_defaultParentId}&" +
                              $"TemplateId={Session["TemplateId"]}&ProjectId={Session["ProjectId"]}");
        }

        protected void RptGcItems_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var gcItem = e.Item.DataItem as GcItem;
            var queryDefaultParentId = Server.UrlDecode(Request.QueryString["DefaultParentId"]);
            var enableItemFlag = true;
            var credentialsStore = GcDynamicCredentials.RetrieveStore().ToList().First();
            var contentStore = GcDynamicImports.RetrieveStore();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var parentId = "";
            var recycleBin = contentRepository.GetDescendents(ContentReference.Parse("2")).ToList();
            Client = new GcConnectClient(credentialsStore.ApiKey, credentialsStore.Email);
            var currentMapping = GcDynamicTemplateMappings
                .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
            if (gcItem == null) return;
            if (e.Item.FindControl("statusName") is Label statusNameLabel)
                statusNameLabel.Text = gcItem.CurrentStatus.Data.Name;
            if (e.Item.FindControl("updatedAt") is Label updatedAtLabel)
                updatedAtLabel.Text = gcItem.UpdatedAt.Date?.ToShortDateString();
            if (e.Item.FindControl("lnkIsImported") is HyperLink linkIsImported)
            {
                linkIsImported.Text = "---------";
                if (currentMapping.PostType == "PageType")
                {
                    _defaultParentId = queryDefaultParentId.IsNullOrEmpty() ? "1" : queryDefaultParentId;
                    foreach (var cs in contentStore)
                    {
                        //Try to create page data of each page in the content store that matches the gcItemId.
                        try
                        {
                            if (cs.ItemId != gcItem.Id) continue;
                            var pageData = contentRepository.Get<PageData>(cs.ContentGuid);
                            //Setting the parentId and making sure the drop down loads from Root Page.
                            parentId = pageData.ParentLink.ID.ToString();
                            enableItemFlag = false;
                            _defaultParentId = "1";
                            //if page is in trash, then set the import status to 'Page in Trash'.
                            if (recycleBin.Contains(pageData.ContentLink))
                            {
                                linkIsImported.Text = "Page in Trash";
                                parentId = "2";
                                break;
                            }
                            linkIsImported.Text = "Page Imported";
                            linkIsImported.NavigateUrl = pageData.LinkURL;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            //This is in case the user moved the page to trash and deleted it permanently.
                            if (ex is TypeMismatchException) continue;
                            GcDynamicImports.DeleteItem(cs.Id);
                        }
                    }
                }
                else
                {
                    _defaultParentId = queryDefaultParentId.IsNullOrEmpty() ? "3" : queryDefaultParentId;
                    foreach (var cs in contentStore)
                    {
                        try
                        {
                            if (cs.ItemId != gcItem.Id) continue;
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var blockData = contentRepository.Get<BlockData>(cs.ContentGuid) as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            //Setting the parentId and making sure the drop down loads from Root Folder.
                            parentId = blockData.ParentLink.ID.ToString();
                            enableItemFlag = false;
                            _defaultParentId = "3";
                            //if the block is in trash, then set the import status to 'Block in Trash'.
                            if (recycleBin.Contains(blockData.ContentLink))
                            {
                                linkIsImported.Text = "Block in Trash";
                                parentId = "2";
                                break;
                            }
                            linkIsImported.Text = "Block Imported";
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            //This is in case the user moved the block to trash and deleted it permanently.
                            if (ex is TypeMismatchException) continue;
                            GcDynamicImports.DeleteItem(cs.Id);
                        }
                    }
                }
            }
            if (e.Item.FindControl("ddlParentId") is DropDownList dropDownListParentId)
            {
                dropDownListParentId.ID = $"ddl{gcItem.Id}";
                if (currentMapping.PostType == "PageType")
                {
                    var parentData = contentRepository.Get<PageData>(ContentReference.Parse(_defaultParentId));
                    dropDownListParentId.Items.Add(new ListItem(parentData.PageName, parentData.ContentLink.ID.ToString()));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse(_defaultParentId)))
                    {
                        try
                        {
                            var pageData = contentRepository.Get<PageData>(cr);
                            if (recycleBin.Contains(pageData.ContentLink) || pageData.ContentLink.ID == 2)
                            {
                                //if the page is in trash, then add recycle bin to the drop down.
                                if (parentId == "2")
                                    dropDownListParentId.Items.Add(new ListItem(pageData.PageName,
                                        pageData.ContentLink.ID.ToString()));
                            }
                            else
                            {
                                dropDownListParentId.Items.Add(new ListItem(pageData.PageName,
                                    pageData.ContentLink.ID.ToString()));
                            }
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                else
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    var parentData = contentRepository.Get<ContentFolder>(ContentReference.Parse(_defaultParentId));
                    // ReSharper disable once PossibleNullReferenceException
                    dropDownListParentId.Items.Add(new ListItem(parentData.Name, parentData.ContentLink.ID.ToString()));

                   
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse(_defaultParentId)))
                    {
                        try
                        {
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var blockData = contentRepository.Get<ContentFolder>(cr) as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            if (recycleBin.Contains(blockData.ContentLink) || blockData.ContentLink.ID == 2)
                            {
                                //if the block is in trash, then add recycle bin to the drop down.
                                if (parentId == "2")
                                    dropDownListParentId.Items.Add(new ListItem(blockData.Name, blockData.ContentLink.ID.ToString()));
                            }
                            dropDownListParentId.Items.Add(new ListItem(blockData.Name, blockData.ContentLink.ID.ToString()));
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                if (enableItemFlag)
                {
                    dropDownListParentId.Enabled = true;
                }
                else
                {
                    dropDownListParentId.SelectedValue = parentId;
                }
            }
            if (e.Item.FindControl("chkItem") is CheckBox checkBoxItem)
            {
                checkBoxItem.ID = $"chk{gcItem.Id}";
                if (enableItemFlag)
                {
                    checkBoxItem.Enabled = true;
                    checkBoxItem.Visible = true;
                    btnImportItem.Enabled = true;
                }
            }
            if (!(e.Item.FindControl("lnkItemName") is HyperLink linkItemName)) return;
            linkItemName.Text = gcItem.Name;
            linkItemName.NavigateUrl = $"https://{Client.GetAccountById(Convert.ToInt32(credentialsStore.AccountId)).Slug}" +
                                       $".gathercontent.com/item/{gcItem.Id}";
        }

        protected void BtnImportItem_OnClick(object sender, EventArgs e)
        {
            var contentStore = GcDynamicImports.RetrieveStore();
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
                var parentId = Request.Form[key.ToString().Replace("chk", "ddl")];
                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                switch (currentMapping.PostType)
                {
                    case "PageType":
                        var pageParent = parentId.IsEmpty() ? ContentReference.RootPage : ContentReference.Parse(parentId);
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
                            foreach (var cs in contentStore)
                            {
                                try
                                {
                                    if (cs.ItemId != item.Id) continue;
                                    var pageData = contentRepository.Get<PageData>(cs.ContentGuid);
                                    if (pageData.ParentLink.ID == 2)
                                    {
                                        GcDynamicImports.DeleteItem(cs.Id);
                                    }
                                    else
                                    {
                                        Response.Write("<script> alert('Page Already Exists!') </script>");
                                        importItem = false;
                                        importCount = 0;
                                        break;
                                    }
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
                                saveActions.RemoveAt(1);
                                var gcStatusIdForThisItem = item.CurrentStatus.Data.Id;
                                saveActions.ForEach(x => {
                                    if (x.ToString() == currentMapping.StatusMaps.Find(i => i.MappedEpiserverStatus.Split('~')[1] ==
                                                                                            gcStatusIdForThisItem).MappedEpiserverStatus.Split('~')[0])
                                    {
                                        contentRepository.Save(myPage, x, AccessLevel.Administer);
                                        var dds = new GcDynamicImports(myPage.ContentGuid, item.Id, DateTime.Now);
                                        GcDynamicImports.SaveStore(dds);
                                    }
                                    else if (currentMapping.StatusMaps.Find(i => i.MappedEpiserverStatus.Split('~')[1] == gcStatusIdForThisItem)
                                                 .MappedEpiserverStatus.Split('~')[0] == "Use Default Status")
                                    {
                                        if (x.ToString() != currentMapping.DefaultStatus) return;
                                        contentRepository.Save(myPage, x, AccessLevel.Administer);
                                        var dds = new GcDynamicImports(myPage.ContentGuid, item.Id, DateTime.Now);
                                        GcDynamicImports.SaveStore(dds);
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
                            foreach (var cs in contentStore)
                            {
                                try
                                {
                                    if (cs.ItemId != item.Id) continue;
                                    // ReSharper disable once SuspiciousTypeConversion.Global
                                    var blockData = contentRepository.Get<BlockData>(cs.ContentGuid) as IContent;
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (blockData.ParentLink.ID == 2)
                                    {
                                        GcDynamicImports.DeleteItem(cs.Id);
                                    }
                                    else
                                    {
                                        Response.Write("<script> alert('Block Already Exists!') </script>");
                                        importItem = false;
                                        break;
                                    }
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
                                saveActions.RemoveAt(1);
                                var gcStatusIdForThisItem = item.CurrentStatus.Data.Id;
                                saveActions.ForEach(x => {
                                    if (x.ToString() == currentMapping.StatusMaps.Find(i => i.MappedEpiserverStatus.Split('~')[1] ==
                                                                                            gcStatusIdForThisItem).MappedEpiserverStatus.Split('~')[0])
                                    {
                                        contentRepository.Save(content, x, AccessLevel.Administer);
                                        var dds = new GcDynamicImports(content.ContentGuid, item.Id, DateTime.Now);
                                        GcDynamicImports.SaveStore(dds);
                                    }
                                    else if (currentMapping.StatusMaps.Find(i => i.MappedEpiserverStatus.Split('~')[1] == gcStatusIdForThisItem)
                                                 .MappedEpiserverStatus.Split('~')[0] == "Use Default Status")
                                    {
                                        if (x.ToString() != currentMapping.DefaultStatus) return;
                                        contentRepository.Save(content, x, AccessLevel.Administer);
                                        var dds = new GcDynamicImports(content.ContentGuid, item.Id, DateTime.Now);
                                        GcDynamicImports.SaveStore(dds);
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
            else if (importCount == 0)
            {
                Response.Write("<script> alert('No items selected! Please select the checkbox next to the item you would " +
                               "like to import!') </script>");
            }
            PopulateForm();
        }
    }
}