// Select your input type file and store it in a variable
var fileInput, imageInput;
const loadFileForm = () => {
    console.log("LOAD FILE FORM")
    fileInput = document.getElementById('file');
    imageInput = document.getElementById('photo'); 
    console.log(fileInput);
    console.log(imageInput);
    // Add a listener on your input
    // It will be triggered when a file will be selected
    fileInput.addEventListener('change', onSelectFile, false);
    imageInput.addEventListener('change', onSelectImage, false);
}

async function upload(file, isPhoto = false) {
    var formData = new FormData();
    formData.append('file', file);
    const headers = {
        'Authorization': 'Bearer ' + getCookie("accessToken")
    }

    var url = baseUrl + 'api/files?isPhoto=false';
    if (isPhoto) {
        url = baseUrl + 'api/files?isPhoto=true';
    }
    var response = await fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: headers,
        body: formData // This is your file object
    });
    console.log(response);

    if (response.status == 200) {
        var data = await response.json();
        await sendMessageWithFile(data.id);
    }
};

async function sendMessageWithFile(fileId) {
    var senderId = getCookie("userId");
    var text = $('#chat-text').val();
    var hasFile = true;
    var chatId = activeChatId;
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


// This will upload the file after having read it
// const upload = (file) => {
//     fetch('http://www.example.net', { // Your POST endpoint
//         method: 'POST',
//         headers: {
//             // Content-Type may need to be completely **omitted**
//             // or you may need something
//             "Content-Type": "You will perhaps need to define a content-type here"
//         },
//         body: file // This is your file object
//     }).then(
//         response => response.json() // if the response is a JSON object
//     ).then(
//         success => console.log(success) // Handle the success response object
//     ).catch(
//         error => console.log(error) // Handle the error response object
//     );
// };

// Event handler executed when a file is selected
const onSelectFile = () => upload(fileInput.files[0], false);

const onSelectImage = () => upload(imageInput.files[0], true);

