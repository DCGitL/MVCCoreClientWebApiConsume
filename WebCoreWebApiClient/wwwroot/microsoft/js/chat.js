"use strict"

"use strict";

$(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    //Disable send button until connection is established
    $("#sendButton").disabled = true;

    connection.on("ReceiveMessage", function (user, message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + " says " + msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.start().then(function () {
        $("#sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $("#sendButton").on("click", function (event) {
        var user = $("#userInput").val();
        var message = $("#messageInput").val();
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });


});
