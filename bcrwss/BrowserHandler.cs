using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace business
{
    public class BrowserHandler
    {
        private ChromiumWebBrowser _browser;
        public BrowserHandler(ChromiumWebBrowser browser)
        {
            this._browser = browser;
        }

        public string GetEvoFrameUrl()
        {
            string frameUrl = RunEvaluateJavascriptToString("document.getElementById('GameFrm').src");
            return frameUrl;
        }

        public string GetEvoSessionId()
        {
            string sessionId = "";
            var evoFrame = GetEvoFrame();
            if(evoFrame!=null && !string.IsNullOrEmpty(evoFrame.Url))
            {
                var evoUri = new Uri(evoFrame.Url);
                sessionId = HttpUtility.ParseQueryString(evoUri.Fragment).Get("EVOSESSIONID");
            }
            return sessionId;
        }

        public bool IsBrowserReady()
        {
            if(_browser.IsBrowserInitialized && !_browser.IsLoading)
            {
                return RunEvaluateJavascriptToString("document.readyState;").ToLower() == "complete";
            }
            return false;
        }

        public bool IsEvoBrowserReady()
        {
            if (_browser.IsBrowserInitialized && !_browser.IsLoading)
            {
                var evoFrame = GetEvoFrame();
                if (evoFrame != null)
                {
                    return RunEvaluateJavascriptToString("document.readyState;", evoFrame).ToLower() == "complete";
                }
                return false;
            }
            return false;
        }

        public void RandomSwitchLayoutEvo()
        {
            var randomInt = new Random().Next(0, 3);
            var betFrame = GetEvoFrame();
            if (betFrame != null)
            {
                betFrame.ExecuteJavaScriptAsync("document.querySelector('.LayoutSwitch--uxSvZ').children[" + randomInt.ToString() + "].click()");
            }
        }

        public void DoLoginQQ()
        {
            this._browser.ExecuteScriptAsync("document.querySelector('#loginId').value='maltanuoiga1';");
            this._browser.ExecuteScriptAsync("document.querySelector('#loginPwd').value='Taolaso01';");
            this._browser.ExecuteScriptAsync("document.querySelector('#LoginButton').click();");
        }

        public void GoToTable(string tableId)
        {
            var betFrame = GetEvoFrame();
            if (betFrame != null)
            {
                betFrame.ExecuteJavaScriptAsync("document.querySelector(`[data-tableid*=" + tableId + "]`).firstElementChild.click()");
            }
        }

        public void RefreshWeb()
        {
            this._browser.Reload();
        }

        //public Dictionary<string, string> GetAllLocalStorageVariablesEvo()
        //{
        //    var evoFrame = GetEvoFrame();
        //    return GetAllLocalStorageVariables(evoFrame);
        //}

        //public string GetLocalStorageValueEvo(string key)
        //{
        //    var evoFrame = GetEvoFrame();
        //    return GetLocalStorageValue(key, evoFrame);
        //}

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

        private void RunJavascript(string script)
        { //semicolumn at the end is not needed, multiple semicolons don't cause error
            _browser.GetMainFrame().ExecuteJavaScriptAsync(script);
        }
        private dynamic RunEvaluateJavascript(string script, IFrame frame = null)
        {
            string scriptTemplate = @"(function () {
                                return " + script + ";" +
                                    "})();";
            if (frame == null)
            {
                frame = _browser.GetMainFrame();
            }
            Task<JavascriptResponse> t = frame.EvaluateScriptAsync(scriptTemplate);
            t.Wait();

            return t.Result.Result;
        }
        private string RunEvaluateJavascriptToString(string script, IFrame frame = null)
        {
            var res = RunEvaluateJavascript(script, frame);
            return res == null ? null : res.ToString();
        }

        private Dictionary<string, string> GetA7llLocalStorageVariables(IFrame frame = null)
        {
            dynamic res = RunEvaluateJavascript("{ ...localStorage };", frame);
            Dictionary<string, string> dictRes = null;

            try
            {
                var dictTmp = new Dictionary<string, object>(res);
                dictRes = dictTmp.ToDictionary(k => k.Key, k => k.Value.ToString());
            }
            catch
            {
                return null;
            }

            return dictRes;
        }

        private string GetLocalStorageValue(string key, IFrame frame = null)
        {
            return RunEvaluateJavascriptToString($"window.localStorage.getItem('{key}');", frame);
        }
    }
}
