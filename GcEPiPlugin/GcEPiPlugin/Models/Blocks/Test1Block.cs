using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEPiPlugin.Models.Blocks
{
    [ContentType(DisplayName = "Test1Block", GUID = "34c6dde0-55b5-40be-8b70-eeb5075725d0", Description = "")]
    public class Test1Block : BlockData
    {

        [CultureSpecific]
        [Display(
            Name = "Heading",
            Description = "Heading field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Main Body",
            Description = "Main Body field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string MainBody { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Description { get; set; }

    }
}