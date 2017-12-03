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
            var currentMapping = GcDynamicTemplateMappings
                .RetrieveStore().First(i => i.TemplateId == Session["TemplateId"].ToString());
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
            Client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            var gcTemplate = Client.GetTemplateById(templateId);
            templateName.Text = gcTemplate.Name;
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            projectName.Text = Client.GetProjectById(projectId).Name;
            templateDescription.Text = gcTemplate.Description;
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            switch (currentMapping.PostType)
            {
                case "PageType":
                    ddlDefaultParent.Items.Add(new ListItem("RootPage", "1"));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.RootPage))
                    {
                        try
                        {
                            var pageData = contentRepository.Get<PageData>(cr);
                            if (pageData.ContentLink.ID == 2 || pageData.ParentLink.ID == 2) continue;
                            ddlDefaultParent.Items.Add(new ListItem(pageData.PageName, pageData.ContentLink.ID.ToString()));
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    break;
                case "BlockType":
                    ddlDefaultParent.Items.Add(new ListItem("Root Folder", "3"));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse("3")))
                    {
                        try
                        {
                            var blockData = contentRepository.Get<BlockData>(cr);
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var content = blockData as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            if (content.ContentLink.ID == 2 || content.ContentLink.ID == 2) continue;
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
                        try
                        {
                            if (cs.ItemId != gcItem.Id) continue;
                            var pageData = contentRepository.Get<PageData>(cs.ContentGuid);
                            if (pageData.ParentLink.ID == 2)
                            {
                                GcDynamicImports.DeleteItem(cs.Id);
                            }
                            else
                            {
                                linkIsImported.Text = "Page Imported";
                                linkIsImported.NavigateUrl = pageData.LinkURL;
                                parentId = pageData.ParentLink.ID.ToString();
                                enableItemFlag = false;
                                break;
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
                    _defaultParentId = queryDefaultParentId.IsNullOrEmpty() ? "3" : queryDefaultParentId;
                    foreach (var cs in contentStore)
                    {
                        try
                        {
                            if (cs.ItemId != gcItem.Id) continue;
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var blockData = contentRepository.Get<BlockData>(cs.ContentGuid) as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            if (blockData.ParentLink.ID == 2)
                            {
                                GcDynamicImports.DeleteItem(cs.Id);
                            }
                            else
                            {
                                linkIsImported.Text = "Block Imported";
                                parentId = blockData.ParentLink.ID.ToString();
                                enableItemFlag = false;
                                break;
                            }
                        }
                        catch (TypeMismatchException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            if (e.Item.FindControl("ddlParentId") is DropDownList dropDownListParentId)
            {
                dropDownListParentId.ID = $"txt{gcItem.Id}";
                if (currentMapping.PostType == "PageType")
                {
                    var parentData = contentRepository.Get<PageData>(ContentReference.Parse(_defaultParentId));
                    dropDownListParentId.Items.Add(new ListItem(parentData.PageName, parentData.ContentLink.ID.ToString())); 
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse(_defaultParentId)))
                    {
                        try
                        {
                            var pageData = contentRepository.Get<PageData>(cr);
                            if (pageData.ContentLink.ID == 2 || pageData.ParentLink.ID == 2) continue;
                            dropDownListParentId.Items.Add(new ListItem(pageData.PageName, pageData.ContentLink.ID.ToString()));
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
                    var parentData = contentRepository.Get<BlockData>(ContentReference.Parse(_defaultParentId)) as IContent;
                    // ReSharper disable once PossibleNullReferenceException
                    dropDownListParentId.Items.Add(new ListItem(parentData.Name, parentData.ContentLink.ID.ToString()));
                    foreach (var cr in contentRepository.GetDescendents(ContentReference.Parse(_defaultParentId)))
                    {
                        try
                        {
                            var blockData = contentRepository.Get<BlockData>(cr);
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var content = blockData as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            if (content.ContentLink.ID == 2 || content.ContentLink.ID == 2) continue;
                            dropDownListParentId.Items.Add(new ListItem(content.Name, content.ContentLink.ID.ToString()));
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
                var parentId = Request.Form[key.ToString().Replace("chk", "txt")];
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