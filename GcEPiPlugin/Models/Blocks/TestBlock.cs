using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEPiPlugin.Models.Blocks
{
    [ContentType(DisplayName = "TestBlock", GUID = "7af3e82f-2fa0-405a-8412-d76792ac97f6", Description = "")]
    public class TestBlock : BlockData
    {
        
                [CultureSpecific]
                [Display(
                    Name = "Heading",
                    Description = "Headline for the block",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
                    Name = "Body",
                    Description = "Body content for the block",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
        public virtual string Body { get; set; }

    }
}