async function sendMessage(chatId) {
    var senderId = getCookie("userId");
    var text = $('#chat-text').val();
    var hasFile = false;
    var fileId = null;
    var message = {
        "senderId": senderId,
        "chatId": chatId,
        "text": text,
        "hasFile": hasFile,
        "fileId": fileId
    };
    var messageString = JSON.stringify(message);
    hubConnection.invoke("SendMessage", messageString);
    $('#chat-text').val('');
    seenChat(chatId);
}

function deleteMessage() {

}

function seenChat(chatId) {
    var userChat = getUserChatFromArrayByChatId(activeChatId);
    if (userChat != null) {
        userChat.isActive = false;
    }
    var newUserChat = getUserChatFromArrayByChatId(chatId);
    newUserChat.isActive = true;
    newUserChat.notSeenMessagesNumber = 0;
    activeChatId = chatId;

    console.log('seenchat');
    var seenChatDto = {
        "userId": getCookie('userId'),
        "chatId": chatId
    };
    hubConnection.invoke("ChatSeen", JSON.stringify(seenChatDto));
}


async function addUserToChat() {
    var userToAdd = $('#user-to-add').val();
    var userId = null;
    friendUserChats.forEach(element => {
        element.chat.usersChats.forEach(el => {
            if (el.user.userName == userToAdd) {
                userId = el.user.id;
            }
        });
    });
    if (userId == null) {
        $('#user-is-not-friend').removeClass('d-none');
        return;
    }
    var data = {
        'chatId': activeChatId,
        'userId': userId
    };

    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    };
    var result = await fetchData('api/chats/addToChat', 'POST', headers, data);
    if (result.status == 200) {
        $('#user-added-to-chat').removeClass('d-none');
    } else if (result.status == 409) {
        $('#user-already-added-to-chat').removeClass('d-none');
    }
}

async function removeUserFromChat(chatId, userId = null) {
    var r = confirm('Do you really want to do this?');
    if (!r)
        return;
    console.log('in remove user from chat');
    if (userId == null) {
        userId = getCookie('userId');
    }
      var data = {
        'chatId': chatId,
        'userId': userId
    };

    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    };
    var result = await fetchData('api/chats/removeFromChat', 'POST', headers, data);
    if (result.status == 200) {
        if (userId == getCookie('userId')) {
            $('#chat-removed').removeClass('d-none');
        } else {
            $('#user-removed-from-chat').removeClass('d-none');
        }
    }
}

async function createChat() {
    var userId = getCookie('userId');
    var chatName = $('#chat-name').val();
    var data = {
        'name': chatName,
        'ownerId': userId
    }
    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    }
    var result = await fetchData('api/chats/create', 'POST', headers, data);
    if (result.status == 201) {
        console.log('chat created successfully');
        $('#chat-created-successfully').removeClass('d-none');
    } else {
        $('#chat-creation-failed').removeClass('d-none');
    }
}

async function deleteChat(chatId) {
    var r = confirm('Do you really want to delete this group chat?');
    if (!r)
        return;
    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    }
    var result = await fetchData('api/chats/delete/' + chatId, 'DELETE', headers);
    if (result.status == 200) {
        console.log('chat deleted successfully');
        $('#chat-deleted-successfully').removeClass('d-none');
    } else {
        $('#chat-deletion-failed').removeClass('d-none');
    }
}