using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace GcEPiPlugin.Models.Pages
{
    [ContentType(DisplayName = "Test3Page", GUID = "e6f26092-1409-44f3-af6a-3dff52a67ca8", Description = "")]
    public class Test3Page : PageData
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
            Name = "Article Name",
            Description = "Article Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string ArticleName { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Subtitle",
            Description = "Subtitle field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Subtitle { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Options",
            Description = "Options field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Options { get; set; }
    }
}