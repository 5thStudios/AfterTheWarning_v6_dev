//
//==================================================
jQuery(function ($) {
    function jsViewMsgsBy() {
        //Instantiate variables
        var btnsViewMsgBy = $('#view-messages-by a');
        var liByVisionary = $('#liByVisionary');
        var liByDate = $('#liByDate');
        var pnlByVisionary = $("#pnlByVisionary");
        var pnlByDate = $("#pnlByDate");
        var paginationLst = $('.paginationLst a');


        //Handles
        $(btnsViewMsgBy).click(function () {
            //Pass how to display msg view
            showMsgsBy($(this).attr('href'));
        })


        //Methods
        function showMsgsBy(viewBy) {
            //Instantiate variables
            var viewByDate = false;

            //Determine how to view page
            if (viewBy.length > 0) {
                if (viewBy === "#by-date") {
                    viewByDate = true;
                }
            }

            if (viewByDate) {
                //Show msgs by date
                liByDate.addClass('active');
                liByVisionary.removeClass('active');

                pnlByVisionary.hide();
                pnlByDate.show();

                updatePaginationLinks(viewBy);

                //Reinitialize equalizers
                Foundation.reInit('equalizer');
            }
            else {
                //Show msgs by visionary
                liByDate.removeClass('active');
                liByVisionary.addClass('active');

                pnlByVisionary.show();
                pnlByDate.hide();
            }
        }
        function updatePaginationLinks(hash) {
            $(paginationLst).each(function (i, v) {
                var href = $(v).attr('href');
                if (this.href.indexOf(hash) == -1) {
                    $(v).attr('href', href + hash);
                }                
            })
        }


        //Initializers
        showMsgsBy(location.hash);
        updatePaginationLinks(location.hash);
    }

    try {
        if ($('#view-messages-by').length > 0) { jsViewMsgsBy(); }
    }
    catch (err) {
        console.log('ERROR: [jsViewMsgsBy] ' + err.message);
    }
});