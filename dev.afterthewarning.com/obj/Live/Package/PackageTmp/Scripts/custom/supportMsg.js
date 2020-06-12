//==================================================
//  Manage the support message
//==================================================
jQuery(function ($) {
    function jsSupportMsg() {
        //Instantiate variables
        var supportMsg = $('.supportMsg');
        var btnRemindLater = $('.supportMsg .button.remindLater');
        var btnNoThanks = $('.supportMsg .button.noThanks');
        var btnSupportUsToday = $('.supportMsg .button.supportUsToday');
        var cookieName = 'ATW_supportMsg';
        var myCookie;

        //Get cookie
        myCookie = Cookies.get(cookieName);
        //Does cookie exist?
        if (typeof myCookie === 'undefined') {
            //show msg
            supportMsg.show();
        }

        //Button clicks
        btnRemindLater.click(function () {
            //save cookie that expires tomorrow
            createCookie_RemindTomorrow();
            //close msg
            closeSupportMsg();

        });
        btnNoThanks.click(function () {
            //save cookie that expires in n months
            createCookie_RemindIn2Months();
            //close msg
            closeSupportMsg();
        });
        btnSupportUsToday.click(function () {
            //save cookie that expires in n months
            createCookie_RemindIn2Months();
            //close msg
            closeSupportMsg();            
        });


        function closeSupportMsg() {
            supportMsg.hide();
        }
        function createCookie_RemindTomorrow() {
            Cookies.set(cookieName, 'hide message for 1 day', { expires: 1 });
        }
        function createCookie_RemindIn2Months() {
            Cookies.set(cookieName, 'hide message for 2 months', { expires: 60 });
        }
        function createCookie_RemindIn3Months() {
            Cookies.set(cookieName, 'hide message for 3 months', { expires: 90 });
        }
    }

    try {
        jsSupportMsg();
    }
    catch (err) {
        console.log('ERROR: [jsSupportMsg] ' + err.message);
    }
});