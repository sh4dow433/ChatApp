var groupUserChats = []
var friendUserChats = []
var activeChatId;
var activeChatName;

var chatInView;
// get chat from api
async function getChats() {
    groupUserChats = []
    friendUserChats = []
    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    }
    var response = await fetchData("api/account/user/full/" + getCookie("userId"), "GET", headers);
    console.log("get user");
    console.log(response);
    if (response.status == 200) {
        response.data.usersChats.forEach(userChat => {
            addUserChatToArray(userChat);
        });
    }
}
//get userChat from list
function getUserChatFromArrayByChatId(chatId) {
    var userChat;
    groupUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            userChat = element;
            return;
        }
    });
    friendUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            userChat = element;
            return;
        }
    });
    return userChat;
}
//get chats from the chat list
function getChatFromArrayById(chatId) {
    var chat;
    groupUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            chat = element.chat;
            return;
        }
    });
    friendUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            chat = element.chat;
            return;
        }
    });
    return chat;
}
function addUserChatToArray(userChat) {
    if (userChat.chat.isGroupChat) {
        groupUserChats.push(userChat);
        groupUserChats.sort((a, b) => (a.name > b.name) ? 1 : -1);
    } else {
        friendUserChats.push(userChat);
        friendUserChats.sort((a, b) => (a.name > b.name) ? 1 : -1);
    }
}

function removeChatFromArray(chatId) {
    groupUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            groupUserChats = arrayRemove(groupUserChats, element);
            return;
        }
    });
    friendUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            friendUserChats = arrayRemove(friendUserChats, element);
            return;
        }
    });
}

function addUserToChatArray(userChat, chatId) {
    groupUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            element.chat.usersChats.push(userChat);
            return;
        }
    });
}

function removeUserFromChatArray(userChatId) {
    console.log('in remove user from chat array');
    groupUserChats.forEach(element => {
        element.chat.usersChats.forEach(userChat => {
            if (userChat.id == userChatId) {
                usersChats = arrayRemove(usersChats, userChat);
                console.log('user removed');
                return;
            }
        });
    });
}

function addMessageToChat(message) {
    console.log('add message');
    console.log(groupUserChats);
    console.log(friendUserChats);
    console.log(message);
    groupUserChats.forEach(element => {
        if (element.chat.id == message.chat.id) {
            element.notSeenMessagesNumber++;
            element.chat.messages.push(message);
            return;
        }
    });
    friendUserChats.forEach(element => {
        if (element.chat.id == message.chat.id) {
            element.notSeenMessagesNumber++;
            element.chat.messages.push(message);
            return;
        }
    });
}

function removeMessageFromChat(messageId) {
    groupUserChats.forEach(element => {
        element.chat.messages.forEach(message => {
            if (message.id == messageId) {
                message.text = "";
                message.isRemoved = true;
                return;
            }
        });
    });
    friendUserChats.forEach(element => {
        element.chat.messages.forEach(message => {
            if (message.id == messageId) {
                message.text = "";
                message.isRemoved = true;
                return;
            }
        });
    });
}
function chatSeenByUser(chatId, userId, lastSeen) {
    groupUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            element.chat.usersChats.forEach(el => {
                if (el.user.id == userId) {
                    el.lastSeen = lastSeen;
                }
            });
        }
    });
    friendUserChats.forEach(element => {
        if (element.chat.id == chatId) {
            element.chat.usersChats.forEach(el => {
                if (el.user.id == userId) {
                    el.lastSeen = lastSeen;
                }
            });
        }
    });
}

function friendOnline(userId) {
    console.log(userId);
    friendUserChats.forEach(element => {
        element.chat.usersChats.forEach(el => {
            if (el.user.id == userId) {
                console.log('f-online in if')
                el.user.isActive = true;
            }
        });
    });
}

function friendOffline(userId) {
    friendUserChats.forEach(element => {
        element.chat.usersChats.forEach(el => {
            if (el.user.id == userId) {
                el.user.isActive = false;
            }
        });
    });
}

function chatNameUpdated() {
    throw new Error("Not implemented");
}


function updateChatsUI() {
    console.log("updateUi");
    var friendsHtml = ``;
    var groupsHtml = ``;

    groupUserChats.forEach(userChat => {
        var id = userChat.chat.id;
        var chatName = userChat.chat.name;
        var unreadMessages = userChat.notSeenMessagesNumber == 0 ? "" : userChat.notSeenMessagesNumber;
        var chatActive = "";

        /// get image first
        var image = "img/cover.jpg"

        var nameColour = 'text-secondary';
        if (userChat.isActive) {
            console.log('chat active')
            chatActive = 'bg-info';
            nameColour = 'text-white'
            activeChatId = id;
            activeChatName = chatName;
        }
        groupsHtml += `
        <a href="#" onclick="return openChat(`+ id + `, '` + chatName + `');"
        class="list-group-item ` + chatActive + ` list-group-item-action d-flex justify-content-between align-items-center">
        <div class="ml-0 pl-0">
            <img src="` + image + `" alt="" class="img-fluid m-0 p-0" width=30>
            <p class="mt-2 ` + nameColour + `" style="display: inline;">` + chatName + `</p>
        </div>
        <span class="badge badge-info badge-pill">`+ unreadMessages + `</span>
        </a>
        `;
    });

    friendUserChats.forEach(userChat => {
        console.log(userChat);
        var id = userChat.chat.id;
        var name = '';
        var isActive = false;
        userChat.chat.usersChats.forEach(uc => {
            if (uc.user.userName != getCookie("userName"))
                name = uc.user.userName;
            isActive = uc.user.isActive;
        });
        var unreadMessages = userChat.notSeenMessagesNumber == 0 ? "" : userChat.notSeenMessagesNumber;
        var chatActive = "";

        /// get image first
        var image = "/img/cover.jpg";

        var nameColour = 'text-secondary';
        if (userChat.isActive) {
            chatActive = 'bg-info';
            nameColour = 'text-white';
            activeChatId = id;
            activeChatName = name;
        }
        if (isActive == true) {
            nameColour = 'text-primary font-weight-bold';
        }
        friendsHtml += `
        <a href="#" onclick="return openChat(`+ id + `, '` + name + `');"
        class="list-group-item ` + chatActive + ` list-group-item-action d-flex justify-content-between align-items-center">
        <div class="ml-0 pl-0">
            <img src="` + image + `" alt="" class="img-fluid m-0 p-0" width=30>
            <p class="mt-2 ` + nameColour + `" style="display: inline;">` + name + `</p>
        </div>
        <span class="badge badge-info badge-pill">`+ unreadMessages + `</span>
        </a>
        `;
    });
    $(".friends").empty();
    $(".friends").html(friendsHtml);
    $(".groups").empty();
    $(".groups").html(groupsHtml);
}

// HELPER FUNCTIONS
function arrayRemove(arr, value) {
    console.log('in array remove');
    return arr.filter(function (ele) {
        return ele != value;
    });
}
// ^^
function buildMessage(id, name, text, image, date, seenByString, isMine = false, isRemoved) {
    var result;
    if (!isMine) {
        if (isRemoved == true) {
            text = 'Message was removed.';
        }
        result = `
        <div class="message-box px-5 py-2 row m-0">
            <div class="mr-3 message-profile-pic">
                <img src="` + image + `" alt="" class="img-fluid m-0 p-0">
            </div>
            <div class="message bg-light border">
            <button class="btn container-fluid bg-light text-decoration-none" data-toggle="collapse" data-target="#collapse-` + id + `" aria-expanded="false" aria-controls="collapse-` + id + `">
                <div class="row px-4">
                    <div class="mt-2  text-info"><b>` + name + `</b></div>
                    <div class="ml-auto mt-2  text-right text-secondary">` + new Date(date).toLocaleString() + `</div>
                </div>
                <div class="row px-4 text-dark">
                    <p>` + text + `</p>
                    <small class="collapse ml-auto mt-5 text-right text-secondary"  id="collapse-` + id + `">Seen by: ` + seenByString + `</small>
                </div>
            </button>
            </div>
        </div>
        `;
    } else {
        if (isRemoved == true) {
            text = 'Message was removed.';
        }
        result = ` 
        <div class="ml-auto message-box px-5 py-2 row m-0">
        <div class="message bg-light border">
        <button class="btn container-fluid bg-light text-decoration-none" data-toggle="collapse" data-target="#collapse-` + id + `" aria-expanded="false" aria-controls="collapse-` + id + `">
           <div class="row px-4">
                <div class="mt-2 text-secondary">` + new Date(date).toLocaleString() + `</div>
                <div class="ml-auto mt-2  text-right  text-success"><b>` + name + `</b></div>
            </div>
            <div class="row px-4 text-dark">
                <p>` + text + `</p>
                <small class="collapse ml-auto mt-5 text-right text-secondary" id="collapse-` + id + `" >Seen by: ` + seenByString + `</small>
            </div>
        </button>
        </div>
        <div class="ml-3 message-profile-pic">
            <img src="` + image + `" alt="" class="img-fluid m-0 p-0">
        </div>
    </div>
    `;
    }
    return result;
}

function buildSeenBar(elements) {
    console.log('in build seen bar')
    var result = `           
        <div class="ml-auto mt-1 seen-bar border-info border-top text-secondary text-right mr-5 px-3">
    `;
    elements.forEach(element => {
        // var profilePic = element.user.profilePic
        //get profile pic
        var profilePicString = '/img/cover.jpg';
        result += `
        <img src="` + profilePicString + `" alt="" class="img-fluid m-0 p-0" width="30" title="` + element.user.userName + `">
        `;
    });
    result += `</div>`;
    return result;
}



function loadChat(chatId, name = "") {

    chatInView = chatId;
    var chat = getChatFromArrayById(chatId);
    $(".send-message-btn").attr('onclick', 'sendMessage(' + chatId + ')');

    console.log("loadChat1");
    console.log(chat);
    var dropdownSettings = $('#chat-setting-dropdown');
    var chat = getChatFromArrayById(chatId);

    // drop down menu with settings
    var dropdownHtml = ``;

    if (chat.isGroupChat) {
        var active = 'd-none';
        if (chat.owner.id == getCookie('userId')) {
            active = '';
        }
        dropdownHtml = `
        <a class="dropdown-item text-center" href="#" onclick="viewChatParticipants(` + chatId + `)">View participants</a>
        <a class="dropdown-item  text-center" href="#" data-toggle="modal" data-target="#add-user-to-chat-modal">Add person</a>
        <div class="dropdown-divider"></div>
        <a class="dropdown-item text-center ` + active + `" href="#" onclick="changeChatName(` + chatId + `)">Change name</a>
        <a class="dropdown-item text-danger text-center" href="#" onclick="removeUserFromChat(` + chatId + `)">Leave group</a>
        <a class="dropdown-item text-danger text-center ` + active + `" href="#" onclick="deleteChat(` + chatId + `);">Delete chat</a>
        `;

    } else {
        var friendsId;
        chat.usersChats.forEach(element => {
            if (element.user.usename != name) {
                friendsId = element.user.id;
            }
        });
        dropdownHtml = `
        <a class="dropdown-item text-center" href="#" onclick="viewProfile('` + friendsId + `')">View Profile</a>
        <div class="dropdown-divider"></div>
        <a class="dropdown-item text-danger text-center" href="#" onclick="removeFriend('` + friendsId + `')">Remove friend</a>
        `;
    }
    dropdownSettings.html(dropdownHtml);
    // ^^^


    var messageList = $(".message-list");
    var chatName = $('.chat-name');

    if (name != '') {
        chatName.html("&nbsp; " + name);
    }
    var dict = [];
    var messages = chat.messages;
    if (messages.length == 0) {
        console.log("No msgs");
        messageList.html('<p class="m-0 mt-3 p-0 text-secondary text-center">No messages found. Say HI!</p>');
        return;
    }
    var userChats = chat.usersChats;
    messageList.html("");
    messages.forEach(msg => {
        dict.push({
            date: msg.date,
            isMessage: true,
            message: msg
        });
    });
    userChats.forEach(userChat => {
        dict.push({
            date: userChat.lastSeen,
            isMessage: false,
            user: userChat.user
        });
    });
    dict.sort(function (a, b) {
        return new Date(a.date) - new Date(b.date);
    });
    console.log(dict);
    var htmlResult = `
        <div class="row m-0 p-0">
            <button class="mx-auto btn btn-small btn-outline-info mt-1">Load more</button>
        </div>  
    `;

    var seenBarElements = [];
    dict.forEach(element => {
        var isMine = false;
        if (element.isMessage) {
            if (seenBarElements.length > 0) {
                htmlResult += buildSeenBar(seenBarElements);
                seenBarElements = [];
            }
            // get profile pic
            var profilePicString = "/img/cover.jpg";
            // var profilePic = await fetchData();
            var seenByString = '';
            dict.forEach(el => {
                if (el.isMessage == false && el.date > element.date) {
                    seenByString += " " + el.user.userName;
                }
            });
            if (seenByString == '')
                seenByString = 'None';
            if (element.message.sender.id == getCookie("userId")) {
                console.log()
                isMine = true;
            }
            htmlResult += buildMessage(element.message.id, element.message.sender.userName, element.message.text, profilePicString, element.date, seenByString, isMine, element.message.isRemoved)
        } else {
            seenBarElements.push(element);
        }
    });
    if (seenBarElements.length > 0) {
        htmlResult += buildSeenBar(seenBarElements);
    }
    messageList.html(htmlResult);
    messageList.scrollTop(messageList[0].scrollHeight);
}

function viewChatParticipants(chatId) {

    $('#view-chat-participants-modal').modal();
    $('#view-chat-participants-modal').on('shown.bs.modal', function () {
        var chat = getChatFromArrayById(chatId);
        var htmlCode = ``;
        chat.usersChats.forEach(element => {
            var user = element.user
            var textColor = 'text-secondary';
            if (user.id == chat.owner.id) {
                textColor = 'text-success';
            }
                    
            htmlCode += `
                <a href="#" class="list-group-item list-group-item-action `+textColor+`" onclick="viewProfile('`+ user.id + `')">` + user.userName + `</a>
            `;
        });
        $('#list-of-participants').html(htmlCode);
    });
}
