"use strict";

function addListItem(processed) {
    var li = document.createElement("div");
    li.classList = 'card mb-4';
    li.style.overflowX = "scroll";
    li.style.opacity = 0;
    li.style.transition = 'opacity 0.5s ease';
    //li.style.display = 'inline';
    document.getElementById("msgList").prepend(li);
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
    let innerHtml =  `           
  <div class="card-header" style="background-color:#43738e;color:white;padding:1em;">
      <a style="color:white;float:right;" href="${processed.postUrl}">${processed.extraInfo} (click for post)</a>
  </div>
  <div class="card-body" style="background-color:white;">
      <div style="text-align:right;margin-right:0.5em;" class="mr-1"> Posted: ${processed.createdDate} (UTC)</div> 
            <div style="padding:0.2em;background-color:white"> <img src="${processed.avatarUrl || 'Lemmy_logo.svg.png'}" style="width:70px"/> 
            <strong>${processed.username} ${type}: </strong>
            <div style="padding:0.5em; margin-bottom:1em;">
                <span class="fas fa-quote-left fa-lg text-warning me-2"></span>
                ${processed.content}
                <span class="fas fa-quote-right fa-lg text-warning me-2" style="float:right;"></span>
            </div>
                               
    </div>
                        
    <div class="p-2" style="background-color:white;border: 4px dashed aliceblue;"><strong>LemmyNanny Reported?</strong> ${processed.reason}</div>`

    if (processed.processedType === 1) {
        innerHtml += `<div> <a style="float:right;" href="${processed.url}">View comment on Lemmy</a></div>`;
    }
    
    innerHtml += `</div>`;

    li.innerHTML = innerHtml;
    time = 0;
    document.getElementById('lastProcessed').innerText = `Last updated: ${time}s ago.`;
    setTimeout(() => {
        li.style.opacity = '1';

    }, 1);
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
    document.getElementById('numWatcher').innerText = `(${viewcount-1} sailors sailing wit ye.)`
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