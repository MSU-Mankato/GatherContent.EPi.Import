using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping Part 4", Description = "part 4 of gc mapping", Area = PlugInArea.AdminMenu,
        Url = "~/GatherContentPlugin/NewGcMappingV4.aspx")]
    public partial class NewGcMappingV4 : SimplePage
    {

        private GcConnectClient _client;

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
                                foreach (var i in pageTypes)
                                {
                                    ddlContentTypes.Items.Add(new ListItem(i.Name, i.Name));
                                    if (ddlContentTypes.SelectedValue != i.Name) continue;
                                    myProperty = i;
                                }
                                var ddlMetaData = new DropDownList { Height = 28, Width = 194 };
                                myProperty.PropertyDefinitions.ToList().ForEach(i => 
                                ddlMetaData.Items.Add(new ListItem(i.Name, i.ID.ToString())));
                                tCell.Controls.Add(ddlContentTypes);
                                tCell.Controls.Add(ddlMetaData);
                            }
                            else if (settingsStore.ToList().First().PostType is "BlockType")
                            {
                                var contentTypeList = contentTypeRepository.List().OfType<BlockType>();
                                var myProperty = new BlockType();
                                var ddlContentTypes = new DropDownList { Height = 28, Width = 194 };
                                var blockTypes = contentTypeList as IList<BlockType> ?? contentTypeList.ToList();
                                foreach (var i in blockTypes)
                                {
                                    ddlContentTypes.Items.Add(new ListItem(i.Name, i.Name));
                                    if (ddlContentTypes.SelectedValue != i.Name) continue;
                                    myProperty = i;
                                }
                                var ddlMetaData = new DropDownList { Height = 28, Width = 194 };
                                myProperty.PropertyDefinitions.ToList().ForEach(i =>
                                    ddlMetaData.Items.Add(new ListItem(i.Name, i.ID.ToString())));
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
                }
            }
        }

        protected void BtnSaveMapping_OnClick(object sender, EventArgs e)
        {
            PopulateForm();
        }
    }
}