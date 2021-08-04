using System;
using System.Collections.Generic;
using System.Text;

namespace business.Input.Lobby
{
    public class Category
    {
        public string categoryId { get; set; }
    }

    public class BetLimits
    {
        public double min { get; set; }
        public int max { get; set; }
        public string currencyCode { get; set; }
        public string currencySymbol { get; set; }
    }

    public class Baccarat
    {
        public bool lightning { get; set; }
    }

    public class Tiles
    {
    }

    public class Views
    {
        public string @default { get; set; }
        public List<string> available { get; set; }
    }

    public class DragonTiger
    {
        public bool oddEvenEnabled { get; set; }
    }

    public class Table
    {
        public BetLimits betLimits { get; set; }
        public bool multiWindow { get; set; }
        public Baccarat baccarat { get; set; }
        public int order { get; set; }
        public string streamName { get; set; }
        public string tableType { get; set; }
        public bool reserved { get; set; }
        public string name { get; set; }
        public bool html5Only { get; set; }
        public bool privateTable { get; set; }
        public bool published { get; set; }
        public Tiles tiles { get; set; }
        public string appVersion { get; set; }
        public bool rng { get; set; }
        public string gameType { get; set; }
        public string lobbyStreamUrl { get; set; }
        public Views views { get; set; }
        public string tableId { get; set; }
        public string tableDetailsKey { get; set; }
        public DragonTiger dragonTiger { get; set; }
    }

    public class Args
    {
        public Category category { get; set; }
        public List<Table> tables { get; set; }
        public string total { get; set; }
    }

    public class LobbyInfo
    {
        public string id { get; set; }
        public string type { get; set; }
        public Args args { get; set; }
        public long time { get; set; }
    }
}
