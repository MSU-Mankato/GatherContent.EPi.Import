using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors.SelectionFactories;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using ISelectionFactory = EPiServer.Personalization.VisitorGroups.ISelectionFactory;

namespace GatherContentImport.Models.Blocks
{
    [ContentType(DisplayName = "TestBlock", GUID = "7af3e82f-2fa0-405a-8412-d76792ac97f6", Description = "")]
    public class TestBlock : BlockData
    {
        
        [CultureSpecific]
        [Display(
            Name = "Heading",
            Description = "Headline for the block",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Body",
            Description = "Body content for the block",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual string Body { get; set; }

        [SelectOne(SelectionFactoryType = typeof(SelectionFactory))]
        public virtual string SingleLanguage { get; set; }

    }

    public class SelectionFactory : ISelectionFactory
    {
        public IEnumerable<SelectListItem> GetSelectListItems(Type propertyType)
        {
            return new List<SelectListItem>();
        }
    }
}