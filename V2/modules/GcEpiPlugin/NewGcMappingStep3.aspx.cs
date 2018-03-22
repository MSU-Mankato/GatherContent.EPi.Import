using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using GatherContentConnect;
using GatherContentImport.GcDynamicClasses;
using GatherContentImport.GcEpiObjects;
using GatherContentImport.GcEpiUtilities;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "New Gc Mapping Step 3", Description = "This is where the user sets mapping defaults", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/NewGcMappingStep3.aspx")]
    public partial class NewGcMappingStep3 : SimplePage
    {


        private GcConnectClient _client;
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        private readonly IContentTypeRepository _contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

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

        private void EnableDdl()
        {
            if (!Request.QueryString.HasKeys()) return;
            if (Server.UrlDecode(Request.QueryString["ddlEpiContentType"]) != "enable") return;
            Session["EpiContentType"] = null;
            ddlEpiContentTypes.Enabled = true;
            ddlEpiContentTypes.Visible = true;
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

            if (Session["ProjectId"] == null || Session["TemplateId"] == null)
            {
                Response.Write("<script>alert('Please select the GatherContent Template!');" +
                               "window.location='/modules/GcEpiPlugin/NewGcMappingStep2.aspx'</script>");
                Visible = false;
                return;
            }
            EnableDdl();

            // Clear all the statically created drop down items to avoid data persistence on post-back.
            ddlAuthors.Items.Clear();
            ddlStatuses.Items.Clear();

            _client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var projectId = Convert.ToInt32(Session["ProjectId"]);
            var templateId = Convert.ToInt32(Session["TemplateId"]);
            projectName.Text = _client.GetProjectById(projectId).Name;
            templateName.Text = _client.GetTemplateById(templateId).Name;
            templateDescription.Text = _client.GetTemplateById(templateId).Description;
            var userProvider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            var epiUsers = userProvider.GetAllUsers(0, 200, out int _);
            epiUsers.ToList().ForEach(epiUser => ddlAuthors.Items.Add(new ListItem(epiUser.Username, epiUser.Username)));
            var saveActions = Enum.GetValues(typeof(SaveAction)).Cast<SaveAction>().ToList();
            saveActions.RemoveAt(1);
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

                // Clear all the statically created drop down items to avoid data persistence on post-back.
                ddlEpiContentTypes.Items.Clear();

                if (Session["PostType"].ToString() is "PageType")
                {
                    var contentTypeList = _contentTypeRepository.List().OfType<PageType>();
                    var pageTypes = contentTypeList as IList<PageType> ?? contentTypeList.ToList();
                    pageTypes.ToList().ForEach(i =>
                    {
                        if (i.ID != 1 && i.ID != 2)
                            ddlEpiContentTypes.Items.Add(new ListItem(i.DisplayName, "page-" + i.Name));
                    });
                    ddlEpiContentTypes.Enabled = true;
                    btnNextStep.Enabled = true;
                }
                else if (Session["PostType"].ToString() is "BlockType")
                {
                    var contentTypeList = _contentTypeRepository.List().OfType<BlockType>();
                    var blockTypes = contentTypeList as IList<BlockType> ?? contentTypeList.ToList();
                    blockTypes.ToList().ForEach(i => ddlEpiContentTypes.Items.Add(new ListItem(i.DisplayName, "block-" + i.Name)));
                    ddlEpiContentTypes.Enabled = true;
                    btnNextStep.Enabled = true;
                }
                else
                {
                    var gcEpiMisc = new GcEpiMiscUtility();
                    gcEpiMisc.GetMediaTypes().ToList().
                        ForEach(i => ddlEpiContentTypes.Items.Add(new ListItem(i.DisplayName, "media-" + i.Name)));
                    ddlEpiContentTypes.Enabled = true;
                    btnNextStep.Enabled = true;
                }
                if (Session["EpiContentType"] != null)
                {
                    ddlEpiContentTypes.SelectedValue = Session["EpiContentType"].ToString();
                }
            }
            var gcStatuses = _client.GetStatusesByProjectId(projectId);
            var tHeadRow = new TableRow { Height = 42 };
            tHeadRow.Cells.Add(new TableCell { Text = "GatherContent Status" });
            tHeadRow.Cells.Add(new TableCell { Text = "Mapped EPiServer Status" });
            //tHeadRow.Cells.Add(new TableCell { Text = "On Import, Change GatherContent Status" });
            tableGcStatusesMap.Rows.Add(tHeadRow);
            foreach (var status in gcStatuses)
            {
                var tRow = new TableRow();
                tableGcStatusesMap.Rows.Add(tRow);
                for (var cellIndex = 1; cellIndex <= 2; cellIndex++)//Need to make it the highest index, 3 in the next version.
                {
                    var tCell = new TableCell();
                    if (cellIndex is 3)
                    {
                        var ddlOnImportGcStatuses = new DropDownList { Height = 30, Width = 250, CssClass = "chosen-select" };
                        ddlOnImportGcStatuses.Items.Add(new ListItem("Do Not Change", "1"));
                        gcStatuses.ToList().ForEach(i => ddlOnImportGcStatuses.Items.Add(new ListItem(i.Name, i.Id)));
                        ddlOnImportGcStatuses.ID = "onImportGc-" + status.Id;
                        tCell.Controls.Add(ddlOnImportGcStatuses);
                    }
                    else if (cellIndex is 2)
                    {
                        var ddlEpiStatuses = new DropDownList { Height = 30, Width = 250, CssClass = "chosen-select" };
                        ddlEpiStatuses.Items.Add(new ListItem("Use Default Status", "Use Default Status"));
                        saveActions.ToList().ForEach(i => ddlEpiStatuses.Items.Add(new ListItem(i.ToString(), i.ToString())));
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
            if (selectedEpiContentType == null)
            {
                Response.Write(selectedPostType == "PageType"
                    ? "<script>alert('Invalid Page type! Please try again!')</script>"
                    : "<script>alert('Invalid Block type! Please try again!')</script>");
                return;
            }
            Session["PostType"] = selectedPostType;
            Session["Author"] = selectedAuthor;
            Session["DefaultStatus"] = selectedEPiStatus;
            Session["EpiContentType"] = selectedEpiContentType;
            var gcEpiStatusMaps = (from string key in Request.Form.Keys
                where key.StartsWith("mappedEPi-")
                select new GcEpiStatusMap
                {
                    MappedEpiserverStatus = Request.Form[key] + "~" + key.Substring(10),
                    OnImportChangeGcStatus = Request.Form[key.Replace("mappedEPi-", "onImportGc-")] + "~" + key.Substring(10)
                }).ToList();
            Session["StatusMaps"] = gcEpiStatusMaps;
            Response.Redirect("~/modules/GcEpiPlugin/NewGcMappingStep4.aspx");
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