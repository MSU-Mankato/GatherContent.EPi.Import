using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Providers.Entities;
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
using Newtonsoft.Json;

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

            if (IsPostBack) return;
            SessionSet();
            PopulateForm();
        }

        private void SessionSet()
        {
            if(!Request.QueryString.HasKeys())return;
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
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            if (credentialsStore.IsNullOrEmpty() || Session["ProjectId"] == null || Session["TemplateId"] == null 
                    || Session["PostType"] == null || (string) Session["PostType"] == "-1")
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
                            if (Session["EpiContentType"].ToString() is "MediaType")
                            {
                                var ddlMetaData = new DropDownList {Height = 28, Width = 194};
                                ddlMetaData.Items.Add(new ListItem("Media Content", "MediaContent"));
                                tCell.Controls.Add(ddlMetaData);
                            }
                            else
                            {
                                if (Session["EpiContentType"].ToString().StartsWith("block-"))
                                {
                                    var contentTypeList = contentTypeRepository.List().OfType<BlockType>();
                                    var myProperty = new BlockType();
                                    var blockTypes = contentTypeList as IList<BlockType> ?? contentTypeList.ToList();
                                    foreach (var i in blockTypes)
                                    {
                                        if (Session["EpiContentType"].ToString().Substring(6) != i.Name) continue;
                                        myProperty = i;
                                        break;
                                    }
                                    var ddlMetaData = new DropDownList {Height = 28, Width = 194};
                                    myProperty.PropertyDefinitions.ToList().ForEach(i =>
                                        ddlMetaData.Items.Add(new ListItem(i.Name, i.Name)));
                                    ddlMetaData.ID = "meta-" + element.Label;
                                    tCell.Controls.Add(ddlMetaData);
                                }
                                else if (Session["EpiContentType"].ToString().StartsWith("page-"))
                                {
                                var contentTypeList = contentTypeRepository.List().OfType<PageType>();
                                var myProperty = new PageType();
                                var pageTypes = contentTypeList as IList<PageType> ?? contentTypeList.ToList();
                                foreach (var i in pageTypes)
                                {
                                    if (Session["EpiContentType"].ToString().Substring(5) != i.Name) continue;
                                    myProperty = i;
                                    break;
                                }
                                var ddlMetaData = new DropDownList {Height = 28, Width = 194};
                                myProperty.PropertyDefinitions.ToList().ForEach(i =>
                                    ddlMetaData.Items.Add(new ListItem(i.Name, i.Name)));
                                ddlMetaData.ID = "meta-" + element.Label;
                                tCell.Controls.Add(ddlMetaData);
                                }
                            }
                        }
                        tRow.Cells.Add(tCell);
                    }
                }
            }
        }

        protected void BtnSaveMapping_OnClick(object sender, EventArgs e)
        {
            var epiFieldMaps = from string key in Request.Form.Keys
                where key.StartsWith("meta-")
                select Request.Form[key] + "~" + key.Substring(5);
			Session["EpiFieldMaps"] = epiFieldMaps.ToList();
            var mappingsStore = GcDynamicTemplateMappings.RetrieveStore();
            var newMapping = new GcDynamicTemplateMappings(Session["AccountId"].ToString(), Session["ProjectId"].ToString(),
                Session["TemplateId"].ToString(), Session["PostType"].ToString(), Session["Author"].ToString(),
                Session["DefaultStatus"].ToString(), Session["EpiContentType"].ToString(), (List<GcEpiStatusMap>)Session["StatusMaps"],
                (List<string>)Session["EpiFieldMaps"], $"{DateTime.Now:G}");
            var existingIndex = mappingsStore.FindIndex(i => i.TemplateId == Session["TemplateId"].ToString());
            if (existingIndex > 0)
            GcDynamicTemplateMappings.DeleteItem(mappingsStore[existingIndex].Id);
            GcDynamicTemplateMappings.SaveStore(newMapping);
            Session.Clear();
            Response.Redirect("~/GatherContentPlugin/GcEpiTemplateMappings.aspx");
        }
    }
}