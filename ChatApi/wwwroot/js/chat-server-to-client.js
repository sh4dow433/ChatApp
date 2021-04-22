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