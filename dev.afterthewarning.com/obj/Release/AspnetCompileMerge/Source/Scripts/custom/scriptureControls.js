//==================================================
//  Manage Sripture Controls
//==================================================
jQuery(function ($) {
    function jsChapterCellHeights() {
        //ENSURES THE CHAPTER ICONS ON THE BIBLE PAGES ARE EQUAL WIDTH/HEIGHT WHEN RESIZED
        var chapterCell = $('.chapterGrid .cell div');
        var width = chapterCell.first().width();
        chapterCell.height(width);
    }

    try {
        jsChapterCellHeights();
        $(window).resize(function () { jsChapterCellHeights(); });
    }
    catch (err) {
        console.log('ERROR: [jsChapterCellHeights] ' + err.message);
    }
});