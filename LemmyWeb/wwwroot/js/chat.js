"use strict";

function changedFocus(processed) {
    var li = document.getElementById("focused");
    li.classList = 'card mb-4';
    li.style.overflowX = "scroll";
    li.style.opacity = 0;
    li.style.transition = 'opacity 1s ease-in';
    //li.style.display = 'inline';
    //document.getElementById("msgList").prepend(li);
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
    let createdDate = new Date(processed.createdDate).toString();
    createdDate = createdDate.split('GMT')[0];
    let innerHtml = `           
  <div class="card-header" style="background-color:#43738e;color:white;padding:1em;">
      <div class="badge bg-secondary"><i class="fas fa-solid fa-group-arrows-rotate"></i> <span>${processed.communityName}</span></div>
          <div style='float:right;'><i class="fas fa-solid fa-comments"></i><span>${processed.commentNumber}</span></div>
      <div><i class="fas fa-solid fa-signs-post"></i> <a style="color:white;" href="${processed.postUrl}">${processed.title}</a></div>

  </div>
  <div class="card-body" style="background-color:white;">
        <div style="text-align:right; margin-right:0.5em"><strong>${processed.processedType === 1 ? 'Comment' : 'Post'} #${processed.id}</strong></div>
      <div style="text-align:right;margin-right:0.5em;" class="mr-1" title="${new Date(processed.createdDate)}"> ${createdDate}</div> 

            <div style="padding:0.2em;background-color:white"> <img src="${processed.avatarUrl || 'Lemmy_logo.svg.png'}" style="width:70px"/> 
            <strong>${processed.username} ${type}: </strong>
            <div style="padding:0.5em; margin-bottom:1em;">
                <span class="fas fa-quote-left fa-lg text-warning me-2"></span>
                ${processed.content ?? `<a href="${processed.postUrl}">${processed.title}</a>` }`;

    //if (processed.thumbnailUrl) {
    //    innerHtml += `<img src=${processed.thumbnailUrl}/>`
    //}
    let processedOn = new Date(processed.processedOn).toString();
    processedOn = processedOn.split('GMT')[0];
    innerHtml += `
                <span class="fas fa-quote-right fa-lg text-warning me-2" style="float:right;"></span>
            </div>                     
    </div>        
    <div class="p-2" style="background-color:white;border: 4px dashed ${processed.isReported ? 'darksalmon' : processed.failed ? '#ffe29b' : 'aliceblue'};"><strong>LemmyNanny Reported?${processed.viewedImages ? '<i class="fas fa-solid fa-images"></i>' : ''}</strong> ${processed.reason}</div>
        <div style="float:left;color:lightgrey;" data-toggle="tooltip" data-placement="top" title="${processedOn}">${processed.extraInfo}</div>`;

    if (processed.processedType === 1) {
        innerHtml += `<div> <a style="float:right;" href="${processed.url}">View comment on Lemmy</a></div>`;
    } else {
        innerHtml += `<div> <a style="float:right;" href="${processed.postUrl}">View post on Lemmy</a></div>`;
    }
    
    innerHtml += `</div>`;

    li.innerHTML = innerHtml;
   // time = 0;
    
    setTimeout(() => {
        li.style.opacity = '1';

    }, 1);
}

let checked = document.getElementById('stayCurrent').checked;
document.getElementById('stayCurrent').addEventListener('change', () => {
    checked = !checked;
})

function addListItem(processed) {
    var li = document.createElement("div");
    li.classList = 'listItem'
    li.style.cursor = 'pointer';
    li.style.marginRight = '1em';
    li.style.borderRadius = '10px';
    li.style.backgroundColor = 'white';
    li.style.textAlign = 'center';
    li.id = processed.id;
    li.style.transition = 'opacity 1.1s ease-in';
    li.style.opacity = 0;
    document.getElementById("msgList").prepend(li);
    time = 0;
    li.innerHTML = `<div style="background-color:#f2eeee">${new Date(processed.processedOn).toString().split(' ')[4].split(':').slice(0, 2).join(':').toString()}</div><div style="overflow: hidden;text-overflow: ellipsis;;text-align:center;border: 4px dashed ${processed.isReported ? 'darksalmon' : processed.failed ? '#ffe29b' : 'aliceblue'};margin-right:0.1em;">
        <div class="badge bg-secondary" style="overflow:hidden;"><i class="fas fa-solid fa-group-arrows-rotate"></i> <span>${processed.communityName}</span></div>
        <div style="text-align:center;">
            <div style="">
            <div>#${processed.id}</div>
                <div>
                    <i class="fas fa-solid ${processed.processedType === 1 ? 'fa-comment' : 'fa-signs-post'}"></i>
                </div>
                <div>
                ${processed.commentNumber}
                </div>
            </div>
            
        </div>
    </div>`;

    li.addEventListener('click', (ev) => {
        var a = document.getElementsByClassName('listItem');
        for (var i = 0; i < a.length; i++) {
            a[i].style.backgroundColor = 'white';
        }
        li.style.backgroundColor = '#fdd063';
        changedFocus(processed);
    });

    setTimeout(() => {
        li.style.opacity = '1';

    }, 1);
    //document.getElementById('lastProcessed').innerText = `Last updated: ${time}s ago.`;
}
let time = 0;
setInterval(() => {
    document.getElementById('lastProcessed').innerText = `Last updated: ${time++}s ago`;
}, 1000);

var connection = new signalR.HubConnectionBuilder().withUrl("/processed").build();
let paused = false;
let pausedQueue = [];
let pauseButton = document.getElementById("btnPause");
//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveProcessed", function (processed) {
    //console.log(processed);
    if (paused) {
        pausedQueue.push(processed);
        pauseButton.value = `Resume feed (${pausedQueue.length} new)`;
    } else {
        addListItem(processed);
        if (checked) {
            var a = document.getElementsByClassName('listItem');
            for (var i = 0; i < a.length; i++) {
                a[i].style.backgroundColor = 'white';
            }
            changedFocus(processed);
            document.getElementById(processed.id).style.backgroundColor = '#fdd063';
        }
        let list = document.getElementById("msgList");
        if (list.children.length > 50) {
            list.removeChild(list.lastElementChild);
        }
    }

});

connection.on("ReceivedInitial", function (processeds) {
    console.log("proceeds");
    var a = document.getElementsByClassName('listItem');

    for (var i = 0; i < processeds.length; i++) {
        addListItem(processeds[i]);
    }
    let showProc = processeds[processeds.length - 1];
    changedFocus(showProc);
    document.getElementById(showProc.id).style.backgroundColor = '#fdd063';

});

connection.on("ViewerCount", function (viewcount) {
    document.getElementById('numWatcher').innerHTML = `${viewcount - 1} sailors sailing wit ye`;
});

//connection.start().then(function () {
//    //document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

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


var startTimeout = null;
var retryTimeout = null;
var tryCount = 0;

async function start() {
    try {
        if (retryTimeout) {
            clearTimeout(retryTimeout);
        }
        startTimeout = null;
        await connection.start();
        console.assert(connection.state === signalR.HubConnectionState.Connected);

        if (isOnline()) {
            document.getElementById('status').classList = "badge badge-pill bg-success";
            document.getElementById('status').innerText = "online";
        }

    } catch (err) {
        retryTimeout = setTimeout(connectionRetryLogic, 1000);
    }
};

document.getElementById('status').addEventListener('click', function (e) {
    start();
});
function connectionRetryLogic() {
    if (isOnline())
        return;

    if (retryTimeout) {
        // allready retrying
    }

    if (startTimeout) {
        clearTimeout(startTimeout);
        tryCount--;
        console.log('cancelling startTimeout');
    }

    console.log('connection closed retrying');
    tryCount++;
    document.getElementById('status').classList = "badge badge-pill bg-warning";
    document.getElementById('status').innerText = "reconnect";
    if (tryCount > 5) {
        document.getElementById('status').classList = "badge badge-pill bg-danger";
        document.getElementById('status').innerText = "offline";
    } else {
        startTimeout = setTimeout(async () => await start(), 1);
    }

}

connection.onclose(async () => {
    //connectionRetryLogic();
    document.getElementById('status').classList = "badge badge-pill bg-danger";
    document.getElementById('status').innerText = "offline";
    connectionRetryLogic();
});

connection.onreconnecting(error => {
    document.getElementById('status').classList = "badge badge-pill bg-warning";
    document.getElementById('status').innerText = "reconnect";
    connectionRetryLogic();
});
connection.onco
connection.onreconnected(connectionId => {
    if (isOnline()) {
        document.getElementById('status').classList = "badge badge-pill bg-success";
        document.getElementById('status').innerText = "online";
    } else {
        console.log("reconnected but not")
        connectionRetryLogic();
    }
});
var isOnline = () => connection.state === signalR.HubConnectionState.Connected && navigator.onLine;

window.ononline = function () {
    connectionRetryLogic();
}

window.onoffline = function () {
    connectionRetryLogic();
}

start();