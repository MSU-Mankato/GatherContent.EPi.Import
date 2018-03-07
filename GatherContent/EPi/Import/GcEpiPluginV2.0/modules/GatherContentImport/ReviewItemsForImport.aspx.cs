using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Security;
using System.Web.WebPages;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using GatherContentConnect;
using GatherContentConnect.Objects;
using GcEpiPluginV2._0.modules.GatherContentImport.GcDynamicClasses;
using GcEpiPluginV2._0.modules.GatherContentImport.GcEpiUtilities;
using EPiServer.ServiceLocation;
using GatherContentConnect.Interface;
using PlugInArea = EPiServer.PlugIn.PlugInArea;

namespace GcEpiPluginV2._0.modules.GatherContentImport
{
    [GuiPlugIn(DisplayName = "Review Items For Import", Description = "This is where the user reviews and imports the items.", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentImport/ReviewItemsForImport.aspx")]
    public partial class ReviewItemsForImport : SimplePage
    {
        // global variables declaration.
        protected GcConnectClient Client;
        private string _defaultParentId;
        private List<GcDynamicTemplateMappings> _mappingsStore;
        private readonly IContentRepository _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        private readonly IContentTypeRepository _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        private readonly List<GcDynamicImports> _contentStore = GcDynamicUtilities.RetrieveStore<GcDynamicImports>();
        private readonly List<SaveAction> _saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
        private readonly List<IContent> _sortedContent = new EditableList<IContent>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                // Allow access to this page only if he/ she has admin access.
                AccessDenied();
            }

            if (IsPostBack) return;
            // Initial form population or http get.
            PopulateForm();
        }

        private void PopulateForm()
        {
            // Set the project and template Ids from query string.
            Session["TemplateId"] = Server.UrlDecode(Request.QueryString["TemplateId"]);
            Session["ProjectId"] = Server.UrlDecode(Request.QueryString["ProjectId"]);

            // If the DDS for credentials is null or empty, turn off the page visibility and alert the user to set up the config.
            if (_credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup your GatherContent config first!');window.location='/modules/GatherContentImport/GatherContent.aspx'</script>");
                Visible = false;
                return;
            }

            // This is to validate the user to not access this page directly.
            if (Session["TemplateId"] == null || Session["ProjectId"] == null)
            {
                Response.Write("<script>alert('This page is not directly accessible! Review your GatherContent items from Template Mappings page!');window.location='/modules/GatherContentImport/GcEpiTemplateMappings.aspx'</script>");
                Visible = false;
                return;
            }

            // Local variables initialization and setting the values for some of the form components.
            Client = new GcConnectClient(_credentialsStore.First().ApiKey, _credentialsStore.First().Email);

            // This is to make sure we only fetch mappings associated with this GcAccount.
            _mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>().
                FindAll(i => i.AccountId == _credentialsStore.First().AccountId);

            // Fetch the mapping for current template.
            var currentMapping = _mappingsStore.First(i => i.TemplateId == Session["TemplateId"].ToString());
            
            // Make a usable templateId and projectId
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            var projectId = Convert.ToInt32(Session["ProjectId"]);

            // Fetch Template details from GatherContentConnect.
            var gcTemplate = Client.GetTemplateById(templateId);

            // Set the labels with the gathered values.
            templateName.Text = gcTemplate.Name;
            projectName.Text = Client.GetProjectById(projectId).Name;
            templateDescription.Text = gcTemplate.Description;

            // Fetch the items (if there are any) from trash.
            var recycleBin = _contentRepository.GetDescendents(ContentReference.WasteBasket).ToList();

            // This is to make sure that the drop down doesn't persist previous values upon page refresh.
            ddlDefaultParent.Items.Clear();

            // Create an empty list to store all the content descendants.
            List<IContent> sortedDescendants = new EditableList<IContent>();

            // Populating the default parent selection drop down based on the type of the post type.
            switch (currentMapping.PostType)
            {
                case "PageType":
                    // Add the root parent before everything else.
                    ddlDefaultParent.Items.Add(new ListItem("Root", "1"));

                    SortContent<PageData>(_contentRepository.Get<PageData>(ContentReference.RootPage), sortedDescendants);
                    foreach (var pageData in sortedDescendants)
                    {
                        // If the page is in recycle bin or if the page itself is recycle bin,
                        // Then do not add it to the drop down.
                        if (recycleBin.Contains(pageData.ContentLink) || pageData.ContentLink.ID == 2) continue;

                        // Fetch the page data of its immediate parent.
                        var parentPage = _contentRepository.Get<PageData>(pageData.ParentLink);

                        // Add the parent's name along with the page name to avoid the confusion between the same page names.
                        ddlDefaultParent.Items.Add(new ListItem(parentPage.Name + " => " + pageData.Name, pageData.ContentLink.ID.ToString()));
                    }
                    break;
                case "BlockType":
                    // Add the root parent before everything else.
                    ddlDefaultParent.Items.Add(new ListItem("SysGlobalAssets", "3"));

                    SortContent<ContentFolder>(_contentRepository.Get<ContentFolder>(ContentReference.GlobalBlockFolder), sortedDescendants);
                    foreach (var contentFolder in sortedDescendants)
                    {
                        // If the block is in recycle bin,
                        // Then do not add it to the drop down.
                        if (recycleBin.Contains(contentFolder.ContentLink)) continue;

                        // Fetch the block data of its immediate parent.
                        var parentFolder = _contentRepository.Get<ContentFolder>(contentFolder.ParentLink);
                        // Add the parent's name along with the block name to avoid the confusion between the same block names.
                        ddlDefaultParent.Items.Add(new ListItem(parentFolder.Name + " => " + contentFolder.Name,
                            contentFolder.ContentLink.ID.ToString()));
                    }
                    break;
            }
            // Add the data source to the repeater and bind it.
            rptGcItems.DataSource = Client.GetItemsByTemplateId(templateId, projectId);
            rptGcItems.DataBind();
        }
        
        // A recursive function to sort the content (This works for both pages and content folders).
        private void SortContent<T>(IContent parent, ICollection<IContent> sortedDescendants) where T:IContent
        {
            // Fetch the immediate children of the parent into a list with the invariant culture (Language is not specific).
            var children = _contentRepository.GetChildren<T>(parent.ContentLink, CultureInfo.InvariantCulture);
            foreach (var child in children)
            {
                // Add the child to sorted descendants list.
                sortedDescendants.Add(child);

                // Check if this child contains any children. If yes, then recursively call the function.   
                if (_contentRepository.GetChildren<T>(child.ContentLink, CultureInfo.InvariantCulture).Any())
                {
                    SortContent<T>(child, sortedDescendants);
                }
            }
        }
        
        protected void BtnDefaultParentSave_OnClick(object sender, EventArgs e)
        {
            // Set the default parent Id.
            _defaultParentId = Request.Form["ddlDefaultParent"];

            // Send in the required session values and default parent Id back to the page.
            Response.Redirect($"~/modules/GatherContentImport/ReviewItemsForImport.aspx?DefaultParentId={_defaultParentId}&" +
                              $"TemplateId={Session["TemplateId"]}&ProjectId={Session["ProjectId"]}");
        }

        protected void RptGcItems_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            // Initializing the local variables.
            Client = new GcConnectClient(_credentialsStore.First().ApiKey, _credentialsStore.First().Email);
            var gcItem = e.Item.DataItem as GcItem;
            var defaultParentIdFromQuery = Server.UrlDecode(Request.QueryString["DefaultParentId"]);
            var enableItemFlag = true;
            var parentId = string.Empty;

            // This is to make sure we only fetch mappings associated with this GcAccount.
            _mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>().
                FindAll(i => i.AccountId == _credentialsStore.First().AccountId);

            // Fetch the mapping for current template.
            var currentMapping = _mappingsStore.First(i => i.TemplateId == Session["TemplateId"].ToString());
            var recycleBin = _contentRepository.GetDescendents(ContentReference.WasteBasket).ToList();
            if (gcItem == null) return;

            // Set the values of form components.
            if (e.Item.FindControl("statusName") is Label statusNameLabel)
                statusNameLabel.Text = gcItem.CurrentStatus.Data.Name;
            if (e.Item.FindControl("updatedAt") is Label updatedAtLabel)
                updatedAtLabel.Text = gcItem.UpdatedAt.Date?.ToLocalTime().ToShortDateString();
            if (e.Item.FindControl("lnkIsImported") is HyperLink linkIsImported)
            {
                linkIsImported.Text = "---------";
                _defaultParentId = currentMapping.PostType == "PageType"
                    ? (defaultParentIdFromQuery ?? "1")
                    : (defaultParentIdFromQuery ?? "3");
                foreach (var cs in _contentStore)
                {
                    // Check if the item in the Gather Content items list is in the content store.
                    if (cs.ItemId != gcItem.Id)
                    {
                        /*
                            <summary>
                                We want to clear this list because we want the drop down to load from selected default parent and 
                                if the previous item was already imported then the drop down would have all the options in them.
                                This helps in avoiding recursion-overhead. This also prevents data persistence on page reloads.
                            </summary>
                         */
                        _sortedContent.Clear();
                        continue;
                    }

                    // Item is already imported, so set the flag to false.
                    enableItemFlag = false;
                    if (currentMapping.PostType == "PageType")
                    {
                        try
                        {
                            var pageData = _contentRepository.Get<PageData>(cs.ContentGuid);

                            // Setting the parentId and making sure the drop down loads from Root Page.
                            _defaultParentId = "1";
                            parentId = pageData.ParentLink.ID.ToString();

                            // If page is in trash, then set the import status to 'Page in Trash'.
                            if (recycleBin.Contains(pageData.ContentLink))
                            {
                                linkIsImported.Text = "Page in Trash";
                                parentId = "2";
                                break;
                            }

                            // Set the import status to 'Page Imported' and add a link to the page.
                            linkIsImported.Text = "Page Imported";
                            linkIsImported.NavigateUrl = pageData.LinkURL;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            if (ex is TypeMismatchException) continue;

                            // This is in case the user moved the page to trash and deleted it permanently.
                            if (!(ex is ContentNotFoundException)) continue;
                            GcDynamicUtilities.DeleteItem<GcDynamicImports>(cs.Id);
                            enableItemFlag = true;
                        }
                    }
                    else
                    {
                        try
                        {
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var blockData = _contentRepository.Get<BlockData>(cs.ContentGuid) as IContent;
                            // ReSharper disable once PossibleNullReferenceException
                            // Setting the parentId and making sure the drop down loads from Root Folder.
                            parentId = blockData.ParentLink.ID.ToString();
                            _defaultParentId = "3"; 

                            // If the block is in trash, then set the import status to 'Block in Trash'.
                            if (recycleBin.Contains(blockData.ContentLink))
                            {
                                linkIsImported.Text = "Block in Trash";
                                parentId = "2";
                                break;
                            }

                            // Set the import status to 'Block Imported'.
                            linkIsImported.Text = "Block Imported";
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            if (ex is TypeMismatchException) continue;

                            // This is in case the user moved the page to trash and deleted it permanently.
                            if (!(ex is ContentNotFoundException)) continue;
                            GcDynamicUtilities.DeleteItem<GcDynamicImports>(cs.Id);
                            enableItemFlag = true;
                        }
                    }
                }
            }
            if (e.Item.FindControl("ddlParentId") is DropDownList dropDownListParentId)
            {
                // This control sets the parent under which the items are imported.
                dropDownListParentId.ID = $"ddl{gcItem.Id}";
                if (currentMapping.PostType == "PageType")
                {
                    // Get the parent page data.
                    var parentData = _contentRepository.Get<PageData>(ContentReference.Parse(_defaultParentId));
                    dropDownListParentId.Items.Add(new ListItem(parentData.PageName, parentData.ContentLink.ID.ToString()));
                    
                    // To reduce the recursion-overhead, we only sort the content once and store it in a global variable instead.
                    if (_sortedContent.IsNullOrEmpty())
                        SortContent<PageData>(_contentRepository.Get<PageData>(ContentReference.Parse(_defaultParentId)), _sortedContent);
                    
                    foreach (var pageData in _sortedContent)
                    {
                        if (recycleBin.Contains(pageData.ContentLink) || pageData.ContentLink.ID == 2)
                        {
                            // If the page is in trash, then add recycle bin page to the drop down so that it can be shown as the parent.
                            if (parentId == "2")
                                dropDownListParentId.Items.Add(new ListItem(
                                    _contentRepository.Get<PageData>(ContentReference.WasteBasket).Name, "2"));
                        }
                        else
                        {
                            var parentPage = _contentRepository.Get<PageData>(pageData.ParentLink);
                            dropDownListParentId.Items.Add(new ListItem(parentPage.Name + " => " + pageData.Name,
                                pageData.ContentLink.ID.ToString()));
                        }
                    }
                }
                else
                {
                    // Get the parent page data.
                    var parentData = _contentRepository.Get<ContentFolder>(ContentReference.Parse(_defaultParentId));
                    dropDownListParentId.Items.Add(new ListItem(parentData.Name, parentData.ContentLink.ID.ToString()));

                    // To reduce the recursion-overhead, we only sort the content once and store it in a global variable instead.
                    if (_sortedContent.IsNullOrEmpty())
                        SortContent<ContentFolder>(_contentRepository.Get<ContentFolder>(ContentReference.Parse(_defaultParentId)), _sortedContent);

                    if (parentId == "2")
                    {
                        // If the block is in trash, then add recycle bin page to the drop down so that it can be shown as the parent.
                        dropDownListParentId.Items.Add(new ListItem(
                            _contentRepository.Get<PageData>(ContentReference.WasteBasket).Name, "2"));
                    }
                    
                    foreach (var contentFolder in _sortedContent)
                    {
                        var parentFolder = _contentRepository.Get<ContentFolder>(contentFolder.ParentLink);
                        dropDownListParentId.Items.Add(new ListItem(parentFolder.Name + " => " + contentFolder.Name, contentFolder.ContentLink.ID.ToString()));
                    }
                }
                
                // If item is enabled, then enable the drop down containing the parents. Else, set the drop down to the content's parent value.
                if (enableItemFlag)
                {
                    dropDownListParentId.Enabled = true;
                }
                else
                {
                    dropDownListParentId.SelectedValue = parentId;
                }
            }
            if (e.Item.FindControl("chkItem") is CheckBox checkBoxItemImport)
            {
                checkBoxItemImport.ID = $"chkImport{gcItem.Id}";
                if (enableItemFlag)
                {
                    checkBoxItemImport.Enabled = true;
                    checkBoxItemImport.Visible = true;
                    btnImportItem.Enabled = true;
                }
            }

            if (e.Item.FindControl("chkUpdateContent") is CheckBox checkBoxItemUpdate)
            {
                if (!enableItemFlag)
                {
                    checkBoxItemUpdate.ID = $"chkUpdate{gcItem.Id}";
                    // ReSharper disable once PossibleInvalidOperationException
                    if (_contentStore.Any(i => i.ItemId == gcItem.Id &&
                                               gcItem.UpdatedAt.Date.Value.ToLocalTime() > i.ImportedAt))
                    {
                        var importedItem = _contentStore.Find(x => x.ItemId == gcItem.Id);
                        var content = currentMapping.PostType == "PageType"
                            ? _contentRepository.Get<PageData>(importedItem.ContentGuid)
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            : _contentRepository.Get<BlockData>(importedItem.ContentGuid) as IContent;
                        // ReSharper disable once PossibleNullReferenceException
                        if (!recycleBin.Contains(content.ContentLink))
                        {
                            checkBoxItemUpdate.Enabled = true;
                            checkBoxItemUpdate.Visible = true;
                            btnUpdateItem.Enabled = true;
                            btnUpdateItem.Visible = true;
                        }
                    }
                }
            }

            if (e.Item.FindControl("importedOn") is Label importedOnLabel)
            {
                importedOnLabel.Text = enableItemFlag ? "---------"
                    : _contentStore.Find(x => x.ItemId == gcItem.Id).ImportedAt.ToShortDateString();
            }

            if (!(e.Item.FindControl("lnkItemName") is HyperLink linkItemName)) return;
            linkItemName.Text = gcItem.Name;
            linkItemName.NavigateUrl = $"https://{Client.GetAccountById(Convert.ToInt32(_credentialsStore.First().AccountId)).Slug}" +
                                       $".gathercontent.com/item/{gcItem.Id}";
        }

        // This method saves the content in accordance with the save action selected by the user for the item.
        private void SaveContent(IContent content, IGcItem item, GcDynamicTemplateMappings currentMapping)
        {
            /*
                <summary>
                    Select the status/SaveAction from the mapping where first part of the 'MappedEpiServerStatus'
                    matches the current Gc status ID. If the 'MappedEpiServerStatus' is set to 'Use Default Status',
                    then fetch the SaveAction that matches the 'DefaultStatus' string from the mapping and select it. 
                    Else, fetch the SaveAction that matches the 'statusFromMapping' string and select it.
                </summary>
            */
            var gcStatusIdForThisItem = item.CurrentStatus.Data.Id;
            SaveAction saveAction;
            
            var statusFromMapping = currentMapping.StatusMaps
                .Find(i => i.MappedEpiserverStatus.Split('~')[1] == gcStatusIdForThisItem)
                .MappedEpiserverStatus.Split('~')[0];

            if (statusFromMapping == "Use Default Status")
            {
                saveAction = _saveActions.Find(i => i.ToString() == currentMapping.DefaultStatus);
                _contentRepository.Save(content, saveAction, AccessLevel.Administer);
            }

            else
            {
                saveAction = _saveActions.Find(i => i.ToString() == statusFromMapping);
                _contentRepository.Save(content, saveAction, AccessLevel.Administer);
            }
            
            var dds = new GcDynamicImports(content.ContentGuid, item.Id, DateTime.Now.ToLocalTime());
            GcDynamicUtilities.SaveStore(dds);
        }

        protected void BtnImportItem_OnClick(object sender, EventArgs e)
        {
            // 
            var importCounter = 0;
            var itemName = string.Empty;
            Client = new GcConnectClient(_credentialsStore.First().ApiKey, _credentialsStore.First().Email);

            // This is to make sure we only fetch mappings associated with this GcAccount.
            _mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>().
                FindAll(i => i.AccountId == _credentialsStore.First().AccountId);

            // Fetch the mapping for current template.
            var currentMapping = _mappingsStore.First(i => i.TemplateId == Session["TemplateId"].ToString());

            // There is a duplicate value called 'Default' in the list of SaveActions. So, it needs to be removed.
            _saveActions.RemoveAt(1);

            // For all the items that were selected in the checkbox, 
            foreach (var key in Request.Form)
            {
                // If the key is not of checkbox type, then continue.
                if (!key.ToString().Contains("chkImport")) continue;

                // Set the  flag initially to 'true'.
                var importItemFlag = true;

                // The key consists of repeater Id in the first part. We only need the second part where 'chkImport' is present
                // and it is after '$'. So, we split the string on '$'.
                var splitString = key.ToString().Split('$');

                // ItemId is extracted from the checkbox Id. The first part of it is always 'chkImport'. So, the Id to be extracted
                // from the 9th index.
                var itemId = splitString[2].Substring(9);

                // Get the itemId from GatherContentConnect API with the Id we extracted in the previous step.
                var item = Client.GetItemById(itemId);

                // Get the item's name. This will be used for displaying the import message.
                itemName = item.Name;

                // We know that the item's parent path Id is in the drop down. And, both checkbox and drop down share the similar
                // naming convention. So, we just get the key that contains the value of that drop down's selected value.
                var parentId = Request.Form[key.ToString().Replace("chkImport", "ddl")];

                // Since the post type of the item is known beforehand, we can separate the import process for different post types.
                switch (currentMapping.PostType)
                {
                    case "PageType":
                        var pageParent = parentId.IsEmpty() ? ContentReference.RootPage : ContentReference.Parse(parentId);
                        var selectedPageType = currentMapping.EpiContentType;
                        var pageTypes = _contentTypeRepository.List().OfType<PageType>().ToList();
                        foreach (var pageType in pageTypes)
                        {
                            if (selectedPageType.Substring(5) != pageType.Name) continue;
                            var myPage = _contentRepository.GetDefault<PageData>(pageParent, pageType.ID);
                            foreach (var cs in _contentStore)
                            {
                                try
                                {
                                    if (cs.ItemId != item.Id) continue;
                                    Response.Write("<script> alert('Page Already Exists!') </script>");
                                    importItemFlag = false;
                                    importCounter = 0;
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
                                configs.ForEach(j => j.Elements.ToList().ForEach(x =>
                                {
                                    if (x.Name != splitStrings[1]) return;
                                    switch (x.Type)
                                    {
                                        case "text":
                                            myPage.Property[propDef.Name].Value = GcEpiContentParser.TextParser(x.Value, propDef.Type.Name);
                                            break;
                                        case "section":
                                            myPage.Property[propDef.Name].Value = GcEpiContentParser.TextParser(x.Subtitle, propDef.Type.Name);
                                            break;
                                        case "choice_radio":
                                        case "choice_checkbox":
                                            myPage.Property[propDef.Name].Value = GcEpiContentParser.ChoiceParser(x.Options, x.Type, propDef);
                                            break;
                                        default:
                                            var files = Client.GetFilesByItemId(item.Id);
                                            files.ToList().ForEach(async i =>
                                            {
                                                await GcEpiContentParser.ImageParserAsync(i.Url, i.FileName);
                                            });
                                            break;
                                    }
                                }));
                            }
                            if (!importItemFlag) continue;
                            {
                                SaveContent(myPage, item, currentMapping);
                                importCounter++;
                            }
                        }
                        break;
                    case "BlockType":
                        var blockParent = parentId.IsEmpty() ? ContentReference.GlobalBlockFolder : ContentReference.Parse(parentId);
                        var selectedBlockType = currentMapping.EpiContentType;
                        var blockTypes = _contentTypeRepository.List().OfType<BlockType>().ToList();
                        foreach (var blockType in blockTypes)
                        {
                            if (selectedBlockType.Substring(6) != blockType.Name) continue;
                            var myBlock = _contentRepository.GetDefault<BlockData>(blockParent, blockType.ID);
                            foreach (var cs in _contentStore)
                            {
                                try
                                {
                                    if (cs.ItemId != item.Id) continue;
                                    Response.Redirect("<script> alert('Block Already Exists!') </script>");
                                    importItemFlag = false;
                                    importCounter = 0;
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
                                var result = new object();
                                configs.ForEach(j => j.Elements.ToList().ForEach(x =>
                                {
                                    if (x.Name != splitStrings[1]) return;
                                    if (x.Type == "text")
                                        result = GcEpiContentParser.TextParser(x.Value, propDef.Type.Name);
                                    else if (x.Type == "section")
                                        result = GcEpiContentParser.TextParser(x.Subtitle, propDef.Type.Name);
                                    else if (x.Type == "choice_radio" || x.Type == "choice_checkbox")
                                    {
                                        result = GcEpiContentParser.ChoiceParser(x.Options, x.Type, propDef);
                                    }
                                    content.Property[propDef.Name].Value = result;
                                }));
                            }
                            if (!importItemFlag) continue;
                            {
                                SaveContent(content, item, currentMapping);
                                importCounter++;
                            }
                        }
                        break;
                }
            }
            string responseMessage;
            if (importCounter == 1)
            {
                responseMessage = $"alert('{itemName} successfully imported!');";
            }

            else if (importCounter > 1)
            {
                responseMessage = $"alert('{itemName} and {importCounter - 1} other items successfully imported!');";
            }

            else
            {
                responseMessage = "alert('No items selected! Please select the checkbox next to the item you would " +
                                  "like to import!');";
            }
            Response.Write($"<script> {responseMessage} window.location = '/modules/GatherContentImport/ReviewItemsForImport.aspx?" +
                           $"&TemplateId={Session["TemplateId"]}&ProjectId={Session["ProjectId"]}'</script>");
        }

        protected void BtnUpdateItem_OnClick(object sender, EventArgs e)
        {
            var updateCount = 0;
            _saveActions.RemoveAt(1);
            Client = new GcConnectClient(_credentialsStore.First().ApiKey, _credentialsStore.First().Email);
            _mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>().
                FindAll(i => i.AccountId == _credentialsStore.First().AccountId);
            foreach (var key in Request.Form)
            {
                if (!key.ToString().Contains("chkUpdate")) continue;
                var splitString = key.ToString().Split('$');
                var itemId = splitString[2].Substring(9);
                var gcItem = Client.GetItemById(itemId);
                var importedItem = _contentStore.Find(x => x.ItemId.ToString() == itemId);
                var currentMapping = _mappingsStore.First(i => i.TemplateId == gcItem.TemplateId.ToString());
                switch (currentMapping.PostType)
                {
                    case "PageType":
                        var pageToUpdate = _contentRepository.Get<PageData>(importedItem.ContentGuid);
                        var pageType = _contentTypeRepository.List().OfType<PageType>()
                            .ToList().Find(i => i.ID == pageToUpdate.ContentTypeID);
                        var pageData = _contentRepository.Get<PageData>(ContentReference.Parse(pageToUpdate.ContentLink.ID.ToString()));
                        var pageClone = pageData.CreateWritableClone();
                        foreach (var map in currentMapping.EpiFieldMaps)
                        {
                            var splitStrings = map.Split('~');
                            var fieldName = splitStrings[0];
                            var propDef = pageType.PropertyDefinitions.ToList().Find(p => p.Name == fieldName);
                            if (propDef == null) continue;
                            var configs = gcItem.Config.ToList();
                            var result = new object();
                            configs.ForEach(j => j.Elements.ToList().ForEach(x =>
                            {
                                if (x.Name != splitStrings[1]) return;
                                if (x.Type == "text")
                                    result = GcEpiContentParser.TextParser(x.Value, propDef.Type.Name);
                                else if (x.Type == "section")
                                    result = GcEpiContentParser.TextParser(x.Subtitle, propDef.Type.Name);
                                else if (x.Type == "choice_radio" || x.Type == "choice_checkbox")
                                {
                                    result = GcEpiContentParser.ChoiceParser(x.Options, x.Type, propDef);
                                }
                                pageClone.Property[propDef.Name].Value = result;
                            }));
                        }
                        GcDynamicUtilities.DeleteItem<GcDynamicImports>(_contentStore[_contentStore.FindIndex(i => i.ItemId.ToString() == itemId)].Id);
                        SaveContent(pageClone, gcItem, currentMapping);
                        updateCount++;
                        break;
                    case "BlockType":
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        var blockToUpdate = _contentRepository.Get<BlockData>(importedItem.ContentGuid) as IContent;
                        // ReSharper disable once PossibleNullReferenceException
                        var blockType = _contentTypeRepository.List().OfType<BlockType>()
                            .ToList().Find(i => i.ID == blockToUpdate.ContentTypeID);
                        // ReSharper disable once PossibleNullReferenceException
                        var blockData = _contentRepository.Get<BlockData>(ContentReference.Parse(blockToUpdate.ContentLink.ID.ToString()));
                        var blockClone = blockData.CreateWritableClone();
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        var cloneContent = blockClone as IContent;
                        foreach (var map in currentMapping.EpiFieldMaps)
                        {
                            var splitStrings = map.Split('~');
                            var fieldName = splitStrings[0];
                            var propDef = blockType.PropertyDefinitions.ToList().Find(p => p.Name == fieldName);
                            if (propDef == null) continue;
                            var configs = gcItem.Config.ToList();
                            var result = new object();
                            configs.ForEach(j => j.Elements.ToList().ForEach(x =>
                            {
                                if (x.Name != splitStrings[1]) return;
                                if (x.Type == "text")
                                    result = GcEpiContentParser.TextParser(x.Value, propDef.Type.Name);
                                else if (x.Type == "section")
                                    result = GcEpiContentParser.TextParser(x.Subtitle, propDef.Type.Name);
                                else if (x.Type == "choice_radio" || x.Type == "choice_checkbox")
                                {
                                    result = GcEpiContentParser.ChoiceParser(x.Options, x.Type, propDef);
                                }
                                blockClone.Property[propDef.Name].Value = result;
                            }));
                        }
                        GcDynamicUtilities.DeleteItem<GcDynamicImports>(_contentStore[_contentStore.FindIndex(i => i.ItemId.ToString() == itemId)].Id);
                        SaveContent(cloneContent, gcItem, currentMapping);
                        updateCount++;
                        break;
                }
                string responseMessage;
                if (updateCount == 1)
                {
                    responseMessage = $"alert('{gcItem.Name} successfully updated!');";
                }

                else if (updateCount > 1)
                {
                    responseMessage = $"alert('{gcItem.Name} and {updateCount - 1} other items successfully updated!');";
                }

                else
                {
                    responseMessage = "alert('No items selected! Please select the checkbox in the Update Content column you would " +
                                      "like to update!');";
                }
                Response.Write($"<script> {responseMessage} window.location = '/modules/GatherContentImport/ReviewItemsForImport.aspx?" +
                               $"&TemplateId={Session["TemplateId"]}&ProjectId={Session["ProjectId"]}'</script>");
            }
        }
    }
}