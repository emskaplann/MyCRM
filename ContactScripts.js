var Sdk = window.Sdk || {};
(
    function () {
        this.formOnLoad = function (executionContext) {
            // To play around with the form.
            var formContext = executionContext.getFormContext();
            // Get 'firstname' field's data.
            var firstName = formContext.getAttribute("firstname").getValue();
            alert("Hello, " + firstName + "!");
        }
    }
).call(Sdk);