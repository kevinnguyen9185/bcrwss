using business;
using CefSharp.WinForms;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bcrwss.BrowserUtils
{
    internal class MGCasinoHandler : ICasinoHandler
    {
        private readonly ChromiumWebBrowser _browser;
        private readonly BrowserHandler _browserHandler;
        private readonly ILogger _logger;

        public MGCasinoHandler(ChromiumWebBrowser browser, ILogger logger)
        {
            _browser = browser;
            _browserHandler = new BrowserHandler(_browser);
            _logger = logger;
        }
        public string GetCasinoFrameUrl()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetCasinoSessionIdAsync()
        {
            var retry = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine("Retry");
                    });
            var result = await retry.ExecuteAsync<string>(async () =>
            {
                var frameIds = _browser.GetBrowser().GetFrameIdentifiers();

                foreach (var frId in frameIds)
                {
                    var frm = _browser.GetBrowser().GetFrame(frId);
                    var dict = _browserHandler.GetAllSessionStorageVariables(frm);
                    _logger.LogInformation(dict.ToDebugString(), null);

                    if (frm == null || !dict.ContainsKey("lastUserToken"))
                    {
                        continue;
                    }

                    var lastUserToken = dict["lastUserToken"];
                    return lastUserToken;
                }
                return "";
            });
            return result;
        }

        public async Task<string> GetWebSessionIdFromStorageAsync()
        {
            var retry = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine("Retry");
                    });

            var result = await retry.ExecuteAsync<string>(async () =>
            {
                var frameIds = _browser.GetBrowser().GetFrameIdentifiers();
                if(frameIds == null || frameIds.Count < 2)
                {
                    return "";
                }
                foreach (var frId in frameIds)
                {
                    var frm = _browser.GetBrowser().GetFrame(frId);
                    var dict = _browserHandler.GetAllSessionStorageVariables(frm);
                    _logger.LogInformation(dict.ToDebugString(), null);

                    if (frm == null || !dict.ContainsKey("landingParameter"))
                    {
                        continue;
                    }

                    var landingParameter = dict["landingParameter"];
                    dynamic data = JsonConvert.DeserializeObject(landingParameter);
                    var webSessionId = data.SessionJWT;
                    return webSessionId;
                }
                return "";
            });
            return result;
        }

        public void GoToTable(string tableId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsBrowserReady()
        {
            if (_browser.IsBrowserInitialized && !_browser.IsLoading)
            {
                var retry = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine("Retry");
                    });
                var result = await retry.ExecuteAsync<bool>(async () =>
                {
                    return _browserHandler.RunEvaluateJavascriptToString("document.readyState;").ToLower() == "complete";
                });

                return result;
            }
            return false;
        }

        public async Task<bool> IsCasinoBrowserReady()
        {
            return await IsBrowserReady();
        }

        public void RandomSwitchLayoutCasino()
        {
            //throw new NotImplementedException();
        }
    }

    public static class Utils
    {
        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
    }
}
