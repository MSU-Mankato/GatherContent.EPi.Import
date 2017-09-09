using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;
using GcEPiPlugin.GatherContentPlugin.GcEpiObjects;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping Part 3", Description = "part 3 of gc mapping", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/NewGcMappingV3.aspx")]
    public partial class NewGcMappingV3 : SimplePage
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
            if (credentialsStore.Count <= 0 || settingsStore.Count <= 0 || string.IsNullOrEmpty(settingsStore.ToList().First().ProjectId)
                || string.IsNullOrEmpty(settingsStore.ToList().First().TemplateId))
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
            var epiUsers = Membership.GetAllUsers();
            epiUsers.ToList().ForEach(epiUser => ddlAuthors.Items.Add(new ListItem(epiUser.UserName, epiUser.UserName)));
            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
            saveActions.ToList().ForEach(i => ddlStatuses.Items.Add(new ListItem(i.ToString(), i.ToString())));
            if (settingsStore.Count <= 0 || string.IsNullOrEmpty(settingsStore.ToList().First().PostType) ||
                string.IsNullOrEmpty(settingsStore.ToList().First().Author) || 
                string.IsNullOrEmpty(settingsStore.ToList().First().EPiStatus))
            {
                ddlPostTypes.SelectedIndex = 0;
                ddlAuthors.SelectedIndex = 0;
                ddlStatuses.SelectedIndex = 0;
                _settings = new GcDynamicSettings(postType: ddlPostTypes.SelectedValue, author: ddlAuthors.SelectedValue,
                    epiStatus: ddlStatuses.SelectedValue);
                GcDynamicSettings.SaveStore(_settings);
                settingsStore = GcDynamicSettings.RetrieveStore();
            }
            ddlPostTypes.SelectedValue = settingsStore.ToList().First().PostType;
            ddlAuthors.SelectedValue = settingsStore.ToList().First().Author;
            ddlStatuses.SelectedValue = settingsStore.ToList().First().EPiStatus;
            var gcStatuses = _client.GetStatusesByProjectId(projectId);
            var tHeadRow = new TableRow {Height = 42};
            tHeadRow.Cells.Add(new TableCell{ Text = "GatherContent Status" });
            tHeadRow.Cells.Add(new TableCell { Text = "Mapped EPiServer Status" });
            tHeadRow.Cells.Add(new TableCell { Text = "On Import, Change GatherContent Status" });
            tableGcStatusesMap.Rows.Add(tHeadRow);
            var storeIndex = 0;
            foreach (var status in gcStatuses)
            {
                var tRow = new TableRow();
                tableGcStatusesMap.Rows.Add(tRow);
                for (var cellIndex = 1; cellIndex <= 3; cellIndex++)
                {
                    var tCell = new TableCell();
                    if (cellIndex is 3)
                    {
                        var ddlOnImportGcStatuses = new DropDownList { Height = 28, Width = 194 };
                        ddlOnImportGcStatuses.Items.Add(new ListItem("Do Not Change", "1"));
                        gcStatuses.ToList().ForEach(i => ddlOnImportGcStatuses.Items.Add(new ListItem(i.Name, i.Id)));
                        ddlOnImportGcStatuses.ID = "onImportGc-" + status.Id;
                        var mapsInStore = settingsStore.ToList().First().StatusMaps;
                        if (!mapsInStore.IsNullOrEmpty() && mapsInStore.Count == gcStatuses.Count)
                        {
                            if (mapsInStore[storeIndex].ProjectId == settingsStore.ToList().First().ProjectId &&
                                mapsInStore[storeIndex].TemplateId == settingsStore.ToList().First().TemplateId)
                            {
                                ddlOnImportGcStatuses.SelectedValue = mapsInStore[storeIndex].OnImportChangeGcStatus;
                            }
                            else
                            {
                                var noMap = new List<GcEpiStatusMap>();
                                var store = new GcDynamicSettings(statusMaps: noMap);
                                GcDynamicSettings.SaveStore(store);
                            }
                        }
                        tCell.Controls.Add(ddlOnImportGcStatuses);
                    }
                    else if (cellIndex is 2)
                    {
                        var ddlEpiStatuses = new DropDownList {Height = 28, Width = 194};
                        saveActions.ToList().ForEach(i => ddlEpiStatuses.Items.Add(new ListItem(i.ToString(), i.ToString())));
                        ddlEpiStatuses.Items.Add(new ListItem("Do Not Change", "noChange"));
                        ddlEpiStatuses.ID = "mappedEPi-" + status.Id;
                        var mapsInStore = GcDynamicSettings.RetrieveStore().ToList().First().StatusMaps;
                        if (!mapsInStore.IsNullOrEmpty() && mapsInStore.Count == gcStatuses.Count)  
                        {
                            if (mapsInStore[storeIndex].ProjectId == settingsStore.ToList().First().ProjectId &&
                                mapsInStore[storeIndex].TemplateId == settingsStore.ToList().First().TemplateId)
                            {
                                ddlEpiStatuses.SelectedValue = mapsInStore[storeIndex].MappedEpiserverStatus;
                            }
                            else
                            {
                                var noMap = new List<GcEpiStatusMap>();
                                var store = new GcDynamicSettings(statusMaps: noMap);
                                GcDynamicSettings.SaveStore(store);
                            }
                        }
                        tCell.Controls.Add(ddlEpiStatuses);
                    }
                    else
                    {
                        tCell.Text = status.Name;
                    }
                    tRow.Cells.Add(tCell);
                }
                storeIndex++;
            }
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
            var selectedPostType = Request.Form["ddlPostTypes"];
            var selectedAuthor = Request.Form["ddlAuthors"];
            var selectedEPiStatus = Request.Form["ddlStatuses"];
            var projectId = GcDynamicSettings.RetrieveStore().ToList().First().ProjectId;
            var templateId = GcDynamicSettings.RetrieveStore().ToList().First().TemplateId;
            var gcEpiStatusMaps = (from string key in Request.Form.Keys
                where key.StartsWith("mappedEPi-")
                select new GcEpiStatusMap
                {
                    MappedEpiserverStatus = Request.Form[key],
                    OnImportChangeGcStatus = Request.Form[key.Replace("mappedEPi-", "onImportGc-")],
                    ProjectId =  projectId,
                    TemplateId = templateId
                }).ToList();
            _settings = new GcDynamicSettings(postType: selectedPostType, author: selectedAuthor,
                epiStatus: selectedEPiStatus, statusMaps: gcEpiStatusMaps);
            GcDynamicSettings.SaveStore(_settings);
            Response.Redirect("~/GatherContentPlugin/NewGcMappingV4.aspx");
        }
    }
}