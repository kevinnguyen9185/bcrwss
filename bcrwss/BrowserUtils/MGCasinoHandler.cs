using business;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
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
            string sessionId = "";
            var cookieManager = Cef.GetGlobalCookieManager();

            var cookies = await cookieManager.VisitUrlCookiesAsync("https://contapp8895.anastre.com/bamboo/", true);
            foreach(var cc in cookies)
            {
                if (cc.Name.ToLower().Contains("refreshtoken"))
                {
                    sessionId = cc.Value;
                    break;
                }
                    
            }

            return sessionId;
        }

        public async Task<string> GetWebSessionIdFromStorage()
        {
            var fI = _browser.GetBrowser().GetFrameIdentifiers()[1];
            var frm = _browser.GetBrowser().GetFrame(fI);
            if(frm != null)
            {
                var landingParameter = _browserHandler.GetAllSessionStorageVariables(frm)["landingParameter"];
                dynamic data = JsonConvert.DeserializeObject(landingParameter);
                var webSessionId = data.SessionJWT;
                return webSessionId;
            }
            
            return "";
        }

        public void GoToTable(string tableId)
        {
            throw new NotImplementedException();
        }

        public bool IsBrowserReady()
        {
            if (_browser.IsBrowserInitialized && !_browser.IsLoading)
            {
                return _browserHandler.RunEvaluateJavascriptToString("document.readyState;").ToLower() == "complete";
            }
            return false;
        }

        public bool IsCasinoBrowserReady()
        {
            return IsBrowserReady();
        }

        public void RandomSwitchLayoutCasino()
        {
            //throw new NotImplementedException();
        }
    }
}
