using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer.Personalization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Util.PlugIns;
using System.Web.UI;
using Castle.Components.DictionaryAdapter;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GatherContentImport.GcEpiObjects;
using Newtonsoft.Json;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "Select Parent", Description = "Folder Selection for page import in EPiServer", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/SelectParent.aspx")]
    public partial class SelectParent : SimplePage
    {
        private readonly IContentRepository _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        public string JsonItemTree;
       
        protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                if (!IsPostBack)
                {
                    PopulateForm();
                }
            }

            public void PopulateForm()
            {
                var postType = Server.UrlDecode(Request.QueryString["PostType"]);
                // Create an empty list to store all the content descendants.
                List<IContent> sortedDescendants = new EditableList<IContent>();

            if (postType.Equals("PageType"))
            {
                var parent = _contentRepository.Get<PageData>(ContentReference.RootPage);
                var contentItemTree = new ItemTree<PageData>(parent.ContentTypeID, parent.Name, parent.ParentLink.ID);
                SortContent(parent, sortedDescendants, contentItemTree);
                JsonItemTree = JsonConvert.SerializeObject(contentItemTree);
            }
                else
                {
                    var parent = _contentRepository.Get<ContentFolder>(ContentReference.GlobalBlockFolder);
                    var contentItemTree = new ItemTree<ContentFolder>(parent.ContentTypeID, parent.Name, parent.ParentLink.ID);
                    SortContent(parent, sortedDescendants, contentItemTree);

                }
               

        }

        private void SortContent<T>(IContent parent, ICollection<IContent> sortedDescendants, ItemTree<T> contentItemTree) where T : IContent
        {
            // Fetch the immediate children of the parent into a list with the invariant culture (Language is not specific).
            var children = _contentRepository.GetChildren<T>(parent.ContentLink, CultureInfo.InvariantCulture);
           
            foreach (var child in children)
            {
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

    }
}
