using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace GatherContentImport.GcEpiMediaModels
{
    [ContentType(DisplayName = "GenericFile", GUID = "6143d87a-740b-44db-bfae-9d0a84570358", Description = "")]
    [MediaDescriptor(ExtensionString = "pdf,doc,docx")]
    public class GenericFile : MediaData
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