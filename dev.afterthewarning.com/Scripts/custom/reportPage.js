//==================================================
//  Report Page
//==================================================
jQuery(function ($) {
    function jsReportPage() {
        //Instantiate variables
        var formHiddenFields = $('.report-page .formulate__field--hidden input[type=hidden]');
        var pageId = $('#pageId').val();
        var pageName = $('#pageName').val();
        var pageUrl = $('#pageUrl').val();


        //Set form fields with data
        formHiddenFields.eq(0).val(pageId);
        formHiddenFields.eq(1).val(pageName);
        formHiddenFields.eq(2).val(pageUrl);
    }

    try {
        if ($('.report-page .formulate__field--hidden').length > 0) { jsReportPage(); }
        
    }
    catch (err) {
        console.log('ERROR: [jsReportPage] ' + err.message);
    }
});