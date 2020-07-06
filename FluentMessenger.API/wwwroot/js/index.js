payWithPaystack();
function payWithPaystack() {
    let popup = document.getElementById("popup");
    let userId = document.getElementById("user-id").innerHTML;
    let email = document.getElementById("user-email").innerHTML;
    let amount = document.getElementById("user-amount").innerHTML;
    let userFirstName = document.getElementById("user-firstname").innerHTML;
    let userLastName = document.getElementById("user-lastname").innerHTML;

    var handler = PaystackPop.setup({
        key: 'pk_test_319b3e4208401e104e038abb88c70462f35b0278',
        email: '' + email,
        amount: 1 * Number.parseFloat(amount),
        currency: "NGN",
        ref: '' + Math.floor((Math.random() * 10000000000) + 1), 
        firstname: '' + userFirstName,
        lastname: '' + userLastName,
        metadata: {
            custom_fields: [
                {
                    display_name: "Mobile Number",
                    variable_name: "mobile_number",
                    value: "+2348012345678"
                }
            ]
        },
        callback: function (response) {
            let responseData = new ResponseObject(response.reference, userId, email);
            PostRequest(responseData);
            popup.innerHTML = "Your transaction was succesful. Please close this page and return to the application!";
        },
        onClose: function () {
            popup.innerHTML = "Transaction Page was closed";
        }
    });
    handler.openIframe();
}

function PostRequest(object) {
    const httpRequest = new XMLHttpRequest();
    httpRequest.open("POST", "https://fluentmessengerapi.herokuapp.com/api/payment");
    httpRequest.setRequestHeader("Content-Type", "application/json");
    httpRequest.send(JSON.stringify(object));
}

function ResponseObject(ref, userId, email) {
    this.transactionReference = ref;
    this.email = email;
    this.UserId = userId;
}
