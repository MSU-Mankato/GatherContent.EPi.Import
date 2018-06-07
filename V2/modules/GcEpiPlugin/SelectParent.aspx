<%@ Page Language="c#" CodeBehind="SelectParent.aspx.cs" AutoEventWireup="False" Inherits="GatherContentImport.modules.GcEpiPlugin.SelectParent" Title="Select Default Parent" Async="true" %>

<html>
<head id="SelectParentHead" runat="server">
    <link rel="stylesheet" type="text/css" href="/EPiServer/Shell/11.2.0.0/ClientResources/epi/themes/legacy/ShellCore.css">
    <link rel="stylesheet" type="text/css" href="/EPiServer/Shell/11.2.0.0/ClientResources/epi/themes/legacy/ShellCoreLightTheme.css">
    <link href="/EPiServer/CMS/App_Themes/Default/Styles/system.css" type="text/css" rel="stylesheet">
    <link href="/EPiServer/CMS/App_Themes/Default/Styles/ToolButton.css" type="text/css" rel="stylesheet">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/chosen.css">
    <link rel="stylesheet" href="/modules/GcEpiPlugin/ClientResources/css/loading.css">
    <link type='text/css' rel='Stylesheet' href='/Util/styles/pagetreeview.css'>
    <title>Select Default Parent</title>

</head>
<body class="epi-applicationSidebar">

    <form id="selectParentForm" runat="server">
        <input name="SelectedItemId" type="hidden" id="FullRegion_selectedItemId" runat="server" />
        <input name="SelectedItemId" type="hidden" id="FullRegion_selectedItemName" runat="server" />
        <div class="episerver-pagebrowserSearch">
            <span id="FullRegion_Label1">Search</span>
            <input name="ctl00$FullRegion$searchKey" type="text" id="FullRegion_searchKey" class="episize240">
            <span class="epi-cmsButton">
                <input class=" epi-cmsButton-tools epi-cmsButton-Search" type="submit" name="ctl00$FullRegion$searchButton" id="FullRegion_searchButton" value=" " title="Search" onmouseover="EPi.ToolButton.MouseDownHandler(this)" onmouseout="EPi.ToolButton.ResetMouseDownHandler(this)"></span>
        </div>

        <div class="episcroll episerver-pagebrowserContainer">
            <div class="episerver-pagetreeview" id="FullRegion_pageTreeView_treeView">
                <ul id="FullRegion_pageTreeView_treeView_ul">
                    <li class="parentlast" id="FullRegion_pageTreeView_treeView0_1">
                        <span class="icon collapselast">&nbsp;</span>
                        <span class="templatecontainer selected">
                            <img class="typeicon" src="/App_Themes/Default/Images/ExplorerTree/PageTree/Root.gif">
                            <a href="#" target="PreviewFrame" class="containernode">Root folder</a>
                        </span>
                    </li>

                </ul>
                <div class="dragmarker" style="display: none; position: absolute;"></div>

            </div>
        </div>
        <div class="episerver-pagebrowserButtonContainer">
            <span class="epi-cmsButton" data-epi-dialog-button="container">
                <input type="button" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Check" onmouseover="EPi.ToolButton.MouseDownHandler(this)" onmouseout="EPi.ToolButton.ResetMouseDownHandler(this)"
                    id="FullRegion_okButton" name="selectBtn" value="Select" onclick="onSelect()">
            </span>
            <span class="epi-cmsButton" data-epi-dialog-button="container">
                <input type="button" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Cancel" data-epi-dialog-button="functioner"
                    name="FullRegion_cancelButton" id="FullRegion_cancelButton" value="Cancel" onmouseover="EPi.ToolButton.MouseDownHandler(this)" onmouseout="EPi.ToolButton.ResetMouseDownHandler(this)"
                    onclick="(function () {
        self.close();
    })(); return false;" />
            </span>

        </div>

    </form>
    <script src="/EPiServer/CMS/javascript/pagetreeview.js" type="text/javascript"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"></script>
    <script src="https://code.jquery.com/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="/EPiServer/Shell/11.2.0.0/ClientResources/ShellCore.js"></script>
    <script type="text/javascript" src="/EPiServer/CMS/11.2.0.0/ClientResources/ReportCenter/ReportCenter.js"></script>

    <script src="/WebResource.axd?d=pynGkmcFUV13He1Qd6_TZHL4XgZJAWRSoJmhp9VIRDSrDSTzS7iFY8NXY_K4Y7eNLiEwmHiX3R-lfQlpFWNSQQ2&amp;t=636590685237014043" type="text/javascript"></script>


    <script src="/Util/javascript/episerverscriptmanager.js" type="text/javascript"></script>
    <script src="/EPiServer/CMS/javascript/system.js" type="text/javascript"></script>
    <script src="/EPiServer/CMS/javascript/dialog.js" type="text/javascript"></script>
    <script src="/EPiServer/CMS/javascript/system.aspx" type="text/javascript"></script>
    <script src="/EPiServer/CMS/javascript/epitooltip.js" type="text/javascript"></script>
    <script type="text/javascript">
        //<![CDATA[
        function _pageExplorerInit(treeView) {
            treeView.OnNodeOver = EPi.ToolTip.Show;
            initPageTreeView(treeView);
        }
//]]>
    </script>
    <link type='text/css' rel='Stylesheet' href='/Util/styles/pagetreeview.css'></link>
    <script src="/EPiServer/CMS/javascript/pagetreeview.js" type="text/javascript"></script>
    <script type="text/javascript">
        var data = '<%= JsonItemTree %>';
        var x = JSON.parse(data);

        function traverseTree(x) {

            if (typeof (x) == 'object') {

                var ul = $('<ul>');
                for (var i in x) {
                    if (typeof x[i].ItemName != 'undefined') {
                        var expandBtnSpan = $('<span />').addClass('icon expand').html('&nbsp');
                        var innerSpan = $('<span />').addClass('templatecontainer').attr('id', 'toggleColor' + x[i].ItemId);
                        var li = $('<li/>').addClass('parent');
                        console.log(x[i].ItemName);
                        li.append(expandBtnSpan).append(innerSpan);
                        var aItem = $('<a/>')
                            .addClass('containernode')
                            .attr(
                            { 'href': '#', 'id': x[i].ItemId, 'onclick': 'highlightSelectedItem(this.id, "' + x[i].ItemName + '")' })
                            .text(x[i].ItemName)
                            .appendTo(innerSpan);
                        ul.append(li);
                    }

                    ul.append(traverseTree(x[i]));
                }
                return ul;
            }
        }

        $("#FullRegion_pageTreeView_treeView ul ").append(traverseTree(x));

        function highlightSelectedItem(id, name) {
            $('#toggleColor' + id).addClass('selected');
            $('#FullRegion_selectedItemId').val(id);
            $('#FullRegion_selectedItemName').val(name);
        }

        function onSelect() {
            var selectId = document.getElementById('FullRegion_selectedItemId').value;
            var selectName = document.getElementById('FullRegion_selectedItemName').value;
            window.opener.document.getElementById('defaultSelectItemId').value = selectId;
            window.opener.document.getElementById('defaultSelectItemName').value = selectName;
            self.close();
            return false;
        }

    </script>

</body>
</html>
