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

        public bool IsBrowserReady()
        {
            if(_browser.IsBrowserInitialized && !_browser.IsLoading)
            {
                return RunEvaluateJavascriptToString("document.readyState;").ToLower() == "complete";
            }
            return false;
        }

        public void DoLoginQQ()
        {
            this._browser.ExecuteScriptAsync("document.querySelector('#loginId').value='maltanuoiga1';");
            this._browser.ExecuteScriptAsync("document.querySelector('#loginPwd').value='Taolaso01';");
            this._browser.ExecuteScriptAsync("document.querySelector('#LoginButton').click();");
        }


        public void RefreshWeb()
        {
            this._browser.Reload();
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
        public string RunEvaluateJavascriptToString(string script, IFrame frame = null)
        {
            var res = RunEvaluateJavascript(script, frame);
            return res == null ? null : res.ToString();
        }

        public Dictionary<string, string> GetAllLocalStorageVariables(IFrame frame = null)
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

        public Dictionary<string, string> GetAllSessionStorageVariables(IFrame frame = null)
        {
            dynamic res = RunEvaluateJavascript("{ ...sessionStorage };", frame);
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
