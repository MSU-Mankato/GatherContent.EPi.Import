using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEpiPluginV2._0.Models.Blocks
{
    [ContentType(DisplayName = "AbcdBlock", GUID = "151114bb-1a7c-431e-9f9c-5e907e4dff03", Description = "")]
    public class AbcdBlock : BlockData
    {
        
                [CultureSpecific]
                [Display(
                    Name = "Name",
                    Description = "Name field's description",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual string Name { get; set; }
         
    }
}