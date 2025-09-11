"use strict";

function addListItem(processed) {
    var li = document.createElement("li");
    
    li.style.overflowX = "scroll";
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
    li.innerHTML = `<div id="${processed.id}" style="width:100%;transition:transform 1s ease-in;transform:scaleY(0);overflow: hidden;display:inline-block;position:relative;margin-bottom:2em;">
    <div style="background-color:#43738e;color:white;padding:1em;">
                 
                    <div>${processed.extraInfo}</div>
  
                    </div>

                    <div style="background-color:white;">
                    <div style='border-radius:2px;border:1px solid navy;padding:2em;'>
                                       <div style="float:right;margin-right:0.5em;" class="mr-1"> Posted: ${processed.createdDate} (UTC)</div> 
                            
                        
                        <div style="border:1px solid lightgrey;padding:0.2em;background-color:#fad371"> <img src="${processed.avatarUrl || 'Lemmy_logo.svg.png'}" style="width:70px"/> 
                                <strong>${processed.username} ${type} <a href="${processed.postUrl}">${processed.title || ''}</a>:</strong>
                                <div style="padding:0.5em;">${processed.content}</div>
                               
                        </div>
                        
                        <div class="p-2" style="background-color:#f7f7f1;"><strong>LemmyNanny Reported?</strong> ${processed.reason}</div>
                        <div> <a style="float:right;" href="${processed.url}">Link on Lemmy</a></div>
                        
                    </div>
                    </div>
                    </div>
                    `;
    time = 0;
    document.getElementById('lastProcessed').innerText = `Last updated: ${time}s ago.`;
    setTimeout(() => {
        li.firstChild.style.transform = 'scaleY(1)';

    }, 10);
}

setInterval(() => {
    document.getElementById('lastProcessed').innerText = `Last updated: ${++time}s ago.`;
}, 1000);
let time = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/processed").build();
let paused = false;
let pausedQueue = [];
let pauseButton = document.getElementById("btnPause");
//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveProcessed", function (processed) {
    if (paused) {
        pausedQueue.push(processed);
        pauseButton.value = `Resume feed (${pausedQueue.length} new)`;
    } else {
        addListItem(processed);
        let list = document.getElementById("messagesList");
        if (list.children.length > 50) {
            list.removeChild(list.lastElementChild);
        }
    }

});

connection.on("ReceivedInitial", function (processeds) {
    console.log("proceeds");
    for (var i = 0; i < processeds.length; i++) {
        addListItem(processeds[i]);
    }
});

connection.on("ViewerCount", function (viewcount) {
    document.getElementById('numWatcher').innerText = `(${viewcount} sailors sailing wit ye.)`
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

pauseButton.addEventListener("click", function (event) {
    paused = !paused;
    if (!paused) {
        //process queue
        for (var i = 0; i < pausedQueue.length; i++) {
            addListItem(pausedQueue[i]);
        }
        pausedQueue = [];
        pauseButton.value = `Pause feed`;
    } else {
        pauseButton.value = `Resume feed`;
    }
    event.preventDefault();
});