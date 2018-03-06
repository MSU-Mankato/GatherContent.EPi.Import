using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace GcEpiPluginV2._0.modules.GatherContentImport.GcEpiUtilities
{
    public class GcEpiMiscUtility
    {
        private readonly IContentTypeRepository _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
        public IEnumerable<ContentType> GetMediaTypes()
        {
            var contentTypeList = _contentTypeRepository.List();
            var allContentTypes = contentTypeList as IList<ContentType> ?? contentTypeList;
            var mediaList = new List<ContentType>();
            allContentTypes.ToList().ForEach(i =>
            {
                try
                {
                    if (i.ModelType.IsSubclassOf(typeof(MediaData)))
                        mediaList.Add(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            return mediaList;
        }
    }
}