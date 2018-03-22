using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using GatherContentConnect.Objects;
using GatherContentImport.Models.Media;

namespace GatherContentImport.GcEpiUtilities
{
    public static class GcEpiContentParser
    {
        private static readonly IContentRepository ContentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        public static object TextParser(string text, string propertyType)
        {
            bool success;
            var regexResult = string.Empty;
            if (propertyType != "XhtmlString")
            {
                regexResult = Regex.Replace(text, "<.*?>|&.*?;", string.Empty).Trim();
            }

            switch (propertyType)
            {
                case "Date":
                    DateTime date;
                    success = DateTime.TryParse(regexResult, out date);
                    return !success ? (object) string.Empty : date;
                case "Number":
                    int number;
                    success = int.TryParse(regexResult, out number);
                    return !success ? (object) string.Empty : number;
                case "FloatNumber":
                    double floatNumber;
                    success = double.TryParse(regexResult, out floatNumber);
                    return !success ? (object)string.Empty : floatNumber;
                case "Url":
                case "String":
                case "LongString":
                    return regexResult;
                case "StringList":
                    var strings = regexResult.Split(',').Select(p => p.Trim()).ToList();
                    return strings;
                default:
                    return text;
            }
        }

        public static object ChoiceParser(ICollection<GcOption> options, string gcType, PropertyDefinition propertyDefinition)
        {
            if (gcType != "choice_checkbox") return new object();
            var radioButtons = new List<SelectListItem>();
            options.ToList().ForEach(i => radioButtons.Add(new SelectListItem{ Value = i.Name, Text = i.Label })); 
            return new SelectList(radioButtons, "Value", "Text", null);
        }

        public static async Task<bool> FileParserAsync(GcFile gcFile, string postType, ContentReference contentLink, SaveAction saveAction, string action)
        {
            // Initialize fileExtensions dictionary with all the supported audio, video and generic files.
            var fileExtensions = new Dictionary<string, List<string>>
            {
                {"Video", new List<string>{"flv","mp4","webm","avi","wmv","mpeg","ogg","mov","ogv","qt","mp3","pcm","aac","wma","flac","alac","wav","aiff"}},
                {"Image", new List<string>{"jpg","jpeg","jpe","ico","gif","bmp","png","tga","tiff","eps","svg","webp"}},
                {"Generic", new List<string>{"pdf","doc","docx","txt","xsl","xslx","html","css","zip","rtf","rar","csv","xml", "log"}}
            };

            // If the item is a page, then create a content link for 'For this Page'. Else, use the provided contentLink.
            if (postType == "PageType")
            {
                // Get an instance of ContentAssetHelper class.
                var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();

                // Get an existing content asset folder or create a new one
                contentLink = contentAssetHelper.GetOrCreateAssetFolder(contentLink).ContentLink;
            }
            
            // Get an instance of IContentRepository.
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            // Extract the file extension of the file by its name.
            var fileExtension = Path.GetExtension(gcFile.FileName).Replace(".","");

            // Initialize a new MediaData object.
            MediaData file;
            if (fileExtensions["Image"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<GcEpiImageFile>(contentLink);
            }

            else if (fileExtensions["Generic"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<GcEpiGenericFile>(contentLink);
            }

            else if (fileExtensions["Video"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<GcEpiVideoFile>(contentLink);
            }
           
            else
            {
                return false;
            }

            file.Name = gcFile.FileName;
            file.Property["GcFileInfo"].Value = gcFile.Id + "~" + gcFile.FileName + "~" + gcFile.ItemId;

            if (action == "Update")
            {
                // Check if the file is already imported.
                var importedFiles = ContentRepository.GetChildren<MediaData>(contentLink, CultureInfo.InvariantCulture).ToList();
                foreach (var importedFile in importedFiles)
                {
                    var propSubStrings = importedFile.Property["GcFileInfo"].Value.ToString().Split('~');
                    var importedFileGcFileId = Convert.ToInt32(propSubStrings[0]);
                    var importedFileGcFileName = propSubStrings[1];
                    var importedFileGcFileItemId = Convert.ToInt32(propSubStrings[2]);
                    if (importedFileGcFileName != gcFile.FileName ||
                        importedFileGcFileItemId != gcFile.ItemId) continue;
                    if (importedFileGcFileId == gcFile.Id)
                        return false;
                    contentRepository.Delete(importedFile.ContentLink, true, AccessLevel.Administer);
                }
            }

            var blobFactory = ServiceLocator.Current.GetInstance<IBlobFactory>();
            try
            {
                var client = new HttpClient();
                var byteArrayData = await client.GetByteArrayAsync(gcFile.Url);

                var blob = blobFactory.CreateBlob(file.BinaryDataContainer, Path.GetExtension(gcFile.FileName));
                using (var s = blob.OpenWrite())
                {
                    var w = new StreamWriter(s);
                    w.BaseStream.Write(byteArrayData, 0, byteArrayData.Length);
                    w.Flush();
                }
                file.BinaryData = blob;
                contentRepository.Save(file, saveAction, AccessLevel.Administer);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}