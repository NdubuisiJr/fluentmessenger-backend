payWithPaystack();
function payWithPaystack() {
    let popup = document.getElementById("popup");
    let email = document.getElementById("user-email").innerHTML;
    let amount = document.getElementById("user-amount").innerHTML;
    let userFirstName = document.getElementById("user-firstname").innerHTML;
    let userLastName = document.getElementById("user-lastname").innerHTML;

    var handler = PaystackPop.setup({
       key: 'pk_live_33a45eee229353b983184458f32ac03cf1a65ec5',
       //key:'pk_test_319b3e4208401e104e038abb88c70462f35b0278',
	    email: '' + email,
        amount: 1 * Number.parseFloat(amount),
        currency: "NGN",
        ref: 'fluentMsger_' + Math.floor((Math.random() * 10000000000) + 1), 
        firstname: '' + userFirstName,
        lastname: '' + userLastName,
        callback: function () {
            popup.innerHTML = "Your transaction was succesful. Please close this page and return to the application!";
        },
        onClose: function () {
            popup.innerHTML = "Transaction Page was closed";
        }
    });
    handler.openIframe();
}

