using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "Template Mappings", Description = "Shows all the template mappings for a particular account.", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/GcEpiTemplateMappings.aspx")]
    public partial class GcEpiTemplateMappings : SimplePage
    {
        private List<GcDynamicTemplateMaps> _mappings;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                AccessDenied();
            }

            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        private void PopulateForm()
        {
            GcDynamicTemplateMaps.ClearStore();
            _mappings = GcDynamicTemplateMaps.RetrieveStore();
        }

        protected void BtnAddNew_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/GatherContentPlugin/NewGcMapping.aspx");
        }
    }
}