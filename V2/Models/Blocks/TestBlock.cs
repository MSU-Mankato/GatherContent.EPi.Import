using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEpiPluginV2._0.Models.Blocks
{
    [ContentType(DisplayName = "TestBlock", GUID = "5d70503a-7e91-4100-a055-e1e8f53ab5fc", Description = "")]
    public class TestBlock : BlockData
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