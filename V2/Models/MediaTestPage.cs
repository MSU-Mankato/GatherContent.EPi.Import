using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using GatherContentImport.GcEpiMediaModels;

namespace GatherContentImport.Models
{
    [ContentType(DisplayName = "MediaTestPage", GUID = "99aa0297-3efb-4ec6-a844-dc1a5417e98c", Description = "")]
    public class MediaTestPage : PageData
    {

        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        public virtual ContentReference Image { get; set; }

    }
}