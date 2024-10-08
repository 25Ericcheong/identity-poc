﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;
using Microsoft.Owin.Security;

namespace MvcApp.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        private static readonly DiscoveryCache DiscoveryCache = new(Urls.IdentityServer);
        
        public async Task<ActionResult> GetNet48Data()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            var errorMessage = "No Access Token found. Please sign in again";
            
            var props = authResult.Properties.Dictionary;

            try
            {
                if (props.TryGetValue("access_token", out var accessToken))
                {
                    if (TokenIsExpired(authResult.Properties))
                    {
                        var tokenResponse = await RefreshToken(authResult.Properties, authResult.Identity);
                        accessToken = tokenResponse.AccessToken;
                    }

                    if (accessToken is null)
                    {
                        throw new Exception("Access token not found please sign in again.");
                    }

                    var http = new HttpClient();
                    http.SetBearerToken(accessToken);
                    var response = await http.GetStringAsync(Urls.ApiFramework + "/financial");
                    var parsed = JsonDocument.Parse(response);

                    var payload = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
                    return View("Net48", model: payload);
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = "Fail to retrieve data from NET48 endpoint: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorMessage = "Unexpected error occured: " + ex.Message;
            }
            
            return View("Error", model: errorMessage);
        }

        public async Task<ActionResult> GetNet8Data()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            var errorMessage = "No Access Token found. Please sign in again";
            
            var props = authResult.Properties.Dictionary;

            try
            {
                if (props.TryGetValue("access_token", out var accessToken))
                {
                    if (TokenIsExpired(authResult.Properties))
                    {
                        var tokenResponse = await RefreshToken(authResult.Properties, authResult.Identity);
                        accessToken = tokenResponse.AccessToken;
                    }

                    if (accessToken is null)
                    {
                        throw new Exception("Access token not found please sign in again.");
                    }

                    var http = new HttpClient();
                    http.SetBearerToken(accessToken);
                    var response = await http.GetStringAsync(Urls.ApiCore + "/company");

                    return View("Net8", model: response);
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = "Fail to retrieve data from NET8 endpoint: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorMessage = "Unexpected error occured: " + ex.Message;
            }
            
            return View("Error", model: errorMessage);
        }
        
        private void SaveTokens(AuthenticationProperties properties, TokenResponse message)
        {
            if (!string.IsNullOrEmpty(message.AccessToken))
            {
                properties.Dictionary["access_token"] = message.AccessToken;
            }

            if (!string.IsNullOrEmpty(message.IdentityToken))
            {
                properties.Dictionary["id_token"] = message.IdentityToken;
            }

            if (!string.IsNullOrEmpty(message.RefreshToken))
            {
                properties.Dictionary["refresh_token"] = message.RefreshToken;
            }

            if (!string.IsNullOrEmpty(message.TokenType))
            {
                properties.Dictionary["token_type"] = message.TokenType;
            }

            if (message.ExpiresIn != 0)
            {
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(message.ExpiresIn);
                properties.Dictionary["expires_at"] = expiresAt.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        private bool TokenIsExpired(AuthenticationProperties props)
        {
            var expirationProp = props.Dictionary["expires_at"];
            var expirationTime = DateTime.Parse(expirationProp);
            return expirationTime < DateTime.UtcNow;
        }
        
        private async Task<TokenResponse> RefreshToken(AuthenticationProperties props, ClaimsIdentity identity)
        {
            if (props.Dictionary.TryGetValue("refresh_token", out var refreshToken))
            {
                var disco = await DiscoveryCache.GetAsync();
                var refreshRequest = new RefreshTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "interactive.mvc.owin.sample",
                    ClientSecret = "secret",
                    RefreshToken = refreshToken
                };
                var http = new HttpClient();
                var response = await http.RequestRefreshTokenAsync(refreshRequest);
                if (!response.IsError)
                {
                    SaveTokens(props, response);
                    HttpContext.GetOwinContext().Authentication.SignIn(props, identity);
                    return response;
                }
                throw new Exception($"Failed to refresh tokens: {response.Error}");
            }
            throw new Exception("Attempted to refresh a token without a refresh token saved");
        }
        
        
    }
}