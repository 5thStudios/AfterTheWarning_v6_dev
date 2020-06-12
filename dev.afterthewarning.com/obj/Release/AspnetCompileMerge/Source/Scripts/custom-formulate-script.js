/* This file listens for events dispatched by Formulate and responds to them appropriately. */
//http://www.formulate.rocks/plain-javascript/render-form



// Wait until the markup has rendered
setTimeout(function () {

    // Variables.
    let i, wrapper, wrappers = document.querySelectorAll(".formulate-wrapper");

    // Process each element that is wrapping a Formulate form.
    for (i = 0; i < wrappers.length; i++) {
        wrapper = wrappers[i];
        (function (wrapper) {

            // Variables.
            let validationListElement;

            // On form submit, remove the form and display a success message. You can do what you like here
            // (e.g., show a hidden element).
            wrapper.addEventListener("formulate form: submit: success", function (e) {
                let form = e.target;

                var thankyouNode = document.createElement("blockquote");
                thankyouNode.innerHTML = "<p class='formThankyouMsg'>Thank you.<br />Your message has been delivered.<br />We will get back to you as soon as possible.</p>";

                form.parentNode.replaceChild(thankyouNode, form);
                if (validationListElement) {
                    validationListElement.parentNode.removeChild(validationListElement);
                }
                validationListElement = null;

                //Transfer page to the Thankyou page.
                //window.location = "/contact-us/thank-you-for-contacting-us/";
            });

            // When there is an error, show an alert dialog. Feel free to change this to something
            // that makes more sense for your project.
            wrapper.addEventListener("formulate form: submit: failure", function () {
                alert("Unknown error. Please try again.");
            });

            // When there are validation errors, add a list of error messages to the bottom of the
            // form. If you remove this, the error messages will still be shown inline below each field.
            wrapper.addEventListener("formulate: submit: validation errors", function (e) {
                //let i, message, messages = e.detail.messages, form = e.target, listElement, itemElement;
                //listElement = document.createElement("ul");
                //listElement.classList.add("formulate__validation-summary");
                //for (i = 0; i < messages.length; i++) {
                //    message = messages[i];
                //    itemElement = document.createElement("li");
                //    itemElement.classList.add("formulate__validation-summary__error");
                //    itemElement.appendChild(document.createTextNode(message));
                //    listElement.appendChild(itemElement);
                //}
                //if (validationListElement) {
                //    validationListElement.parentNode.replaceChild(listElement, validationListElement);
                //}
                //validationListElement = listElement;
                //form.parentNode.appendChild(listElement);
            });

        })(wrapper);
    }


    try {
        //Run only if formulate form exists
        if ($('.formulate__form').length > 0) {
            //Instantiate variables
            var rows = $('.formulate__row');
            var cells = $('.formulate__cell');
            var textarea = $('.formulate__form textarea');
            var submitBtn = $('.formulate__form button[type=submit]');
            var inputs = $('.formulate__form input');

            //<div class="grid-x grid-padding-x">
            rows.addClass(" grid-x grid-padding-x");
            textarea.attr('rows', '10');
            
            jQuery.each(cells, function () {
                $(this).addClass(" cell");

                if ($(this).hasClass("formulate__cell--1-columns")) { $(this).addClass(" medium-2"); }
                if ($(this).hasClass("formulate__cell--2-columns")) { $(this).addClass(" medium-4"); }
                if ($(this).hasClass("formulate__cell--3-columns")) { $(this).addClass(" medium-6"); }
                if ($(this).hasClass("formulate__cell--4-columns")) { $(this).addClass(" medium-8"); }
                if ($(this).hasClass("formulate__cell--5-columns")) { $(this).addClass(" medium-10"); }
                if ($(this).hasClass("formulate__cell--6-columns")) { $(this).addClass(" medium-12"); }
                if ($(this).hasClass("formulate__cell--7-columns")) { $(this).addClass(" medium-14"); }
                if ($(this).hasClass("formulate__cell--8-columns")) { $(this).addClass(" medium-16"); }
                if ($(this).hasClass("formulate__cell--9-columns")) { $(this).addClass(" medium-18"); }
                if ($(this).hasClass("formulate__cell--10-columns")) { $(this).addClass(" medium-20"); }
                if ($(this).hasClass("formulate__cell--11-columns")) { $(this).addClass(" medium-22"); }
                if ($(this).hasClass("formulate__cell--12-columns")) { $(this).addClass(" medium-24"); }
            });

            
            submitBtn.addClass(' button large expanded primary');
            inputs.removeAttr('placeholder');
            textarea.removeAttr('placeholder');
        }
    }
    catch (err) {
        console.log('ERROR: [custom-formulate-script] ' + err.message);
    }

}, 0);
