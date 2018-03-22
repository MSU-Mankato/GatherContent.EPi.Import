using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace GatherContentImport.Models.Media
{
    [ContentType(DisplayName = "GcEpi-Generic File", GUID = "6143d87a-740b-44db-bfae-9d0a84570358", Description = "")]
    [MediaDescriptor(ExtensionString = "pdf,doc,docx,txt,xsl,xslx,html,css,zip,rtf,rar,csv,xml,log")]
    public class GcEpiGenericFile : MediaData
    {
        [CultureSpecific]
        [Display(
            Name = "Copyright",
            Description = "This is the copyright.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Copyright { get; set; }

        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual string Description { get; set; }

        public virtual string GcFileInfo { get; set; }
    }
}