using business;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Threading.Tasks;
using System.Web;

namespace bcrwss.BrowserUtils
{
    public class EvoCasinoHandler : ICasinoHandler
    {
        private readonly ChromiumWebBrowser _browser;
        private readonly BrowserHandler _browserHandler;

        public EvoCasinoHandler(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _browserHandler = new BrowserHandler(_browser);
        }

        private IFrame GetEvoFrame()
        {
            var frmList = this._browser.GetBrowser().GetFrameIdentifiers();
            if (frmList.Count > 1)
            {
                var betFrame = this._browser.GetBrowser().GetFrame(frmList[1]);
                return betFrame;
            }
            return null;
        }

        public string GetCasinoFrameUrl()
        {
            string frameUrl = _browserHandler.RunEvaluateJavascriptToString("document.getElementById('GameFrm').src");
            return frameUrl;
        }

        public Task<string> GetCasinoSessionIdAsync()
        {
            string sessionId = "";
            var evoFrame = GetEvoFrame();
            if (evoFrame != null && !string.IsNullOrEmpty(evoFrame.Url))
            {
                var evoUri = new Uri(evoFrame.Url);
                sessionId = HttpUtility.ParseQueryString(evoUri.Fragment).Get("EVOSESSIONID");
            }
            return Task.FromResult(sessionId);
        }

        public void GoToTable(string tableId)
        {
            var betFrame = GetEvoFrame();
            if (betFrame != null)
            {
                betFrame.ExecuteJavaScriptAsync("document.querySelector(`[data-tableid*=" + tableId + "]`).firstElementChild.click()");
            }
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
            var randomInt = new Random().Next(0, 3);
            var betFrame = GetEvoFrame();
            if (betFrame != null)
            {
                betFrame.ExecuteJavaScriptAsync("document.querySelector('.LayoutSwitch--uxSvZ').children[" + randomInt.ToString() + "].click()");
            }
        }

        public async Task<string> GetWebSessionIdFromStorageAsync()
        {
            return "";
        }
    }
}
