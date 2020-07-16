angular
    .module('umbraco')
    .controller('EmailUpdatesController', function ($scope) {
        //Instantiate variables
        var btnGenerate = $('.EmailUpdatesController button.btnEmailUpdates');
        var resultPanel = $('.EmailUpdatesController .msg');
        var alertMsgPanel = $('.EmailUpdatesController .alertMsg');
        var lblErrorMsg = $('.EmailUpdatesController .alertMsg .errorMsg');
        var btnExportToExcel = $('.EmailUpdatesController button.btnExportToExcel');
        var lblSuccessfulEmails = $('#lblSuccessfulEmails');
        var lblFailedEmails = $('#lblFailedEmails');


        //Handles
        btnGenerate.click(function (e) {
            //Prevent 
            e.preventDefault();

            //Call webservice
            SendUpdatesViaEmail();

            //disable button to prevent multiple clicks.
            btnGenerate.prop('disabled', true);
            btnGenerate.addClass('disabled');
        });


        //Methods
        function SendUpdatesViaEmail() {
            //Valid email address. Instantiate variables
            var urlPath = window.location.protocol + '//' + window.location.host;
            var ashxUrl = urlPath + '/Services/CustomServices.asmx/SendUpdatesByEmail';
            var data = '';

            //Call AJAX service
            var response = CallService_POST();
            var promise = $.when(response);
            promise.done(function () { ServiceSucceeded(response); });
            promise.fail(function () { ServiceFailed(response); });

            //METHODS
            function CallService_POST() {
                try {
                    return $.ajax({
                        url: ashxUrl, // Location of the service
                        type: "POST", //GET or POST or PUT or DELETE verb
                        data: data, //Data sent to server
                        dataType: "json", //Expected data format from server
                        contentType: "application/json; charset=utf-8", // content type sent to server
                        processdata: true //True or False
                    });
                }
                catch (err) {
                    if (err.message !== null) {
                        console.log(err.message);
                    }
                }
            }
            function ServiceFailed(result) {
                //Error message
                console.log('Service call failed: ' + result.status + ' ' + result.statusText + ' ' + result.responseText);
                console.log('Error Msg:');
                console.log(result);

                //Display error message
                lblErrorMsg.text(result.status + '<br />' + result.statusText + '<br />' + result.responseText);
                alertMsgPanel.show();
            }
            function ServiceSucceeded(result) {
                console.log('Service call succeeded:');
                var data = $.parseJSON(result.responseJSON.d);
                console.log(data);

                //Display data
                lblSuccessfulEmails.text(data.successful);
                lblFailedEmails.text(data.failed);

                //Show results
                resultPanel.show()
            }
        }
    });