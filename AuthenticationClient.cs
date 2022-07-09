﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Uber.Models;

namespace UberSDKForNet
{
    class AuthenticationClient
    {
        public string ApiVersion { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        private const string TokenUrl = "https://login.uber.com/oauth/token";
        private string _apiVersion = "v2";
        private readonly HttpClient _httpClient;

        public AuthenticationClient()
            : this(new HttpClient())
        {
        }

        public AuthenticationClient(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
            ApiVersion = "v2";
        }

        public async Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentNullException("redirectUri");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if (!Common.IsValidUri(redirectUri)) throw new ArgumentException("Invalid redirectUri");

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code", code)
                });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(TokenUrl),
                Content = content
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.access_token;
                RefreshToken = authToken.refresh_token;
            }
            else
            {
                //TODO: Create appropriate error response
                //var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                //throw new ForceAuthException(errorResponse.error, errorResponse.error_description);
                throw new Exception("error");
            }
        }






    }
}
