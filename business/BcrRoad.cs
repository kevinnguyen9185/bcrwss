using System;
using System.Collections.Generic;
using System.Text;

namespace business.BcrRoad
{
    public class BigRoad
    {
        public int ties { get; set; }
        public bool playerPair { get; set; }
        public int score { get; set; }
        public int row { get; set; }
        public string color { get; set; }
        public bool lightning { get; set; }
        public int col { get; set; }
        public bool natural { get; set; }
        public bool bankerPair { get; set; }
    }

    public class Args
    {
        public string tableId { get; set; }
        public List<BigRoad> bigRoad { get; set; }
    }

    public class Road
    {
        public string id { get; set; }
        public string type { get; set; }
        public Args args { get; set; }
        public long time { get; set; }
    }
}
