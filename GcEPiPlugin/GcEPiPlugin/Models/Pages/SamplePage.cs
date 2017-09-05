using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace GcEPiPlugin.Models.Pages
{
    [ContentType(DisplayName = "SamplePage", GUID = "4ce9889d-8c71-4dae-b1dc-cf26aa08b543", Description = "")]
    public class SamplePage : PageData
    {
        
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}