using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;
using GcEPiPlugin.GatherContentPlugin.GcEpiObjects;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping Part 4", Description = "part 4 of gc mapping", Area = PlugInArea.AdminMenu,
        Url = "~/GatherContentPlugin/NewGcMappingV4.aspx")]
    public partial class NewGcMappingV4 : SimplePage
    {

        private GcConnectClient _client;
        private GcDynamicSettings _settings;
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
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            var settingsStore = GcDynamicSettings.RetrieveStore();
            if (credentialsStore.Count <= 0 || settingsStore.Count <= 0 ||
                string.IsNullOrEmpty(settingsStore.ToList().First().ProjectId) || string.IsNullOrEmpty(settingsStore.ToList().First().TemplateId) || 
                string.IsNullOrEmpty(settingsStore.ToList().First().PostType) || string.IsNullOrEmpty(settingsStore.ToList().First().Author) || 
                string.IsNullOrEmpty(settingsStore.ToList().First().EPiStatus))
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,
                credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(settingsStore.ToList().First().ProjectId);
            var templateId = Convert.ToInt32(settingsStore.ToList().First().TemplateId);
            projectName.Text = _client.GetProjectById(projectId).Name;
            templateName.Text = _client.GetTemplateById(templateId).Name;
            templateDescription.Text = _client.GetTemplateById(templateId).Description;
            var tHeadRow = new TableRow { Height = 42 };
            tHeadRow.Cells.Add(new TableCell { Text = "GatherContent Field" });
            tHeadRow.Cells.Add(new TableCell { Text = "Mapped EPiServer Field" });
            tableMappings.Rows.Add(tHeadRow);
            var gcFields = _client.GetTemplateById(templateId).Config.ToList();
            var storeIndex = 0;
            foreach (var field in gcFields)
            {
                foreach (var element in field.Elements)
                {
                    var tRow = new TableRow();
                    tableMappings.Rows.Add(tRow);
                    for (var cellIndex = 1; cellIndex <= 2; cellIndex++)
                    {
                        var tCell = new TableCell { Width = 500 };
                        if (cellIndex is 1)
                        {
                            tCell.Text =
                                $"<span style='font-weight: Bold;'>{element.Label}</span><br>Type: {element.Type}<br>Limit: " +
                                $"{element.Limit}<br>Description: {element.MicroCopy}";
                        }
                        else
                        {
                            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                            if (settingsStore.ToList().First().PostType is "PageType")
                            {
                                var contentTypeList = contentTypeRepository.List().OfType<PageType>();
                                var myProperty = new PageType();
                                var ddlContentTypes = new DropDownList { Height = 28, Width = 194 };
                                var pageTypes = contentTypeList as IList<PageType> ?? contentTypeList.ToList();
                                pageTypes.ForEach(i => ddlContentTypes.Items.Add(new ListItem(i.Name, i.Name)));
                                ddlContentTypes.ID = "cType-" + element.Label;
                                var mapsInStore = settingsStore.ToList().First().ContentTypeMaps;
                                if (!mapsInStore.IsNullOrEmpty())
                                {
                                    if (mapsInStore[storeIndex].ProjectId == settingsStore.ToList().First().ProjectId &&
                                        mapsInStore[storeIndex].TemplateId == settingsStore.ToList().First().TemplateId &&
                                        mapsInStore[storeIndex].PostType == settingsStore.ToList().First().PostType)
                                    {
                                        ddlContentTypes.SelectedValue = mapsInStore[storeIndex].ContentType;
                                    }
                                    else
                                    {
                                        var noMap = new List<GcEpiContentTypeMap>();
                                        var store = new GcDynamicSettings(contentTypeMaps: noMap);
                                        GcDynamicSettings.SaveStore(store);
                                    }
                                }
                                foreach (var i in pageTypes)
                                {
                                    if (ddlContentTypes.SelectedValue != i.Name) continue;
                                    myProperty = i;
                                }
                                var ddlMetaData = new DropDownList { Height = 28, Width = 194 };
                                myProperty.PropertyDefinitions.ToList().ForEach(i => 
                                    ddlMetaData.Items.Add(new ListItem(i.Name, i.ID.ToString())));
                                ddlMetaData.ID = "meta-" + element.Label;
                                tCell.Controls.Add(ddlContentTypes);
                                tCell.Controls.Add(ddlMetaData);
                            }
                            else if (settingsStore.ToList().First().PostType is "BlockType")
                            {
                                var contentTypeList = contentTypeRepository.List().OfType<BlockType>();
                                var myProperty = new BlockType();
                                var ddlContentTypes = new DropDownList { Height = 28, Width = 194 };
                                var blockTypes = contentTypeList as IList<BlockType> ?? contentTypeList.ToList();
                                blockTypes.ForEach(i => ddlContentTypes.Items.Add(new ListItem(i.Name, i.Name)));
                                ddlContentTypes.ID = "cType-" + element.Label;
                                var mapsInStore = settingsStore.ToList().First().ContentTypeMaps;
                                if (!mapsInStore.IsNullOrEmpty())
                                {
                                    if (mapsInStore[storeIndex].ProjectId == settingsStore.ToList().First().ProjectId &&
                                        mapsInStore[storeIndex].TemplateId == settingsStore.ToList().First().TemplateId &&
                                        mapsInStore[storeIndex].PostType == settingsStore.ToList().First().PostType)
                                    {
                                        ddlContentTypes.SelectedValue = mapsInStore[storeIndex].ContentType;
                                    }
                                    else
                                    {
                                        var noMap = new List<GcEpiContentTypeMap>();
                                        var store = new GcDynamicSettings(contentTypeMaps: noMap);
                                        GcDynamicSettings.SaveStore(store);
                                    }
                                }
                                foreach (var i in blockTypes)
                                {
                                    if (ddlContentTypes.SelectedValue != i.Name) continue;
                                    myProperty = i;
                                }
                                var ddlMetaData = new DropDownList { Height = 28, Width = 194 };
                                myProperty.PropertyDefinitions.ToList().ForEach(i =>
                                    ddlMetaData.Items.Add(new ListItem(i.Name, i.ID.ToString())));
                                ddlMetaData.ID = "meta-" + element.Label;
                                tCell.Controls.Add(ddlContentTypes);
                                tCell.Controls.Add(ddlMetaData);
                            }
                            else
                            {
                                var ddlContentTypes = new DropDownList { Height = 28, Width = 194 };
                                ddlContentTypes.Items.Add(new ListItem("Media Content", "MediaContent"));
                                tCell.Controls.Add(ddlContentTypes);
                            }
                        }
                        tRow.Cells.Add(tCell);
                    }
                    storeIndex++;
                }
            }
        }

        protected void BtnSaveMapping_OnClick(object sender, EventArgs e)
        {
            var projectId = GcDynamicSettings.RetrieveStore().ToList().First().ProjectId;
            var templateId = GcDynamicSettings.RetrieveStore().ToList().First().TemplateId;
            var postType = GcDynamicSettings.RetrieveStore().ToList().First().PostType;
            var gcEpiContentTypeMaps = (from string key in Request.Form.Keys
                where key.StartsWith("cType-")
                select new GcEpiContentTypeMap()
                {
                    ContentType = Request.Form[key],
                    Metadata = Request.Form[key.Replace("cType-", "meta-")],
                    ProjectId = projectId,
                    TemplateId = templateId,
                    PostType = postType
                }).ToList();
            _settings = new GcDynamicSettings(contentTypeMaps: gcEpiContentTypeMaps);
            GcDynamicSettings.SaveStore(_settings);
            PopulateForm();
        }
    }
}