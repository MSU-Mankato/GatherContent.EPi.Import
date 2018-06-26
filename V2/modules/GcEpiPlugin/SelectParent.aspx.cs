using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.PlugIn;
using Castle.Components.DictionaryAdapter;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GatherContentImport.GcEpiObjects;
using Newtonsoft.Json;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "Select Parent", Description = "Folder Selection for page import in EPiServer", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/SelectParent.aspx")]
    public partial class SelectParent : SimplePage
    {
        private readonly IContentRepository _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        private List<ContentReference> _recycleBin;
        protected string JsonItemTree;
        protected string JsonItemList;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Fetch the items (if there are any) from trash.
            _recycleBin = _contentRepository.GetDescendents(ContentReference.WasteBasket).ToList();

            if (!IsPostBack)
            {
                PopulateForm();
            }
        }


        private void PopulateForm()
        {
            var postType = Server.UrlDecode(Request.QueryString["PostType"]);
            if (postType == null)
            {
                Response.Write("<script>alert('Please navigate to Gc-Epi Template Mappings and review the Items');" +
                               "window.location='/modules/GcEpiPlugin/GcEpiTemplateMappings.aspx'</script>");
            }

            // Create an empty list to store all the content descendants.
            var sortedDescendants = new EditableList<IContent>();

            if (postType != null && postType.Equals("PageType"))
            {
                var parent = _contentRepository.Get<PageData>(ContentReference.RootPage);
                var contentItemTree = new ItemTree<PageData>(parent.ContentTypeID, parent.Name, parent.ParentLink.ID);
                SortContent(parent, sortedDescendants, contentItemTree);
                JsonItemTree = JsonConvert.SerializeObject(contentItemTree);
                JsonItemList = GetJsonItemList(parent, sortedDescendants);
            }
            else
            {
                var parent = _contentRepository.Get<ContentFolder>(ContentReference.GlobalBlockFolder);
                var contentItemTree = new ItemTree<ContentFolder>(parent.ContentTypeID, parent.Name, parent.ParentLink.ID);
                SortContent(parent, sortedDescendants, contentItemTree);
                JsonItemTree = JsonConvert.SerializeObject(contentItemTree);
                JsonItemList = GetJsonItemList(parent, sortedDescendants);
            }
        }

        private void SortContent<T>(IContent parent, ICollection<IContent> sortedDescendants, ItemTree<T> contentItemTree) where T : IContent
        {
            // Fetch the immediate children of the parent into a list with the invariant culture (Language is not specific).
            var children = _contentRepository.GetChildren<T>(parent.ContentLink, CultureInfo.InvariantCulture);

            foreach (var child in children)
            {
                // If the page is in recycle bin or if the page itself is recycle bin,
                // Then do not add it to the drop down.
                if (_recycleBin.Contains(child.ContentLink) || child.ContentLink.ID == 2) continue;

                // Add the child to sorted descendants list.
                sortedDescendants.Add(child);
                contentItemTree.AddChild(child.ContentLink.ID, child.Name, child.ParentLink.ID);

                // Check if this child contains any children. If yes, then recursively call the function.   
                if (_contentRepository.GetChildren<T>(child.ContentLink, CultureInfo.InvariantCulture).Any())
                {
                    SortContent<T>(child, sortedDescendants, contentItemTree);
                }
            }
        }

        protected void Select_OnClick(object sender, EventArgs e)
        {
            var selectedItemId = FullRegion_selectedItemId.Value;
        }

        private static string GetJsonItemList(IContent parent, ICollection<IContent> sortedDescendants)
        {
            sortedDescendants.Add(parent);
            sortedDescendants.Remove(sortedDescendants.FirstOrDefault(i => i.ContentLink.ID == 2));
            var jsonItemDictionary = sortedDescendants.Select(descendant => new Dictionary<string, string>
                {
                    {"ItemId", descendant.ContentLink.ID.ToString()},
                    {"ItemName", descendant.Name},
                    {"ParentItemId", descendant.ParentLink.ID.ToString()}
                })
                .ToList();
            return JsonConvert.SerializeObject(jsonItemDictionary);
        }
    }
}
