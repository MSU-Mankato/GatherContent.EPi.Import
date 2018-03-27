using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace GatherContentImport.Models.Media
{
    [ContentType(DisplayName = "GcEpi-Video File", GUID = "1fd342c6-002e-4312-bee7-e35ba7866a29", Description = "")]
    [MediaDescriptor(ExtensionString = "flv,mp4,webm,avi,wmv,mpeg,ogg,mov,ogv,qt,mp3,pcm,aac,wma,flac,alac,wav,aiff,pcm")]
    public class GcEpiVideoFile : VideoData
    {
        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Copyright",
            Description = "This is the copyright",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Copyright { get; set; }
        
        public virtual ContentReference PreviewImage { get; set; }
        
        public virtual string GcFileInfo { get; set; }
    }
}