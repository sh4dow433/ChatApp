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
        }
    }
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
    if (chat.owner.id == getCookie("userId")) {
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
    var r = confirm('Do you really want to delete this group chat?');
    if (!r)
        return;
    if (refresh) {
        await getChats();
        updateChatsUI();
        if (activeChatId == null || activeChatName == null) {
            var chat = groupUserChats[0]?.chat;
            if (chat != null) {
                loadChat(chat.id, chat.name);
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
                }
                $('.chat-name').html("No chats found");
                $('.message-list').html("<p class='text-center text-secondary'>Add a friend to start chatting</p>")
            }
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