using System;
using System.Collections.Generic;

namespace business.Entity
{
    public class BcrTable
    {
        public int Id { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public List<RoadSession> Sessions { get; set; }
        public string Vendor { get; set; }
    }

    public class RoadSession
    {
        public string SessionId { get; set; }
        public string ResultString { get; set; }
        public DateTime Dts { get; set; }
    }
}
