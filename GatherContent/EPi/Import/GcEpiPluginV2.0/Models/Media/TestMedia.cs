using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace GcEpiPluginV2._0.Models.Media
{
    [ContentType(DisplayName = "TestMedia", GUID = "1a09b2e3-c323-4d7b-a87e-56f35c7fb128", Description = "")]
    [MediaDescriptor(ExtensionString = "pdf,doc,docx")]
    public class TestMedia : MediaData
    {

        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Description { get; set; }

    }
}