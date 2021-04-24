$(function () {
    loadPage();
});

async function loadPage() {
    $("main").empty();
    if (isUserLoggedIn()) {
        $("nav").load("pages/logged-in-nav.html");
        $("main").load("pages/application.html");
        // 
        await new Promise(resolve => setTimeout(resolve, 100));
        $("#my-account-name").text(getCookie("userName"));
    } else {
        $("nav").load("pages/not-logged-in-nav.html");
        $("main").load("pages/login.html");
    }
}

function loadLoginPage() {
    $("main").empty();
    $("main").load("pages/login.html");
    return false;
}
function loadRegisterPage() {
    $("main").empty();
    $("main").load("pages/register.html");
    return false;
}

function loadSettingsPage() {

}
async function loadApp() {
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
    } else {
        loadChat(activeChatId, activeChatName);
    }
}

function openChat(chatId, name) {
    loadChat(chatId, name);
    seenChat(chatId);
    updateChatsUI();
    return false;
}