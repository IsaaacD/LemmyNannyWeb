"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/processed").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveProcessed", function (processed) {
    var li = document.createElement("li");
    document.getElementById("messagesList").prepend(li);
    let type = "post";
    switch (processed.processedType) {
        case 0:
            break;
        case 1:
            type = "comments";
            break;
        case 1:
            type = "posts";
            break;
    }
    li.innerHTML = `<div><img src="${processed.avatarUrl}" style="width:100px"/> ${processed.username} ${type} ${processed.title} ${processed.content}</div><div>${processed.isReported ? 'Reported': 'Not reported'} : Reason ${processed.reason}</div><div><a href="${processed.url}">permaink</a></div>`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});