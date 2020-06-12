//==================================================
//  Pagination
//==================================================
jQuery(function ($) {

    function jsPagination() {
        // Instantiate variables
        var setSize = 5;
        var existingPageIDs = [];
        var index = 1;
        var pageIdSets = [];
        var currentSet = [];
        var paginationLst = $(".paginationLst");
        var pageNo = paginationLst.eq(0).find("#pageNo").val();
        var totalPages = paginationLst.eq(0).find("#totalPages").val();
        var pageNoLink = $(".paginationLst .page-no");
        var prevSetIcon = $(".paginationLst .pagination-previous-set");
        var nextSetIcon = $(".paginationLst .pagination-next-set");
        var prevSetBtn = $(".paginationLst .pagination-previous-set a");
        var nextSetBtn = $(".paginationLst .pagination-next-set a");
        var paginationFirst = $(".paginationLst .pagination-first");
        var prevPage = $(".paginationLst .pagination-previous");
        var nextPage = $(".paginationLst .pagination-next");
        var paginationLast = $(".paginationLst .pagination-last");

        //console.log('pageNo: ' + pageNo);
        //console.log('totalPages: ' + totalPages);


        //IF LESS THAN SET SIZE, SHOW ALL PAGES
        if (totalPages === "0") {
            paginationLst.hide();
        }
        else if (totalPages === "1") {
            paginationLst.hide();
        }
        else if (totalPages <= setSize) {
            //SHOW ALL LINKS
            pageNoLink.show();
            paginationFirst.show();
            paginationLast.show();
            prevSetIcon.hide();
            nextSetIcon.hide();
        }
        else {
            //CREATE ARRAY OF PAGE NUMBERS
            while (index <= totalPages) {
                existingPageIDs.push(index);
                index++;
            }
            //console.log(existingPageIDs);


            //SPLIT ARRAY OF PAGE NUMBERS INTO GROUPS
            while (existingPageIDs.length > 0)
                pageIdSets.push(existingPageIDs.splice(0, setSize));
            //console.log(pageIdSets);


            //FIND WHICH ARRAY SET THE CURRENT PAGE EXISTS IN
            currentSet = parseInt((pageNo - 1) / setSize);
            //console.log('currentSet: ' + currentSet);
            //console.log('pageIdSets: ' + pageIdSets.length);


            //HIDE ALL BUT PAGES LISTED IN CURRENT SET
            pageNoLink.hide();
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }


            //SHOW SET ICONS IF NEEDED
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }


            //SHOW FIRST/LAST LINKS IF NEEDED
            if (parseInt(pageNo) > 1) { paginationFirst.show(); }
            if (parseInt(pageNo) < parseInt(totalPages)) { paginationLast.show(); }


            //SHOW PREV/NEXT PAGE LINKS IF NEEDED
            prevPage.hide();
            nextPage.hide();
            if (parseInt(pageNo) > 1) { prevPage.show(); }
            if (parseInt(pageNo) < parseInt(totalPages)) { nextPage.show();}
        }


        //HANDLES
        $(prevSetBtn).click(function () {
            pageNoLink.hide();
            currentSet = currentSet - 1;
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }
            //SHOW SET ICONS IF NEEDED
            prevSetIcon.hide();
            nextSetIcon.hide();
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }
        });
        $(nextSetBtn).click(function () {
            pageNoLink.hide();
            currentSet = currentSet + 1;
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }
            //SHOW SET ICONS IF NEEDED
            prevSetIcon.hide();
            nextSetIcon.hide();
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }
        });


    }

    //Run only if element exists
    try {
        if ($('.paginationLst').length > 0) { jsPagination(); }
    }
    catch (err) {
        console.log('ERROR: [jsPagination] ' + err.message);
    }
});