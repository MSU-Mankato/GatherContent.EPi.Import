using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GatherContentImport.Models.Blocks
{
    [ContentType(DisplayName = "SampleBlock", GUID = "c2dcbceb-045f-449c-9b46-ea9a840d08ba", Description = "")]
    public class SampleBlock : BlockData
    {
        
                [CultureSpecific]
                [Display(
                    Name = "Title",
                    Description = "Title of the section",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual string Title { get; set; }

                [CultureSpecific]
                [Display(
                    Name = "Description",
                    Description = "Content",
                    GroupName = SystemTabNames.Content,
                    Order = 2)]
                public virtual string Description { get; set; }

    }
}