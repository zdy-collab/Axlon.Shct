using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Axlon.Services.Contracts.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new JsonStringEnumConverter()  // 允许int转枚举
            }
        };

        /// <summary>
        /// GET 请求
        /// </summary>
        public static async Task<TResponse?> GetAsync<TResponse>(
            this HttpClient httpClient,
            string url,
            string? token = null,
            CancellationToken cancellationToken = default)
        {

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                url);

            AddBearerToken(request, token);

            var response = await httpClient.SendAsync(
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>(
                    JsonOptions,
                    cancellationToken);
        }

        /// <summary>
        /// POST 请求
        /// </summary>
        public static async Task<TResponse?> PostAsync<TRequest, TResponse>(
            this HttpClient httpClient,
            string url,
            TRequest requestData,
            string? token = null,
            CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            AddBearerToken(request, token);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestData, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.SendAsync(
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>(
                    JsonOptions,
                    cancellationToken);
        }

        /// <summary>
        /// 添加 Bearer Token
        /// </summary>
        private static void AddBearerToken(
            HttpRequestMessage request,
            string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }
    }
}
