function sendMessage(chatId) {
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
    var userChat = getUserChatFromArrayById(activeChatId);
    if (userChat != null) {
        userChat.isActive = false;
    }
    var newUserChat = getUserChatFromArrayById(chatId);
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


function addUserToChat() {

}

function removeUserFromChat() {

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

function deleteChat() {

}