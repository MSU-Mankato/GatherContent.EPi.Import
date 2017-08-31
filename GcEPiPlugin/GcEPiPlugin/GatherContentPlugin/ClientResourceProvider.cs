using System.Collections.Generic;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell;

namespace GcEPiPlugin.GatherContentPlugin
{
    [ClientResourceProvider]
    public class ClientResourceProvider : IClientResourceProvider
    {
        public IEnumerable<ClientResource> GetClientResources()
        {
            return new[]
            {
                new ClientResource
                {
                    Name = "epi.samples.Module.FormHandler",
                    Path = Paths.ToClientResource("SampleModuleName", "ClientResources/FormHandler.js"),
                    ResourceType = ClientResourceType.Script,
                    Dependencies = new List<string> { "OtherResourceName" }
                }
            };
        }
    }
}