using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEPiPlugin.Models.Blocks
{
    [ContentType(DisplayName = "Test3Block", GUID = "5eaa8d43-98bf-436e-9c2c-2d59f0ccab30", Description = "")]
    public class Test3Block : BlockData
    {

        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Name { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Overview",
            Description = "Overview field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Overview { get; set; }

    }
}