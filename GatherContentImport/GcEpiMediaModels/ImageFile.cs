using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;

namespace GatherContentImport.GcEpiMediaModels
{
    [ContentType(DisplayName = "Image File", GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageFile : ImageData
    {
        [CultureSpecific]
        [Display(
            Name = "Heading",
            Description = "Add a heading.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }


        [Display(
            Name = "Main Body",
            Description = "This is where the body of the content goes.",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual string MainBody { get; set; }
    }
}