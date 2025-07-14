window.applyCssAndFavicon = (cssUrl, faviconUrl) => {
    if (cssUrl) {
        var linkElement = document.createElement("link");
        linkElement.setAttribute("rel", "stylesheet");
        linkElement.setAttribute("href", cssUrl);
        document.head.appendChild(linkElement);
    }

    if (faviconUrl) {
        var faviconElement = document.createElement("link");
        faviconElement.setAttribute("rel", "icon");
        faviconElement.setAttribute("href", faviconUrl);
        document.head.appendChild(faviconElement);
    }
};

window.applyPwaManifest = (manifestUrl) => {
    if (manifestUrl) {
        var linkElement = document.createElement("link");
        linkElement.setAttribute("rel", "manifest");
        linkElement.setAttribute("href", manifestUrl);
        document.head.appendChild(linkElement);
    }
};