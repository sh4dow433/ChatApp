var hubUrl = baseUrl + 'chathub';
var hubConnection = new signalR.HubConnectionBuilder().withUrl(hubUrl, { accessTokenFactory: () => getCookie("accessToken") }).build();

async function startConnection() {
    if(hubConnection.state == "Connected") {
        console.log("Already connected");
        return;
    }
    console.log(getCookie("accessToken"));
    console.log(hubConnection);
    try {
        await hubConnection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};
//ON CLOSE EVENT
hubConnection.onclose(function (event) {
    $('#connection-lost').removeClass('d-none');
});
async function stopConnection() {
    if(!hubConnection || hubConnection.state !== "Connected") {
        console.log("Hub Not Connected");
    }
    console.log('SignalR Disconnected.');
    await hubConnection.stop();
}
