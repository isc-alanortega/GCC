/**
 * Adds a window resize event listener that calls a .NET method whenever the window is resized.
 * 
 * @param {DotNetObjectReference} dotNetHelper - The DotNetObjectReference that provides the ability to call .NET methods from JavaScript.
 * 
 * This function listens for the `resize` event on the window. When the window is resized, it calls 
 * the `OnResize` method of the provided `dotNetHelper`, passing the new window width to the .NET method.
 * Additionally, it makes an initial call to `.NET` with the current window width when the listener is first set up.
 */
export function addResizeListener(dotNetHelper) {
    // Add a resize event listener to the window
    window.addEventListener('resize', () => {
        // Call the .NET method 'OnResize' with the new window width whenever the window is resized
        dotNetHelper.invokeMethodAsync('OnResize', window.innerWidth);
    });

    // Initial call to get the current window width when the listener is set up
    dotNetHelper.invokeMethodAsync('OnResize', window.innerWidth);
}
