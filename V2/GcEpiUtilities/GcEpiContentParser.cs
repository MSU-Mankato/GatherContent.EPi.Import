using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Editor.TinyMCE.Plugins;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using GatherContentConnect.Objects;
using GatherContentImport.GcEpiMediaModels;
using Image = System.Drawing.Image;

namespace GatherContentImport.GcEpiUtilities
{
    public static class GcEpiContentParser
    {
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
                default:
                    return text;
            }
        }

        public static object ChoiceParser(ICollection<GcOption> options, string gcType, PropertyDefinition propertyDefinition)
        {
            if (gcType == "choice_checkbox")
            {
                var radioButtons = new List<SelectListItem>();
                options.ToList().ForEach(i => radioButtons.Add(new SelectListItem{ Value = i.Name, Text = i.Label })); 
                return new SelectList(radioButtons, "Value", "Text", null);
            }

            else
            {
                return new object();
            }
        }

        public static async Task<object> ImageParserAsync(string url, string imageName, ContentReference contentLink)
        {
            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            // get an existing content asset folder or create a new one
            var assetsFolder = contentAssetHelper.GetOrCreateAssetFolder(contentLink);
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var imageFile = contentRepository.GetDefault<ImageFile>(assetsFolder.ContentLink);
            imageFile.Name = imageName;
            var blobFactory = ServiceLocator.Current.GetInstance<IBlobFactory>();
            using (var stream = await GcEpiImageExtractor.GetImageStreamAsync(url))
            {
                if (stream == null) return null;

                var blob = blobFactory.CreateBlob(imageFile.BinaryDataContainer, Path.GetExtension(imageFile.Name));
                blob.Write(stream);
                imageFile.BinaryData = blob;
                return contentRepository.Save(imageFile, SaveAction.Default, AccessLevel.Administer);
            }
        }

        public static async Task<object> FileParserAsync(string url, string fileName, ContentReference contentLink)
        {
            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            // get an existing content asset folder or create a new one
            var assetsFolder = contentAssetHelper.GetOrCreateAssetFolder(contentLink);
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var file = contentRepository.GetDefault<ImageFile>(assetsFolder.ContentLink);
            file.Name = fileName;
            var blobFactory = ServiceLocator.Current.GetInstance<IBlobFactory>();
            var client = new HttpClient();
            var byteArrayData = await client.GetByteArrayAsync(url);

            var blob = blobFactory.CreateBlob(file.BinaryDataContainer, Path.GetExtension(file.Name));
            using (var s = blob.OpenWrite())
            {
                var w = new StreamWriter(s);
                w.BaseStream.Write(byteArrayData, 0, byteArrayData.Length);
                w.Flush();
            }

            file.BinaryData = blob;
            return contentRepository.Save(file, SaveAction.Publish);
        }
    }
}