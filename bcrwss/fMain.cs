using bcrwss.BrowserUtils;
using bcrwss.Communication;
using business.DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bcrwss
{
    public partial class fMain : Form
    {
        private readonly ILogger _logger;
        business.bcr bcrBusiness = new business.bcr();
        business.BrowserHandler bHandler;
        ICasinoHandler casinoHandler;
        int RefreshInterval = 10 * 60000;
        bool IsWebSocketError = false;
        UIInteraction uiInteraction;
        ICansinoActions casinoActions;

        public fMain(ILogger<fMain> logger)
        {
            _logger = logger;
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            bHandler = new business.BrowserHandler(this.cBrowser);
            casinoHandler = new MGCasinoHandler(this.cBrowser, _logger);
            this.tRefreshSession.Interval = RefreshInterval;
            uiInteraction = new UIInteraction(lstLog, toolStripStatusMain, toolStripStatusEvoSessionId, this);
            casinoActions = new MGCasinoAction(cBrowser, casinoHandler, uiInteraction);
        }

        private async void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            await casinoActions.StopWsAsync();
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            tvData.Nodes.Clear();
            var tableInfo = bcrBusiness.GetBcrTableInfo();
            foreach(var table in tableInfo)
            {
                var tbNode = tvData.Nodes.Add(table.TableName);
                foreach(var session in table.Sessions)
                {
                    tbNode.Nodes.Add(session.SessionId.Substring(0,6) + " : " + session.ResultString + " : " + session.Dts.ToString());
                }
            }
        }

        private async void tMetricPing_Tick(object sender, EventArgs e)
        {
            if (IsWebSocketError)
            {
                await StartSessionWithoutLogin();
            }
        }

        private async Task StartSessionWithoutLogin()
        {
            StopCurrentSession();
            await DoGoToLobbyAsync();
            await UpdateLobbyInfo();
        }

        private async void tRefreshSession_Tick(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Information, "Session refreshed", null);
            await StartSessionWithoutLogin();
            tMetricPing.Stop();
            tMetricPing.Start();
        }

        private async Task DoLoginAsync()
        {
            await cBrowser.LoadUrlAsync(Config.LoginUrl);
            while (!bHandler.IsBrowserReady())
            {
                await Task.Delay(1000);
            }
            await Task.Delay(1500);
            bHandler.DoLoginQQ();
            await Task.Delay(3000);
        }

        private async Task DoGoToLobbyAsync()
        {
            await casinoActions.DoGoToCasinoLobbyAsync();
        }

        private async Task UpdateLobbyInfo()
        {
            try
            {
                await Task.Delay(3000);
                await casinoActions.UpdateLobbyInfoAsync();
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.StackTrace, null);
            }
        }

        private void StopCurrentSession()
        {
            this.lstLog.Items.Clear();
            uiInteraction.UpdateToolTipMain("Stopped");
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Information, "Login", null);

            await DoLoginAsync();
            await DoGoToLobbyAsync();
            await UpdateLobbyInfo();

            tMetricPing.Start();
            tRefreshSession.Start();
        }
    }
}
