using bcrwss.BrowserUtils;
using business;
using business.DAL;
using business.Input.BcrRoad;
using business.Input.Lobby;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Websocket.Client;

namespace bcrwss.Communication
{
    internal class MGCasinoAction : ICansinoActions
    {
        static string _url = "wss://contapp8895.anastre.com/bamboo/";
        private ChromiumWebBrowser _browser;
        private ICasinoHandler _casinoHandler;
        WebsocketClient wsClient;
        List<Table> bcrTables = new List<Table>();
        List<business.Entity.BcrTable> tableInfos = new List<business.Entity.BcrTable>();
        bcr bcrBusiness = new bcr();
        bool IsWebSocketError = false;
        UIInteraction _uiInteraction;
        string CookieSession = string.Empty;

        public MGCasinoAction(ChromiumWebBrowser browser, ICasinoHandler casinoHalder, UIInteraction uiInteraction)
        {
            _browser = browser;
            _casinoHandler = casinoHalder;
            _uiInteraction = uiInteraction;
        }
        public async Task DoGoToCasinoLobbyAsync()
        {
            await _browser.LoadUrlAsync(Config.MGCcasinoUrl);
            while (!await _casinoHandler.IsCasinoBrowserReady())
            {
                await Task.Delay(1000);
            }
            await Task.Delay(1500);
            _uiInteraction.UpdateToolTipSessionId(CookieSession);
        }

        public async Task UpdateLobbyInfoAsync()
        {
            if(wsClient != null)
            {
                wsClient.Dispose();
            }

            var sessionWebId = await _casinoHandler.GetWebSessionIdFromStorageAsync();
            CookieSession = await _casinoHandler.GetCasinoSessionIdAsync();

            if (wsClient == null || !wsClient.IsRunning)
            {
                var factory = new Func<ClientWebSocket>(() => new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                        UseDefaultCredentials = true,
                    }
                });

                wsClient = new WebsocketClient(new Uri(_url), factory);
                await wsClient.Start();
                _uiInteraction.Log("Client started");

                wsClient
                    .MessageReceived
                    .Where(msg => msg.Text != null)
                    .Where(msg => msg.Text.StartsWith("{"))
                    .Subscribe(obj => { OnWebsocketReceive(obj.Text); });

                _uiInteraction.UpdateToolTipMain("Fetching");
            }

            if (wsClient != null)
            {
                wsClient.Send(BaseResource.MG_WSS_Request_Login
                    .Replace("{0}", CookieSession)
                    .Replace("{webSessionId}", sessionWebId)
                    );
            }
        }

        private void OnWebsocketReceive(string json)
        {
            try
            {
                IsWebSocketError = false;
                if (json.IndexOf("getLoginReply") > -1)
                {
                    dynamic dataLogin = JsonConvert.DeserializeObject(json);
                    var loginSuccess = Convert.ToBoolean(dataLogin.Data.IsSuccess);
                    if (loginSuccess)
                    {
                        _uiInteraction.UpdateToolTipMain("Login Successfully");

                        wsClient.Send("{ 'id':'adfadsfasd','type':'lobby.updateSubscriptions','args':{ 'subscribeTopics':[{ 'topic':'lobby','orientation':'landscape','category':'baccarat_sicbo','table':'leqhceumaq6qfoug'}],'unsubscribeTopics':[]} }");
                    }
                }
                if (json.IndexOf("updateLobbyInfo") > -1)
                {
                    bcrTables = ExtractTableList(json);
                    foreach (var tbl in bcrTables)
                    {
                        bcrBusiness.UpsertTableMetadata(tbl, "MGC");
                    }
                }
                else if (json.IndexOf("updateTableInfo") > -1)
                {
                    UpdateTableInfo(json);
                }
                else if (json.IndexOf("kickout") > -1)
                {
                    IsWebSocketError = true;
                    _uiInteraction.UpdateToolTipMain("Error");
                    wsClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                _uiInteraction.Log(ex.Message);
            }

            // Table list
            //{"id":"1627721200999-1609","type":"lobby.tables","args":{"category":{"categoryId":"baccarat_sicbo"},"tables":[{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":0,"streamName":"bac5_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ A","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac5_bi_med","views":{"default":"hd1","available":[]},"tableId":"leqhceumaq6qfoug"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":1,"streamName":"bac6_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ B","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac6_bi_med","views":{"default":"hd1","available":[]},"tableId":"lv2kzclunt2qnxo5"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":2,"streamName":"bact1_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ C","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact1_bs_med","views":{"default":"hd1","available":[]},"tableId":"ndgvwvgthfuaad3q"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":3,"streamName":"bact2_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ D","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact2_bs_med","views":{"default":"hd1","available":[]},"tableId":"ndgvz5mlhfuaad6e"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":4,"streamName":"bact3_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ E","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact3_bs_med","views":{"default":"hd1","available":[]},"tableId":"ndgv45bghfuaaebf"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":5,"streamName":"bact6_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ F","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact6_bs_med","views":{"default":"hd1","available":[]},"tableId":"nmwde3fd7hvqhq43"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":6,"streamName":"bact7_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ G","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact7_bi_med","views":{"default":"hd1","available":[]},"tableId":"nmwdzhbg7hvqh6a7"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":7,"streamName":"bact10_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ H","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact10_bi_med","views":{"default":"hd1","available":[]},"tableId":"nxpj4wumgclak2lx"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":8,"streamName":"bact9_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ I","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact9_bs_med","views":{"default":"hd1","available":[]},"tableId":"nxpkul2hgclallno"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":9,"streamName":"bact8_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ J","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact8_bs_med","views":{"default":"hd1","available":[]},"tableId":"obj64qcnqfunjelj"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":10,"streamName":"bact11_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc Độ K","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact11_bi_med","views":{"default":"hd1","available":[]},"tableId":"ocye2ju2bsoyq6vv"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":11,"streamName":"bact13_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ L","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact13_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5cwp54ccmymck"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":12,"streamName":"bact14_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ M","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact14_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5dsly4ccmynil"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":13,"streamName":"bact15_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ N","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact15_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5eja74ccmyoiq"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":14,"streamName":"bact16_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ O","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact16_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5fbxm4ccmypmb"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":15,"streamName":"bact18_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ P","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact18_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5fzje4ccmyqnr"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":16,"streamName":"bact22_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ Q","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact22_bi_med","views":{"default":"hd1","available":[]},"tableId":"o4kyj7tgpwqqy4m4"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":true},"order":17,"streamName":"lbac_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat nhanh","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/lbac_bs_med","views":{"default":"hd1","available":[]},"tableId":"LightningBac0001"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":18,"streamName":"bact4_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ Không hoa hồng A","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact4_bs_med","views":{"default":"hd1","available":[]},"tableId":"ndgv76kehfuaaeec"},{"reserved":false,"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":19,"streamName":"bact12_bs_med","name":"Baccarat Tốc độ Không Hoa hồng B","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact12_bi_med","views":{"default":"hd1","available":[]},"tableId":"ocye5hmxbsoyrcii"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":20,"streamName":"bact17_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Tốc độ Không Hoa hồng C","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact17_bi_med","views":{"default":"hd1","available":[]},"tableId":"ovu5h6b3ujb4y53w"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":21,"streamName":"ncr1_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Không Hoa hồng","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/ncr1_bi_med","views":{"default":"hd1","available":[]},"tableId":"NoCommBac0000001"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":22,"streamName":"bac2_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat A","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac2_bi_med","views":{"default":"hd1","available":[]},"tableId":"oytmvb9m1zysmc44"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":23,"streamName":"bac3_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat B","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac3_bi_med","views":{"default":"hd1","available":[]},"tableId":"60i0lcfx5wkkv3sy"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":24,"streamName":"bact5_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat C","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bact5_bi_med","views":{"default":"hd1","available":[]},"tableId":"ndgvs3tqhfuaadyg"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":25,"streamName":"bac1_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Squeeze","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac1_bi_med","tableDetailsKey":"squeeze","views":{"default":"hd1","available":[]},"tableId":"zixzea8nrf1675oh"},{"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":26,"streamName":"bac4_bs_med","tableType":"bacpro:newhd","reserved":false,"name":"Baccarat Kiểm Soát Bóp","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/bac4_bs_med","views":{"available":[]},"tableId":"k2oswnib7jjaaznw"},{"reserved":false,"betLimits":{"min":20000,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":27,"streamName":"spr1_bs_med","name":"Salon Privé Baccarat A","html5Only":true,"privateTable":true,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/spr1_bi_med","tableDetailsKey":"private","views":{"default":"hd1","available":[]},"tableId":"SalPrivBac000001"},{"reserved":false,"betLimits":{"min":20000,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":28,"streamName":"spr2_bs_med","name":"Salon Privé Baccarat B","html5Only":true,"privateTable":true,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/spr2_bi_med","tableDetailsKey":"private","views":{"default":"hd1","available":[]},"tableId":"n7ltqx5j25sr7xbe"},{"reserved":false,"betLimits":{"min":20000,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":29,"streamName":"spr7_bs_med","name":"Salon Privé Baccarat C","html5Only":true,"privateTable":true,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/spr7_bi_med","tableDetailsKey":"private","views":{"default":"hd1","available":[]},"tableId":"ok37hvy3g7bofp4l"},{"reserved":false,"betLimits":{"min":20000,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":30,"streamName":"spr3_bs_med","name":"Salon Privé Baccarat D","html5Only":true,"privateTable":true,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/spr3_bi_med","tableDetailsKey":"private","views":{"default":"hd1","available":[]},"tableId":"SalPrivBac000004"},{"reserved":false,"betLimits":{"min":20000,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"baccarat":{"lightning":false},"order":31,"streamName":"spr4_bs_med","name":"Salon Privé Baccarat E","html5Only":true,"privateTable":true,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"baccarat","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/spr4_bi_med","tableDetailsKey":"private","views":{"default":"hd1","available":[]},"tableId":"pctte34dt6bqbtps"},{"reserved":false,"betLimits":{"min":4.00,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"order":32,"streamName":"sicbo_dice_med","name":"Siêu Tài Xỉu","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"sicbo","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/42/sicbo_dice_med","views":{"available":["hd1","view2"]},"tableId":"SuperSicBo000001"},{"reserved":false,"betLimits":{"min":20,"max":50000,"currencyCode":"VN2","currencySymbol":"VN2"},"multiWindow":true,"dragonTiger":{"oddEvenEnabled":false},"order":33,"streamName":"dtr1_bs_med","name":"Long Hổ","html5Only":true,"privateTable":false,"published":true,"tiles":{},"appVersion":"4","rng":false,"gameType":"dragontiger","lobbyStreamUrl":"wss://live1-lufb.egcvi.com/ws/video/30/dtr1_bi_med","views":{"available":[]},"tableId":"DragonTiger00001"}],"total":"34"},"time":1627721200999}
            // Bacarat updated
            //{ "id":"1627721227646-4426","type":"lobby.baccaratRoadUpdated","args":{ "tableId":"nxpkul2hgclallno","bigRoad":[{ "ties":0,"playerPair":false,"score":8,"row":0,"color":"Blue","lightning":false,"col":0,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":0,"color":"Red","lightning":false,"col":1,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":1,"color":"Red","lightning":false,"col":1,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":6,"row":2,"color":"Red","lightning":false,"col":1,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":3,"color":"Red","lightning":false,"col":1,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":4,"color":"Red","lightning":false,"col":1,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":0,"color":"Blue","lightning":false,"col":2,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":6,"row":1,"color":"Blue","lightning":false,"col":2,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":6,"row":2,"color":"Blue","lightning":false,"col":2,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":0,"color":"Red","lightning":false,"col":3,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":0,"color":"Blue","lightning":false,"col":4,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":1,"color":"Blue","lightning":false,"col":4,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":8,"row":2,"color":"Blue","lightning":false,"col":4,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":8,"row":3,"color":"Blue","lightning":false,"col":4,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":0,"color":"Red","lightning":false,"col":5,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":8,"row":0,"color":"Blue","lightning":false,"col":6,"natural":false,"bankerPair":true},{ "ties":0,"playerPair":false,"score":5,"row":0,"color":"Red","lightning":false,"col":7,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":8,"row":0,"color":"Blue","lightning":false,"col":8,"natural":true,"bankerPair":false},{ "ties":1,"playerPair":true,"score":8,"row":1,"color":"Blue","lightning":false,"col":8,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":6,"row":2,"color":"Blue","lightning":false,"col":8,"natural":false,"bankerPair":true},{ "ties":0,"playerPair":false,"score":8,"row":3,"color":"Blue","lightning":false,"col":8,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":4,"color":"Blue","lightning":false,"col":8,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":5,"color":"Blue","lightning":false,"col":8,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":8,"row":0,"color":"Red","lightning":false,"col":9,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":5,"row":0,"color":"Blue","lightning":false,"col":10,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":5,"row":0,"color":"Red","lightning":false,"col":11,"natural":false,"bankerPair":false},{ "ties":0,"playerPair":false,"score":9,"row":1,"color":"Red","lightning":false,"col":11,"natural":true,"bankerPair":false},{ "ties":0,"playerPair":false,"score":7,"row":2,"color":"Red","lightning":false,"col":11,"natural":false,"bankerPair":false}]},"time":1627721227646}
        }

        private bool CheckLoginResponse(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            return Convert.ToBoolean(data.Data.IsSuccess);
        }

        private List<Table> ExtractTableList(string json)
        {
            dynamic lobbyInfo = JsonConvert.DeserializeObject(json);

            if (lobbyInfo == null) return null;

            var tableList = lobbyInfo.Data.Tables;
            var tables = new List<Table>();

            foreach(var tbl in tableList)
            {
                var isOpen = Convert.ToBoolean(tbl.IsOpen);
                if (!isOpen) continue;

                var table = new Table
                {
                    name = tbl.TableCode,
                    tableId = tbl.TableCode,
                    gameType = tbl.GameType,
                    tableType = tbl.TableType
                };
                tables.Add(table);
            }

            return tables;
        }

        private void UpdateTableInfo(string json)
        {
            dynamic tableInfo = JsonConvert.DeserializeObject(json);

            if (tableInfo == null) return;

            var tableList = tableInfo.Data.Tables;

            foreach (var tbl in tableList)
            {
                if (tbl.UpdateType != 5) continue;
                
                var roundCount = Convert.ToInt32(tbl.RoundCount);
                var historyList = tbl.GameStatus.History;
                var result = ToResult(historyList);
                var tableId = Convert.ToString(tbl.TableCode);

                bcrBusiness.UpsertTablesession(tableId, result, roundCount);

                _uiInteraction.Log($"Table : {tableId} result: {result} in round {roundCount}");
            }
        }

        private string ToResult(dynamic history)
        {
            string result = "";
            foreach(var h in history)
            {
                var value = Convert.ToInt32(h);
                var tie = (value & 1) == 1 ? true : false;
                var player = (value & 2) == 2 ? true : false;

                if (tie) result += "T";
                if (player)
                {
                    result += "P";
                }
                else
                {
                    result += "B";
                }
            }
            return result;
        }

        public async Task StopWsAsync()
        {
            if (wsClient != null)
            {
                wsClient.Stop(WebSocketCloseStatus.NormalClosure, "");
            }
        }
    }
}
