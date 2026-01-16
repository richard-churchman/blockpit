namespace Blockpit.Listener.Helpers
{
    using System.Text.Json;

    public static class HttpJsonHelper
    {
        public static async Task<T> GetAsync<T>(string url, JsonSerializerOptions jsonSerializerOptions, bool ignoreSsl = false)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));
            }

            HttpClient httpClient;
            if (ignoreSsl)
            {
                httpClient = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
            }
            else
            {
                httpClient = new HttpClient();
            }

            using var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);

            return result ?? throw new InvalidOperationException("Deserialization returned null.");

        }
    }

}
