using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GatherContentImport.Models.Blocks
{
    [ContentType(DisplayName = "Media_Block", GUID = "da600f75-d256-4f11-a065-23e0c312099e", Description = "")]
    public class Media_Block : BlockData
    {
            [CultureSpecific]
                [Display(
                    Name = "Title",
                    Description = "Title of block",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(
                    Name = "Body",
                    Description = "Main Body",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
        public virtual string Main_Body { get; set; }

    }
}