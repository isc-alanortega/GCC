function initializeTurnstile(siteKey, dotNetHelper) {
    turnstile.render('#turnstile-widget', {
        sitekey: siteKey,
        callback: function (token) {
            dotNetHelper.invokeMethodAsync('OnTurnstileSuccess', token);
        }
    });
}