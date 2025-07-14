let beforeUnloadHandler = (event) => {
    event.preventDefault();
    event.returnValue = "";
};

window.preventPageUnload = function (enable) {
    if (enable) {
        window.addEventListener("beforeunload", beforeUnloadHandler);
    } else {
        window.removeEventListener("beforeunload", beforeUnloadHandler);
    }
};
