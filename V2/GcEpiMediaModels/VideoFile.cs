using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web;

namespace GatherContentImport.GcEpiMediaModels
{
    [ContentType(DisplayName = "DefaultMedia1", GUID = "1fd342c6-002e-4312-bee7-e35ba7866a29", Description = "")]
    [MediaDescriptor(ExtensionString = "flv,mp4,webm,avi,wmv,mpeg")]
    public class VideoFile : VideoData
    {
            [CultureSpecific]
            [Editable(true)]
            [Display(
                Name = "Description",
                Description = "Description field's description",
                GroupName = SystemTabNames.Content,
                Order = 1)]
            public virtual string Copyright { get; set; }

        [UIHint(UIHint.Image)]
        public virtual ContentReference PreviewImage { get; set; }
    }
}