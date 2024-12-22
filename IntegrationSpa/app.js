const { app, BrowserWindow } = require("electron");

let appWindow;

function createWindow() {
    appWindow = new BrowserWindow({
        width: 1000,
        height: 800
    });
    appWindow.loadFile("dist/electronfile/index.html");
    appWindow.on("close", function () {
        appWindow = null;
    });
}

// Use the app's `whenReady` method
app.whenReady().then(() => {
    createWindow();

    // For macOS, re-create a window when the dock icon is clicked
    app.on("activate", function () {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

// Quit the app when all windows are closed
app.on("window-all-closed", function () {
    if (process.platform !== "darwin") {
        app.quit();
    }
});
