using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using GatherContentConnect.Objects;

namespace GatherContentImport.Models.Media
{
    [ContentType(DisplayName = "Image File", GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png,tga,tiff,eps,svg,webp")]
    public class ImageFile : ImageData
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

        public virtual int GcFileId { get; set; }

        public virtual string GcFileName { get; set; }

        public virtual int GcFileItemId { get; set; }
    }
}