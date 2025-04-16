using DevKnowledgeBase.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace DevKnowledgeBase.UI.Common
{
    public class HttpErrorMessage
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public static class NavigationManagerExtensions
    {
        /// <summary>
        /// Extracts the error message from an HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <returns>A <see cref="ResponseMessage"/> indicating success or failure.</returns>
        public static async Task<ResponseMessage> GetMessageAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return new ResponseMessage(true, string.Empty);
            }

            try
            {
                if (response.Content != null) // Ensure Content is not null
                {
                    var error = await response.Content.ReadFromJsonAsync<HttpErrorMessage>();
                    return new ResponseMessage(false, error?.Message ?? "An unknown error occurred.");
                }
                else
                {
                    return new ResponseMessage(false, "Response content is null.");
                }
            }
            catch (JsonException)
            {
                return new ResponseMessage(false, "Failed to parse error response.");
            }
        }

        /// <summary>
        /// Attempts to retrieve a query string value from the current URI.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="navManager">The navigation manager.</param>
        /// <param name="key">The query string key.</param>
        /// <param name="value">The retrieved value, if successful.</param>
        /// <returns>True if the value was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Contains("..") || key.Contains("/"))
            {
                value = default;
                return false;
            }

            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
            {
                var parsers = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(int), s => int.TryParse(s, out int result) ? result : null },
                    { typeof(decimal), s => decimal.TryParse(s, out var result) ? result : null },
                    { typeof(string), s => s }
                };

                if (parsers.TryGetValue(typeof(T), out var parser))
                {
                    var parsedValue = parser(valueFromQueryString);
                    if (parsedValue != null)
                    {
                        value = (T)parsedValue;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }
    }
}
