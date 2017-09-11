using System;
using System.Collections.Generic;
using System.Web;

namespace GcEPiPlugin.GatherContentPlugin.GcEpiObjects
{
	public class GcEpiSessionObject : HttpSessionStateBase
	{
		public string AccountId { get; set; }
		public string ProjectId { get; set; }
		public string TemplateId { get; set; }
		public string PostType { get; set; }
		public string Author { get; set; }
		public string EPiStatus { get; set; }
		public List<GcEpiStatusMap> StatusMaps { get; set; }
		public List<GcEpiContentTypeMap> ContentTypeMaps { get; set; }	
	}
}