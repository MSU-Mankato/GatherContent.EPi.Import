using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace GcEPiPlugin.Models.Pages
{
    [ContentType(DisplayName = "Test1Page", GUID = "bdf65642-fa9e-499d-ac01-818441fc8bbd", Description = "")]
    public class Test1Page : PageData
    {

        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Heading",
            Description = "Heading field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Description { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Article",
            Description = "Article field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Article { get; set; }
    }
}