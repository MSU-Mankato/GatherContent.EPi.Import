using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace GcEPiPlugin.Models.Blocks
{
    [ContentType(DisplayName = "Test2Block", GUID = "427a43e1-6343-43af-b8d8-238e02119568", Description = "")]
    public class Test2Block : BlockData
    {

        [CultureSpecific]
        [Display(
            Name = "Title",
            Description = "Title field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Heading",
            Description = "Heading field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
            Name = "SubTitle",
            Description = "SubTitle field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string SubTitle { get; set; }
    }
}