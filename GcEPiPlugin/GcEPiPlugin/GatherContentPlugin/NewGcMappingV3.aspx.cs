using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;
using GcEPiPlugin.GatherContentPlugin.GcEpiObjects;
using Castle.Core.Internal;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping Part 3", Description = "part 3 of gc mapping", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/NewGcMappingV3.aspx")]
    public partial class NewGcMappingV3 : SimplePage
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
            if (credentialsStore.IsNullOrEmpty() || Session["ProjectId"] == null || Session["TemplateId"] == null)
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,
                credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            projectName.Text = _client.GetProjectById(projectId).Name;
            templateName.Text = _client.GetTemplateById(templateId).Name;
            templateDescription.Text = _client.GetTemplateById(templateId).Description;
            var epiUsers = Membership.GetAllUsers();
            epiUsers.ToList().ForEach(epiUser => ddlAuthors.Items.Add(new ListItem(epiUser.UserName, epiUser.UserName)));
            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
            saveActions.ToList().ForEach(i => ddlStatuses.Items.Add(new ListItem(i.ToString(), i.ToString())));
            if (Session["PostType"] == null || Session["Author"] == null || Session["DefaultStatus"] == null)
            {
                ddlPostTypes.SelectedIndex = 0;
                ddlAuthors.SelectedIndex = 0;
                ddlStatuses.SelectedIndex = 0;
                Session["PostType"] = ddlPostTypes.SelectedValue;
                Session["Author"] = ddlAuthors.SelectedValue;
                Session["DefaultStatus"] = ddlStatuses.SelectedValue;
            }
            else
            {
                ddlPostTypes.Items.Remove(ddlPostTypes.Items.FindByValue("-1"));
                ddlPostTypes.SelectedValue = Session["PostType"].ToString();
                ddlAuthors.SelectedValue = Session["Author"].ToString();
                ddlStatuses.SelectedValue = Session["DefaultStatus"].ToString();
                var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                if (Session["PostType"].ToString() is "PageType")
                {
                    var contentTypeList = contentTypeRepository.List().OfType<PageType>();
                    var pageTypes = contentTypeList as IList<PageType> ?? contentTypeList.ToList();
                    pageTypes.ForEach(i => ddlEpiContentTypes.Items.Add(new ListItem(i.Name, "page-" + i.Name)));
                    ddlEpiContentTypes.Enabled = true;
                    btnNextStep.Enabled = true;
                }
                else if (Session["PostType"].ToString() is "BlockType")
                {
                    var contentTypeList = contentTypeRepository.List().OfType<BlockType>();
                    var blockTypes = contentTypeList as IList<BlockType> ?? contentTypeList.ToList();
                    blockTypes.ForEach(i => ddlEpiContentTypes.Items.Add(new ListItem(i.Name, "block-" + i.Name)));
                    ddlEpiContentTypes.Enabled = true;
                    btnNextStep.Enabled = true;
                }
                //else if (Session["PostType"].ToString() is "MediaType")
                //{
                //    ddlEpiContentTypes.Items.Add(new ListItem("Media Data", "MediaType"));
                //    ddlEpiContentTypes.Enabled = true;
                //    btnNextStep.Enabled = true;
                //}
                if (Session["EpiContentType"] != null)
                {
                    ddlEpiContentTypes.SelectedValue = Session["EpiContentType"].ToString();
                }
            }
            var gcStatuses = _client.GetStatusesByProjectId(projectId);
            var tHeadRow = new TableRow {Height = 42};
            tHeadRow.Cells.Add(new TableCell{ Text = "GatherContent Status" });
            tHeadRow.Cells.Add(new TableCell { Text = "Mapped EPiServer Status" });
            tHeadRow.Cells.Add(new TableCell { Text = "On Import, Change GatherContent Status" });
            tableGcStatusesMap.Rows.Add(tHeadRow);
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
                        tCell.Controls.Add(ddlOnImportGcStatuses);
                    }
                    else if (cellIndex is 2)
                    {
                        var ddlEpiStatuses = new DropDownList {Height = 28, Width = 194};
                        saveActions.ToList().ForEach(i => ddlEpiStatuses.Items.Add(new ListItem(i.ToString(), i.ToString())));
                        ddlEpiStatuses.Items.Add(new ListItem("Do Not Change", "noChange"));
                        ddlEpiStatuses.ID = "mappedEPi-" + status.Id;
						tCell.Controls.Add(ddlEpiStatuses);
                    }
                    else if (cellIndex is 1)
                    {
                        tCell.Text = status.Name;
                    }
                    tRow.Cells.Add(tCell);
                }
            }
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
            var selectedPostType = Request.Form["ddlPostTypes"];
            var selectedAuthor = Request.Form["ddlAuthors"];
            var selectedEPiStatus = Request.Form["ddlStatuses"];
            var selectedEpiContentType = Request.Form["ddlEpiContentTypes"];
			Session["PostType"] = selectedPostType;
			Session["Author"] = selectedAuthor;
			Session["DefaultStatus"] = selectedEPiStatus;
            Session["EpiContentType"] = selectedEpiContentType;
            var gcEpiStatusMaps = (from string key in Request.Form.Keys
                where key.StartsWith("mappedEPi-")
                select new GcEpiStatusMap
                {
                    MappedEpiserverStatus = Request.Form[key],
                    OnImportChangeGcStatus = Request.Form[key.Replace("mappedEPi-", "onImportGc-")]
                }).ToList();
			Session["StatusMaps"] = gcEpiStatusMaps;
            Response.Redirect("~/GatherContentPlugin/NewGcMappingV4.aspx");
        }

        protected void DdlPostTypes_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ddlEpiContentTypes.Visible = true;
            ddlEpiContentTypes.Enabled = true;
			Session["PostType"] = ddlPostTypes.SelectedValue;
            PopulateForm();
        }
    }
}