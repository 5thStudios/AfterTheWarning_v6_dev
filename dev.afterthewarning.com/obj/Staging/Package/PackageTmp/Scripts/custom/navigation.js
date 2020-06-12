//==================================================
//  Manage Sripture Controls
//==================================================
jQuery(function ($) {
    function jsStickyNav() {
        //  CONTROLS THE DESKTOP MENU TO ENSURE IT STICKS TO THE TOP AFTER SCROLLING IN DESKTOP MODE
        if ($(window).scrollTop() > topOfMenu) {
            $(desktopMenu).addClass('fixed');
        }
        else {
            topOfMenu = $(desktopMenu).offset().top;
            $(desktopMenu).removeClass('fixed');
        }
    }

    try {
        var desktopMenu = $('#desktopMenu');
        var topOfMenu = $(desktopMenu).offset().top;

        $(window).bind('scroll', function () {
            jsStickyNav();
        });

        jsStickyNav();
    }
    catch (err) {
        console.log('ERROR: [jsStickyNav] ' + err.message);
    }
});