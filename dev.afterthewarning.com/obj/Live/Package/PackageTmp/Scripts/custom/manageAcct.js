//==================================================
//  Manage Account
//==================================================
jQuery(function ($) {
    function jsManageAcct() {
        //Instantiate variables
        var filters = $('.filters li:not(.inactive)');
        var inactiveFilters = $('.filters li.inactive a');
        var pnlEditAcct = $('#pnlEditAcct');
        var pnlPrayerRequests = $('#pnlPrayerRequests');
        var pnlNewPrayer = $('#pnlNewPrayer');
        var pnlEditPrayer = $('#pnlEditPrayer');
        var pnlPrayerSummary = $('#pnlPrayerSummary');
        var pnlPrayerPledges = $('#pnlPrayerPledges');
        var pnlAddEditIlluminationStory = $('#pnlAddEditIlluminationStory');

        //Handles
        $(filters).click(function () {
            //Instantiate variable
            var filter = $(this);

            //Set the filter buttons
            filters.removeClass("active");
            filter.addClass("active");
        });
        $(inactiveFilters).click(function (e) {
            e.preventDefault();
        });


        function showActiveBtn() {
            filters.removeClass("active");
            if (pnlEditAcct.length === 1) { filters.eq(0).addClass("active"); }
            if (pnlPrayerRequests.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlNewPrayer.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlEditPrayer.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlPrayerSummary.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlPrayerPledges.length === 1) { filters.eq(2).addClass("active"); }
            if (pnlAddEditIlluminationStory.length === 1) { filters.eq(3).addClass("active"); }
        }
        showActiveBtn();
    }

    try {
        jsManageAcct();
    }
    catch (err) {
        console.log('ERROR: [jsManageAcct] ' + err.message);
    }
});