﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using EPiServer.Framework.DataAnnotations;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using GatherContentConnect.Objects;
using GatherContentImport.GcEpiMediaModels;

namespace GatherContentImport.GcEpiUtilities
{
    public static class GcEpiContentParser
    {
        public static Dictionary<string, List<string>> FileExtensions = new Dictionary<string, List<string>>
        {
            {"Video", new List<string>{"flv","mp4","webm","avi","wmv","mpeg","ogg","mov","ogv","qt","mp3","pcm","aac","wma","flac","alac","wav","aiff"}},
            {"Image", new List<string>{"jpg","jpeg","jpe","ico","gif","bmp","png","tga","tiff","eps","svg","webp"}},
            {"Generic", new List<string>{"pdf","doc","docx","txt","xsl","xslx","html","css","zip","rtf","rar","csv","xml", "log"}}
        };
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
            if (gcType != "choice_checkbox") return new object();
            var radioButtons = new List<SelectListItem>();
            options.ToList().ForEach(i => radioButtons.Add(new SelectListItem{ Value = i.Name, Text = i.Label })); 
            return new SelectList(radioButtons, "Value", "Text", null);
        }

        public static async Task<bool> FileParserAsync(string url, string fileName, ContentReference contentLink)
        {
            // Get an instance of ContentAssetHelper class.
            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();

            // Get an existing content asset folder or create a new one
            var assetsFolder = contentAssetHelper.GetOrCreateAssetFolder(contentLink);

            // Get an instance of IContentRepository.
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            // Extract the file extension of the file by its name.
            var fileExtension = Path.GetExtension(fileName).Replace(".","");

            // 
            MediaData file;
            if (FileExtensions["Image"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<ImageFile>(assetsFolder.ContentLink);
            }

            else if (FileExtensions["Generic"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<GenericFile>(assetsFolder.ContentLink);
            }

            else if (FileExtensions["Video"].Contains(fileExtension))
            {
                file = contentRepository.GetDefault<VideoFile>(assetsFolder.ContentLink);
            }
           
            else
            {
                return false;
            }

            file.Name = fileName;
            var blobFactory = ServiceLocator.Current.GetInstance<IBlobFactory>();
            try
            {
                var client = new HttpClient();
                var byteArrayData = await client.GetByteArrayAsync(url);

                var blob = blobFactory.CreateBlob(file.BinaryDataContainer, Path.GetExtension(fileName));
                using (var s = blob.OpenWrite())
                {
                    var w = new StreamWriter(s);
                    w.BaseStream.Write(byteArrayData, 0, byteArrayData.Length);
                    w.Flush();
                }
                file.BinaryData = blob;
                contentRepository.Save(file, SaveAction.Default, AccessLevel.Administer);
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