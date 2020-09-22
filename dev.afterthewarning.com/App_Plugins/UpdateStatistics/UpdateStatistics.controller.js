angular
    .module('umbraco')
    .controller('UpdateStatisticsController', function ($scope) {
        //Instantiate variables
        var btnGenerate = $('.UpdateStatisticsController button.btnUpdateStatistics');
        var resultPanel = $('.UpdateStatisticsController .msg');
        var alertMsgPanel = $('.UpdateStatisticsController .alertMsg');
        var lblErrorMsg = $('.UpdateStatisticsController .alertMsg .errorMsg');
        var btnExportToExcel = $('.UpdateStatisticsController button.btnExportToExcel');
        var lblSuccessfulEmails = $('#lblSuccessfulEmails');
        var lblFailedEmails = $('#lblFailedEmails');


        //Handles
        btnGenerate.click(function (e) {
            //Prevent 
            e.preventDefault();

            //Call webservice
            UpdateStats();

            //disable button to prevent multiple clicks.
            btnGenerate.prop('disabled', true);
            btnGenerate.addClass('disabled');
        });


        //Methods
        function UpdateStats() {
            //Valid email address. Instantiate variables
            var urlPath = window.location.protocol + '//' + window.location.host;
            var ashxUrl = urlPath + '/Services/CustomServices.asmx/UpdateStats';
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

                var str = JSON.stringify(data, undefined, 4);
                resultPanel.html("<pre>" + syntaxHighlight(str) + "</pre>");

                //Show results
                resultPanel.show()
            }
        }


        function syntaxHighlight(json) {
            json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
            return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
                var cls = 'number';
                if (/^"/.test(match)) {
                    if (/:$/.test(match)) {
                        cls = 'key';
                    } else {
                        cls = 'string';
                    }
                } else if (/true|false/.test(match)) {
                    cls = 'boolean';
                } else if (/null/.test(match)) {
                    cls = 'null';
                }
                return '<span class="' + cls + '">' + match + '</span>';
            });
        }


    });