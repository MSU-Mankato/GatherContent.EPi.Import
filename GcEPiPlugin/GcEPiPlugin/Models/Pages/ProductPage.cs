using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace GcEPiPlugin.Models.Pages
{
    [ContentType(DisplayName = "ProductPage", GUID = "38802069-bd5e-4e35-ac6e-35bfe38c74e4", Description = "")]
    public class ProductPage : PageData
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
        public virtual XhtmlString Heading { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString Description { get; set; }

    }
}