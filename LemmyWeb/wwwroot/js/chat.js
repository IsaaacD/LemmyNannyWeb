"use strict";

function addListItem(processed) {
    var li = document.createElement("li");
    document.getElementById("messagesList").prepend(li);
    let type = "post";
    switch (processed.processedType) {
        case 0:
            break;
        case 1:
            type = "comments on";
            break;
        case 1:
            type = "posts about";
            break;
    }
    li.innerHTML = `<div>
    <div style="background-color:grey;color:white;padding:1em;">
                    <div>Posted Date: ${processed.createdDate} </div>
                    <div>${processed.extraInfo}</div>
    </div>
                    <div style='border-radius:2px;border:1px solid navy;padding:2em;'>
                        <div style="border:1px solid lightgrey;padding:0.2em;"> <img src="${processed.avatarUrl || 'Lemmy_logo.svg.png'}" style="width:70px"/> 
                                <strong>${processed.username} ${type} <a href="${processed.postUrl}">${processed.title || ''}</a>:</strong>
                                <div style="background-color:#f7f7f1;padding:0.5em;">${processed.content}</div>
                                
                        </div>
                        <div class="p-2" style="background-color:#f7f7f1;"><strong>Reported?</strong> ${processed.reason}</div>
                        <div> <a style="float:right;" href="${processed.url}">Link on Lemmy</a></div>
                        
                    </div>
                    <div style="margin-bottom:1em;">Processed on ${processed.processedOn}</div>
                    </div>`;
    
}
var connection = new signalR.HubConnectionBuilder().withUrl("/processed").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveProcessed", function (processed) {
    addListItem(processed);
});

connection.on("ReceivedInitial", function (processeds) {
    console.log("proceeds");
    for (var i = 0; i < processeds.length; i++) {
        addListItem(processeds[i]);
    }
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
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