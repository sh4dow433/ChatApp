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