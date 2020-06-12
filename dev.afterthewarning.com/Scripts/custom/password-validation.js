//Validate password change.
//=========================================
//try {
//$(window).load(function () {

    //Instantiate variables
    var $pwd1Valid = $('.pwd1 .valid');
    var $pwd1Invalid = $('.pwd1 .invalid');
    var $pwd2Valid = $('.pwd2 .valid');
    var $pwd2Invalid = $('.pwd2 .invalid');
    var txbPassword = $('#Password');
    var txbPasswordReenter = $('#ConfirmPassword');
    var hfldAcceptPwChange = $('#ValidPassword');

    //Run initial validations on load.
    validatePw($pwd1Valid, $pwd1Invalid, txbPassword.val().trim(), txbPasswordReenter.val().trim());
    validatePw($pwd2Valid, $pwd2Invalid, txbPasswordReenter.val().trim(), txbPassword.val().trim());

    //Validate password entry 1 if txt is updated.
    txbPassword.bind('input', function (e) {
        validatePw($pwd1Valid, $pwd1Invalid, $(this).val().trim(), txbPasswordReenter.val().trim());
    });
    //Validate password entry 2 if txt is updated.
    txbPasswordReenter.bind('input', function (e) {
        validatePw($pwd2Valid, $pwd2Invalid, $(this).val().trim(), txbPassword.val().trim());
    });


    function validatePw($pwdValid, $pwdInvalid, pwValue, otherPwValue) {
        //Call validation functions
        validate_charLength(0, pwValue);
        validate_uppercase(1, pwValue);
        validate_lowercase(2, pwValue);
        validate_numeric(3, pwValue);
        validate_alphanumeric(4, pwValue);
        validate_bothMatch(5, pwValue, otherPwValue);
        acceptPwChange();


        function validate_charLength(i, value) {
            //               
            if (value.length > 5) {
                $pwdInvalid.eq(i).hide();
                $pwdValid.eq(i).show();
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }

        function validate_uppercase(i, value) {
            //            
            if (/[A-Z]/.test(value)) {
                $pwdInvalid.eq(i).hide();
                $pwdValid.eq(i).show();
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }

        function validate_lowercase(i, value) {
            //
            if (/[a-z]/.test(value)) {
                $pwdInvalid.eq(i).hide();
                $pwdValid.eq(i).show();
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }

        function validate_numeric(i, value) {
            //
            if (/[0-9]/.test(value)) {
                $pwdInvalid.eq(i).hide();
                $pwdValid.eq(i).show();
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }

        function validate_bothMatch(i, value, otherValue) {
            //
            if (value.length > 0) {
                //
                if (value == otherValue) {
                    $pwd1Invalid.eq(i).hide();
                    $pwd2Invalid.eq(i).hide();
                    $pwd1Valid.eq(i).show();
                    $pwd2Valid.eq(i).show();
                }
                else {
                    $pwd1Valid.eq(i).hide();
                    $pwd2Valid.eq(i).hide();
                    $pwd1Invalid.eq(i).show();
                    $pwd2Invalid.eq(i).show();
                }
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }

        function validate_alphanumeric(i, alphane) {
            var pattern = new RegExp('^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$');
            if (pattern.test(alphane)) {
                $pwdInvalid.eq(i).hide();
                $pwdValid.eq(i).show();
            }
            else {
                $pwdInvalid.eq(i).show();
                $pwdValid.eq(i).hide();
            }
        }
    }

    function acceptPwChange() {
        //Instantiate variables
        var isValid = true;

        //Are all validations valid?
        $pwd1Valid.each(function () {
            if (isValid == true) {
                if ($(this).is(':hidden')) {
                    isValid = false;
                };
            };
        });

        //Are all validations valid?
        $pwd2Valid.each(function () {
            if (isValid == true) {
                if ($(this).is(':hidden')) {
                    isValid = false;
                };
            };
        });

        //Set hidden field to if a new password can be saved or not.
        hfldAcceptPwChange.val(isValid);
    }

//});
//}
//catch (err) {
//    console.log('ERROR: [Password Validation] ' + err.message + ' | ' + err);
//}
