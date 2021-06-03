hubConnection.on('ReceiveMessage', async function (messageString) {
    console.log('receive msg is called');
    var message = JSON.parse(messageString);
    addMessageToChat(message);
    if (message.chat.isGroupChat == true && chatInView == message.chat.id) {
        loadChat(message.chat.id, message.chat.name);
    } else {
        var name = '';
        message.chat.usersChats.forEach(element => {
            if (element.user.id != getCookie('userId')) {
                name = element.user.userName;
            }
        });
        if (chatInView == message.chat.id) {
            loadChat(message.chat.id, name);
        } else {
            updateChatsUI();
        }
    }
});

hubConnection.on('MessageDeleted', function (messageId) {
    console.log('message deleted is called');
    removeMessageFromChat(messageId);
    loadChat(activeChatId, activeChatName);
});


hubConnection.on("ChatSeen", function (chatId, userId, lastSeen) {
    chatSeenByUser(chatId, userId, lastSeen);
    if (chatInView == chatId)
        loadChat(chatInView);
});

hubConnection.on("FriendOnline", function (userId) {
    console.log('friendOnline');
    friendOnline(userId);
    updateChatsUI();
});

hubConnection.on("FriendOffline", function (userId) {
    console.log('friendOffline');
    friendOffline(userId);
    updateChatsUI();
});

hubConnection.on("NewChatCreated", async function (chat) {
    chat = JSON.parse(chat);
    console.log('new chat created');
    await getChats();
    if (chat.isGroupChat && chat.owner.id == getCookie("userId")) {
        groupUserChats.forEach(element => {
            if (element.chat.id == chat.id) {
                loadChat(chat.id, chat.name);
                seenChat(chat.id);
                return;
            }
        });
    }
    updateChatsUI();
});

hubConnection.on("ChatDeleted", async function (chatId, refresh) {
    console.log('chat deleted (sr)');
    if (refresh) {
        await getChats();
        updateChatsUI();
        if (activeChatId == null || activeChatName == null) {
            var chat = groupUserChats[0]?.chat;
            if (chat != null) {
                loadChat(chat.id, chat.name);
                seenChat(chat.id);
            } else {
                chat = friendUserChats[0]?.chat;
                if (chat != null) {
                    var friendName;
                    chat.usersChats.forEach(element => {
                        if (element.user.id != getCookie('userId')) {
                            friendName = element.user.userName
                        }
                    });
                    loadChat(chat.id, friendName);
                    seenChat(chat.id);
                }
                $('.chat-name').html("No chats found");
                $('.message-list').html("<p class='text-center text-secondary'>Add a friend to start chatting</p>")
            }
        } else {
            loadChat(activeChatId, activeChatName);
        }
    } else {
        removeChatFromArray(chatId);
        updateChatsUI();
    }
});

hubConnection.on('UserAddedToChat', function (userChatDtoString, chatId) {
    console.log('in user added to chat func');
    var userChat = JSON.parse(userChatDtoString);
    var chat = getChatFromArrayById(chatId);
    chat.usersChats.push(userChat);
});


hubConnection.on('UserRemovedFromChat', function (userId, chatId) {
    console.log('in user remove from chat (sr)');
    var chat = getChatFromArrayById(chatId);
    chat.usersChats.forEach(element => {
        if (element.user.id == userId) {
            chat.usersChats = arrayRemove(chat.usersChats, element);
            console.log('user removed')
        }
    });
});

hubConnection.on('Error', function(error) {
    console.log('general-error:', error);
    $('#general-error').removeClass('d-none');
    $('#general-error-text').text(error);
});