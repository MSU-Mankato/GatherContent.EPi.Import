using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using GatherContentConnect;
using GatherContentConnect.Interface;
using GatherContentConnect.Objects;
using GatherContentImport.GcDynamicClasses;
using GatherContentImport.GcEpiObjects;
using GatherContentImport.GcEpiUtilities;
using Newtonsoft.Json;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "New Gc Mapping Step 4", Description = "This is where the user does the field mappings", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/NewGcMappingStep4.aspx")]
    public partial class NewGcMappingStep4 : SimplePage
    {
        private GcConnectClient _client;
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        private readonly IContentTypeRepository _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
        private readonly List<GcElement> _elements = new List<GcElement>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                AccessDenied();
            }

            if (IsPostBack) return;
            SessionSet();
            PopulateForm();
        }

        private void SessionSet()
        {
            if (!Request.QueryString.HasKeys()) return;
            Session["AccountId"] = Server.UrlDecode(Request.QueryString["AccountId"]);
            Session["ProjectId"] = Server.UrlDecode(Request.QueryString["ProjectId"]);
            Session["TemplateId"] = Server.UrlDecode(Request.QueryString["TemplateId"]);
            Session["PostType"] = Server.UrlDecode(Request.QueryString["PostType"]);
            Session["Author"] = Server.UrlDecode(Request.QueryString["Author"]);
            Session["DefaultStatus"] = Server.UrlDecode(Request.QueryString["DefaultStatus"]);
            Session["EpiContentType"] = Server.UrlDecode(Request.QueryString["EpiContentType"]);
            Session["StatusMaps"] = JsonConvert.DeserializeObject<List<GcEpiStatusMap>>(Server.UrlDecode(Request.QueryString["StatusMaps"]));
            Session["EpiFieldMaps"] = JsonConvert.DeserializeObject<List<string>>(Server.UrlDecode(Request.QueryString["EpiFieldMaps"]));
            Session["PublishedDateTime"] = Server.UrlDecode(Request.QueryString["PublishedDateTime"]);
        }

        private void PopulateForm()
        {

            if (_credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup your GatherContent config first!');" +
                               "window.location='/modules/GcEpiPlugin/GatherContentConfigSetup.aspx'</script>");
                Visible = false;
                return;
            }

            if (Session["ProjectId"] == null || Session["TemplateId"] == null
                || Session["PostType"] == null || (string)Session["PostType"] == "-1")
            {
                Response.Write("<script>alert('Please set the MetaDataProducer Defaults!');" +
                               "window.location='/modules/GcEpiPlugin/NewGcMappingStep3.aspx'</script>");
                Visible = false;
                return;
            }
            // Variables initialization.
            _client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            var gcFields = _client.GetTemplateById(templateId).Config.ToList();
            gcFields.ForEach(i => _elements.AddRange(i.Elements));

            // Setting the mark-up labels.
            projectName.Text = _client.GetProjectById(projectId).Name;
            templateName.Text = _client.GetTemplateById(templateId).Name;
            templateDescription.Text = _client.GetTemplateById(templateId).Description;

            // Table rows instantiation and updating.
            var tHeadRow = new TableRow { Height = 42 };
            tHeadRow.Cells.Add(new TableCell { Text = "GatherContent Field" });
            tHeadRow.Cells.Add(new TableCell { Text = "Mapped EPiServer Field" });
            tableMappings.Rows.Add(tHeadRow);
            foreach (var element in _elements.OrderByDescending(i => i.Type))
            {
                var tRow = new TableRow();
                tableMappings.Rows.Add(tRow);
                for (var cellIndex = 1; cellIndex <= 2; cellIndex++)
                {
                    var tCell = new TableCell { Width = 500 };
                    if (cellIndex is 1)
                    {
                        tCell.Text =
                            $"<span style='font-weight: Bold;'>{element.Label + element.Title}</span><br>Type: {element.Type}<br>Limit: " +
                            $"{element.Limit}<br>Description: {element.MicroCopy}<br>";
                    }
                    else
                    {
                        if (Session["EpiContentType"].ToString().StartsWith("block-"))
                        {
                            var blockTypes = _contentTypeRepository.List().OfType<BlockType>();
                            tCell.Controls.Add(MetaDataProducer(blockTypes, element, 6));
                        }

                        else if (Session["EpiContentType"].ToString().StartsWith("page-"))
                        {
                            var pageTypes = _contentTypeRepository.List().OfType<PageType>();
                            tCell.Controls.Add(MetaDataProducer(pageTypes, element, 5));
                        }

                        else
                        {
                            var gcEpiMisc = new GcEpiMiscUtility();
                            var dropDownList = MetaDataProducer(gcEpiMisc.GetMediaTypes(), element, 6);
                            tCell.Controls.Add(dropDownList);
                        }
                    }
                    tRow.Cells.Add(tCell);
                }
            }
        }

        private DropDownList MetaDataProducer(IEnumerable<ContentType> contentTypeList, IGcElement element, int substringIndex)
        {
            // Define the dictionary of all the available Episerver types for GatherContent types.
            var typeDictionary = new Dictionary<string, List<string>>
            {
                {"text", new List<string> {"String", "LongString", "XhtmlString", "Number", "FloatNumber", "Date", "StringList", "Url"}},
                {"section", new List<string> {"String", "LongString", "XhtmlString", "Number", "FloatNumber", "Date", "Url"} },
                {"files", new List<string>()},
                {"choice_checkbox", new List<string>()},
                {"choice_radio", new List<string> ()}
            };

            // Create a temporary content type object.
            var contentType = new ContentType();

            // Set the content type object to the selected content type.
            foreach (var i in contentTypeList)
            {
                if (Session["EpiContentType"].ToString().Substring(substringIndex) != i.Name) continue;
                contentType = i;
                break;
            }

            // Create a drop down list with field names as the options.
            var ddlMetaData = new DropDownList { Height = 28, Width = 194, CssClass = "chosen-select" };
            ddlMetaData.Items.Add(new ListItem("Do Not Import", "-1"));
            
            // For GcFields that are attachment types, add an option called 'Import'.
            if (element.Type == "files")
                ddlMetaData.Items.Add(new ListItem("Import Attachments", "Import-Attachments"));

            contentType.PropertyDefinitions.ToList().
                ForEach(i =>
                {
                    // Select the only Epi fields whose data types are compatible with GC field data types.
                    if (typeDictionary[element.Type].Contains(i.Type.Name))
                    {
                        ddlMetaData.Items.Add(new ListItem(i.TranslateDisplayName(), i.Name));
                    }
                });
            // Add the element name as the ID of the drop down along with meta as the prefix.
            ddlMetaData.ID = "meta-" + element.Name;
            return ddlMetaData;
        }

        protected void BtnSaveMapping_OnClick(object sender, EventArgs e)
        {
            var epiFieldMaps = from string key in Request.Form.Keys
                where key.StartsWith("meta-")
                select Request.Form[key] + "~" + key.Substring(5);
            Session["EpiFieldMaps"] = epiFieldMaps.ToList();
            var mappingsStore = GcDynamicUtilities.RetrieveStore<GcDynamicTemplateMappings>();
            var newMapping = new GcDynamicTemplateMappings(Session["AccountId"].ToString(), Session["ProjectId"].ToString(),
                Session["TemplateId"].ToString(), Session["PostType"].ToString(), Session["Author"].ToString(),
                Session["DefaultStatus"].ToString(), Session["EpiContentType"].ToString(), (List<GcEpiStatusMap>)Session["StatusMaps"],
                (List<string>)Session["EpiFieldMaps"], $"{DateTime.Now.ToLocalTime():g}");
            var existingIndex = mappingsStore.FindIndex(i => i.TemplateId == Session["TemplateId"].ToString());
            if (existingIndex >= 0)
                GcDynamicUtilities.DeleteItem<GcDynamicTemplateMappings>(mappingsStore[existingIndex].Id);
            GcDynamicUtilities.SaveStore(newMapping);
            Session.Clear();
            Response.Redirect("~/modules/GcEpiPlugin/GcEpiTemplateMappings.aspx");
        }

    }
}