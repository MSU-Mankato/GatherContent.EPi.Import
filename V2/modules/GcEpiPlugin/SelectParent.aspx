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
    <link type='text/css' rel='Stylesheet' href='/Util/styles/pagetreeview.css' />

    <title>Select Default Parent</title>
    <style>
        .found {
           background: #abaaaa;
        }
    </style>
</head>
<body class="epi-applicationSidebar">
    <form id="selectParentForm" runat="server">
        <input name="SelectedItemId" type="hidden" id="FullRegion_selectedItemId" runat="server" />
        <input name="SelectedItemId" type="hidden" id="FullRegion_selectedItemName" runat="server" />
        <div class="episerver-pagebrowserSearch">
            <span id="FullRegion_Label1">Search</span>
            <input name="FullRegion$searchKey" type="text" id="FullRegion_searchKey" class="episize240">
            <span class="epi-cmsButton">
                <input class=" epi-cmsButton-tools epi-cmsButton-Search" type="button" name="FullRegion$searchButton" id="FullRegion_searchButton" value=" " title="Search" onmouseover="EPi.ToolButton.MouseDownHandler(this)" onmouseout="EPi.ToolButton.ResetMouseDownHandler(this)"></span>
        </div>

        <div class="episcroll episerver-pagebrowserContainer">
            <div class="episerver-pagetreeview">
                <ul id="FullRegion_treeView">
                    <li class="parentlast">
                        <span class="icon collapselast" id="btnExpandRoot">&nbsp;</span>
                        <span class="templatecontainer">
                            <img class="typeicon" src="/App_Themes/Default/Images/ExplorerTree/PageTree/Root.gif">
                        </span>
                        <ul id="FullRegion_pageTreeView_treeView">
                        </ul>
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
    <script src="/EPiServer/CMS/javascript/pagetreeview.js" type="text/javascript"></script>
    <script>

        var x = <%= JsonItemTree %>;
        //var x = JSON.parse(data);
        //var jsonItemListData = '<%= JsonItemList %>';
        var parsedJsonItemListData = <%= JsonItemList %>;//JSON.parse(jsonItemListData);
        //Array of searched li elements
        var searchedLi = [];

        // Creates tree structure out of nested json
        $(function () {
            // Parsing of root item of the json object
            var aRootTag = $('<a />').addClass('containernode').attr(
                { 'href': '#', 'id': x.ItemId, 'onclick': 'highlightSelectedItem(this.id, "' + x.ItemName + '")' }).text(x.ItemName);
            $('span.templatecontainer').attr('id', 'toggleColor' + x.ItemId).append(aRootTag);

            var tree = $('#FullRegion_pageTreeView_treeView');
            parseTree(tree, x.Children);

            // Add toggle function for nodes
            $("span#btnExpand").click(toggleChild);
            $("span#btnExpandRoot").click(toggleRoot);

            // Call searh function
            $('#FullRegion_searchButton').click(searchItem);

        });

        // For root item toggle button
        function toggleRoot() {
            $(this).siblings("ul#FullRegion_pageTreeView_treeView").toggle();
            $(this).toggleClass("collapselast expandlast");
            return false;
        }

        // For children item toggle button
        function toggleChild() {
            $(this).siblings("ul.ullist").toggle();

            // Filter leaf nodes and assign respective classes.
            if (!$(this).siblings("ul.ullist").children('li').length) {
                if ($(this).hasClass("collapse")) {
                    $(this).removeClass("collapse").addClass("leafnode");
                } else {
                    $(this).removeClass("collapselast").addClass("leafnodelast");
                }
            }
            else {
                if ($(this).hasClass("collapselast") || $(this).hasClass("expandlast")) {
                    $(this).toggleClass("collapselast expandlast");
                }
                if ($(this).hasClass("collapse") || $(this).hasClass("expand")) {
                    $(this).toggleClass("expand collapse");
                }
            }
            return false;
        }

        function searchItem() {
            var li;
            //remove selection from previous search
            searchedLi.forEach(function (item) {
                $(item).removeClass('found');
            });
            searchedLi = [];

            var ul = $('#FullRegion_treeView').find("li");
            var searchText = $('#FullRegion_searchKey').val().toUpperCase();

            for (var i = 0; i < ul.length; i++) {
                var a = $(ul[0]).find('a')[i];
                if (a != undefined) {
                    if (a.text.toUpperCase().indexOf(searchText) > -1) {
                        li = $(ul[0]).find('.templatecontainer')[i];
                        $(li).addClass('found');
                        searchedLi.push(li);
                    }
                }
            }
            if (searchedLi.length < 1) {
                alert("This item does not exist!");
            }
        }

        var selectedId = 0;
        // Highlight selected item from tree.
        function highlightSelectedItem(id, name) {
            // Removes selected class from previous selected item.
            $('#toggleColor' + selectedId).removeClass('selected');

            // Adds selected class to selected item.
            $('#toggleColor' + id).addClass('selected');
            $('#FullRegion_selectedItemId').val(id);
            $('#FullRegion_selectedItemName').val(name);

            selectedId = id;
        }

        // Select item and pass selected item to parent page
        function onSelect() {
            var selectId = document.getElementById('FullRegion_selectedItemId').value;
            var selectName = document.getElementById('FullRegion_selectedItemName').value;
            window.opener.document.getElementById('defaultSelectItemId').value = selectId;
            window.opener.document.getElementById('defaultSelectItemName').value = selectName;
            self.close();
            return false;
        }

        // Creates tree structure out of nested json
        function parseTree(ul, tree) {
            for (var i = 0; i < tree.length; i++) {
                var expandBtnSpan = $('<span />');
                var innerSpan = $('<span />').addClass('templatecontainer').attr('id', 'toggleColor' + tree[i].ItemId);
                var li = $('<li/>');

                if (i === tree.length - 1) {
                    expandBtnSpan.addClass('icon collapselast').attr('id', 'btnExpand')
                        .html('&nbsp');
                    li.addClass('parentlast').appendTo(ul);
                } else {
                    li.addClass('parent').appendTo(ul);
                    expandBtnSpan.addClass('icon collapse').attr('id', 'btnExpand')
                        .html('&nbsp');
                }

                li.append(expandBtnSpan).append(innerSpan);

                var aItem = $('<a/>')
                    .addClass('containernode')
                    .attr(
                        { 'href': '#', 'id': tree[i].ItemId, 'onclick': 'highlightSelectedItem(this.id, "' + tree[i].ItemName + '")' })
                    .text(tree[i].ItemName)
                    .appendTo(innerSpan);

                if (tree[i].Children != null) {
                    var subul = $('<ul class="ullist"></ul>');
                    $(li).append(subul);
                    parseTree($(subul), tree[i].Children);
                }
            }
        }
        //Prevent postback on hitting enter in search input area
        $('#FullRegion_searchKey').keydown(function (event) {
            if (event.keyCode === 13) {
                event.preventDefault();
                return false;
            }
        });

    </script>
</body>
</html>
