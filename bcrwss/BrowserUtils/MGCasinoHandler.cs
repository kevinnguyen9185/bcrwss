using business;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bcrwss.BrowserUtils
{
    internal class MGCasinoHandler : ICasinoHandler
    {
        private readonly ChromiumWebBrowser _browser;
        private readonly BrowserHandler _browserHandler;

        public MGCasinoHandler(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _browserHandler = new BrowserHandler(_browser);
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
                var fI = _browser.GetBrowser().GetFrameIdentifiers()[1];
                var frm = _browser.GetBrowser().GetFrame(fI);
                if (frm != null)
                {
                    var lastUserToken = _browserHandler.GetAllSessionStorageVariables(frm)["lastUserToken"];
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
                var fI = frameIds[1];
                var frm = _browser.GetBrowser().GetFrame(fI);
                if (frm != null)
                {
                    var landingParameter = _browserHandler.GetAllSessionStorageVariables(frm)["landingParameter"];
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
}
