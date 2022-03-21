namespace Mjml.Net
{
    /// <summary>
    /// Main entry point for MJML rendering.
    /// </summary>
    public interface IMjmlRenderer
    {
        /// <summary>
        /// Adds a component to the renderer.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns>
        /// The current instance to chain method calls.
        /// </returns>
        IMjmlRenderer Add<T>() where T : IComponent, new();

        /// <summary>
        /// Adds a custom helper to the renderer.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns>
        /// The current instance to chain method calls.
        /// </returns>
        IMjmlRenderer Add(IHelper helper);

        /// <summary>
        /// Clears the list of default components.
        /// </summary>
        /// <returns>
        /// The current instance to chain method calls.
        /// </returns>
        IMjmlRenderer ClearComponents();

        /// <summary>
        /// Clears the list of default helpers.
        /// </summary>
        /// <returns>
        /// The current instance to chain method calls.
        /// </returns>
        IMjmlRenderer ClearHelpers();

        /// <summary>
        /// Renders MJML from a string.
        /// </summary>
        /// <param name="mjml">The MJML as string.</param>
        /// <param name="options">Optional options.</param>
        /// <returns>
        /// The result of rendering, including the HTML and validation errors.
        /// </returns>
        RenderResult Render(string mjml, MjmlOptions? options = null);

        /// <summary>
        /// Renders MJML from a stream.
        /// </summary>
        /// <param name="mjml">The MJML as stream.</param>
        /// <param name="options">Optional options.</param>
        /// <returns>
        /// The result of rendering, including the HTML and validation errors.
        /// </returns>
        RenderResult Render(Stream mjml, MjmlOptions? options = null);

        /// <summary>
        /// Renders MJML from a text renderer.
        /// </summary>
        /// <param name="mjml">The MJML as text renderer.</param>
        /// <param name="options">Optional options.</param>
        /// <returns>
        /// The result of rendering, including the HTML and validation errors.
        /// </returns>
        RenderResult Render(TextReader mjml, MjmlOptions? options = null);
    }
}
