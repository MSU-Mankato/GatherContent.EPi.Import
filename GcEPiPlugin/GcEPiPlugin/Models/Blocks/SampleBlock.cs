using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEPiPlugin.Models.Blocks
{
    [ContentType(DisplayName = "SampleBlock", GUID = "aea8ae83-cd8e-4e00-a9b9-d5e7438d0fb5", Description = "")]
    public class SampleBlock : BlockData
    {

        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString Name { get; set; }

    }
}