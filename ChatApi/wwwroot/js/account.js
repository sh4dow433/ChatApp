
function isUserLoggedIn() {
    if (checkStr(getCookie("accessToken")) && checkStr(getCookie("userId")) && checkStr(getCookie("userName")))
        return true;
    return false;
}
function checkStr(str) {
    return (!str || str.length === 0) == false
}

async function login1(e) {
    if (e != null) {
        e.preventDefault();
    }
    var name = $("#name").val();
    var password = $("#password").val();
    var rememberMe = $('#remember-me').is(":checked") ? true : false;

    var headers = {
        'Content-Type': 'application/json'
    }

    var data = {
        "UserName": name,
        "Password": password,
        "StayLoggedIn": rememberMe
    }

    console.log('Data:');
    console.log(data);

    var response = await fetchData("api/account/login", "POST", headers, data)
    console.log("Raspuns:");
    console.log(response);
    if (response.status == 200) {
        var token = response.data.accessToken
        var tokenJSON = parseJwt(token);

        if (tokenJSON.isPersistent == true) {
            setCookie("accessToken", token, 3);
            setCookie("userId", tokenJSON.sub, 3);
            setCookie("userName", tokenJSON.unique_name, 3);
        } else {
            setCookie("accessToken", token, 0.125);
            setCookie("userId", tokenJSON.sub, 0.125);
            setCookie("userName", tokenJSON.unique_name, 0.125);
        }
        $("nav").load("pages/logged-in-nav.html");
        $("main").load("pages/application.html");
        await new Promise(resolve => setTimeout(resolve, 100));
        $("#my-account-name").text(getCookie("userName"));
    } else {
        $("#login-failed").removeClass("d-none");
    }
    // 
    return false;
}
function parseJwt(token) {
    return JSON.parse(atob(token.split('.')[1]));
};
async function register(e) {
    if (e != null) {
        e.preventDefault();
    }
    var name = $("#name").val();
    var email = $("#email").val();
    var password = $("#password").val();
    var confirmPassword = $("#password-confirmation").val();

    var headers = {
        'Content-Type': 'application/json'
    }
    var data = {
        "UserName": name,
        "Email": email,
        "Password": password,
        "PasswordConfirmation": confirmPassword
    }

    console.log("Data:")
    console.log(data);

    var response = await fetchData("api/account/register", "POST", headers, data)
    if (response.status == 201) {
        $("main").load("pages/login.html");
        $("#account-created-successfully").removeClass("d-none");
    }
    console.log("Raspuns:");
    console.log(response);
    return false;
}

function logout() {
    stopConnection();
    deleteCookie("accessToken", "/", "");
    deleteCookie("userName", "/", "");
    deleteCookie("userId", "/", "");

    $("nav").load("pages/not-logged-in-nav.html");
    $("main").load("pages/login.html");

    $("#logout-successful").removeClass("d-none");
    $('#connection-lost').hide();
    return false;
}

function changeName() {

}

function changePassword() {

}

function changeEmail() {

}

async function addFriend(e) {
    if (e != null) {
        e.preventDefault();
    }
    var newFriend = $('#friend-name').val();
    var data = {
        'UserId': getCookie("userId"),
        'FriendsName': newFriend
    }
    console.log("data");
    console.log(data);
    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + getCookie("accessToken")
    }
    var response = await fetchData('api/friends/add', 'POST', headers, data);
    console.log("rasp");
    console.log(response);
    if (response.status == 201) {
        $('#friend-added').removeClass('d-none');
    } else if(response.status == 404) {
        $('#friend-not-found').removeClass('d-none');
    } else {
        $('#friend-couldnt-be-added').removeClass('d-none');
    }
    return false;
}

function removeFriend() {

}



// COOKIE
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function deleteCookie(name, path, domain) {
    if (getCookie(name)) {
        document.cookie = name + "=" +
            ((path) ? ";path=" + path : "") +
            ((domain) ? ";domain=" + domain : "") +
            ";expires=Thu, 01 Jan 1970 00:00:01 GMT";
    }
}