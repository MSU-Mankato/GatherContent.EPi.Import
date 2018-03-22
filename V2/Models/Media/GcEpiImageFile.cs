using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;

namespace GatherContentImport.Models.Media
{
    [ContentType(DisplayName = "GcEpi-Image File", GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png,tga,tiff,eps,svg,webp")]
    public class GcEpiImageFile : ImageData
    {
        [CultureSpecific]
        [Display(
            Name = "Copyright",
            Description = "This is the copyright.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Copyright { get; set; }


        [Display(
            Name = "Description",
            Description = "This is the file description",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual string Description { get; set; }

        public virtual string GcFileInfo { get; set; }
    }
}