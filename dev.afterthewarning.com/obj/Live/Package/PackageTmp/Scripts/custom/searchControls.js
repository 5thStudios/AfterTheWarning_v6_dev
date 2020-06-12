//==================================================
//  Manage Search Controls
//==================================================
jQuery(function ($) {
    function jsSearchControls() {
        //Instantiate variables
        var txtSearch = $('#searchPnl #txtSearch');
        var btnSearch = $('#searchPnl #btnSearch');
        var $cbFilter = $('#searchPnl .cbFilter');
        var $cbFilterPnl = $('#searchPnl .cbFilterPnl');
        var cbMessages = $('#searchPnl #cbMessages');
        var cbPrayers = $('#searchPnl #cbPrayers');
        var cbArticles = $('#searchPnl #cbArticles');
        var cbBible = $('#searchPnl #cbBible');
        var cbIlluminations = $('#searchPnl #cbIlluminations');
        var cbFilterMessages = $('#searchPnl .cbFilterPnl.messages');
        var cbFilterArticles = $('#searchPnl .cbFilterPnl.articles');
        var cbFilterPrayers = $('#searchPnl .cbFilterPnl.prayers');
        var cbFilterBible = $('#searchPnl .cbFilterPnl.bible');
        var cbFilterIlluminations = $('#searchPnl .cbFilterPnl.illuminations');
        var active = "active";


        //Handles
        $(txtSearch).keyup(function (event) {
            if (event.keyCode === 13) {
                //On enter key click submit search.
                $(btnSearch).click();
            }
        });
        $(btnSearch).click(function () {
            //Variables
            var searchIn = "";
            //get search text
            var searchFor = txtSearch.val().trim().toLowerCase();

            //Get filter
            switch (true) {
                case cbMessages.prop("checked"):
                    searchIn = cbMessages.val();
                    break;
                case cbPrayers.prop("checked"):
                    searchIn = cbPrayers.val();
                    break;
                case cbArticles.prop("checked"):
                    searchIn = cbArticles.val();
                    break;
                case cbBible.prop("checked"):
                    searchIn = cbBible.val();
                    break;
                case cbIlluminations.prop("checked"):
                    searchIn = cbIlluminations.val();
                    break;
            }

            searchFor = txtSearch.val().trim().toLowerCase();

            //Refresh page with new parameters
            location = window.location.pathname + "?" + $.param({ 'searchIn': searchIn, 'searchFor': searchFor });
        });
        $($cbFilter).click(function () {
            //Extract clicked cb
            var $this = $(this);

            //clear all checkboxes except selected one
            $cbFilter.prop("checked", false);
            $this.prop("checked", true);

            //Clear all active pnls except selected one
            $cbFilterPnl.removeClass(active);
            $cbFilterPnl.eq($cbFilter.index($this)).addClass(active);
        });


        //Methods
        function setFilters() {
            //Uncheck all checkboxes
            $cbFilter.prop("checked", false);
            $cbFilterPnl.removeClass(active);

            //Set filter
            switch (getUrlParam("searchIn")) {
                case "messages":
                    cbMessages.prop("checked", true);
                    cbFilterMessages.addClass(active);
                    break;
                case "articles":
                    cbArticles.prop("checked", true);
                    cbFilterArticles.addClass(active);
                    break;
                case "prayers":
                    cbPrayers.prop("checked", true);
                    cbFilterPrayers.addClass(active);
                    break;
                case "bible":
                    cbBible.prop("checked", true);
                    cbFilterBible.addClass(active);
                    break;
                case "illuminations":
                    cbIlluminations.prop("checked", true);
                    cbFilterIlluminations.addClass(active);
                    break;
            }

            //Set search field
            txtSearch.val(getUrlParam("searchFor"));
        }
        function getUrlParam(parameter) {
            var urlparameter = "";
            if (window.location.href.indexOf(parameter) > -1) {
                urlparameter = getUrlVars()[parameter];
            }
            return urlparameter;
        }
        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }

        //Init Calls
        setFilters();
    }

    try {
        jsSearchControls();
    }
    catch (err) {
        console.log('ERROR: [jsSearchControls] ' + err.message);
    }
});