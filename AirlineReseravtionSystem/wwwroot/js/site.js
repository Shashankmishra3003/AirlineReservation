// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ----< Performing validation on the number of seats selcted >------

function ValidatePetSelection(numberOfTickets) {
    var checkboxes = document.getElementsByName("seatSelection");
    var CheckedItems = 0;
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked)
            CheckedItems++;
    }
    if (CheckedItems > numberOfTickets) {
        alert("Selection Exceeding the Number of tickets");
        return false;
    }
}  