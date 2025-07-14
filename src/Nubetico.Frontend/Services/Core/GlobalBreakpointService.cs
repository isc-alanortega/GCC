using Microsoft.JSInterop;
using Nubetico.Frontend.Models.Enums.Core;

namespace Nubetico.Frontend.Services.Core
{
    /// <summary>
    /// A service that detects window resize events and adjusts the layout based on breakpoints (screen widths).
    /// Uses JavaScript interop to add a resize listener to the browser window and triggers .NET methods when the window size changes.
    /// </summary>
    public class GlobalBreakpointService(IJSRuntime jsRuntime)
    {
        /// <summary>
        /// The IJSRuntime instance used to interact with JavaScript code from .NET.
        /// </summary>
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        /// <summary>
        /// Reference to the imported JavaScript module that handles the resize listener.
        /// </summary>
        private IJSObjectReference _jsRecizeReference;

        /// <summary>
        /// Stores the current width of the window.
        /// </summary>
        private int _currentWidth = 0;

        /// <summary>
        /// Stores the current breakpoint based on the window width.
        /// Default is Breakpoint.Xs (extra small).
        /// </summary>
        private Breakpoint _currentBreakpoint = Breakpoint.Xs;

        /// <summary>
        /// Event that is triggered whenever the state of the service changes (e.g., breakpoint change or resize event).
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Initializes the resize listener by importing the JavaScript module and setting up the window resize event listener.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Logs any error encountered during initialization.</exception>
        public async Task InitializeAsync()
        {
            try
            {
                // Import the JavaScript module for resize listener
                _jsRecizeReference = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./assets/nubetico/js/resizeListener.js");

                // Create a reference to this service to pass it to JavaScript
                var dotNetHelper = DotNetObjectReference.Create(this);

                // Invoke the JavaScript function to add the resize listener
                await _jsRecizeReference.InvokeVoidAsync("addResizeListener", dotNetHelper);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Called from JavaScript when the window is resized. Updates the current width and calculates the new breakpoint.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        [JSInvokable]
        public void OnResize(int width)
        {
            if (_currentWidth == width) return;

            _currentWidth = width;
            _currentBreakpoint = GetBreakpoint(width);
            NotifyStateChanged();
        }

        /// <summary>
        /// Determines the breakpoint based on the given window width.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <returns>The breakpoint corresponding to the given width.</returns>
        private Breakpoint GetBreakpoint(int width) => width switch
        {
            <= 767 => Breakpoint.Xs,     // Extra small         (576px a 767px)
            <= 1023 => Breakpoint.Sm,    // Small               (768px a 1023px)
            <= 1279 => Breakpoint.Md,    // Medium              (1024px a 1279px)
            <= 1289 => Breakpoint.Lg,    // Large               (1280px a 1289px)
            <= 2559 => Breakpoint.Xl,    // Extra large         (1920px a 2559px)
            _ => Breakpoint.Xxl          // Extra Exra large    (>= 2560px)
        };

        /// <summary>
        /// Notifies subscribers that the state has changed (e.g., due to a breakpoint change).
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Gets the current breakpoint based on the window's width.
        /// </summary>
        /// <returns>The current breakpoint.</returns>
        public Breakpoint GetCurrentBreakpoint() => _currentBreakpoint;
    }
}
